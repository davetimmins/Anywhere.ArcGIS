namespace Anywhere.ArcGIS
{
    using Logging;
    using Operation;
    using Operation.Admin;
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// ArcGIS Server token provider
    /// </summary>
    public class TokenProvider : ITokenProvider, IDisposable
    {
        HttpClient _httpClient;
        protected GenerateToken TokenRequest;
        Token _token;
        PublicKeyResponse _publicKey;
        protected bool CanAccessPublicKeyEndpoint = true;
        readonly ILog _logger;

        /// <summary>
        /// Create a token provider to authenticate against ArcGIS Server
        /// </summary>
        /// <param name="rootUrl">Made up of scheme://host:port/site</param>
        /// <param name="username">ArcGIS Server user name</param>
        /// <param name="password">ArcGIS Server user password</param>
        /// <param name="serializer">Used to (de)serialize requests and responses</param>
        /// <param name="referer">Referer url to use for the token generation</param>
        /// <param name="cryptoProvider">Used to encrypt the token reuqest. If not set it will use the default from CryptoProviderFactory</param>
        public TokenProvider(string rootUrl, string username, string password, ISerializer serializer = null, string referer = "", ICryptoProvider cryptoProvider = null, Func<HttpClient> httpClientFunc = null)
            : this (() => LogProvider.For<TokenProvider>(), rootUrl, username, password, serializer, referer, cryptoProvider, httpClientFunc)
        { }

        internal TokenProvider(Func<ILog> log, string rootUrl, string username, string password, ISerializer serializer = null, string referer = "", ICryptoProvider cryptoProvider = null, Func<HttpClient> httpClientFunc = null)
        {
            if (string.IsNullOrWhiteSpace(rootUrl))
            {
                throw new ArgumentNullException(nameof(rootUrl), "rootUrl is null.");
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username), "username is null.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password), "password is null.");
            }

            Serializer = serializer ?? SerializerFactory.Get();
            
            RootUrl = rootUrl.AsRootUrl();
            CryptoProvider = cryptoProvider ?? CryptoProviderFactory.Get();
            var httpFunc = httpClientFunc ?? HttpClientFactory.Get;
            _httpClient = httpFunc();
            TokenRequest = new GenerateToken(username, password) { Referer = referer };
            UserName = username;

            _logger = log() ?? LogProvider.For<TokenProvider>();
            _logger.DebugFormat("Created new token provider for {0}", RootUrl);
        }

        ~TokenProvider()
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

        public bool CancelPendingRequests { get; set; }

        public ICryptoProvider CryptoProvider { get; private set; }

        public string RootUrl { get; private set; }

        public string UserName { get; private set; }

        public ISerializer Serializer { get; private set; }

        void CheckRefererHeader()
        {
            if (_httpClient == null || string.IsNullOrWhiteSpace(TokenRequest.Referer)) return;

            bool validReferrerUrl = Uri.TryCreate(TokenRequest.Referer, UriKind.Absolute, out Uri referer);
            if (!validReferrerUrl)
            {
                throw new HttpRequestException(string.Format("Not a valid url for referrer: {0}", TokenRequest.Referer));
            }
            _httpClient.DefaultRequestHeaders.Referrer = referer;
        }

        //Token _token;
        /// <summary>
        /// Generates a token using the username and password set for this provider.
        /// </summary>
        /// <returns>The generated token or null if not applicable</returns>
        /// <remarks>This sets the Token property for the provider. It will be auto appended to
        /// any requests sent through the gateway used by this provider.</remarks>
        public async Task<Token> CheckGenerateToken(CancellationToken ct = default(CancellationToken))
        {
            if (TokenRequest == null)
            {
                return null;
            }

            if (_token != null && !_token.IsExpired)
            {
                return _token;
            }

            _token = null; // reset the Token
            _publicKey = null;

            CheckRefererHeader();

            var url = TokenRequest.BuildAbsoluteUrl(RootUrl).Split('?').FirstOrDefault();
            bool validUrl = Uri.TryCreate(url, UriKind.Absolute, out Uri uri);
            if (!validUrl)
            {
                throw new HttpRequestException(string.Format("Not a valid url: {0}", url));
            }
            _logger.DebugFormat("Token request {0}", uri.AbsoluteUri);

            if (CryptoProvider != null && _publicKey == null && CanAccessPublicKeyEndpoint)
            {
                var publicKey = new PublicKey();
                var encryptionInfoEndpoint = publicKey.BuildAbsoluteUrl(RootUrl) + PortalGatewayBase.AsRequestQueryString(Serializer, publicKey);
                _logger.DebugFormat("Encrypted token request {0}", encryptionInfoEndpoint);

                string publicKeyResultString = null;
                try
                {
                    if (CancelPendingRequests)
                    {
                        _httpClient.CancelPendingRequests();
                    }
                    HttpResponseMessage response = await _httpClient.GetAsync(encryptionInfoEndpoint, ct).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    publicKeyResultString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                catch (TaskCanceledException tce)
                {
                    _logger.WarnException("Token request cancelled (exception swallowed)", tce);
                    return default(Token);
                }
                catch (HttpRequestException hex)
                {
                    _logger.WarnException("Token request exception (exception swallowed)", hex);
                    CanAccessPublicKeyEndpoint = false;
                }

                if (ct.IsCancellationRequested) return null;

                if (CanAccessPublicKeyEndpoint)
                {
                    _publicKey = Serializer.AsPortalResponse<PublicKeyResponse>(publicKeyResultString);
                    if (_publicKey.Error != null)
                    {
                        throw new InvalidOperationException(_publicKey.Error.ToString());
                    }

                    TokenRequest = CryptoProvider.Encrypt(TokenRequest, _publicKey.Exponent, _publicKey.Modulus);
                }
            }

            if (ct.IsCancellationRequested)
            {
                return null;
            }

            HttpContent content = new FormUrlEncodedContent(Serializer.AsDictionary(TokenRequest));

            if (CancelPendingRequests)
            {
                _httpClient.CancelPendingRequests();
            }

            string resultString = string.Empty;
            try
            {
                _logger.DebugFormat("HTTP call: {0} {1}", uri, content);
                HttpResponseMessage response = await _httpClient.PostAsync(uri, content, ct).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                resultString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                _logger.DebugFormat("HTTP call response: {0}", resultString);
            }
            catch (TaskCanceledException tce)
            {
                _logger.WarnException("Token request cancelled (exception swallowed)", tce);
                return default(Token);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Token request failed", ex);
                return default(Token);
            }
            // TODO ; add verbose logging
            Token result = null;

            try
            {
                result = Serializer.AsPortalResponse<Token>(resultString);
            }
            catch (Exception ex)
            {
                _logger.WarnException("unable to deserialize token", ex);
                return default(Token);
            }

            if (result?.Error != null)
            {
                throw new InvalidOperationException(result.Error.ToString());
            }

            if (!string.IsNullOrWhiteSpace(TokenRequest.Referer))
            {
                result.Referer = TokenRequest.Referer;
            }

            _token = result;
            return _token;
        }
    }
}
