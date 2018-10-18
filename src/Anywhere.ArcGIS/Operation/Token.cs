using Anywhere.ArcGIS.Common;
using System;
using System.Runtime.Serialization;

namespace Anywhere.ArcGIS.Operation
{
    /// <summary>
    /// This operation generates an access token in exchange for user credentials that can be used by clients when working with the ArcGIS Portal API. 
    /// The call is only allowed over HTTPS and must be a POST.
    /// </summary>
    [DataContract]
    public class GenerateToken : CommonParameters, IEndpoint
    {
        public GenerateToken(string username, string password)
        {
            Username = username;
            Password = password;
            ExpirationInMinutes = 60;
        }

        string _client;
        /// <summary>
        /// The client identification type for which the token is to be granted.
        /// </summary>
        /// <remarks>The default value is referer. Setting it to null will also set the Referer to null</remarks>
        [DataMember(Name = "client")]
        public string Client { get { return _client; } set { _client = value; if (string.IsNullOrWhiteSpace(_client)) Referer = null; } }

        string _referer;
        /// <summary>
        /// The base URL of the web app that will invoke the Portal API.
        /// This parameter must be specified if the value of the client parameter is referer.
        /// </summary>
        [DataMember(Name = "referer")]
        public string Referer { get { return _referer; } set { _referer = value; if (!string.IsNullOrWhiteSpace(_referer)) Client = "referer"; } }

        /// <summary>
        /// Username of user who wants to get a token.
        /// </summary>
        [DataMember(Name = "username")]
        public string Username { get; private set; }

        /// <summary>
        /// Password of user who wants to get a token.
        /// </summary>
        [DataMember(Name = "password")]
        public string Password { get; private set; }

        [DataMember(Name = "encrypted")]
        public bool Encrypted { get; private set; }

        /// <summary>
        /// The token expiration time in minutes.
        /// </summary>
        /// <remarks> The default is 60 minutes.</remarks>
        [IgnoreDataMember]
        public int ExpirationInMinutes { get; set; }

        string _expiration;
        [DataMember(Name = "expiration")]
        public string Expiration { get { return string.IsNullOrWhiteSpace(_expiration) ? ExpirationInMinutes.ToString() : _expiration; } }

        /// <summary>
        /// Set this to true to prevent the BuildAbsoluteUrl returning https as the default scheme
        /// </summary>
        [IgnoreDataMember]
        public bool DontForceHttps { get; set; }

        public void Encrypt(string username, string password, string expiration = "", string client = "", string referer = "")
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username), "username is null.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password), "password is null.");
            }

            Username = username;
            Password = password;

            if (!string.IsNullOrWhiteSpace(expiration))
            {
                _expiration = expiration;
            }

            if (!string.IsNullOrWhiteSpace(client))
            {
                _client = client;
            }

            if (!string.IsNullOrWhiteSpace(referer))
            {
                _referer = referer;
            }

            Encrypted = true;
            DontForceHttps = false;
        }

        /// <summary>
        /// Set this to indicate that the request is for a federated server
        /// </summary>
        public bool IsFederated { get; set; }

        public string RelativeUrl
        {
            get { return IsFederated ? Operations.GenerateToken : $"tokens/{Operations.GenerateToken}"; }
        }

        public string BuildAbsoluteUrl(string rootUrl)
        {
            if (string.IsNullOrWhiteSpace(rootUrl))
            {
                throw new ArgumentNullException("rootUrl", "rootUrl is null.");
            }

            return IsFederated
                ? (DontForceHttps ? rootUrl.Replace("sharing/rest/", "").Replace("sharing/", "") + "sharing/rest/" : rootUrl.Replace("http://", "https://").Replace("sharing/rest/", "").Replace("sharing/", "") + "sharing/rest/") + RelativeUrl.Replace("tokens/", "")
                : (DontForceHttps ? rootUrl : rootUrl.Replace("http://", "https://")) + RelativeUrl;
        }
    }

    /// <summary>
    /// Request for generating a token from the hosted OAuth provider on ArcGIS Online
    /// that can be used to access credit-based ArcGIS Online services
    /// </summary>
    [DataContract]
    public class GenerateOAuthToken : CommonParameters
    {
        public GenerateOAuthToken(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            ExpirationInMinutes = 120;
        }

        /// <summary>
        /// The Client Id from your API access section of your application from developers.arcgis.com
        /// </summary>
        [DataMember(Name = "client_id")]
        public string ClientId { get; private set; }

        /// <summary>
        /// The Client Secret from your API access section of your application from developers.arcgis.com
        /// </summary>
        [DataMember(Name = "client_secret")]
        public string ClientSecret { get; private set; }

        [DataMember(Name = "grant_type")]
        public virtual string Type { get { return "client_credentials"; } }

        /// <summary>
        /// The token expiration time in minutes.
        /// </summary>
        /// <remarks> The default is 120 minutes. Maximum value is 14 days (20160 minutes)</remarks>
        [DataMember(Name = "expiration")]
        public int ExpirationInMinutes { get; set; }
    }

    [DataContract]
    public class GenerateFederatedToken : CommonParameters
    {
        public GenerateFederatedToken(string serverUrl, ITokenProvider tokenProvider)
        {
            FederatedServerUrl = serverUrl;
            TokenProvider = tokenProvider;
        }

        /// <summary>
        /// URL of a federated server for which a server-token needs to be generated.
        /// A server-token will be returned only if the serverUrl contains the URL of a server that is registered with the portal.
        /// A server-token will not be generated for a server that is not registered with the portal.
        /// </summary>
        [DataMember(Name = "serverUrl")]
        public string FederatedServerUrl { get; protected set; }

        [DataMember(Name = "request")]
        public string Request { get { return "getToken"; } }

        [DataMember(Name = "token")]
        public string TokenValue { get { return FederatedToken.Value; } }

        [DataMember(Name = "referer")]
        public string Referer { get; set; }

        [IgnoreDataMember]
        public Token FederatedToken { get; set; }

        /// <summary>
        /// Set this to true to prevent the BuildAbsoluteUrl returning https as the default scheme
        /// </summary>
        [IgnoreDataMember]
        public bool DontForceHttps { get; set; }

        [IgnoreDataMember]
        public ITokenProvider TokenProvider { get; protected set; }

        public string RelativeUrl
        {
            get { return Operations.GenerateToken; }
        }

        public string BuildAbsoluteUrl(string rootUrl)
        {
            if (string.IsNullOrWhiteSpace(rootUrl))
            {
                throw new ArgumentNullException("rootUrl", "rootUrl is null.");
            }

            return (DontForceHttps ?
                rootUrl.Replace("sharing/rest/", "").Replace("sharing/", "") + "sharing/rest/" :
                rootUrl.Replace("http://", "https://").Replace("sharing/rest/", "").Replace("sharing/", "") + "sharing/rest/") + RelativeUrl;
        }
    }

    [DataContract]
    public class OAuthToken : PortalResponse
    {
        [DataMember(Name = "access_token")]
        public string Value { get; set; }

        /// <summary>
        /// The expiration time of the token in seconds.
        /// </summary>
        [DataMember(Name = "expires_in")]
        public long Expiry { get; set; }

        [DataMember(Name = "error")]
        public new ArcGISErrorDetail Error { get; set; }

        public Token AsToken()
        {
            return new Token
            {
                Value = Value,
                Error = Error,
                AlwaysUseSsl = true,
                Expiry = DateTime.UtcNow.AddSeconds(Expiry).ToUnixTime()
            };
        }
    }

    /// <summary>
    /// Represents a token object with a value that can be used to access secure resources
    /// </summary>
    [DataContract]
    public class Token : PortalResponse
    {
        [DataMember(Name = "token")]
        public string Value { get; set; }

        /// <summary>
        /// The expiration time of the token in milliseconds since Jan 1st, 1970.
        /// </summary>
        [DataMember(Name = "expires")]
        public long Expiry { get; set; }

        /// <summary>
        /// If we have a token value then check if it has expired
        /// </summary>
        [IgnoreDataMember]
        public bool IsExpired
        {
            get { return !string.IsNullOrWhiteSpace(Value) && Expiry > 0 && DateTime.Compare(Expiry.FromUnixTime(), DateTime.UtcNow) < 1; }
        }

        /// <summary>
        /// Local date and time when the token will expire
        /// </summary>
        [IgnoreDataMember]
        public DateTime ExpiresAt
        {
            get { return (string.IsNullOrWhiteSpace(Value) || Expiry == 0) ? DateTime.Now : Expiry.FromUnixTime().ToLocalTime(); }
        }

        [IgnoreDataMember]
        public string Referer { get; set; }

        /// <summary>
        /// True if the token must always pass over ssl.
        /// </summary>
        [DataMember(Name = "ssl")]
        public bool AlwaysUseSsl { get; set; }

        [DataMember(Name = "error")]
        public new ArcGISErrorDetail Error { get; set; }
    }

    [DataContract]
    public class ArcGISErrorDetail
    {
        [DataMember(Name = "code")]
        public int Code { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "details")]
        public string Details { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        public override string ToString()
        {
            return string.Format("Code {0}: {1}.{2}\n{3}", Code, Message, Description, Details == null ? "" : string.Join(" ", Details));
        }
    }
}
