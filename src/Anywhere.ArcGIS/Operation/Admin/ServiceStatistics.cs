using Anywhere.ArcGIS.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Anywhere.ArcGIS.Operation.Admin
{
    [DataContract]
    public class ServiceStatistics : ArcGISServerOperation
    {
        public ServiceStatistics(ServiceDescription serviceDescription, Action beforeRequest = null, Action afterRequest = null)
            : base(new ArcGISServerAdminEndpoint(string.Format(Operations.ServiceStatistics, serviceDescription.Name, serviceDescription.Type)), beforeRequest, afterRequest)
        { }
    }

    [DataContract]
    public class ServiceStatisticsResponse : PortalResponse
    {
        [DataMember(Name = "summary")]
        public StatisticsSummary Summary { get; set; }

        [DataMember(Name = "perMachine")]
        public List<MachineStatistics> PerMachine { get; set; }
    }

    [DataContract]
    public class StatisticsSummary
    {
        [DataMember(Name = "folderName")]
        public string FolderName { get; set; }

        [DataMember(Name = "serviceName")]
        public string ServiceName { get; set; }

        [DataMember(Name = "type")]
        public string ServiceType { get; set; }

        [DataMember(Name = "startTime")]
        public string startTime { get; set; }

        [DataMember(Name = "max")]
        public int Max { get; set; }

        [DataMember(Name = "busy")]
        public int Busy { get; set; }

        [DataMember(Name = "free")]
        public int Free { get; set; }

        [DataMember(Name = "initializing")]
        public int Initializing { get; set; }

        [DataMember(Name = "notCreated")]
        public int NotCreated { get; set; }

        [DataMember(Name = "transactions")]
        public int Transactions { get; set; }

        [DataMember(Name = "totalBusyTime")]
        public int TotalBusyTime { get; set; }

        [DataMember(Name = "isStatisticsAvailable")]
        public bool StatisticsAvailable { get; set; }
    }

    [DataContract]
    public class MachineStatistics
    {
        [DataMember(Name = "folderName")]
        public string FolderName { get; set; }

        [DataMember(Name = "serviceName")]
        public string ServiceName { get; set; }

        [DataMember(Name = "type")]
        public string ServiceType { get; set; }

        [DataMember(Name = "machineName")]
        public string MachineName { get; set; }

        [DataMember(Name = "max")]
        public int Max { get; set; }

        [DataMember(Name = "busy")]
        public int Busy { get; set; }

        [DataMember(Name = "free")]
        public int Free { get; set; }

        [DataMember(Name = "initializing")]
        public int Initializing { get; set; }

        [DataMember(Name = "notCreated")]
        public int NotCreated { get; set; }

        [DataMember(Name = "transactions")]
        public int Transactions { get; set; }

        [DataMember(Name = "totalBusyTime")]
        public int TotalBusyTime { get; set; }

        [DataMember(Name = "isStatisticsAvailable")]
        public bool StatisticsAvailable { get; set; }
    }
}
