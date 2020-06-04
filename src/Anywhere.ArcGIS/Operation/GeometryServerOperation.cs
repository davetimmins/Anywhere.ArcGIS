using Anywhere.ArcGIS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Anywhere.ArcGIS.Operation
{
    [DataContract]
    public class GeometryOperationResponse<T> : PortalResponse
        where T : IGeometry
    {
        [DataMember(Name = "geometries")]
        public List<T> Geometries { get; set; }
    }

    [DataContract]
    public class SimplifyGeometry<T> : ArcGISServerOperation
        where T : IGeometry
    {
        public SimplifyGeometry(IEndpoint endpoint, List<Feature<T>> features = null, SpatialReference spatialReference = null, Action beforeRequest = null, Action afterRequest = null)
            : base((endpoint is AbsoluteEndpoint)
                ? (IEndpoint)new AbsoluteEndpoint(endpoint.RelativeUrl.Trim('/') + "/" + Operations.Simplify)
                : (IEndpoint)new ArcGISServerEndpoint(endpoint.RelativeUrl.Trim('/') + "/" + Operations.Simplify),
                beforeRequest, afterRequest)
        {
            Geometries = new GeometryCollection<T> { Geometries = features?.Select(f => f.Geometry).ToList() };
            SpatialReference = spatialReference;
        }

        [DataMember(Name = "geometries")]
        public GeometryCollection<T> Geometries { get; set; }

        [DataMember(Name = "sr")]
        public SpatialReference SpatialReference { get; set; }
    }

    [DataContract]
    public class BufferGeometry<T> : GeometryOperation<T>
        where T : IGeometry
    {
        public BufferGeometry(IEndpoint endpoint, List<Feature<T>> features, SpatialReference spatialReference, double distance)
            : base(endpoint, features, spatialReference, Operations.Buffer)
        {
            Geometries.Geometries.First().SpatialReference = spatialReference;
            BufferSpatialReference = spatialReference;
            Distances = new List<double> { distance };
        }

        [DataMember(Name = "bufferSR")]
        public SpatialReference BufferSpatialReference { get; set; }

        public List<double> Distances { get; set; }

        [DataMember(Name = "distances")]
        public string DistancesCSV
        {
            get
            {
                string strDistances = "";
                foreach (double distance in Distances)
                {
                    strDistances += distance.ToString("0.000") + ", ";
                }

                if (strDistances.Length >= 2)
                {
                    strDistances = strDistances.Substring(0, strDistances.Length - 2);
                }

                return strDistances;
            }
        }

        /// <summary>
        /// See http://resources.esri.com/help/9.3/ArcGISDesktop/ArcObjects/esriGeometry/esriSRUnitType.htm and http://resources.esri.com/help/9.3/ArcGISDesktop/ArcObjects/esriGeometry/esriSRUnit2Type.htm
        /// If not specified, derived from bufferSR, or inSR.
        /// </summary>
        [DataMember(Name = "unit")]
        public string Unit { get; set; }

        /// <summary>
        /// If true, all geometries buffered at a given distance are unioned into a single (possibly multipart) polygon,
        /// and the unioned geometry is placed in the output array.
        /// The default is false.
        /// </summary>
        [DataMember(Name = "unionResults")]
        public bool UnionResults { get; set; }

        /// <summary>
        /// Set geodesic to true to buffer the input geometries using geodesic distance. Geodesic distance is the shortest path between two points along the ellipsoid of the earth.
        /// If geodesic is set to false, the 2D Euclidean distance is used to buffer the input geometries.
        /// </summary>
        [DataMember(Name = "geodesic")]
        public bool? Geodesic { get; set; }
    }

    [DataContract]
    public class ProjectGeometry<T> : GeometryOperation<T>
        where T : IGeometry
    {
        public ProjectGeometry(IEndpoint endpoint, List<Feature<T>> features, SpatialReference outputSpatialReference)
            : base(endpoint, features, outputSpatialReference, Operations.Project)
        { }

        /// <summary>
        /// The WKID or a JSON object specifying the geographic transformation (also known as datum transformation) to be applied to the
        /// projected geometries.
        /// Note that a transformation is needed only if the output spatial reference contains a different geographic coordinate system
        /// than the input spatial reference.
        /// </summary>
        [DataMember(Name = "transformation")]
        public string Transformation { get; set; }

        /// <summary>
        /// A Boolean value indicating whether or not to transform forward.
        /// The forward or reverse direction of transformation is implied in the name of the transformation.
        /// If <c>Transformation</c> is specified, a value for the <c>TransformForward</c> parameter must also be specified. The default value is <c>false</c>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [transform forward]; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "transformForward")]
        public bool TransformForward { get; set; }
    }

    [DataContract]
    public class GeometryCollection<T>
        where T : IGeometry
    {
        [DataMember(Name = "geometryType")]
        public string GeometryType
        {
            get
            {
                return Geometries == null
                    ? string.Empty
                    : GeometryTypes.TypeMap[Geometries.First().GetType()]();
            }
        }

        [DataMember(Name = "geometries")]
        public List<T> Geometries { get; set; }
    }

    public abstract class GeometryOperation<T> : ArcGISServerOperation
        where T : IGeometry
    {
        public GeometryOperation(IEndpoint endpoint,
            List<Feature<T>> features,
            SpatialReference outputSpatialReference,
            string operation,
            Action beforeRequest = null, Action afterRequest = null)
            : base((endpoint is AbsoluteEndpoint)
                ? (IEndpoint)new AbsoluteEndpoint(endpoint.RelativeUrl.Trim('/') + "/" + operation)
                : (IEndpoint)new ArcGISServerEndpoint(endpoint.RelativeUrl.Trim('/') + "/" + operation),
                beforeRequest, afterRequest)
        {
            Features = features;
            if (features.Any())
            {
                Geometries = new GeometryCollection<T> { Geometries = new List<T>(features.Select(f => f.Geometry)) };

                if (Geometries.Geometries.First()?.SpatialReference == null && features?.First()?.Geometry?.SpatialReference != null)
                    Geometries.Geometries.First().SpatialReference = new SpatialReference { Wkid = features?.First()?.Geometry?.SpatialReference?.Wkid };
            }
            OutputSpatialReference = outputSpatialReference;
        }

        [IgnoreDataMember]
        public List<Feature<T>> Features { get; private set; }

        [DataMember(Name = "geometries")]
        public GeometryCollection<T> Geometries { get; protected set; }

        /// <summary>
        /// Taken from the spatial reference of the first geometry, if that is null then assumed to be using Wgs84
        /// </summary>
        [DataMember(Name = "inSR")]
        public SpatialReference InputSpatialReference { get { return Geometries.Geometries.First()?.SpatialReference ?? SpatialReference.WGS84; } }

        /// <summary>
        /// The spatial reference of the returned geometry.
        /// If not specified, the geometry is returned in the spatial reference of the input.
        /// </summary>
        [DataMember(Name = "outSR")]
        public SpatialReference OutputSpatialReference { get; protected set; }
    }
}
