namespace Anywhere.ArcGIS
{
    using Anywhere.ArcGIS.Logging;
    using Anywhere.ArcGIS.Operation;
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// ArcGIS Server token provider for a federated server
    /// </summary>
    public class FederatedTokenProvider : ITokenProvider, IDisposable
    {
        HttpClient _httpClient;
        protected readonly GenerateFederatedToken TokenRequest;
        Token _token;
        readonly ILog _logger;

        /// <summary>
        /// Create a token provider to authenticate against an ArcGIS Server that is federated
        /// </summary>
        /// <param name="tokenProvider"></param>
        /// <param name="rootUrl"></param>
        /// <param name="serverUrl"></param>
        /// <param name="serializer">Used to (de)serialize requests and responses</param>
        /// <param name="referer">Referer url to use for the token generation. For federated servers this will be the portal rootUrl</param>
        public FederatedTokenProvider(ITokenProvider tokenProvider, string rootUrl, string serverUrl, ISerializer serializer = null, string referer = null, Func<HttpClient> httpClientFunc = null)
            : this(() => LogProvider.For<FederatedTokenProvider>(), tokenProvider, rootUrl, serverUrl, serializer, referer, httpClientFunc)
        { }

        internal FederatedTokenProvider(Func<ILog> log, ITokenProvider tokenProvider, string rootUrl, string serverUrl, ISerializer serializer = null, string referer = null, Func<HttpClient> httpClientFunc = null)
        {
            if (tokenProvider == null)
            {
                throw new ArgumentNullException(nameof(tokenProvider));
            }

            if (string.IsNullOrWhiteSpace(rootUrl))
            {
                throw new ArgumentNullException(nameof(rootUrl), "rootUrl is null.");
            }

            if (string.IsNullOrWhiteSpace(serverUrl))
            {
                throw new ArgumentNullException(nameof(serverUrl), "serverUrl is null.");
            }

            Serializer = serializer ?? SerializerFactory.Get();

            RootUrl = rootUrl.AsRootUrl();
            var httpFunc = httpClientFunc ?? HttpClientFactory.Get;
            _httpClient = httpFunc();
            TokenRequest = new GenerateFederatedToken(serverUrl, tokenProvider) { Referer = referer };

            _logger = log() ?? LogProvider.For<FederatedTokenProvider>();
            _logger.DebugFormat("Created new token provider for {0}", RootUrl);
        }

        ~FederatedTokenProvider()
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

        public ISerializer Serializer { get; private set; }

        public ICryptoProvider CryptoProvider { get { return null; } }

        public bool CancelPendingRequests { get; set; }

        public string RootUrl { get; private set; }

        public string UserName { get { return null; } }

        public async Task<Token> CheckGenerateToken(CancellationToken ct)
        {
            if (TokenRequest == null) return null;

            if (_token != null && !_token.IsExpired) return _token;

            _token = null; // reset the Token

            TokenRequest.FederatedToken = await TokenRequest.TokenProvider.CheckGenerateToken(ct).ConfigureAwait(false);

            HttpContent content = new FormUrlEncodedContent(Serializer.AsDictionary(TokenRequest));

            if (CancelPendingRequests)
            {
                _httpClient.CancelPendingRequests();
            }

            var url = TokenRequest.BuildAbsoluteUrl(RootUrl).Split('?').FirstOrDefault();
            bool validUrl = Uri.TryCreate(url, UriKind.Absolute, out Uri uri);
            if (!validUrl)
            {
                throw new HttpRequestException(string.Format("Not a valid url: {0}", url));
            }
            _logger.DebugFormat("Token request {0}", uri.AbsoluteUri);

            string resultString = string.Empty;
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(url, content, ct).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                resultString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch (TaskCanceledException tce)
            {
                _logger.WarnException("Token request cancelled (exception swallowed)", tce);
                return default(Token);
            }

            var result = Serializer.AsPortalResponse<Token>(resultString);

            if (result.Error != null)
            {
                throw new InvalidOperationException(result.Error.ToString());
            }

            _token = result;
            return _token;
        }
    }
}
