using System;
using System.Net.Http;

namespace Anywhere.ArcGIS
{
    /// <summary>
    /// ArcGIS Online token provider
    /// </summary>
    public class ArcGISOnlineTokenProvider : TokenProvider
    {
        /// <summary>
        /// Create a token provider to authenticate against ArcGIS Online
        /// </summary>
        /// <param name="username">ArcGIS Online user name</param>
        /// <param name="password">ArcGIS Online user password</param>
        /// <param name="serializer">Used to (de)serialize requests and responses</param>
        /// <param name="referer">Referer url to use for the token generation</param>
        public ArcGISOnlineTokenProvider(string username, string password, ISerializer serializer = null, string referer = "https://www.arcgis.com", Func<HttpClient> httpClientFunc = null)
            : base(PortalGatewayBase.AGOPortalUrl, username, password, serializer, referer, httpClientFunc: httpClientFunc)
        {
            CanAccessPublicKeyEndpoint = false;
            TokenRequest.IsFederated = true;
        }
    }
}
