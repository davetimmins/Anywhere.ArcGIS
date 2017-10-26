using Anywhere.ArcGIS.Operation;
using System.Threading.Tasks;

namespace Anywhere.ArcGIS
{
    /// <summary>
    /// Used for generating a token which can then be appended to requests made through a gateway
    /// </summary>
    public interface ITokenProvider
    {
        /// <summary>
        /// Made up of scheme://host:port/site
        /// </summary>
        string RootUrl { get; }

        /// <summary>
        /// Used for (de)serializtion of requests and responses.
        /// </summary>
        ISerializer Serializer { get; }

        /// <summary>
        /// Returns a valid token for the corresponding request
        /// </summary>
        /// <returns>A token that can be used for subsequent requests to secure resources</returns>
        Task<Token> CheckGenerateToken(System.Threading.CancellationToken ct);

        /// <summary>
        /// Used for automatic encryption of token requests if the admin operations are accessible for the server.
        /// </summary>
        ICryptoProvider CryptoProvider { get; }

        /// <summary>
        /// The username that this token provider is for
        /// </summary>
        string UserName { get; }
    }
}
