using Anywhere.ArcGIS.Common;
using Anywhere.ArcGIS.GeoJson;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Anywhere.ArcGIS
{
    /// <summary>
    /// Extension methods for converting GeoJSON <-> ArcGIS Feature Set
    /// </summary>
    public static class FeatureCollectionExtensions
    {
        readonly static Dictionary<string, Func<Type>> _typeMap = new Dictionary<string, Func<Type>>
            {
                { "Point", () => typeof(Point) },
                { "MultiPoint", () => typeof(MultiPoint) },
                { "LineString", () => typeof(Polyline) },
                { "MultiLineString", () => typeof(Polyline) },
                { "Polygon", () => typeof(Polygon) },
                { "MultiPolygon", () => typeof(Polygon) }
            };

        /// <summary>
        /// Convert a GeoJSON FeatureCollection into an ArcGIS FeatureSet
        /// </summary>
        /// <typeparam name="TGeometry">The type of GeoJSON geometry to convert. Can be Point, MultiPoint, LineString, MultiLineString, Polygon, MultiPolygon</typeparam>
        /// <param name="featureCollection">A collection of one or more features of the same geometry type</param>
        /// <returns>A converted set of features that can be used in ArcGIS Server operations</returns>
        public static List<Feature<IGeometry>> ToFeatures<TGeometry>(this FeatureCollection<TGeometry> featureCollection)
            where TGeometry : IGeoJsonGeometry
        {
            if (featureCollection == null || featureCollection.Features == null || !featureCollection.Features.Any())
            {
                return null;
            }

            var features = new List<Feature<IGeometry>>();

            foreach (var geoJson in featureCollection.Features)
            {
                var geometry = geoJson.Geometry.ToGeometry(_typeMap[geoJson.Geometry.Type]());
                if (geometry == null)
                {
                    continue;
                }

                features.Add(new Feature<IGeometry> { Geometry = geometry, Attributes = geoJson.Properties });
            }
            return features;
        }

        /// <summary>
        /// Convert an ArcGIS Feature Set into a GeoJSON FeatureCollection
        /// </summary>
        /// <typeparam name="TGeometry">The type of ArcGIS geometry to convert.</typeparam>
        /// <param name="features">A collection of one or more ArcGIS Features</param>
        /// <returns>A converted FeatureCollection of GeoJSON Features</returns>
        public static FeatureCollection<IGeoJsonGeometry> ToFeatureCollection<TGeometry>(this List<Feature<TGeometry>> features)
            where TGeometry : IGeometry
        {
            if (features == null || !features.Any())
            {
                return null;
            }

            var featureCollection = new FeatureCollection<IGeoJsonGeometry> { Features = new List<GeoJsonFeature<IGeoJsonGeometry>>() };
            if (features.First().Geometry.SpatialReference != null)
            {
                featureCollection.CoordinateReferenceSystem = new Crs
                {
                    Type = "EPSG",
                    Properties = new CrsProperties { Wkid = (int)features.First().Geometry.SpatialReference.Wkid }
                };
            }

            foreach (var feature in features)
            {
                var geoJsonGeometry = feature.Geometry.ToGeoJson();
                if (geoJsonGeometry == null)
                {
                    continue;
                }

                featureCollection.Features.Add(new GeoJsonFeature<IGeoJsonGeometry>
                {
                    Type = "Feature",
                    Geometry = geoJsonGeometry,
                    Properties = feature.Attributes
                });
            }
            return featureCollection;
        }

        /// <summary>
        /// Updates the geometries of the feature collection but preserves the attributes and order of features
        /// </summary>
        /// <typeparam name="T">The type of geometry</typeparam>
        /// <param name="features">collection of features to update</param>
        /// <param name="geometries">The updated geometries</param>
        /// <returns>An updated collection of features in the same order as was passed in</returns>
        public static List<Feature<T>> UpdateGeometries<T>(this List<Feature<T>> features, List<T> geometries) where T : IGeometry
        {
            var result = new List<Feature<T>>();

            for (int i = 0; i < features.Count; i++)
            {
                var attr = i < features.Count ? features[i].Attributes : null;
                var feature = new Feature<T> { Attributes = attr };
                if (i < geometries.Count)
                {
                    feature.Geometry = geometries[i];
                }
                result.Insert(i, feature);
            }
            if (geometries.Count > features.Count)
            {
                result.InsertRange(features.Count, geometries.Skip(features.Count).Select(g => new Feature<T> { Geometry = g }));
            }

            return result;
        }
    }
}
