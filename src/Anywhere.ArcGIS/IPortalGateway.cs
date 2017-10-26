
namespace Anywhere.ArcGIS
{
    /// <summary>
    /// Used to broker calls to ArcGIS Server, ArcGIS Online or ArcGIS for Portal
    /// </summary>
    public interface IPortalGateway
    {
        /// <summary>
        /// Made up of scheme://host:port/site
        /// </summary>
        string RootUrl { get; }

        /// <summary>
        /// Used for generating a token which can then be appended to requests made through this gateway automatically
        /// </summary>
        ITokenProvider TokenProvider { get; }

        /// <summary>
        /// Used for (de)serializtion of requests and responses. 
        /// </summary>
        ISerializer Serializer { get; }
    }
}
