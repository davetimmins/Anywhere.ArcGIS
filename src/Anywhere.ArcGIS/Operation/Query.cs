using Anywhere.ArcGIS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Anywhere.ArcGIS.Operation
{
    /// <summary>
    /// Basic query request operation
    /// </summary>
    [DataContract]
    public class Query : ArcGISServerOperation
    {
        public Query(string relativeUrl, Action beforeRequest = null, Action afterRequest = null)
            : this(relativeUrl.AsEndpoint(), beforeRequest, afterRequest)
        { }

        /// <summary>
        /// Represents a request for a query against a service resource
        /// </summary>
        /// <param name="endpoint">Resource to apply the query against</param>
        public Query(ArcGISServerEndpoint endpoint, Action beforeRequest = null, Action afterRequest = null)
            : base(endpoint.RelativeUrl.Trim('/') + "/" + Operations.Query, beforeRequest, afterRequest)
        {
            Where = "1=1";
            OutFields = new List<string>();
            ReturnGeometry = true;
            SpatialRelationship = SpatialRelationshipTypes.Intersects;
        }

        /// <summary>
        /// A where clause for the query filter. Any legal SQL where clause operating on the fields in the layer is allowed.
        /// </summary>
        /// <remarks>Default is '1=1'</remarks>
        [DataMember(Name = "where")]
        public string Where { get; set; }

        /// <summary>
        /// The geometry to apply as the spatial filter.
        /// The structure of the geometry is the same as the structure of the json geometry objects returned by the ArcGIS REST API.
        /// </summary>
        /// <remarks>Default is empty</remarks>
        [DataMember(Name = "geometry")]
        public IGeometry Geometry { get; set; }

        /// <summary>
        /// The spatial reference of the input geometry.
        /// </summary>
        [DataMember(Name = "inSR")]
        public SpatialReference InputSpatialReference
        {
            get { return Geometry == null ? null : Geometry.SpatialReference ?? null; }
        }

        /// <summary>
        /// The type of geometry specified by the geometry parameter.
        /// The geometry type can be an envelope, point, line, or polygon.
        /// The default geometry type is "esriGeometryEnvelope".
        /// Values: esriGeometryPoint | esriGeometryMultipoint | esriGeometryPolyline | esriGeometryPolygon | esriGeometryEnvelope
        /// </summary>
        /// <remarks>Default is esriGeometryEnvelope</remarks>
        [DataMember(Name = "geometryType")]
        public string GeometryType
        {
            get
            {
                return Geometry == null
                    ? GeometryTypes.Envelope
                    : GeometryTypes.TypeMap[Geometry.GetType()]();
            }
        }

        /// <summary>
        ///  The names of the fields to search.
        /// </summary>
        [IgnoreDataMember]
        public List<string> OutFields { get; set; }

        /// <summary>
        /// The list of fields to be included in the returned resultset. This list is a comma delimited list of field names.
        /// If you specify the shape field in the list of return fields, it is ignored. To request geometry, set returnGeometry to true.
        /// </summary>
        /// <remarks>Default is '*' (all fields)</remarks>
        [DataMember(Name = "outFields")]
        public string OutFieldsValue { get { return OutFields == null || !OutFields.Any() ? "*" : string.Join(",", OutFields); } }

        /// <summary>
        ///  The object IDs of this layer/table to be queried.
        /// </summary>
        [IgnoreDataMember]
        public List<long> ObjectIds { get; set; }

        /// <summary>
        /// The list of object Ids to be queried. This list is a comma delimited list of field names.
        /// </summary>
        [DataMember(Name = "objectIds")]
        public string ObjectIdsValue { get { return ObjectIds == null || !ObjectIds.Any() ? null : string.Join(",", ObjectIds); } }

        /// <summary>
        /// The spatial reference of the returned geometry.
        /// If not specified, the geometry is returned in the spatial reference of the input.
        /// </summary>
        [DataMember(Name = "outSR")]
        public SpatialReference OutputSpatialReference { get; set; }

        /// <summary>
        /// The spatial relationship to be applied on the input geometry while performing the query.
        /// The supported spatial relationships include intersects, contains, envelope intersects, within, etc.
        /// The default spatial relationship is "esriSpatialRelIntersects".
        /// Values: esriSpatialRelIntersects | esriSpatialRelContains | esriSpatialRelCrosses | esriSpatialRelEnvelopeIntersects | esriSpatialRelIndexIntersects | esriSpatialRelOverlaps | esriSpatialRelTouches | esriSpatialRelWithin | esriSpatialRelRelation
        /// </summary>
        [DataMember(Name = "spatialRel")]
        public string SpatialRelationship { get; set; }

        /// <summary>
        /// If true, the resultset includes the geometry associated with each result.
        /// </summary>
        /// <remarks>Default is true</remarks>
        [DataMember(Name = "returnGeometry")]
        public bool ReturnGeometry { get; set; }

        [IgnoreDataMember]
        public DateTime? From { get; set; }

        [IgnoreDataMember]
        public DateTime? To { get; set; }

        /// <summary>
        /// The time instant or the time extent to query.
        /// </summary>
        /// <remarks>If no To value is specified we will use the From value again, equivalent of using a time instant.</remarks>
        [DataMember(Name = "time")]
        public string Time
        {
            get
            {
                return (From == null) ? null : string.Format("{0},{1}",
                  From.Value.ToUnixTime(),
                  (To ?? From.Value).ToUnixTime());
            }
        }

        /// <summary>
        /// This option can be used to specify the maximum allowable offset to be used for generalizing geometries returned by the query operation.
        /// </summary>
        [DataMember(Name = "maxAllowableOffset")]
        public int? MaxAllowableOffset { get; set; }

        /// <summary>
        /// This option can be used to specify the number of decimal places in the response geometries returned by the query operation.
        /// This applies to X and Y values only (not m or z values).
        /// </summary>
        [DataMember(Name = "geometryPrecision")]
        public int? GeometryPrecision { get; set; }

        /// <summary>
        /// If true, Z values will be included in the results if the features have Z values. Otherwise, Z values are not returned.
        /// </summary>
        /// <remarks>Default is false. This parameter only applies if returnGeometry=true.</remarks>
        [DataMember(Name = "returnZ")]
        public bool ReturnZ { get; set; }

        /// <summary>
        /// If true, M values will be included in the results if the features have M values. Otherwise, M values are not returned.
        /// </summary>
        /// <remarks>Default is false. This parameter only applies if returnGeometry=true.</remarks>
        [DataMember(Name = "returnM")]
        public bool ReturnM { get; set; }

        /// <summary>
        /// GeoDatabase version to query.
        /// </summary>
        [DataMember(Name = "gdbVersion")]
        public string GeodatabaseVersion { get; set; }

        /// <summary>
        /// If true, returns distinct values based on the fields specified in outFields.
        /// This parameter applies only if supportsAdvancedQueries property of the layer is true.
        [DataMember(Name = "returnDistinctValues")]
        public bool ReturnDistinctValues { get; set; }

        /// <summary>
        ///  The names of the fields to order by.
        /// </summary>
        [IgnoreDataMember]
        public List<string> OrderBy { get; set; }

        /// <summary>
        /// One or more field names on which the features/records need to be ordered.
        /// Use ASC or DESC for ascending or descending, respectively, following every field to control the ordering.
        /// Defaults to ASC (ascending order) if <ORDER> is unspecified.
        /// </summary>
        /// <remarks>Default is '*' (all fields)</remarks>
        [DataMember(Name = "orderByFields")]
        public string OrderByValue { get { return OrderBy == null || !OrderBy.Any() ? null : string.Join(",", OrderBy); } }

        /// <summary>
        /// This option was added at 10.3.
        /// Description: This option can be used for fetching query results by skipping the specified number of records and
        /// starts from the next record (i.e., resultOffset + 1th). The default is 0.
        /// This parameter only applies if supportsPagination is true.
        /// You can use this option to fetch records that are beyond maxRecordCount.
        /// For example, if maxRecordCountis 1000, you can get the next 100 records by setting resultOffset=1000
        /// and resultRecordCount = 100, query results can return the results in the range of 1001 to 1100.
        /// </summary>
        [DataMember(Name = "resultOffset")]
        public int? ResultOffset { get; set; }

        /// <summary>
        /// This option was added at 10.3.
        /// Description: This option can be used for fetching query results up to the resultRecordCount specified.
        /// When resultOffset is specified but this parameter is not, map service defaults it to maxRecordCount.
        /// The maximum value for this parameter is the value of the layer's maxRecordCount property.
        /// This parameter only applies if supportsPagination is true.
        /// Example: resultRecordCount=10 to fetch up to 10 records
        /// </summary>
        [DataMember(Name = "resultRecordCount")]
        public int? ResultRecordCount { get; set; }

        /// <summary>
        ///  The names of the fields to group by for output statistics.
        /// </summary>
        [IgnoreDataMember]
        public List<string> GroupByFields { get; set; }

        /// <summary>
        /// The list of fields to be included in the group by clause. This list is a comma delimited list of field names.
        /// </summary>
        /// <remarks>Default is ''</remarks>
        [DataMember(Name = "groupByFieldsForStatistics")]
        public string GroupByFieldsValue { get { return GroupByFields == null || !GroupByFields.Any() ? "" : string.Join(",", GroupByFields); } }


        /// <summary>
        /// The list of output statistics.
        /// </summary>
        [DataMember(Name = "outStatistics")]
        public List<OutputStatistic> OutputStatistics { get; set; }
    }

    [DataContract]
    public class QueryResponse<T> : PortalResponse
        where T : IGeometry
    {
        public QueryResponse()
        {
            FieldAliases = new Dictionary<string, string>();
        }

        [DataMember(Name = "displayFieldName")]
        public string DisplayFieldName { get; set; }

        [DataMember(Name = "objectIdFieldName")]
        public string ObjectIdFieldName { get; set; }

        [DataMember(Name = "globalIdFieldName")]
        public string GlobalIdFieldName { get; set; }

        [DataMember(Name = "geometryType")]
        public string GeometryTypeString { get; set; }

        [IgnoreDataMember]
        public Type GeometryType { get { return GeometryTypes.ToTypeMap[GeometryTypeString](); } }

        [DataMember(Name = "features")]
        public IEnumerable<Feature<T>> Features { get; set; }

        [DataMember(Name = "spatialReference")]
        public SpatialReference SpatialReference { get; set; }

        [DataMember(Name = "fieldAliases")]
        public Dictionary<string, string> FieldAliases { get; set; }

        [DataMember(Name = "fields")]
        public IEnumerable<Field> Fields { get; set; }

        [DataMember(Name = "exceededTransferLimit")]
        public bool? ExceededTransferLimit { get; set; }

        [IgnoreDataMember]
        public string DisplayFieldNameAlias
        {
            get
            {
                return string.IsNullOrWhiteSpace(DisplayFieldName) || FieldAliases == null || !FieldAliases.Any() || !FieldAliases.ContainsKey(DisplayFieldName)
                    ? string.Empty
                    : FieldAliases[DisplayFieldName];
            }
        }
    }

    [DataContract]
    public class Field
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "alias")]
        public string Alias { get; set; }

        [DataMember(Name = "length")]
        public int? Length { get; set; }

        [DataMember(Name = "sqlType")]
        public string SqlType { get; set; }

        [DataMember(Name = "domain")]
        public string Domain { get; set; }

        [DataMember(Name = "defaultValue")]
        public string DefaultValue { get; set; }
    }

    /// <summary>
    /// Perform a query that only returns the ObjectIds for the results
    /// </summary>
    [DataContract]
    public class QueryForIds : Query
    {
        public QueryForIds(string relativeUrl, Action beforeRequest = null, Action afterRequest = null)
            : this(relativeUrl.AsEndpoint(), beforeRequest, afterRequest)
        { }

        // TODO : make these work with or without input geometry
        public QueryForIds(ArcGISServerEndpoint endpoint, Action beforeRequest = null, Action afterRequest = null)
            : base(endpoint, beforeRequest, afterRequest)
        {
            ReturnGeometry = false;
        }

        [DataMember(Name = "returnIdsOnly")]
        public bool ReturnIdsOnly { get { return true; } }
    }

    [DataContract]
    public class QueryForIdsResponse : PortalResponse
    {
        [DataMember(Name = "objectIdFieldName")]
        public string ObjectIdFieldName { get; set; }

        [DataMember(Name = "objectIds")]
        public long[] ObjectIds { get; set; }
    }

    /// <summary>
    /// Perform a query that only returns a count of the results
    /// </summary>
    [DataContract]
    public class QueryForCount : Query
    {
        public QueryForCount(string relativeUrl, Action beforeRequest = null, Action afterRequest = null)
            : this(relativeUrl.AsEndpoint(), beforeRequest, afterRequest)
        { }

        public QueryForCount(ArcGISServerEndpoint endpoint, Action beforeRequest = null, Action afterRequest = null)
            : base(endpoint, beforeRequest, afterRequest)
        {
            ReturnGeometry = false;
        }

        [DataMember(Name = "returnCountOnly")]
        public bool ReturnCountOnly { get { return true; } }
    }

    [DataContract]
    public class QueryForCountResponse : PortalResponse
    {
        [DataMember(Name = "count")]
        public int NumberOfResults { get; set; }
    }

    /// <summary>
    /// Perform a query that returns the count of features and the bounding extent
    /// </summary>
    [DataContract]
    public class QueryForExtent : QueryForCount
    {
        public QueryForExtent(string relativeUrl, Action beforeRequest = null, Action afterRequest = null)
            : this(relativeUrl.AsEndpoint(), beforeRequest, afterRequest)
        { }

        public QueryForExtent(ArcGISServerEndpoint endpoint, Action beforeRequest = null, Action afterRequest = null)
            : base(endpoint, beforeRequest, afterRequest)
        { }

        [DataMember(Name = "returnExtentOnly")]
        public bool ReturnExtentOnly { get { return true; } }
    }

    [DataContract]
    public class QueryForExtentResponse : QueryForCountResponse
    {
        [DataMember(Name = "extent")]
        public Extent Extent { get; set; }
    }

    public static class GeometryTypes
    {
        public readonly static Dictionary<Type, Func<string>> TypeMap = new Dictionary<Type, Func<string>>
        {
            { typeof(Point), () => GeometryTypes.Point },
            { typeof(MultiPoint), () => GeometryTypes.MultiPoint },
            { typeof(Extent), () => GeometryTypes.Envelope },
            { typeof(Polygon), () => GeometryTypes.Polygon },
            { typeof(Polyline), () => GeometryTypes.Polyline }
        };

        public readonly static Dictionary<string, Func<Type>> ToTypeMap = new Dictionary<string, Func<Type>>
        {
            { GeometryTypes.Point, () => typeof(Point) },
            { GeometryTypes.MultiPoint, () => typeof(MultiPoint) },
            { GeometryTypes.Envelope, () => typeof(Extent) },
            { GeometryTypes.Polygon, () => typeof(Polygon) },
            { GeometryTypes.Polyline, () => typeof(Polyline) }
        };

        public const string Point = "esriGeometryPoint";
        public const string MultiPoint = "esriGeometryMultipoint";
        public const string Polyline = "esriGeometryPolyline";
        public const string Polygon = "esriGeometryPolygon";
        public const string Envelope = "esriGeometryEnvelope";
    }

    public static class SpatialRelationshipTypes
    {
        public const string Intersects = "esriSpatialRelIntersects";
        public const string Contains = "esriSpatialRelContains";
        public const string Crosses = "esriSpatialRelCrosses";
        public const string EnvelopeIntersects = "esriSpatialRelEnvelopeIntersects";
        public const string IndexIntersects = "esriSpatialRelIndexIntersects";
        public const string Overlaps = "esriSpatialRelOverlaps";
        public const string Touches = "esriSpatialRelTouches";
        public const string Within = "esriSpatialRelWithin";
        public const string Relation = "esriSpatialRelRelation";
    }

    public static class FieldDataTypes
    {
        public readonly static Dictionary<Type, Func<string>> FieldDataTypeMap = new Dictionary<Type, Func<string>>
        {
            { typeof(string), () => FieldDataTypes.EsriString },
            { typeof(int), () => FieldDataTypes.EsriInteger },
            { typeof(short), () => FieldDataTypes.EsriInteger },
            { typeof(long), () => FieldDataTypes.EsriInteger },
            { typeof(decimal), () => FieldDataTypes.EsriDouble },
            { typeof(double), () => FieldDataTypes.EsriDouble },
            { typeof(float), () => FieldDataTypes.EsriDouble },
            { typeof(DateTime), () => FieldDataTypes.EsriDate },
            { typeof(bool), () => FieldDataTypes.EsriString }
        };

        public const string EsriString = "esriFieldTypeString";
        public const string EsriInteger = "esriFieldTypeInteger";
        public const string EsriDouble = "esriFieldTypeDouble";
        public const string EsriDate = "esriFieldTypeDate";
        public const string EsriOID = "esriFieldTypeOID";
    }

    [DataContract]
    public class OutputStatistic
    {
        [DataMember(Name = "statisticType")]
        public string StatisticType { get; set; }

        [DataMember(Name = "onStatisticField")]
        public string OnField { get; set; }

        [DataMember(Name = "outStatisticFieldName")]
        public string OutField { get; set; }
    }

    public static class StatisticTypes
    {
        public const string Count = "count";
        public const string Sum = "sum";
        public const string Min = "min";
        public const string Max = "max";
        public const string Average = "avg";
        public const string StandardDeviation = "stddev";
        public const string Variance = "var";
    }
}
