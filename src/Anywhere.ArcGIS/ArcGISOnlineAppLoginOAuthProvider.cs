namespace Anywhere.ArcGIS
{
    using Anywhere.ArcGIS.Logging;
    using Anywhere.ArcGIS.Operation;
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// ArcGIS Online application login type OAuth 2.0 token provider
    /// </summary>
    public class ArcGISOnlineAppLoginOAuthProvider : ITokenProvider, IDisposable
    {
        HttpClient _httpClient;
        protected readonly GenerateOAuthToken OAuthRequest;
        Token _token;
        readonly ILog _logger;

        /// <summary>
        /// Create an OAuth token provider to authenticate against ArcGIS Online
        /// </summary>
        /// <param name="clientId">The Client Id from your API access section of your application from developers.arcgis.com</param>
        /// <param name="clientSecret">The Client Secret from your API access section of your application from developers.arcgis.com</param>
        /// <param name="serializer">Used to (de)serialize requests and responses</param>
        public ArcGISOnlineAppLoginOAuthProvider(string clientId, string clientSecret, ISerializer serializer = null, Func<HttpClient> httpClientFunc = null)
            : this(() => LogProvider.For<ArcGISOnlineAppLoginOAuthProvider>(), clientId, clientSecret, serializer)
        { }

        internal ArcGISOnlineAppLoginOAuthProvider(Func<ILog> log, string clientId, string clientSecret, ISerializer serializer = null, Func<HttpClient> httpClientFunc = null)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentNullException(nameof(clientId), "clientId is null.");
            }

            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                throw new ArgumentNullException(nameof(clientSecret), "clientSecret is null.");
            }

            Serializer = serializer ?? SerializerFactory.Get();     
            OAuthRequest = new GenerateOAuthToken(clientId, clientSecret);
            var httpFunc = httpClientFunc ?? HttpClientFactory.Get;
            _httpClient = httpFunc();

            _logger = log() ?? LogProvider.For<ArcGISOnlineAppLoginOAuthProvider>();
            _logger.DebugFormat("Created new token provider for {0}", RootUrl);
        }

        ~ArcGISOnlineAppLoginOAuthProvider()
        {
            Dispose(false);
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
                _token = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public string RootUrl
        {
            get { return "https://www.arcgis.com/sharing/rest/oauth2/token"; }
        }

        public bool CancelPendingRequests { get; set; }

        public string UserName { get { return null; } }

        public ISerializer Serializer { get; private set; }

        public ICryptoProvider CryptoProvider { get { return null; } }

        public async Task<Token> CheckGenerateToken(CancellationToken ct)
        {
            if (OAuthRequest == null) return null;

            if (_token != null && !_token.IsExpired) return _token;

            _token = null; // reset the Token

            HttpContent content = new FormUrlEncodedContent(Serializer.AsDictionary(OAuthRequest));

            if (CancelPendingRequests)
            {
                _httpClient.CancelPendingRequests();
            }

            string resultString = string.Empty;
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(RootUrl, content, ct).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                resultString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch (TaskCanceledException tce)
            {
                _logger.WarnException("Token request cancelled (exception swallowed)", tce);
                return default(Token);
            }

            var result = Serializer.AsPortalResponse<OAuthToken>(resultString);

            if (result.Error != null)
            {
                throw new InvalidOperationException(result.Error.ToString());
            }

            _token = result.AsToken();
            return _token;
        }
    }
}
