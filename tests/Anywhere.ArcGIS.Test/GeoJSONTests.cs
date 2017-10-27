using Anywhere.ArcGIS.Common;
using Anywhere.ArcGIS.GeoJson;
using Newtonsoft.Json;
using System.Linq;
using Xunit;

namespace Anywhere.ArcGIS.Test
{
    public class GeoJSONTests
    {
        [Fact]
        public void CanConvertToFeatures()
        {
            var start = "{ \"type\": \"FeatureCollection\", \"features\": [ { \"type\": \"Feature\", \"geometry\": ";
            var end = "} ] }";

            var pointData = "{ \"type\": \"Point\", \"coordinates\": [-77.038193, 38.901345] }";
            Convert<GeoJsonPoint, Point>(start + pointData + end);

            var lineData = "{ \"type\": \"LineString\", \"coordinates\": [ [100.0, 0.0], [101.0, 1.0] ] }";
            Convert<GeoJsonLineString, Polyline>(start + lineData + end);

            Convert<GeoJsonLineString, MultiPoint>(start + lineData.Replace("LineString", "MultiPoint") + end);

            var polygonData = "{ \"type\": \"Polygon\", \"coordinates\": [ [ [100.0, 0.0], [101.0, 0.0], [101.0, 1.0], [100.0, 1.0], [100.0, 0.0] ] ] }";
            Convert<GeoJsonPolygon, Polygon>(start + polygonData + end);

            Convert<GeoJsonPolygon, Polyline>(start + polygonData.Replace("Polygon", "MultiLineString") + end);

            var polygonData2 = "{ \"type\": \"Polygon\", \"coordinates\": [ [ [100.0, 0.0], [101.0, 0.0], [101.0, 1.0], [100.0, 1.0], [100.0, 0.0] ], [ [100.2, 0.2], [100.8, 0.2], [100.8, 0.8], [100.2, 0.8], [100.2, 0.2] ] ] }";
            Convert<GeoJsonPolygon, Polygon>(start + polygonData2 + end);

            Convert<GeoJsonPolygon, Polyline>(start + polygonData2.Replace("Polygon", "MultiLineString") + end);

            var multiPolygonData = "{ \"type\": \"Polygon\", \"coordinates\": [ [[[102.0, 2.0], [103.0, 2.0], [103.0, 3.0], [102.0, 3.0], [102.0, 2.0]]], [[[100.0, 0.0], [101.0, 0.0], [101.0, 1.0], [100.0, 1.0], [100.0, 0.0]], [[100.2, 0.2], [100.8, 0.2], [100.8, 0.8], [100.2, 0.8], [100.2, 0.2]]] ] }";

            Convert<GeoJsonMultiPolygon, Polygon>(start + multiPolygonData + end);
        }

        void Convert<TGeoJSON, TGeometry>(string data)
            where TGeoJSON : IGeoJsonGeometry
            where TGeometry : IGeometry
        {
            var featureCollection = JsonConvert.DeserializeObject<FeatureCollection<TGeoJSON>>(data);

            Assert.NotNull(featureCollection);

            var features = featureCollection.ToFeatures();

            Assert.NotNull(features);
            Assert.Equal(featureCollection.Features.Count, features.Count);
            Assert.IsType<TGeometry>(features.First().Geometry);
            Assert.True(features.All(f => f.Geometry != null));
        }
    }
}
