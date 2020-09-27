using Anywhere.ArcGIS.Common;
using System;
using System.Runtime.Serialization;

namespace Anywhere.ArcGIS.Operation.Admin
{
    [DataContract]
    public class StartService : ArcGISServerOperation
    {
        public StartService(ServiceDescription serviceDescription, Action beforeRequest = null, Action<string> afterRequest = null)
            : base(new ArcGISServerAdminEndpoint(string.Format(Operations.StartService, serviceDescription.Name, serviceDescription.Type)), beforeRequest, afterRequest)
        { }
    }

    [DataContract]
    public class StopService : ArcGISServerOperation
    {
        public StopService(ServiceDescription serviceDescription, Action beforeRequest = null, Action<string> afterRequest = null)
            : base(new ArcGISServerAdminEndpoint(string.Format(Operations.StopService, serviceDescription.Name, serviceDescription.Type)), beforeRequest, afterRequest)
        { }
    }

    [DataContract]
    public class StartStopServiceResponse : PortalResponse
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }
    }
}
