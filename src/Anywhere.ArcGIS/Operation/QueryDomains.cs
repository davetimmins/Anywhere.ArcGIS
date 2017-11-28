using Anywhere.ArcGIS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Anywhere.ArcGIS.Operation
{
    [DataContract]
    public class QueryDomains : ArcGISServerOperation
    {
        public QueryDomains(string relativeUrl, Action beforeRequest = null, Action afterRequest = null)
            : this(relativeUrl.AsEndpoint(), beforeRequest, afterRequest)
        { }

        public QueryDomains(ArcGISServerEndpoint endpoint, Action beforeRequest = null, Action afterRequest = null)
            : base(endpoint.RelativeUrl.Trim('/') + "/" + Operations.QueryDomains, beforeRequest, afterRequest)
        {
            LayerIdsToSearch = new List<int>();
        }

        /// <summary>
        /// The layer IDs to be queried. 
        /// The set of domains to return is based on the domains referenced by these layers.
        /// </summary>
        [IgnoreDataMember]
        public List<int> LayerIdsToSearch { get; set; }

        /// <summary>
        /// The list of object Ids to be queried. This list is a comma delimited list of field names.
        /// </summary>
        [DataMember(Name = "layers")]
        public int[] LayerIdsToSearchValue { get { return LayerIdsToSearch == null || !LayerIdsToSearch.Any() ? null : LayerIdsToSearch.ToArray(); } }
    }

    [DataContract]
    public class QueryDomainsResponse : PortalResponse
    {
        [DataMember(Name = "domains")]
        public List<Domain> Domains { get; set; }
    }

    [DataContract]
    public class Domain
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "fieldType")]
        public string FieldType { get; set; }

        [DataMember(Name = "range")]
        public List<double> Range { get; set; }

        [DataMember(Name = "mergePolicy")]
        public string MergePolicy { get; set; }

        [DataMember(Name = "splitPolicy")]
        public string SplitPolicy { get; set; }

        [DataMember(Name = "codedValues")]
        public List<CodedValue> CodedValues { get; set; }
    }

    [DataContract]
    public class CodedValue
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "code")]
        public object Value { get; set; }
    }
}
