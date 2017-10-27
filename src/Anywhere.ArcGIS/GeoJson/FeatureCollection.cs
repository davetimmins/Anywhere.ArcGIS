using Anywhere.ArcGIS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Anywhere.ArcGIS.GeoJson
{
    [DataContract]
    public class FeatureCollection<TGeometry> where TGeometry : IGeoJsonGeometry
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "bbox")]
        public double[] BoundingBox { get; set; }

        [DataMember(Name = "features")]
        public List<GeoJsonFeature<TGeometry>> Features { get; set; }

        [DataMember(Name = "crs")]
        public Crs CoordinateReferenceSystem { get; set; }
    }

    [DataContract]
    public class Crs
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "properties")]
        public CrsProperties Properties { get; set; }
    }

    [DataContract]
    public class CrsProperties
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "href")]
        public string Href { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "code")]
        public int Wkid { get; set; }
    }

    [DataContract]
    public class GeoJsonFeature<TGeometry> where TGeometry : IGeoJsonGeometry
    {
        [DataMember(Name = "id")]
        public object Id { get; set; }

        [DataMember(Name = "properties")]
        public Dictionary<string, object> Properties { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "bbox")]
        public double[] BoundingBox { get; set; }

        [DataMember(Name = "geometry")]
        public TGeometry Geometry { get; set; }
    }

    [DataContract]
    public class GeoJsonPoint : IGeoJsonGeometry
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "coordinates")]
        public double[] Coordinates { get; set; }

        public IGeometry ToGeometry(Type type)
        {
            if (Coordinates == null || Coordinates.Count() != 2)
            {
                return null;
            }

            return new Point { X = Coordinates.First(), Y = Coordinates.Last() };
        }
    }

    [DataContract]
    public class GeoJsonLineString : IGeoJsonGeometry
    {
        readonly static Dictionary<Type, Func<PointCollection, IGeometry>> _geoJsonFactoryMap = new Dictionary<Type, Func<PointCollection, IGeometry>>
        {
            { typeof(MultiPoint), (coords) => new MultiPoint { Points = coords } },
            { typeof(Polyline), (coords) => new Polyline { Paths = new PointCollectionList { coords } } }
        };

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "coordinates")]
        public PointCollection Coordinates { get; set; }

        public IGeometry ToGeometry(Type type)
        {
            if (Coordinates == null)
            {
                return null;
            }

            return _geoJsonFactoryMap[type](Coordinates);
        }
    }

    [DataContract]
    public class GeoJsonPolygon : IGeoJsonGeometry
    {
        static Dictionary<Type, Func<PointCollectionList, IGeometry>> _geoJsonFactoryMap = new Dictionary<Type, Func<PointCollectionList, IGeometry>>
        {
            { typeof(Polygon), (coords) => new Polygon { Rings = coords } },
            { typeof(Polyline), (coords) => new Polyline { Paths =  coords  } }
        };

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "coordinates")]
        public PointCollectionList Coordinates { get; set; }

        public IGeometry ToGeometry(Type type)
        {
            if (Coordinates == null)
            {
                return null;
            }

            return _geoJsonFactoryMap[type](Coordinates);
        }
    }

    [DataContract]
    public class GeoJsonMultiPolygon : IGeoJsonGeometry
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "coordinates")]
        public List<PointCollectionList> Coordinates { get; set; }

        public IGeometry ToGeometry(Type type)
        {
            if (Coordinates == null) return null;

            var poly = new Polygon { Rings = new PointCollectionList() };

            foreach (var polygon in Coordinates)
            {
                poly.Rings.AddRange(polygon);
            }

            return poly;
        }
    }

    public interface IGeoJsonGeometry
    {
        [DataMember(Name = "type")]
        string Type { get; set; }

        IGeometry ToGeometry(Type type);
    }
}
