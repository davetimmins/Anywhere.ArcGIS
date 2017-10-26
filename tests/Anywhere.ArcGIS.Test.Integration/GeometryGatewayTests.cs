namespace Anywhere.ArcGIS.Test.Integration
{
    using Anywhere.ArcGIS;
    using Anywhere.ArcGIS.Common;
    using Anywhere.ArcGIS.Operation;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    public class GeometryGatewayTests : IClassFixture<IntegrationTestFixture>
    {
        public GeometryGatewayTests(IntegrationTestFixture fixture, ITestOutputHelper output)
        {
            fixture.SetTestOutputHelper(output);
        }

        [Fact]
        public async Task CanProject()
        {
            var gateway = new PortalGateway("http://sampleserver1.arcgisonline.com/ArcGIS");
            var result = await IntegrationTestFixture.TestPolicy.ExecuteAsync(() =>
            {
                return gateway.Query<Polygon>(new Query("Demographics/ESRI_Census_USA/MapServer/5".AsEndpoint()) { Where = "STATE_NAME = 'Oregon'" });
            });
            var features = result.Features.Where(f => f.Geometry.Rings.Any()).ToList();

            Assert.NotNull(features);
            Assert.True(result.SpatialReference.Wkid != SpatialReference.WGS84.Wkid);
            Assert.True(features[0].Geometry.Rings.Count > 0);
            features[0].Geometry.SpatialReference = result.SpatialReference;

            var projectedFeatures = await IntegrationTestFixture.TestPolicy.ExecuteAsync(() =>
            {
                return new ArcGISOnlineGateway().Project<Polygon>(features, SpatialReference.WGS84);
            });

            Assert.NotNull(projectedFeatures);
            Assert.Equal(features.Count, projectedFeatures.Count);

            Assert.True(features[0].Geometry.Rings.Count > 0);  // If this fails, 2 issues: 1) features has been shallow copied, and 2) geometries aren't being populated.
            Assert.True(projectedFeatures[0].Geometry.Rings.Count > 0); // If this fails, just problem 2 above - geometries aren't being copied.
            Assert.NotEqual(features, projectedFeatures);
            Assert.NotEqual(projectedFeatures[0].Geometry.Rings[0], features[0].Geometry.Rings[0]);
        }

        [Fact]
        public async Task CanBuffer()
        {
            var gateway = new PortalGateway("http://sampleserver1.arcgisonline.com/ArcGIS");

            var result = await IntegrationTestFixture.TestPolicy.ExecuteAsync(() =>
            {
                return gateway.Query<Polygon>(new Query("Demographics/ESRI_Census_USA/MapServer/5".AsEndpoint()) { Where = "STATE_NAME = 'Texas'" });
            });

            var features = result.Features.Where(f => f.Geometry.Rings.Any()).ToList();
            Assert.NotNull(features);

            await Buffer(gateway, features, result.SpatialReference);
        }

        async Task Buffer(PortalGatewayBase gateway, List<Feature<Polygon>> features, SpatialReference spatialReference)
        {
            int featuresCount = features.Count;

            double distance = 10.0;
            var featuresBuffered = await IntegrationTestFixture.TestPolicy.ExecuteAsync(() =>
            {
                return new ArcGISOnlineGateway().Buffer<Polygon>(features, spatialReference, distance);
            });

            Assert.NotNull(featuresBuffered);
            Assert.Equal(featuresCount, featuresBuffered.Count);
        }

        [Fact]
        public async Task CanSimplify()
        {
            var gateway = new PortalGateway("http://sampleserver1.arcgisonline.com/ArcGIS");
            var result = await IntegrationTestFixture.TestPolicy.ExecuteAsync(() =>
            {
                return gateway.Query<Polygon>(new Query("Demographics/ESRI_Census_USA/MapServer/5".AsEndpoint()) { Where = "STATE_NAME = 'Oregon'" });
            });

            var features = result.Features.Where(f => f.Geometry.Rings.Any()).ToList();

            Assert.NotNull(features);
            Assert.True(result.SpatialReference.Wkid != SpatialReference.WGS84.Wkid);
            Assert.True(features[0].Geometry.Rings.Count > 0);
            features[0].Geometry.SpatialReference = result.SpatialReference;

            var simplifiedFeatures = await IntegrationTestFixture.TestPolicy.ExecuteAsync(() =>
            {
                return new ArcGISOnlineGateway().Simplify<Polygon>(features, result.SpatialReference);
            });

            Assert.NotNull(simplifiedFeatures);
            Assert.Equal(features.Count, simplifiedFeatures.Count);

            Assert.True(features[0].Geometry.Rings.Count > 0);
            Assert.True(simplifiedFeatures[0].Geometry.Rings.Count > 0);
            Assert.NotEqual(features, simplifiedFeatures);
            Assert.NotEqual(simplifiedFeatures[0].Geometry.Rings[0], features[0].Geometry.Rings[0]);
        }
    }
}
