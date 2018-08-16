using System;
using System.Net.Http;

namespace Anywhere.ArcGIS
{
    /// <summary>
    /// Provides a token for ArcGIS Server when it is federated with Portal for ArcGIS
    /// </summary>
    public class ServerFederatedWithPortalTokenProvider : TokenProvider
    {
        /// <summary>
        /// Create a token provider to authenticate against an ArcGIS Server federated with Portal for ArcGIS
        /// </summary>
        /// <param name="rootUrl"></param>
        /// <param name="username">Portal for ArcGIS user name</param>
        /// <param name="password">Portal for ArcGIS user password</param>
        /// <param name="serializer">Used to (de)serialize requests and responses</param>
        /// <param name="referer">Referer url to use for the token generation. For federated servers this will be the rootUrl + '/rest'</param>
        /// <param name="cryptoProvider">Used to encrypt the token reuqest. If not set it will use the default from CryptoProviderFactory</param>
        public ServerFederatedWithPortalTokenProvider(string rootUrl, string username, string password, ISerializer serializer = null, string referer = null, ICryptoProvider cryptoProvider = null, Func<HttpClient> httpClientFunc = null)
            : base(rootUrl, username, password, serializer, referer, cryptoProvider, httpClientFunc)
        {
            TokenRequest.IsFederated = true;
            if (string.IsNullOrWhiteSpace(referer))
            {
                TokenRequest.Referer = rootUrl.TrimEnd('/') + "/rest";
            }
        }
    }
}
