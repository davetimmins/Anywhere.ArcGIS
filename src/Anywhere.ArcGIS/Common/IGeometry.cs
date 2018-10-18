using Anywhere.ArcGIS.GeoJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Anywhere.ArcGIS.Common
{
    /// <summary>
    /// The ArcGIS Server REST API supports the following five geometry types:
    /// Points,
    /// Multipoints,
    /// Polylines,
    /// Polygons,
    /// Envelopes
    /// </summary>
    /// <remarks>Starting at ArcGIS Server 10.1, geometries containing m and z values are supported</remarks>
    public interface IGeometry : ICloneable
    {
        /// <summary>
        /// The spatial reference can be defined using a well-known ID (wkid) or well-known text (wkt)
        /// </summary>
        [DataMember(Name = "spatialReference")]
        SpatialReference SpatialReference { get; set; }

        /// <summary>
        /// Calculates the minimum bounding extent for the geometry
        /// </summary>
        /// <returns>Extent that can contain the geometry </returns>
        Extent GetExtent();

        /// <summary>
        /// Calculates the center of the minimum bounding extent for the geommetry
        /// </summary>
        /// <returns>The value for the center of the extent for the geometry</returns>
        Point GetCenter();

        /// <summary>
        /// Converts the geometry to its GeoJSON representation
        /// </summary>
        /// <returns>The corresponding GeoJSON for the geometry</returns>
        IGeoJsonGeometry ToGeoJson();
    }

    /// <summary>
    /// Spatial reference used for operations. If WKT is set then other properties are nulled
    /// </summary>
    [DataContract]
    public class SpatialReference : IEquatable<SpatialReference>, ICloneable
    {
        /// <summary>
        /// World Geodetic System 1984 (WGS84)
        /// </summary>
        public readonly static SpatialReference WGS84 = new SpatialReference
        {
            Wkid = 4326,
            LatestWkid = 4326
        };

        /// <summary>
        /// WGS 1984 Web Mercator (Auxiliary Sphere)
        /// </summary>
        public readonly static SpatialReference WebMercator = new SpatialReference
        {
            Wkid = 102100,
            LatestWkid = 3857
        };

        [DataMember(Name = "wkid")]
        public int? Wkid { get; set; }

        [DataMember(Name = "latestWkid")]
        public int? LatestWkid { get; set; }

        [DataMember(Name = "vcsWkid")]
        public int? VCSWkid { get; set; }

        [DataMember(Name = "latestVcsWkid")]
        public int? LatestVCSWkid { get; set; }

        string _wkt;
        [DataMember(Name = "wkt")]
        public string Wkt
        {
            get { return _wkt; }
            set
            {
                _wkt = value;
                Wkid = null;
                LatestWkid = null;
                VCSWkid = null;
                LatestVCSWkid = null;
            }
        }

        public static bool operator ==(SpatialReference left, SpatialReference right)
        {
            if (ReferenceEquals(null, left) && ReferenceEquals(null, right)) return true;
            if (ReferenceEquals(null, left)) return false;
            return left.Equals(right);
        }

        public static bool operator !=(SpatialReference left, SpatialReference right)
        {
            if (ReferenceEquals(null, left) && ReferenceEquals(null, right)) return false;
            if (ReferenceEquals(null, left)) return true;
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SpatialReference)obj);
        }

        public bool Equals(SpatialReference other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return (string.IsNullOrWhiteSpace(Wkt))
                ? (Wkid == other.Wkid || LatestWkid == other.LatestWkid) && (VCSWkid == other.VCSWkid || LatestVCSWkid == other.LatestVCSWkid)
                : string.Equals(Wkt, other.Wkt, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Wkid != null ? Wkid.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (LatestWkid != null ? LatestWkid.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (VCSWkid != null ? VCSWkid.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (LatestVCSWkid != null ? LatestVCSWkid.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Wkt != null ? Wkt.GetHashCode() : 0);
                return hashCode;
            }
        }

        public object Clone()
        {
            return new SpatialReference
            {
                LatestVCSWkid = LatestVCSWkid,
                LatestWkid = LatestWkid,
                VCSWkid = VCSWkid,
                Wkid = Wkid,
                Wkt = string.IsNullOrWhiteSpace(Wkt) ? null : string.Copy(Wkt)
            };
        }
    }

    [DataContract]
    public class Point : IGeometry, IEquatable<Point>
    {
        [DataMember(Name = "spatialReference", Order = 5)]
        public SpatialReference SpatialReference { get; set; }

        [DataMember(Name = "x", Order = 1)]
        public double X { get; set; }

        [DataMember(Name = "y", Order = 2)]
        public double Y { get; set; }

        [DataMember(Name = "z", Order = 3)]
        public double? Z { get; set; }

        [DataMember(Name = "m", Order = 4)]
        public double? M { get; set; }

        public Extent GetExtent()
        {
            return new Extent { XMin = X, YMin = Y, XMax = X, YMax = Y, SpatialReference = SpatialReference };
        }

        public Point GetCenter()
        {
            return new Point { X = X, Y = Y, SpatialReference = SpatialReference };
        }

        public bool Equals(Point other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && M.Equals(other.M) && Equals(SpatialReference, other.SpatialReference);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                hashCode = (hashCode * 397) ^ M.GetHashCode();
                hashCode = (hashCode * 397) ^ (SpatialReference != null ? SpatialReference.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Point)obj);
        }

        public IGeoJsonGeometry ToGeoJson()
        {
            return new GeoJsonPoint { Type = "Point", Coordinates = new[] { X, Y } };
        }

        public object Clone()
        {
            return new Point
            {
                X = X,
                Y = Y,
                M = M,
                Z = Z,
                SpatialReference = (SpatialReference)SpatialReference?.Clone()
            };
        }
    }

    [DataContract]
    public class MultiPoint : IGeometry, IEquatable<MultiPoint>
    {
        [DataMember(Name = "spatialReference", Order = 4)]
        public SpatialReference SpatialReference { get; set; }

        [DataMember(Name = "hasM", Order = 1)]
        public bool HasM { get; set; }

        [DataMember(Name = "hasZ", Order = 2)]
        public bool HasZ { get; set; }

        [DataMember(Name = "points", Order = 3)]
        public PointCollection Points { get; set; }

        public Extent GetExtent()
        {
            return Points.CalculateExtent(SpatialReference);
        }

        public Point GetCenter()
        {
            return GetExtent().GetCenter();
        }

        public bool Equals(MultiPoint other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(SpatialReference, other.SpatialReference) && HasM.Equals(other.HasM) && HasZ.Equals(other.HasZ) && Equals(Points, other.Points);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (SpatialReference != null ? SpatialReference.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ HasM.GetHashCode();
                hashCode = (hashCode * 397) ^ HasZ.GetHashCode();
                hashCode = (hashCode * 397) ^ (Points != null ? Points.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MultiPoint)obj);
        }

        public IGeoJsonGeometry ToGeoJson()
        {
            return new GeoJsonLineString { Type = "MultiPoint", Coordinates = Points };
        }

        public object Clone()
        {
            return new MultiPoint
            {
                HasM = HasM,
                HasZ = HasZ,
                Points = (PointCollection)Points?.Clone(),
                SpatialReference = (SpatialReference)SpatialReference?.Clone()
            };
        }
    }

    [DataContract]
    public class Polyline : IGeometry, IEquatable<Polyline>
    {
        [DataMember(Name = "spatialReference", Order = 4)]
        public SpatialReference SpatialReference { get; set; }

        [DataMember(Name = "hasM", Order = 1)]
        public bool HasM { get; set; }

        [DataMember(Name = "hasZ", Order = 2)]
        public bool HasZ { get; set; }

        [DataMember(Name = "paths", Order = 3)]
        public PointCollectionList Paths { get; set; }

        public Extent GetExtent()
        {
            Extent extent = null;
            foreach (PointCollection path in Paths)
            {
                if (extent == null)
                {
                    extent = path.CalculateExtent(SpatialReference);
                }
                else
                {
                    extent = extent.Union(path.CalculateExtent(SpatialReference));
                }
            }

            if (extent != null)
            {
                extent.SpatialReference = SpatialReference;
            }

            return extent;
        }

        public Point GetCenter()
        {
            return GetExtent().GetCenter();
        }

        public bool Equals(Polyline other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(SpatialReference, other.SpatialReference) && HasM.Equals(other.HasM) && HasZ.Equals(other.HasZ) && Equals(Paths, other.Paths);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (SpatialReference != null ? SpatialReference.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ HasM.GetHashCode();
                hashCode = (hashCode * 397) ^ HasZ.GetHashCode();
                hashCode = (hashCode * 397) ^ (Paths != null ? Paths.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Polyline)obj);
        }

        public IGeoJsonGeometry ToGeoJson()
        {
            if (Paths == null || !Paths.Any())
            {
                return null;
            }

            var coordinates = new PointCollection();

            foreach (PointCollection path in Paths)
            {
                foreach (var point in path)
                {
                    coordinates.Add(point);
                }                
            }

            return new GeoJsonLineString { Type = "LineString", Coordinates = coordinates };
        }

        public object Clone()
        {
            return new Polyline
            {
                HasM = HasM,
                HasZ = HasZ,
                Paths = Paths?.Clone(),
                SpatialReference = (SpatialReference)SpatialReference?.Clone()
            };
        }
    }

    public class PointCollection : List<double[]>
    {
        public Extent CalculateExtent(SpatialReference spatialReference)
        {
            double x = double.NaN;
            double y = double.NaN;
            double x1 = double.NaN;
            double y1 = double.NaN;

            foreach (var point in Points.Where(p => p != null))
            {
                if (point.X < x || double.IsNaN(x))
                {
                    x = point.X;
                }

                if (point.Y < y || double.IsNaN(y))
                {
                    y = point.Y;
                }

                if (point.X > x1 || double.IsNaN(x1))
                {
                    x1 = point.X;
                }

                if (point.Y > y1 || double.IsNaN(y1))
                {
                    y1 = point.Y;
                }
            }

            if (double.IsNaN(x) || double.IsNaN(y) || double.IsNaN(x1) || double.IsNaN(y1))
            {
                return null;
            }

            return new Extent { XMin = x, YMin = y, XMax = x1, YMax = y1, SpatialReference = spatialReference };
        }

        public List<Point> Points
        {
            get
            {
                return this.Select(point => point != null ? new Point { X = point.First(), Y = point.Last() } : null)
                    .Where(p => p != null)
                    .ToList();
            }
        }

        public List<double[]> Clone()
        {
            return Points.Select(x => new double[] { x.X, x.Y }).ToList();
        }
    }

    public class PointCollectionList : List<PointCollection>
    {
        public PointCollectionList Clone()
        {
            return this;
        }
    }

    [DataContract]
    public class Polygon : IGeometry, IEquatable<Polygon>
    {
        [DataMember(Name = "spatialReference", Order = 4)]
        public SpatialReference SpatialReference { get; set; }

        [DataMember(Name = "hasM", Order = 1)]
        public bool HasM { get; set; }

        [DataMember(Name = "hasZ", Order = 2)]
        public bool HasZ { get; set; }

        [DataMember(Name = "rings", Order = 3)]
        public PointCollectionList Rings { get; set; }

        public Extent GetExtent()
        {
            Extent extent = null;
            foreach (var ring in Rings.Where(r => r != null))
            {
                if (extent == null)
                {
                    extent = ring.CalculateExtent(SpatialReference);
                }
                else
                {
                    extent = extent.Union(ring.CalculateExtent(SpatialReference));
                }
            }

            if (extent != null && extent.SpatialReference == null)
            {
                extent.SpatialReference = SpatialReference;
            }

            return extent;
        }

        public Point GetCenter()
        {
            return GetExtent().GetCenter();
        }

        public bool Equals(Polygon other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(SpatialReference, other.SpatialReference) && HasM.Equals(other.HasM) && HasZ.Equals(other.HasZ) && Equals(Rings, other.Rings);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (SpatialReference != null ? SpatialReference.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ HasM.GetHashCode();
                hashCode = (hashCode * 397) ^ HasZ.GetHashCode();
                hashCode = (hashCode * 397) ^ (Rings != null ? Rings.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Polygon)obj);
        }

        public IGeoJsonGeometry ToGeoJson()
        {
            return new GeoJsonPolygon { Type = "Polygon", Coordinates = Rings };
        }

        public object Clone()
        {
            return new Polygon
            {
                HasM = HasM,
                HasZ = HasZ,
                Rings = Rings?.Clone(),
                SpatialReference = (SpatialReference)SpatialReference?.Clone()
            };
        }
    }

    [DataContract]
    public class Extent : IGeometry, IEquatable<Extent>
    {
        [DataMember(Name = "spatialReference", Order = 5)]
        public SpatialReference SpatialReference { get; set; }

        [DataMember(Name = "xmin", Order = 1)]
        public double XMin { get; set; }

        [DataMember(Name = "xmax", Order = 3)]
        public double XMax { get; set; }

        [DataMember(Name = "ymin", Order = 2)]
        public double YMin { get; set; }

        [DataMember(Name = "ymax", Order = 4)]
        public double YMax { get; set; }

        public Extent GetExtent()
        {
            return this;
        }

        public Point GetCenter()
        {
            return new Point { X = ((XMin + XMax) / 2), Y = ((YMin + YMax) / 2), SpatialReference = SpatialReference };
        }

        public Extent Union(Extent extent)
        {
            if (extent == null)
            {
                extent = this;
            }

            if (!SpatialReference.Equals(extent.SpatialReference))
            {
                throw new ArgumentException("Spatial references must match for union operation.");
            }

            var envelope = new Extent { SpatialReference = SpatialReference ?? extent.SpatialReference };
            if (double.IsNaN(XMin))
            {
                envelope.XMin = extent.XMin;
            }
            else if (!double.IsNaN(extent.XMin))
            {
                envelope.XMin = Math.Min(extent.XMin, XMin);
            }
            else
            {
                envelope.XMin = XMin;
            }

            if (double.IsNaN(XMax))
            {
                envelope.XMax = extent.XMax;
            }
            else if (!double.IsNaN(extent.XMax))
            {
                envelope.XMax = Math.Max(extent.XMax, XMax);
            }
            else
            {
                envelope.XMax = XMax;
            }

            if (double.IsNaN(YMin))
            {
                envelope.YMin = extent.YMin;
            }
            else if (!double.IsNaN(extent.YMin))
            {
                envelope.YMin = Math.Min(extent.YMin, YMin);
            }
            else
            {
                envelope.YMin = YMin;
            }

            if (double.IsNaN(YMax))
            {
                envelope.YMax = extent.YMax;
            }
            else if (!double.IsNaN(extent.YMax))
            {
                envelope.YMax = Math.Max(extent.YMax, YMax);
            }
            else
            {
                envelope.YMax = YMax;
            }

            return envelope;
        }

        public bool Equals(Extent other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(SpatialReference, other.SpatialReference) && XMin.Equals(other.XMin) && XMax.Equals(other.XMax) && YMin.Equals(other.YMin) && YMax.Equals(other.YMax);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (SpatialReference != null ? SpatialReference.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ XMin.GetHashCode();
                hashCode = (hashCode * 397) ^ XMax.GetHashCode();
                hashCode = (hashCode * 397) ^ YMin.GetHashCode();
                hashCode = (hashCode * 397) ^ YMax.GetHashCode();
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Extent)obj);
        }

        public IGeoJsonGeometry ToGeoJson()
        {
            return new GeoJsonPolygon
            {
                Type = "Polygon",
                Coordinates = new PointCollectionList
                {
                    new PointCollection
                    {
                        new[]{ XMin, YMin },
                        new[]{ XMax, YMin },
                        new[]{ XMax, YMax },
                        new[]{ XMin, YMax },
                        new[]{ XMin, YMin }
                    }
                }
            };
        }

        public object Clone()
        {
            return new Extent
            {
                XMax = XMax,
                XMin = XMin,
                YMax = YMax,
                YMin = YMin,
                SpatialReference = (SpatialReference)SpatialReference?.Clone()
            };
        }
    }
}
