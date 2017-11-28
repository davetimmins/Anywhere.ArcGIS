using Anywhere.ArcGIS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Anywhere.ArcGIS.Operation
{
    [DataContract]
    public class DeleteFeatures : ArcGISServerOperation
    {
        public DeleteFeatures(string relativeUrl, Action beforeRequest = null, Action afterRequest = null)
            : this(relativeUrl.AsEndpoint(), beforeRequest, afterRequest)
        { }

        public DeleteFeatures(ArcGISServerEndpoint endpoint, Action beforeRequest = null, Action afterRequest = null)
            : base(endpoint.RelativeUrl.Trim('/') + "/" + Operations.DeleteFeatures, beforeRequest, afterRequest)
        {
            SpatialRelationship = SpatialRelationshipTypes.Intersects;
            RollbackOnFailure = true;
        }

        /// <summary>
        /// A where clause for the query filter. Any legal SQL where clause operating on the fields in the layer is allowed.
        /// Features conforming to the specified where clause will be deleted.
        /// </summary>
        [DataMember(Name = "where")]
        public string Where { get; set; }

        /// <summary>
        ///  The object IDs of this layer/table to be deleted.
        /// </summary>
        [IgnoreDataMember]
        public List<long> ObjectIds { get; set; }

        /// <summary>
        /// The list of object Ids to be deleted. This list is a comma delimited list of Ids.
        /// </summary>
        [DataMember(Name = "objectIds")]
        public string ObjectIdsValue { get { return ObjectIds == null || !ObjectIds.Any() ? null : string.Join(",", ObjectIds); } }

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
        /// The spatial relationship to be applied on the input geometry while performing the query.
        /// The supported spatial relationships include intersects, contains, envelope intersects, within, etc.
        /// The default spatial relationship is "esriSpatialRelIntersects".
        /// Values: esriSpatialRelIntersects | esriSpatialRelContains | esriSpatialRelCrosses | esriSpatialRelEnvelopeIntersects | esriSpatialRelIndexIntersects | esriSpatialRelOverlaps | esriSpatialRelTouches | esriSpatialRelWithin | esriSpatialRelRelation
        /// </summary>
        [DataMember(Name = "spatialRel")]
        public string SpatialRelationship { get; set; }

        /// <summary>
        /// Geodatabase version to apply the edits. This parameter applies only if the isDataVersioned property of the layer is true.
        /// If the gdbVersion parameter is not specified, edits are made to the published map’s version.
        /// This option was added at 10.1.
        /// </summary>
        [DataMember(Name = "gdbVersion")]
        public string GeodatabaseVersion { get; set; }

        /// <summary>
        /// Optional parameter specifying whether the response will report the time features were deleted.
        /// If returnEditMoment = true, the server will report the time in the response's editMoment key.
        /// The default value is false.
        /// This option was added at 10.5 and works with ArcGIS Server services only.
        /// </summary>
        [DataMember(Name = "returnEditMoment")]
        public bool ReturnEditMoment { get; set; }

        /// <summary>
        /// Optional parameter to specify if the edits should be applied only if all submitted edits succeed.
        /// If false, the server will apply the edits that succeed even if some of the submitted edits fail.
        /// If true, the server will apply the edits only if all edits succeed. The default value is true.
        /// Not all data supports setting this parameter.
        /// Query the supportsRollbackonFailureParameter property of the layer to determine whether or not a layer supports setting this parameter.
        /// If supportsRollbackOnFailureParameter = false for a layer, then when editing this layer, rollbackOnFailure will always be true, regardless of how the parameter is set.
        /// However, if supportsRollbackonFailureParameter = true, this means the rollbackOnFailure parameter value will be honored on edit operations.
        /// This option was added at 10.1.
        /// </summary>
        [DataMember(Name = "rollbackOnFailure")]
        public bool RollbackOnFailure { get; set; }
    }

    [DataContract]
    public class DeleteFeaturesResponse : PortalResponse
    {
        [DataMember(Name = "deleteResults")]
        public DeleteFeatureResult[] Results { get; set; }

        /// <summary>
        /// Only set when ObjectIds are not specified
        /// </summary>
        [DataMember(Name = "success")]
        public bool? Success { get; set; }
    }

    [DataContract]
    public class DeleteFeatureResult : PortalResponse
    {
        [DataMember(Name = "objectId")]
        public long ObjectId { get; set; }

        [DataMember(Name = "globalId")]
        public string GlobalId { get; set; }

        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "editMoment")]
        public string EditMoment { get; set; }
    }
}
