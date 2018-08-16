using System;
using System.Net.Http;

namespace Anywhere.ArcGIS
{
    public class ArcGISOnlineFederatedTokenProvider : FederatedTokenProvider
    {
        public ArcGISOnlineFederatedTokenProvider(ITokenProvider tokenProvider, string serverUrl, ISerializer serializer = null, string referer = "https://www.arcgis.com", Func<HttpClient> httpClientFunc = null)
            : base(tokenProvider, PortalGatewayBase.AGOPortalUrl, serverUrl, serializer, referer, httpClientFunc)
        { }
    }
}
