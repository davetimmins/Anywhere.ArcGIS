namespace Anywhere.ArcGIS.Operation
{
    using Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Request for the details of an ArcGIS Server service layer
    /// </summary>
    [DataContract]
    public class ServiceLayerDescription : ArcGISServerOperation
    {
        /// <summary>
        /// Request for the details of an ArcGIS Server service layer
        /// </summary>
        /// <param name="serviceEndpoint"></param>
        public ServiceLayerDescription(IEndpoint serviceEndpoint, Action beforeRequest = null, Action afterRequest = null)
            : base(serviceEndpoint, beforeRequest, afterRequest)
        { }
    }

    [DataContract]
    public class ServiceLayerDescriptionResponse : PortalResponse
    {
        [DataMember(Name = "currentVersion")]
        public double CurrentVersion { get; set; }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [IgnoreDataMember]
        public bool IsGroupLayer { get { return string.Equals(Type, "group layer", StringComparison.CurrentCultureIgnoreCase); } }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "definitionExpression")]
        public string DefinitionExpression { get; set; }

        [DataMember(Name = "geometryType")]
        public string GeometryTypeString { get; set; }

        [IgnoreDataMember]
        public Type GeometryType { get { return GeometryTypes.ToTypeMap[GeometryTypeString](); } }

        [DataMember(Name = "copyrightText")]
        public string CopyrightText { get; set; }

        [DataMember(Name = "parentLayer")]
        public RelatedLayer ParentLayer { get; set; }

        [DataMember(Name = "subLayers")]
        public List<RelatedLayer> SubLayers { get; set; }

        [DataMember(Name = "minScale")]
        public int MinimumScale { get; set; }

        [DataMember(Name = "maxScale")]
        public int MaximumScale { get; set; }

        [DataMember(Name = "defaultVisibility")]
        public bool DefaultVisibility { get; set; }

        [DataMember(Name = "extent")]
        public Extent Extent { get; set; }

        [DataMember(Name = "timeInfo")]
        public LayerTimeInfo TimeInfo { get; set; }

        [DataMember(Name = "hasAttachments")]
        public bool HasAttachments { get; set; }

        [DataMember(Name = "htmlPopupType")]
        public string HtmlPopupType { get; set; }

        [DataMember(Name = "displayField")]
        public string DisplayField { get; set; }

        [DataMember(Name = "canModifyLayer")]
        public bool? CanModifyLayer { get; set; }

        [DataMember(Name = "canScaleSymbols")]
        public bool? CanScaleSymbols { get; set; }

        [DataMember(Name = "hasLabels")]
        public bool? HasLabels { get; set; }

        [DataMember(Name = "capabilities")]
        public string CapabilitiesValue { get; set; }

        [IgnoreDataMember]
        public List<string> Capabilities { get { return string.IsNullOrWhiteSpace(CapabilitiesValue) ? null : CapabilitiesValue.Split(',').ToList(); } }

        [DataMember(Name = "maxRecordCount")]
        public int MaximumRecordCount { get; set; }

        [DataMember(Name = "supportsStatistics")]
        public bool? SupportsStatistics { get; set; }

        [DataMember(Name = "supportsAdvancedQueries")]
        public bool? SupportsAdvancedQueries { get; set; }

        [DataMember(Name = "supportsValidateSQL")]
        public bool? SupportsValidateSQL { get; set; }

        [DataMember(Name = "supportedQueryFormats")]
        public string SupportedQueryFormatsValue { get; set; }

        [IgnoreDataMember]
        public List<string> SupportedQueryFormats { get { return string.IsNullOrWhiteSpace(SupportedQueryFormatsValue) ? null : SupportedQueryFormatsValue.Split(',').ToList(); } }

        [DataMember(Name = "isDataVersioned")]
        public bool? IsDataVersioned { get; set; }

        [DataMember(Name = "fields")]
        public IEnumerable<Field> Fields { get; set; }

        [DataMember(Name = "advancedQueryCapabilities")]
        public AdvancedQueryCapabilities AdvancedQueryCapabilities { get; set; }

        [DataMember(Name = "useStandardizedQueries")]
        public bool? UseStandardizedQueries { get; set; }

        [DataMember(Name = "hasZ")]
        public bool? HasZ { get; set; }

        [DataMember(Name = "hasM")]
        public bool? HasM { get; set; }

        [DataMember(Name = "allowGeometryUpdates")]
        public bool? AllowGeometryUpdates { get; set; }

        [DataMember(Name = "supportsCalculate")]
        public bool? SupportsCalculate { get; set; }

        [DataMember(Name = "supportsAttachmentsByUploadId")]
        public bool? SupportsAttachmentsByUploadId { get; set; }

        [DataMember(Name = "supportsApplyEditsWithGlobalIds")]
        public bool? SupportsApplyEditsWithGlobalIds { get; set; }

        [DataMember(Name = "supportsRollbackOnFailures")]
        public bool? SupportsRollbackOnFailures { get; set; }

        [DataMember(Name = "objectIdField")]
        public string ObjectIdField { get; set; }

        [DataMember(Name = "globalIdField")]
        public string GlobalIdField { get; set; }

        [DataMember(Name = "typeIdField")]
        public string TypeIdField { get; set; }
    }

    [DataContract]
    public class RelatedLayer
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    [DataContract]
    public class AdvancedQueryCapabilities
    {
        [DataMember(Name = "supportsStatistics")]
        public bool SupportsStatistics { get; set; }

        [DataMember(Name = "supportsOrderBy")]
        public bool SupportsOrderBy { get; set; }

        [DataMember(Name = "supportsDistinct")]
        public bool SupportsDistinct { get; set; }

        [DataMember(Name = "supportsPagination")]
        public bool SupportsPagination { get; set; }

        [DataMember(Name = "supportsTrueCurve")]
        public bool SupportsTrueCurve { get; set; }

        [DataMember(Name = "supportsReturningQueryExtent")]
        public bool SupportsReturningQueryExtent { get; set; }

        [DataMember(Name = "supportsQueryWithDistance")]
        public bool SupportsQueryWithDistance { get; set; }
    }

    [DataContract]
    public class LayerTimeInfo
    {
        [DataMember(Name = "startTimeField")]
        public string StartTimeField { get; set; }

        [DataMember(Name = "endTimeField")]
        public string EndTimeField { get; set; }

        [DataMember(Name = "trackIdField")]
        public string TrackIdField { get; set; }

        [DataMember(Name = "timeExtent")]
        public List<long> TimeExtent { get; set; }

        [DataMember(Name = "timeReference")]
        public TimeReference TimeReference { get; set; }

        [DataMember(Name = "timeInterval")]
        public int TimeInterval { get; set; }

        [DataMember(Name = "timeIntervalUnits")]
        public string TimeIntervalUnits { get; set; }

        [DataMember(Name = "exportOptions")]
        public ExportOptions ExportOptions { get; set; }

        [DataMember(Name = "hasLiveData")]
        public bool HasLiveData { get; set; }
    }

    [DataContract]
    public class ExportOptions
    {
        [DataMember(Name = "useTime")]
        public bool UseTime { get; set; }

        [DataMember(Name = "timeDataCumulative")]
        public bool TimeDataCumulative { get; set; }

        [DataMember(Name = "timeOffset")]
        public int TimeOffset { get; set; }

        [DataMember(Name = "timeOffsetUnits")]
        public string TimeOffsetUnits { get; set; }
    }

    [DataContract]
    public class TimeReference
    {
        [DataMember(Name = "timeZone")]
        public string TimeZone { get; set; }

        [DataMember(Name = "respectsDaylightSaving")]
        public bool RespectsDaylightSaving { get; set; }
    }
}
