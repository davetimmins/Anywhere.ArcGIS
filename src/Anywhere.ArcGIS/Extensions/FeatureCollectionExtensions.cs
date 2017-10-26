using Anywhere.ArcGIS.Common;
using System.Collections.Generic;
using System.Linq;

namespace Anywhere.ArcGIS
{
    /// <summary>
    /// Extension methods for converting GeoJSON <-> ArcGIS Feature Set
    /// </summary>
    public static class FeatureCollectionExtensions
    {
        /// <summary>
        /// Updates the geometries of the feature collection but preserves the attributes and order of features
        /// </summary>
        /// <typeparam name="T">The type of geometry</typeparam>
        /// <param name="features">collection of features to update</param>
        /// <param name="geometries">The updated geometries</param>
        /// <returns>An updated collection of features in the same order as was passed in</returns>
        public static List<Feature<T>> UpdateGeometries<T>(this List<Feature<T>> features, List<T> geometries) where T : IGeometry<T>
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
                result
                    .InsertRange(features.Count, geometries.Skip(features.Count)
                    .Select(g => new Feature<T> { Geometry = g }));
            }

            return result;
        }
    }
}
