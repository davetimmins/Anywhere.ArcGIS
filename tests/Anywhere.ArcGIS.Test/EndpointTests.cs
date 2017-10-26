namespace ArcGIS.Test
{
    using Anywhere.ArcGIS;
    using Anywhere.ArcGIS.Common;
    using System;
    using Xunit;

    public class EndpointTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void InvalidArcGISServerEndpointThrowsArgumentNullException(string relativeUrl)
        {
            Assert.Throws<ArgumentNullException>(() => new ArcGISServerEndpoint(relativeUrl));
            Assert.Throws<ArgumentNullException>(() => relativeUrl.AsEndpoint());
        }

        [Theory]
        [InlineData("/rest/Services/REST/services/rest/services/")]
        [InlineData("/rest/services/rest/services/rest/services/")]
        [InlineData("/rest/services/")]
        [InlineData("rest/services/")]
        [InlineData("rest/services")]
        [InlineData("/rest/services")]
        [InlineData("/")]
        [InlineData("http://www.google.co.nz/rest/services")]
        [InlineData("http://www.google.co.nz")]
        [InlineData("http://www.google.co.nz/")]
        [InlineData("https://www.google.co.nz/")]
        public void ArcGISServerEndpointHasCorrectFormat(string relativeUrl)
        {
            var endpoint = new ArcGISServerEndpoint(relativeUrl);
            Assert.Equal("rest/services/", endpoint.RelativeUrl);

            endpoint = relativeUrl.AsEndpoint();
            Assert.Equal("rest/services/", endpoint.RelativeUrl);
        }

        [Theory]
        [InlineData("something/MapServer")]
        [InlineData("something/FeatureServer")]
        public void ArcGISServerEndpointHasCorrectFormatWithRelativeUrl(string relativeUrl)
        {
            var endpoint = new ArcGISServerEndpoint(relativeUrl);
            Assert.Equal("rest/services/" + relativeUrl, endpoint.RelativeUrl);

            endpoint = relativeUrl.AsEndpoint();
            Assert.Equal("rest/services/" + relativeUrl, endpoint.RelativeUrl);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void InvalidArcGISServerAdminEndpointThrowsArgumentNullException(string relativeUrl)
        {
            Assert.Throws<ArgumentNullException>(() => new ArcGISServerAdminEndpoint(relativeUrl));
            Assert.Throws<ArgumentNullException>(() => relativeUrl.AsAdminEndpoint());
        }

        [Theory]
        [InlineData("/admin/admin/admin/")]
        [InlineData("/admin/ADmin/admin/")]
        [InlineData("/admin/")]
        [InlineData("admin/")]
        [InlineData("admin")]
        [InlineData("/admin")]
        [InlineData("/")]
        [InlineData("http://www.google.co.nz/admin")]
        [InlineData("http://www.google.co.nz")]
        [InlineData("http://www.google.co.nz/")]
        [InlineData("https://www.google.co.nz")]
        public void ArcGISServerAdminEndpointHasCorrectFormat(string relativeUrl)
        {
            var endpoint = new ArcGISServerAdminEndpoint(relativeUrl);
            Assert.Equal("admin/", endpoint.RelativeUrl);

            endpoint = relativeUrl.AsAdminEndpoint();
            Assert.Equal("admin/", endpoint.RelativeUrl);
        }

        [Theory]
        [InlineData("something/MapServer")]
        [InlineData("something/FeatureServer")]
        public void ArcGISServerAdminEndpointHasCorrectFormatWithRelativeUrl(string relativeUrl)
        {
            var endpoint = new ArcGISServerAdminEndpoint(relativeUrl);
            Assert.Equal("admin/" + relativeUrl, endpoint.RelativeUrl);

            endpoint = relativeUrl.AsAdminEndpoint();
            Assert.Equal("admin/" + relativeUrl, endpoint.RelativeUrl);
        }
    }
}
