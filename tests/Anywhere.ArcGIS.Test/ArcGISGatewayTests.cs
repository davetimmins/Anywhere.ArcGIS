namespace ArcGIS.Test
{
    using Anywhere.ArcGIS;
    using System;
    using Xunit;

    public class ArcGISGatewayTests
    {
        [Theory]
        [InlineData("http://sampleserver3.arcgisonline.com/ArcGIS/")]
        [InlineData("http://services.arcgisonline.co.nz/arcgis/rest/")]
        [InlineData("https://services.arcgisonline.co.nz/arcgis/rest/")]
        [InlineData("http://services.arcgisonline.co.nz/arcgis/rest")]
        [InlineData("https://services.arcgisonline.co.nz/arcgis/rest")]
        [InlineData("http://services.arcgisonline.co.nz/arcgis/")]
        [InlineData("https://services.arcgisonline.co.nz/arcgis/")]
        [InlineData("http://services.arcgisonline.co.nz/arcgis")]
        [InlineData("https://services.arcgisonline.co.nz/arcgis")]
        [InlineData("https://services.arcgisonline.co.nz/arcgis/tokens/")]
        [InlineData("https://services.arcgisonline.co.nz/arcgis/TokeNS")]

        public void GatewayRootUrlHasCorrectFormat(string rootUrl)
        {
            var gateway = new PortalGateway(rootUrl);
            Assert.EndsWith("/", gateway.RootUrl, StringComparison.Ordinal);
            Assert.True(gateway.RootUrl.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) || gateway.RootUrl.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase));
            Assert.DoesNotContain("/rest/services/", gateway.RootUrl.ToLowerInvariant());
            Assert.DoesNotContain("/tokens/", gateway.RootUrl.ToLowerInvariant());
        }

        [Fact]
        public void EmptyRootUrlThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => "".AsRootUrl());
        }

        [Theory]
        [InlineData("http://www.arcgis.com/arcgis")]
        [InlineData("http://www.arcgis.com/ArcGIS")]
        [InlineData("http://www.arcgis.com/ArcGIS/")]
        [InlineData("http://www.arcgis.com/ArcGIS/rest/services")]
        [InlineData("http://www.arcgis.com/ArcGIS/rest/services/")]
        [InlineData("http://www.arcgis.com/ArcGIS/rest/services/rest/services")]
        [InlineData("http://www.arcgis.com/ArcGIS/rest/services/rest/services/")]
        [InlineData("http://www.arcgis.com/ArcGIS/admin")]
        [InlineData("http://www.arcgis.com/ArcGIS/admin/")]
        [InlineData("http://www.arcgis.com/ArcGIS/rest/admin/services")]
        [InlineData("http://www.arcgis.com/ArcGIS/rest/admin/services/")]
        [InlineData("http://www.arcgis.com/ArcGIS/rest/ADMIN/services/")]
        [InlineData("http://www.arcgis.com/ArcGIS/tokens")]
        [InlineData("http://www.arcgis.com/ArcGIS/tokens/")]
        public void HttpRootUrlHasCorrectFormat(string urlToTest)
        {
            var rootUrl = urlToTest.AsRootUrl();
            Assert.Equal("http://www.arcgis.com/arcgis/", rootUrl, true);
        }

        [Theory]
        [InlineData("https://www.arcgis.com/arcgis")]
        [InlineData("https://www.arcgis.com/ArcGIS")]
        [InlineData("https://www.arcgis.com/ArcGIS/")]
        [InlineData("https://www.arcgis.com/ArcGIS/rest/services")]
        [InlineData("https://www.arcgis.com/ArcGIS/rest/services/")]
        [InlineData("https://www.arcgis.com/ArcGIS/rest/services/rest/services")]
        [InlineData("https://www.arcgis.com/ArcGIS/rest/services/rest/services/")]
        [InlineData("https://www.arcgis.com/ArcGIS/admin")]
        [InlineData("https://www.arcgis.com/ArcGIS/admin/")]
        [InlineData("https://www.arcgis.com/ArcGIS/rest/admin/services")]
        [InlineData("https://www.arcgis.com/ArcGIS/rest/admin/services/")]
        [InlineData("https://www.arcgis.com/ArcGIS/rest/ADMIN/services/")]
        [InlineData("https://www.arcgis.com/ArcGIS/tokens")]
        [InlineData("https://www.arcgis.com/ArcGIS/tokens/")]
        public void HttpsRootUrlHasCorrectFormat(string urlToTest)
        {
            var rootUrl = urlToTest.AsRootUrl();
            Assert.Equal("https://www.arcgis.com/arcgis/", rootUrl, true);
        }
    }
}
