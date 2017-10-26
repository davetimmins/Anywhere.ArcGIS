namespace ArcGIS.Test
{
    using Anywhere.ArcGIS;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class SecureGISGatewayTests
    {
        [Fact]
        public void TokenIsExpiredHasCorrectValue()
        {
            var expiry = DateTime.UtcNow.AddSeconds(1).ToUnixTime();
            var token = new Anywhere.ArcGIS.Operation.Token { Value = "blah", Expiry = expiry };

            Assert.NotNull(token);
            Assert.NotNull(token.Value);
            Assert.False(token.IsExpired);
            Thread.Sleep(TimeSpan.FromSeconds(2));
            Assert.True(token.IsExpired);
        }

        // [Fact]
        public async Task CanStopAndStartService()
        {
            var gateway = new PortalGateway("", "", "");
            var folder = ""; // set this to only get a specific folder
            var site = await gateway.SiteReport(folder);
            Assert.NotNull(site);
            var services = site.ServiceReports.Where(s => s.Type == "MapServer");
            Assert.NotNull(services);
            Assert.True(services.Any());
            foreach (var service in services)
            {
                var sd = service.AsServiceDescription();
                if (string.Equals("STARTED", service.Status.Actual, StringComparison.OrdinalIgnoreCase))
                {
                    var stoppedResult = await gateway.StopService(sd);
                    Assert.NotNull(stoppedResult);
                    Assert.Null(stoppedResult.Error);
                    Assert.Equal("success", stoppedResult.Status);

                    var startedResult = await gateway.StartService(sd);
                    Assert.NotNull(startedResult);
                    Assert.Null(startedResult.Error);
                    Assert.Equal("success", startedResult.Status);
                }
                else
                {
                    var startedResult = await gateway.StartService(sd);
                    Assert.NotNull(startedResult);
                    Assert.Null(startedResult.Error);
                    Assert.Equal("success", startedResult.Status);

                    var stoppedResult = await gateway.StopService(sd);
                    Assert.NotNull(stoppedResult);
                    Assert.Null(stoppedResult.Error);
                    Assert.Equal("success", stoppedResult.Status);
                }
            }
        }
    }
}
