namespace Anywhere.ArcGIS.Operation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Anywhere.ArcGIS.Common;
    using Anywhere.ArcGIS.Operation;
    using System.Text.RegularExpressions;
    using System.IO;


    /// <summary>
    /// The createReplica operation is performed on a feature service resource.
    /// This operation creates the replica between the feature service and a client based on a client-supplied replica definition.
    /// It requires the Sync capability.
    /// </summary>
    [DataContract]
    public class CreateReplica : ArcGISServerOperation
    {
        public CreateReplica(string relativeUrl, Action beforeRequest = null, Action afterRequest = null)
            : this(relativeUrl.AsEndpoint(), beforeRequest, afterRequest)
        { }

        public CreateReplica(ArcGISServerEndpoint endpoint, Action beforeRequest = null, Action afterRequest = null)
            : base(endpoint.RelativeUrl.Trim('/') + "/" + Operations.CreateReplica, beforeRequest, afterRequest)
        {
            ReturnAttachments = false;
            TargetType = "client";
            TransportType = "esriTransportTypeEmbedded";
            Layers = new List<int>();
            LayerQueries = new List<LayerQuery>();
            ReturnAttachmentsDataByUrl = false;
            SyncModel = "none";
            AttachmentsSyncDirection = "none";
            DataFormat = "json";
            IsAsync = false;
        }

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
        /// Gets or sets the spatial reference of the input geometry.
        /// If InputGeometrySpatialReference is not specified, the geometry is assumed to be in the spatial reference of the map.
        /// </summary>
        [DataMember(Name = "inSR")]
        public SpatialReference InputGeometrySpatialReference { get; set; }

        /// <summary>
        ///  (Required) The geometry to apply as the spatial filter.
        ///  All the features in layers intersecting this geometry will be replicated
        /// </summary>
        [DataMember(Name = "geometry")]
        public IGeometry Geometry { get; set; }

        /// <summary>
        /// Gets or sets the name of the replica on the server. The replica name is unique per feature service.
        /// This is not a required parameter.
        /// If not specified, a replica name will be assigned and returned in the createReplica response.
        /// If specified, but the replicaName already exists on the server,
        /// a unique name will be returned in the response using the given replicaName as a
        /// base (that is, MyReplica may be returned as MyReplica_0 if there is already a replica named MyReplica on the server).
        /// </summary>
        [DataMember(Name = "replicaName")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of layers and tables to include in the replica
        /// </summary>
        [IgnoreDataMember]
        public List<int> Layers { get; set; }

        [DataMember(Name = "layers")]
        public string LayersValue { get { return Layers == null || !Layers.Any() ? "0" : string.Join(",", Layers); } }

        [IgnoreDataMember]
        public List<LayerQuery> LayerQueries { get; set; }

        [DataMember(Name = "layerQueries")]
        public Dictionary<int, LayerQuery> LayerQueriesValue
        { get { return LayerQueries == null || !LayerQueries.Any() ? null : LayerQueries.ToDictionary(k => k.Id, v => v); } }

        /// <summary>
        ///  Specifies whether the replica is to be used on a client, such as a mobile device or ArcGIS Pro, or on another server.
        ///  Specifying server allows you to publish the replica to another portal and then synchronize changes between two feature services.
        ///  The default is client.
        /// </summary>
        [DataMember(Name = "targetType")]
        public string TargetType { get; set; }

        /// <summary>
        /// The transportType represents the response format.
        /// If the transportType is esriTransportTypeUrl, the JSON response is contained in a file, and the URL link to the file is returned.
        /// Otherwise, the JSON object is returned directly.
        /// The default is esriTransportTypeEmbedded
        /// </summary>
        [DataMember(Name = "transportType")]
        public string TransportType { get; set; }

        bool _returnAttachments;
        /// <summary>
        /// Gets or sets whether to return attachments.
        /// If true, attachments are added to the replica and returned in the response.
        /// Otherwise, attachments are not included. The default is false.
        /// This parameter is only applicable if the feature service has attachments.
        /// </summary>
        [DataMember(Name = "returnAttachments")]
        public bool ReturnAttachments
        {
            get => _returnAttachments;
            set
            {
                _returnAttachments = value;
                AttachmentsSyncDirection = value ? "bidirectional" : "none";
            }
        }

        /// <summary>
        /// If true, a reference to a URL will be provided for each attachment returned from createReplica.
        /// Otherwise, attachments are embedded in the response. The default is false.
        /// This parameter is only applicable if the feature service has attachments and if returnAttachments is true and f=json
        /// </summary>
        [DataMember(Name = "returnAttachmentsDataByUrl")]
        public bool ReturnAttachmentsDataByUrl { get; set; }

        [DataMember(Name = "async")]
        public bool IsAsync { get; set; }

        /// <summary>
        /// This parameter is used to indicate that the replica is being created for per-layer sync or per-replica sync.
        /// To determine which model types are supported by a service, query the supportsPerReplicaSync, supportsPerLayerSync,
        /// and supportsSyncModelNone properties of the Feature Service.
        /// If syncModel is perReplica, the syncDirection specified during sync applies to all layers in the replica.
        /// If the syncModel is perLayer, the syncDirection is defined on a layer-by-layer basis.
        /// If syncModel is perReplica, the response will have replicaServerGen.
        /// A perReplica syncModel requires the replicaServerGen on sync.
        /// The replicaServerGen tells the server the point in time from which to send back changes.
        /// If syncModel is perLayer, the response will include an array of server generation numbers for the layers in layerServerGens.
        /// A perLayer sync model requires the layerServerGens on sync.
        /// The layerServerGens tell the server the point in time from which to send back changes for a specific layer.
        /// syncModel=none can be used to export the data without creating a replica.
        /// syncModel=none is the default value.
        /// </summary>
        [DataMember(Name = "syncModel")]
        public string SyncModel { get; set; }

        /// <summary>
        /// Values include:
        /// bidirectional - Attachment edits can be both uploaded from the client and downloaded from the service when syncing.
        /// Upload - Attachment edits can only be uploaded from the client when syncing.When the client calls synchronizeReplica, feature and row edits will be downloaded but attachments will not be. This is useful in cases where the data collector does not want to consume space with attachments from the service, but does need to collect new attachments.
        /// None - Attachment edits are never synced from either the client or the server.
        /// When returnAttachments is set to true, you can set attachmentsSyncDirection to either bidirectional (default) or upload.In this case, create replica includes attachments from the service.
        /// When returnAttachments is set to false, you can set attachmentsSyncDirection to either upload or none (default). In this case, create replica does not include attachments from the service.
        /// All other combinations are not valid.
        /// </summary>
        [DataMember(Name = "attachmentsSyncDirection")]
        public string AttachmentsSyncDirection { get; set; }

        /// <summary>
        /// The format of the replica geodatabase returned in the response.
        /// sqlite or json are valid values.
        /// The default is json.
        /// </summary>
        [DataMember(Name = "dataFormat")]
        public string DataFormat { get; set; }

        /// <summary>
        /// Gets or sets the spatial reference of the replica geometry.
        /// If the ReplicaSpatialReference is not specified, the replica data will be in the spatial reference of the map.
        /// </summary>
        [DataMember(Name = "replicaSR")]
        public SpatialReference ReplicaSpatialReference { get; set; }
    }

    public class LayerQuery
    {
        public LayerQuery()
        {
            QueryOption = "useFilter";
            UseGeometry = true;
            IncludeRelated = true;
        }

        [IgnoreDataMember]
        public int Id { get; set; }

        /// <summary>
        /// Defines whether and how filters will be applied to a layer.
        /// Values can be none, useFilter, all
        /// </summary>
        [DataMember(Name = "queryOption")]
        public string QueryOption { get; set; }

        /// <summary>
        /// Defines an attribute query for a layer or table. The default is no where clause.
        /// </summary>
        [DataMember(Name = "where")]
        public string Where { get; set; }

        /// <summary>
        /// Determines whether or not to apply the geometry for the layer.
        /// The default is true. If set to false, features from the layer that intersect the geometry are not added.
        /// </summary>
        [DataMember(Name = "useGeometry")]
        public bool UseGeometry { get; set; }

        /// <summary>
        /// Determines whether or not to add related rows.
        /// The default is true.
        /// Value true is honored only for queryOption = none.
        /// This is only applicable if your data has relationship classes.
        /// Relationships are only processed in a forward direction from origin to destination.
        /// </summary>
        [DataMember(Name = "includeRelated")]
        public bool IncludeRelated { get; set; }
    }

    [DataContract]
    public class UnregisterReplica : ArcGISServerOperation
    {
        public UnregisterReplica(string relativeUrl, Action beforeRequest = null, Action afterRequest = null)
            : this(relativeUrl.AsEndpoint(), beforeRequest, afterRequest)
        { }

        public UnregisterReplica(ArcGISServerEndpoint endpoint, Action beforeRequest = null, Action afterRequest = null)
            : base(endpoint.RelativeUrl.Trim('/') + "/" + Operations.UnregisterReplica, beforeRequest, afterRequest)
        { }

        [DataMember(Name = "replicaID")]
        public string Id { get; set; }
    }

    [DataContract]
    public class ArcGISReplica<T> : PortalResponse
        where T : IGeometry
    {
        [DataMember(Name = "replicaName")]
        public string Name { get; set; }

        [DataMember(Name = "replicaID")]
        public string Id { get; set; }

        /// <summary>
        /// Can be esriTransportTypeUrl or esriTransportTypeEmbedded
        /// </summary>
        [DataMember(Name = "transportType")]
        public string TransportType { get; set; }

        /// <summary>
        /// Can be esriReplicaResponseTypeData or esriReplicaResponseTypeInfo
        /// </summary>
        [DataMember(Name = "responseType")]
        public string ResponseType { get; set; }

        /// <summary>
        /// Can be perReplica or perLayer
        /// </summary>
        [DataMember(Name = "syncModel")]
        public string SyncModel { get; set; }

        [DataMember(Name = "layers")]
        public List<ReplicaLayer<T>> ReplicaLayers { get; set; }
    }

    [DataContract]
    public class ReplicaLayer<T>
        where T : IGeometry
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [IgnoreDataMember]
        public string RelativeUrl { get; set; }

        [DataMember(Name = "features")]
        public List<Feature<T>> Features { get; set; }

        [DataMember(Name = "attachments")]
        public List<Attachment> Attachments { get; set; }
    }

    [DataContract]
    public class Attachment
    {
        [DataMember(Name = "attachmentId")]
        public long AttachmentID { get; set; }

        [DataMember(Name = "globalId")]
        public string GlobalID { get; set; }

        [DataMember(Name = "parentGlobalId")]
        public string ParentGlobalID { get; set; }

        [DataMember(Name = "contentType")]
        public string ContentType { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [IgnoreDataMember]
        public string SafeFileName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Name))
                {
                    return string.Empty;
                }

                var r = new Regex(string.Format("[{0}]", Regex.Escape(new string(Path.GetInvalidFileNameChars()))));
                return r.Replace(Name, string.Empty);
            }
        }

        [DataMember(Name = "size")]
        public long Size { get; set; }

        [DataMember(Name = "data")]
        public string Base64EncodedData { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        public override string ToString()
        {
            return $"Id: {AttachmentID} - Name: {Name} ({Url})";
        }
    }
}
