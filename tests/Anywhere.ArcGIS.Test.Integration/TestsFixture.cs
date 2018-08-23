namespace Anywhere.ArcGIS.Test.Integration
{
    using Polly;
    using Serilog;
    using Serilog.Events;
    using System;
    using System.Net;
    using System.Net.Http;
    using Xunit.Abstractions;

    public class IntegrationTestFixture : IDisposable
    {
        public static Policy TestPolicy { get; private set; }
        IDisposable _logCapture;

        static IntegrationTestFixture()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            TestPolicy = Policy
                .Handle<InvalidOperationException>()
                .Or<HttpRequestException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.XunitTestOutput()
                .CreateLogger();
        }

        public void SetTestOutputHelper(ITestOutputHelper output)
        {
            _logCapture = XUnitTestOutputSink.Capture(output);
        }

        public void Dispose()
        {
            _logCapture.Dispose();
        }
    }
}
