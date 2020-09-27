namespace Anywhere.ArcGIS.Operation
{
    using Common;
    using System.Runtime.Serialization;
    using System;

    /// <summary>
    /// Represents a request for a query against the info route for a server
    /// </summary>
    [DataContract]
    public class ServerInfo : ArcGISServerOperation
    {
        public ServerInfo(Action beforeRequest = null, Action<string> afterRequest = null)
            : base(new RootServerEndpoint(Operations.ServerInfoRoute), beforeRequest, afterRequest)
        { }   
    }

    [DataContract]
    public class ServerInfoResponse : PortalResponse
    {
        [DataMember(Name = "currentVersion")]
        public double CurrentVersion { get; set; }

        [DataMember(Name = "fullVersion")]
        public string FullVersion { get; set; }

        [DataMember(Name = "soapUrl")]
        public string SoapUrl { get; set; }

        [DataMember(Name = "secureSoapUrl")]
        public string SecureSoapUrl { get; set; }

        [DataMember(Name = "owningSystemUrl")]
        public string OwningSystemUrl { get; set; }

        [DataMember(Name = "owningTenant")]
        public string OwningTenant { get; set; }

        [DataMember(Name = "authInfo")]
        public AuthInfo AuthenticationInfo { get; set; }
    }

    [DataContract]
    public class AuthInfo
    {
        [DataMember(Name = "isTokenBasedSecurity")]
        public bool TokenBasedSecurity { get; set; }

        [DataMember(Name = "tokenServicesUrl")]
        public string TokenServicesUrl { get; set; }

        [DataMember(Name = "shortLivedTokenValidity")]
        public int ShortLivedTokenValidity { get; set; }
    }

    /// <summary>
    /// The health check reports if the responding machine in the ArcGIS Server site is able to receive and process requests.
    /// For example, during site creation, this URL reports the site is unhealthy because it can't take requests at that time.
    /// This endpoint is useful if you're setting up a third-party load balancer or other monitoring software that supports a health check function.
    /// </summary>
    [DataContract]
    public class HealthCheck : ArcGISServerOperation
    {
        public HealthCheck(Action beforeRequest = null, Action<string> afterRequest = null)
            : base(new RootServerEndpoint(Operations.ServerHealthCheckRoute), beforeRequest, afterRequest)
        { }
    }

    [DataContract]
    public class HealthCheckResponse : PortalResponse
    {
        [DataMember(Name = "success")]
        public bool IsHealthy { get; set; }
    }
}
