namespace ArcGIS.Test
{
    using Xunit;

    public class ArcGISOnlineTests
    {
        //[Fact]
        //public async Task CanSearchForHostedFeatureServices()
        //{
        //    var serializer = new ServiceStackSerializer();
        //    var gateway = new ArcGISOnlineGateway(serializer, new ArcGISOnlineTokenProvider("", "", serializer));

        //    var hostedServices = await gateway.DescribeSite();

        //    Assert.NotNull(hostedServices);
        //    Assert.Null(hostedServices.Error);
        //    Assert.NotNull(hostedServices.Results);
        //    Assert.NotEmpty(hostedServices.Results);
        //    Assert.False(hostedServices.Results.All(s => string.IsNullOrWhiteSpace(s.Id)));
        //}

        //[Fact]
        //public async Task OAuthTokenCanBeGenerated()
        //{
        //    // Set your client Id and secret here
        //    var tokenProvider = new ArcGISOnlineAppLoginOAuthProvider("", "", _serviceStackSerializer);

        //    var token = await tokenProvider.CheckGenerateToken();

        //    Assert.NotNull(token);
        //    Assert.NotNull(token.Value);
        //    Assert.False(token.IsExpired);
        //    Assert.Null(token.Error);
        //}
    }
}
