using Anywhere.ArcGIS.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Anywhere.ArcGIS.Operation
{
    [DataContract]
    public class LayerFeature : ArcGISServerOperation
    {
        public LayerFeature(ArcGISServerEndpoint endpoint, long featureId, Action beforeRequest = null, Action afterRequest = null)
            : base($"{endpoint.RelativeUrl.Trim('/')}/{featureId}", beforeRequest, afterRequest)
        { }

        /// <summary>
        /// If true, Z values will be included in the results if the features have Z values. Otherwise, Z values are not returned.
        /// </summary>
        /// <remarks>Default is false.</remarks>
        [DataMember(Name = "returnZ")]
        public bool ReturnZ { get; set; }

        /// <summary>
        /// If true, M values will be included in the results if the features have M values. Otherwise, M values are not returned.
        /// </summary>
        /// <remarks>Default is false.</remarks>
        [DataMember(Name = "returnM")]
        public bool ReturnM { get; set; }

        /// <summary>
        /// GeoDatabase version to query. 
        /// This parameter applies only if hasVersionedData property of the service and isDataVersioned property of the layer(s) queried are true. 
        /// If this is not specified, query will apply to published map's version.
        /// </summary>
        [DataMember(Name = "gdbVersion")]
        public string GeodatabaseVersion { get; set; }
    }

    [DataContract]
    public class LayerFeatureResponse<T> : PortalResponse
        where T : IGeometry
    {
        [DataMember(Name = "feature")]
        public Feature<T> Feature { get; set; }
    }
}
