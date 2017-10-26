namespace Serilog
{
    using Serilog.Configuration;
    using Serilog.Context;
    using Serilog.Core;
    using Serilog.Events;
    using Serilog.Formatting;
    using Serilog.Formatting.Display;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading;
    using Xunit.Abstractions;

    /// <summary>
    /// Adds the WriteTo.XunitTestOutput(output) extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class LoggerConfigurationXunitTestOutputExtensions
    {
        const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}";

        /// <summary>
        /// Adds a sink that writes log events to the output of an xUnit test.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="outputTemplate">Message template describing the output format.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration XunitTestOutput(
            this LoggerSinkConfiguration loggerConfiguration,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            string outputTemplate = DefaultOutputTemplate,
            IFormatProvider formatProvider = null)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException("loggerConfiguration");
            if (outputTemplate == null) throw new ArgumentNullException("outputTemplate");

            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);

            return loggerConfiguration.Sink(new XUnitTestOutputSink(formatter), restrictedToMinimumLevel)
                .Enrich.FromLogContext();
        }
    }
        
    public class XUnitTestOutputSink : IObservable<LogEvent>, ILogEventSink, IDisposable
    {
        readonly object _syncRoot = new object();
        IList<IObserver<LogEvent>> _observers = new List<IObserver<LogEvent>>();
        bool _disposed;
        static ITextFormatter _textFormatter;
        static readonly Subject<LogEvent> _logEventSubject = new Subject<LogEvent>();

        const string CaptureCorrelationIdKey = "CaptureCorrelationId";

        public XUnitTestOutputSink(ITextFormatter textFormatter)
        {
            if (textFormatter == null) throw new ArgumentNullException("textFormatter");

            _observers.Add(_logEventSubject);

            XUnitTestOutputSink._textFormatter = textFormatter;
        }

        /// <summary>
        /// Configures the log subscription for the xunit output
        /// </summary>
        /// <param name="testOutputHelper">Xunit <see cref="TestOutputHelper"/> that writes to test output</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static IDisposable Capture(ITestOutputHelper testOutputHelper)
        {
            if (testOutputHelper == null) throw new ArgumentNullException("testOutputHelper");

            var captureId = Guid.NewGuid();

            Func<LogEvent, bool> filter = logEvent =>
                logEvent.Properties.ContainsKey(CaptureCorrelationIdKey) &&
                logEvent.Properties[CaptureCorrelationIdKey].ToString() == captureId.ToString();

            var subscription = XUnitTestOutputSink._logEventSubject.Where(filter).Subscribe(logEvent =>
            {
                using (var writer = new StringWriter())
                {
                    XUnitTestOutputSink._textFormatter.Format(logEvent, writer);
                    testOutputHelper.WriteLine(writer.ToString());
                }
            });
            var pushProperty = LogContext.PushProperty(CaptureCorrelationIdKey, captureId);

            return new DisposableAction(() =>
            {
                subscription.Dispose();
                pushProperty.Dispose();
            });
        }

        private class DisposableAction : IDisposable
        {
            private readonly Action _action;

            public DisposableAction(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                _action();
            }
        }

        sealed class Unsubscriber : IDisposable
        {
            readonly XUnitTestOutputSink _sink;
            readonly IObserver<LogEvent> _observer;

            public Unsubscriber(XUnitTestOutputSink sink, IObserver<LogEvent> observer)
            {
                if (sink == null) throw new ArgumentNullException("sink");
                if (observer == null) throw new ArgumentNullException("observer");
                _sink = sink;
                _observer = observer;
            }

            public void Dispose()
            {
                _sink.Unsubscribe(_observer);
            }
        }

        public IDisposable Subscribe(IObserver<LogEvent> observer)
        {
            if (observer == null) throw new ArgumentNullException("observer");

            lock (_syncRoot)
            {
                // Makes the assumption that list iteration is not
                // mutating - correct but not guaranteed by the BCL.
                var newObservers = _observers.ToList();
                newObservers.Add(observer);
                Interlocked.Exchange(ref _observers, newObservers);
            }

            return new Unsubscriber(this, observer);
        }

        void Unsubscribe(IObserver<LogEvent> observer)
        {
            if (observer == null) throw new ArgumentNullException("observer");

            lock (_syncRoot)
                _observers.Remove(observer);
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null) throw new ArgumentNullException("logEvent");

            Interlocked.MemoryBarrier();

            IList<Exception> exceptions = null;

            foreach (var observer in _observers)
            {
                try
                {
                    observer.OnNext(logEvent);
                }
                catch (Exception ex)
                {
                    if (exceptions == null)
                        exceptions = new List<Exception>();
                    exceptions.Add(ex);
                }
            }

            if (exceptions != null)
                throw new AggregateException("At least one observer failed to accept the event", exceptions);
        }

        public void Dispose()
        {
            lock (_syncRoot)
            {
                if (_disposed) return;

                _disposed = true;
                foreach (var observer in _observers)
                {
                    observer.OnCompleted();
                }
            }
        }
    }
}
