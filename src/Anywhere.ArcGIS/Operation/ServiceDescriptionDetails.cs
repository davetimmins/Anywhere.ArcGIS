namespace Anywhere.ArcGIS.Operation
{
    using Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Request for the details of an ArcGIS Server service
    /// </summary>
    [DataContract]
    public class ServiceDescriptionDetails : ArcGISServerOperation
    {
        /// <summary>
        /// Request for the details of an ArcGIS Server service
        /// </summary>
        /// <param name="serviceDescription">A <see cref="ServiceDescription"/> from a previous call to DescribeSite</param>
        public ServiceDescriptionDetails(ServiceDescription serviceDescription, Action beforeRequest = null, Action afterRequest = null)
            : base(serviceDescription.ArcGISServerEndpoint, beforeRequest, afterRequest)
        {
            if (serviceDescription == null)
            {
                throw new ArgumentNullException(nameof(serviceDescription));
            }
        }

        /// <summary>
        /// Request for the details of an ArcGIS Server service
        /// </summary>
        /// <param name="serviceEndpoint"></param>
        public ServiceDescriptionDetails(IEndpoint serviceEndpoint, Action beforeRequest = null, Action afterRequest = null)
            : base(serviceEndpoint, beforeRequest, afterRequest)
        { }
    }

    /// <summary>
    /// Common base for service details response across different service types (MapServer, FeatureServer etc.)
    /// </summary>
    [DataContract]
    public class ServiceDescriptionDetailsResponse : PortalResponse
    {
        [DataMember(Name = "currentVersion")]
        public double CurrentVersion { get; set; }

        [DataMember(Name = "serviceDescription")]
        public string ServiceDescription { get; set; }

        [DataMember(Name = "supportedQueryFormats")]
        public string SupportedQueryFormatsValue { get; set; }

        [IgnoreDataMember]
        public List<string> SupportedQueryFormats { get { return string.IsNullOrWhiteSpace(SupportedQueryFormatsValue) ? null : SupportedQueryFormatsValue.Split(',').ToList(); } }

        [DataMember(Name = "minScale")]
        public double? MinimumScale { get; set; }

        [DataMember(Name = "maxScale")]
        public double? MaximumScale { get; set; }

        [DataMember(Name = "maxRecordCount")]
        public int? MaximumRecordCount { get; set; }

        [DataMember(Name = "capabilities")]
        public string CapabilitiesValue { get; set; }

        [IgnoreDataMember]
        public List<string> Capabilities { get { return string.IsNullOrWhiteSpace(CapabilitiesValue) ? null : CapabilitiesValue.Split(',').ToList(); } }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "copyrightText")]
        public string CopyrightText { get; set; }

        [DataMember(Name = "spatialReference")]
        public SpatialReference SpatialReference { get; set; }

        [DataMember(Name = "singleFusedMapCache")]
        public bool? SingleFusedMapCache { get; set; }

        [DataMember(Name = "tileInfo")]
        public TileInfo TileInfo { get; set; }

        [DataMember(Name = "initialExtent")]
        public Extent InitialExtent { get; set; }

        [DataMember(Name = "fullExtent")]
        public Extent FullExtent { get; set; }

        [DataMember(Name = "timeInfo")]
        public TimeInfo TimeInfo { get; set; }

        [DataMember(Name = "documentInfo")]
        public DocumentInfo DocumentInfo { get; set; }

        [DataMember(Name = "layers")]
        public List<LayerDetails> Layers { get; set; }

        [DataMember(Name = "tables")]
        public List<TableDetails> Tables { get; set; }
    }

    [DataContract]
    public class TimeInfo
    {
        [DataMember(Name = "timeExtent")]
        public List<long> TimeExtent { get; set; }

        [DataMember(Name = "timeRelation")]
        public string TimeRelation { get; set; }

        [DataMember(Name = "defaultTimeInterval")]
        public int DefaultTimeInterval { get; set; }

        [DataMember(Name = "defaultTimeIntervalUnits")]
        public string DefaultTimeIntervalUnits { get; set; }

        [DataMember(Name = "defaultTimeWindow")]
        public double DefaultTimeWindow { get; set; }

        [DataMember(Name = "defaultTimeWindowUnits")]
        public string DefaultTimeWindowUnits { get; set; }

        [DataMember(Name = "hasLiveData")]
        public bool HasLiveData { get; set; }
    }

    [DataContract]
    public class DocumentInfo
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "author")]
        public string Author { get; set; }

        [DataMember(Name = "comments")]
        public string Comments { get; set; }

        [DataMember(Name = "subject")]
        public string Subject { get; set; }

        [DataMember(Name = "category")]
        public string Category { get; set; }

        [DataMember(Name = "antialiasingMode")]
        public string AntialiasingMode { get; set; }

        [DataMember(Name = "textAntialiasingMode")]
        public string TextAntialiasingMode { get; set; }

        [DataMember(Name = "keywords")]
        public string Keywords { get; set; }
    }

    [DataContract]
    public class LayerDetails
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "parentLayerId")]
        public int? ParentLayerId { get; set; }

        [DataMember(Name = "defaultVisibility")]
        public bool? DefaultVisibility { get; set; }

        [DataMember(Name = "subLayerIds")]
        public IEnumerable<int> SubLayerIds { get; set; }

        [DataMember(Name = "minScale")]
        public double? MinimumScale { get; set; }

        [DataMember(Name = "maxScale")]
        public double? MaximumScale { get; set; }

        [IgnoreDataMember]
        public bool IsGroupLayer { get { return SubLayerIds != null && SubLayerIds.Any() && SubLayerIds.FirstOrDefault() > -1; } }
    }

    [DataContract]
    public class TableDetails
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    [DataContract]
    public class TileInfo
    {
        [DataMember(Name = "rows")]
        public int Rows { get; set; }

        [DataMember(Name = "cols")]
        public int Cols { get; set; }

        [DataMember(Name = "dpi")]
        public int Dpi { get; set; }

        [DataMember(Name = "format")]
        public string Format { get; set; }

        [DataMember(Name = "compressionQuality")]
        public int CompressionQuality { get; set; }

        [DataMember(Name = "origin")]
        public Point Origin { get; set; }

        [DataMember(Name = "spatialReference")]
        public SpatialReference SpatialReference { get; set; }

        [DataMember(Name = "lods")]
        public List<Lod> Lods { get; set; }
    }

    [DataContract]
    public class Lod
    {
        [DataMember(Name = "level")]
        public int Level { get; set; }

        [DataMember(Name = "resolution")]
        public double Resolution { get; set; }

        [DataMember(Name = "scale")]
        public double Scale { get; set; }
    }
}
