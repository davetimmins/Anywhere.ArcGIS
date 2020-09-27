using Anywhere.ArcGIS.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Anywhere.ArcGIS.Operation
{
    /// <summary>
    /// Find request operation
    /// </summary>
    [DataContract]
    public class Find : ArcGISServerOperation
    {
        public Find(string relativeUrl, Action beforeRequest = null, Action<string> afterRequest = null)
            : this(relativeUrl.AsEndpoint(), beforeRequest, afterRequest)
        { }

        /// <summary>
        /// Represents a request for a find against a service resource
        /// </summary>
        /// <param name="endpoint">Resource to apply the query against</param>
        public Find(ArcGISServerEndpoint endpoint, Action beforeRequest = null, Action<string> afterRequest = null)
            : base(endpoint.RelativeUrl.Trim('/') + "/" + Operations.Find, beforeRequest, afterRequest)
        {
            FuzzySearch = true;
            ReturnGeometry = true;
            ReturnZ = true;
        }

        /// <summary>
        /// The search string. This is the text that is searched across the layers and fields the user specifies.
        /// </summary>
        [DataMember(Name = "searchText")]
        public string SearchText { get; set; }

        /// <summary>
        /// If false, the operation searches for an exact match of the SearchText string. An exact match is case sensitive.
        /// Otherwise, it searches for a value that contains the searchText provided. This search is not case sensitive
        /// </summary>
        /// <remarks>Default is true</remarks>
        [DataMember(Name = "contains")]
        public bool FuzzySearch { get; set; }

        /// <summary>
        /// If true, the resultset includes the geometry associated with each result.
        /// </summary>
        /// <remarks>Default is true</remarks>
        [DataMember(Name = "returnGeometry")]
        public bool ReturnGeometry { get; set; }

        /// <summary>
        /// The well-known ID of the spatial reference of the output geometries.
        /// </summary>
        [DataMember(Name = "sr")]
        public int? OutputSpatialReferenceValue { get { return OutputSpatialReference?.Wkid; } }

        [IgnoreDataMember]
        public SpatialReference OutputSpatialReference { get; set; }

        /// <summary>
        ///  The names of the fields to search.
        /// </summary>
        [IgnoreDataMember]
        public List<string> SearchFields { get; set; }

        [DataMember(Name = "searchFields")]
        public string SearchFieldsValue { get { return SearchFields == null ? string.Empty : string.Join(",", SearchFields); } }

        /// <summary>
        ///  The layers to perform the find operation on.
        /// </summary>
        [IgnoreDataMember]
        public List<int> LayerIdsToSearch { get; set; }

        [DataMember(Name = "layers")]
        public string LayerIdsToSearchValue { get { return LayerIdsToSearch == null ? string.Empty : string.Join(",", LayerIdsToSearch); } }

        /// <summary>
        /// This option can be used to specify the maximum allowable offset to be used for generalizing geometries returned by the find operation.
        /// </summary>
        [DataMember(Name = "maxAllowableOffset")]
        public int? MaxAllowableOffset { get; set; }

        /// <summary>
        /// This option can be used to specify the number of decimal places in the response geometries returned by the find operation.
        /// This applies to X and Y values only (not m or z values).
        /// </summary>
        [DataMember(Name = "geometryPrecision")]
        public int? GeometryPrecision { get; set; }

        /// <summary>
        /// If true, Z values will be included in the results if the features have Z values. Otherwise, Z values are not returned.
        /// </summary>
        /// <remarks>Default is true. This parameter only applies if returnGeometry=true.</remarks>
        [DataMember(Name = "returnZ")]
        public bool ReturnZ { get; set; }

        /// <summary>
        /// If true, M values will be included in the results if the features have M values. Otherwise, M values are not returned.
        /// </summary>
        /// <remarks>Default is false. This parameter only applies if returnGeometry=true.</remarks>
        [DataMember(Name = "returnM")]
        public bool ReturnM { get; set; }

        /// <summary>
        /// Switch map layers to point to an alternate geodabase version.
        /// </summary>
        [DataMember(Name = "gdbVersion")]
        public string GeodatabaseVersion { get; set; }

        /// <summary>
        /// If true, the values in the result will not be formatted i.e. numbers will returned as is and dates will be returned as epoch values.
        /// This option was added at 10.5.
        /// </summary>
        [DataMember(Name = "returnUnformattedValues")]
        public bool ReturnUnformattedValues { get; set; }

        /// <summary>
        /// If true, field names will be returned instead of field aliases.
        /// This option was added at 10.5.
        /// </summary>
        [DataMember(Name = "returnFieldName")]
        public bool ReturnFieldNames { get; set; }
    }

    [DataContract]
    public class FindResponse : PortalResponse
    {
        [DataMember(Name = "results")]
        public FindResult[] Results { get; set; }
    }

    [DataContract]
    public class FindResult
    {
        [DataMember(Name = "layerId")]
        public int LayerId { get; set; }

        [DataMember(Name = "layerName")]
        public string LayerName { get; set; }

        [DataMember(Name = "displayFieldName")]
        public string DisplayFieldName { get; set; }

        [DataMember(Name = "foundFieldName")]
        public string FoundFieldName { get; set; }

        [DataMember(Name = "value")]
        public string Value { get; set; }

        [DataMember(Name = "attributes")]
        public Dictionary<string, object> Attributes { get; set; }

        [DataMember(Name = "geometryType")]
        public string GeometryType { get; set; }

        [DataMember(Name = "geometry")]
        public object Geometry { get; set; }
    }
}
