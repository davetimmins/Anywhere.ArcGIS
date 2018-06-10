using Anywhere.ArcGIS.Common;
using Anywhere.ArcGIS.Logging;
using Anywhere.ArcGIS.Operation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Anywhere.ArcGIS
{
    public class AttachmentWorker : IPortalGateway, IDisposable
    {
        HttpClient _httpClient;
        readonly ILog _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentGateway"/> class.
        /// Create an ArcGIS Server gateway to access secure resources
        /// </summary>
        /// <param name="rootUrl">Made up of scheme://host:port/site</param>
        /// <param name="serializer">Used to (de)serialize requests and responses</param>
        /// <param name="tokenProvider">Provide access to a token for secure resources</param>
        public AttachmentWorker(string rootUrl, ISerializer serializer = null, ITokenProvider tokenProvider = null, Func<HttpClient> httpClientFunc = null)
        {
            if (string.IsNullOrWhiteSpace(rootUrl))
            {
                throw new ArgumentNullException(nameof(rootUrl), "rootUrl is null.");
            }

            RootUrl = rootUrl.AsRootUrl();
            TokenProvider = tokenProvider;
            Serializer = serializer ?? SerializerFactory.Get();

            var httpFunc = httpClientFunc ?? HttpClientFactory.Get;
            _httpClient = httpFunc();
            _logger = LogProvider.For<AttachmentWorker>();
        }

        ~AttachmentWorker()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_httpClient != null)
                {
                    _httpClient.Dispose();
                    _httpClient = null;
                }
            }
        }

        public string RootUrl { get; private set; }

        public ITokenProvider TokenProvider { get; private set; }

        public ISerializer Serializer { get; private set; }

        public Task<AddAttachmentResponse> AddAttachment(AttachmentToPost attachment, CancellationToken ct = default(CancellationToken))
        {
            if (attachment == null)
            {
                throw new ArgumentNullException(nameof(attachment));
            }

            if (attachment.Attachment == null)
            {
                throw new ArgumentNullException(nameof(attachment.Attachment));
            }

            return Post<AddAttachmentResponse, AttachmentToPost>(attachment, ct);
        }

        public Task<UpdateAttachmentResponse> UpdateAttachment(AttachmentToPost attachment, CancellationToken ct = default(CancellationToken))
        {
            if (attachment == null)
            {
                throw new ArgumentNullException(nameof(attachment));
            }

            if (attachment.Attachment == null)
            {
                throw new ArgumentNullException(nameof(attachment.Attachment));
            }

            return Post<UpdateAttachmentResponse, AttachmentToPost>(attachment, ct);
        }

        protected async Task<T> Post<T, TRequest>(TRequest requestObject, CancellationToken ct)
            where TRequest : ArcGISServerOperation, IAttachment
            where T : IPortalResponse
        {
            if (requestObject == null)
            {
                throw new ArgumentNullException(nameof(requestObject));
            }

            if (requestObject.Endpoint == null)
            {
                throw new ArgumentNullException(nameof(requestObject.Endpoint));
            }

            requestObject.BeforeRequest?.Invoke();

            var endpoint = requestObject.Endpoint;
            var url = endpoint.BuildAbsoluteUrl(RootUrl).Split('?').FirstOrDefault();

            var token = await CheckGenerateToken(ct);
            if (ct.IsCancellationRequested)
            {
                return default(T);
            }

            // these should have already been added
            if (string.IsNullOrWhiteSpace(requestObject.Token) && token != null && !string.IsNullOrWhiteSpace(token.Value))
            {
                requestObject.Token = token.Value;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.Value);
                if (token.AlwaysUseSsl)
                {
                    url = url.Replace("http:", "https:");
                }
            }

            _httpClient.CancelPendingRequests();

            Uri uri;
            bool validUrl = Uri.TryCreate(url, UriKind.Absolute, out uri);
            if (!validUrl)
            {
                throw new HttpRequestException(string.Format("Not a valid url: {0}", url));
            }

            _logger.DebugFormat("Post attachment to {0}", uri);
            string resultString = string.Empty;
            var attachment = (IAttachment)requestObject;

            using (var content = new MultipartFormDataContent())
            {
                var streamContent = new StreamContent(new MemoryStream(attachment.Attachment));
                var header = new ContentDispositionHeaderValue("form-data");
                header.Name = "\"attachment\"";
                header.FileName = attachment.FileName;
                streamContent.Headers.ContentDisposition = header;
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(attachment.ContentType);

                content.Add(streamContent);
                content.Add(new StringContent("json"), "f");
                if (!string.IsNullOrWhiteSpace(requestObject.Token))
                {
                    content.Add(new StringContent(requestObject.Token), "token");
                }

                try
                {
                    using (var message = await _httpClient.PostAsync(uri, content))
                    {
                        message.EnsureSuccessStatusCode();
                        resultString = await message.Content.ReadAsStringAsync();
                    }
                }
                catch (TaskCanceledException)
                {
                    return default(T);
                }
            }

            var result = Serializer.AsPortalResponse<T>(resultString);
            if (result.Error != null)
            {
                throw new InvalidOperationException(result.Error.ToString());
            }

            requestObject.AfterRequest?.Invoke();

            return result;
        }

        async Task<Token> CheckGenerateToken(CancellationToken ct)
        {
            if (TokenProvider == null)
            {
                return null;
            }

            var token = await TokenProvider.CheckGenerateToken(ct);

            if (token != null)
            {
                CheckRefererHeader(token.Referer);
            }

            return token;
        }

        void CheckRefererHeader(string referrer)
        {
            if (_httpClient == null || string.IsNullOrWhiteSpace(referrer))
            {
                return;
            }

            Uri referer;
            bool validReferrerUrl = Uri.TryCreate(referrer, UriKind.Absolute, out referer);
            if (!validReferrerUrl)
            {
                throw new HttpRequestException(string.Format("Not a valid url for referrer: {0}", referrer));
            }

            _httpClient.DefaultRequestHeaders.Referrer = referer;
        }
    }
}
