using Anywhere.ArcGIS.Common;
using System.Collections.Generic;

namespace Anywhere.ArcGIS
{
    public static class GeometryExtensions
    {
        /// <summary>
        /// Convert a <see cref="Point"/> into a double[] so that it can be added to a <see cref="PointCollection"/>
        /// </summary>
        /// <param name="point"></param>
        /// <returns>Array with 2 values</returns>
        public static double[] ToPointCollectionEntry(this Point point)
        {
            return new[] { point.X, point.Y };
        }

        /// <summary>
        /// Convert a collection of points into a <see cref="PointCollectionList"/> that can be used as paths in <see cref="Polyline"/> types
        /// or rings in <see cref="Polygon"/> types
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static PointCollectionList ToPointCollectionList(this IEnumerable<Point> points)
        {
            var result = new PointCollectionList();
            var pointCollection = new PointCollection();
            foreach (var point in points)
            {
                pointCollection.Add(point.ToPointCollectionEntry());
            }
            result.Add(pointCollection);
            return result;
        }
    }
}
