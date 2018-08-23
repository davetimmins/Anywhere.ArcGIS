namespace Anywhere.ArcGIS
{
    using Anywhere.ArcGIS.Common;
    using Anywhere.ArcGIS.Operation;
    using Anywhere.ArcGIS.Operation.Admin;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// ArcGIS Server gateway
    /// </summary>
    public class PortalGateway : PortalGatewayBase
    {
        /// <summary>
        /// Create a new <see cref="PortalGateway"/> using the default token service as discovered using the Info operation for the server
        /// </summary>
        /// <param name="rootUrl"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="serializer"></param>
        /// <param name="httpClientFunc"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public new static async Task<PortalGateway> Create(
            string rootUrl, string username, string password,
            ISerializer serializer = null, Func<HttpClient> httpClientFunc = null,
            CancellationToken ct = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(rootUrl))
            {
                throw new ArgumentNullException(nameof(rootUrl), "rootUrl is null.");
            }

            var gateway = new PortalGateway(rootUrl, serializer: serializer, httpClientFunc: httpClientFunc);
            var info = await gateway.Info(ct);

            if (info == null)
            {
                throw new Exception($"Unable to get ArcGIS Server information for {gateway.RootUrl}. Check the ArcGIS Server URL and try again.");
            }

            ITokenProvider tokenProvider = null;
            if (!string.IsNullOrWhiteSpace(info.OwningSystemUrl) && (info.OwningSystemUrl.StartsWith("http://www.arcgis.com", StringComparison.OrdinalIgnoreCase) || info.OwningSystemUrl.StartsWith("https://www.arcgis.com", StringComparison.OrdinalIgnoreCase)))
            {
                tokenProvider = new ArcGISOnlineTokenProvider(username, password, serializer: serializer, httpClientFunc: httpClientFunc);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(info.AuthenticationInfo?.TokenServicesUrl))
                {
                    if (!info.AuthenticationInfo.TokenServicesUrl.StartsWith(gateway.RootUrl, StringComparison.OrdinalIgnoreCase))
                    {
                        tokenProvider = new FederatedTokenProvider(
                            new ServerFederatedWithPortalTokenProvider(info.AuthenticationInfo.TokenServicesUrl.Replace("/generateToken", ""), username, password, serializer: serializer, httpClientFunc: httpClientFunc),
                            info.AuthenticationInfo.TokenServicesUrl.Replace("/generateToken", ""),
                            gateway.RootUrl,
                            referer: info.AuthenticationInfo.TokenServicesUrl.Replace("/sharing/rest/generateToken", "/rest"),
                            serializer: serializer, 
                            httpClientFunc: httpClientFunc);
                    }
                    else
                    {
                        tokenProvider = new TokenProvider(info.AuthenticationInfo?.TokenServicesUrl, username, password, serializer: serializer, httpClientFunc: httpClientFunc);
                    }
                }
            }

            return new PortalGateway(
                rootUrl,
                tokenProvider: tokenProvider,
                serializer: serializer,
                httpClientFunc: httpClientFunc);
        }

        public PortalGateway(string rootUrl, ISerializer serializer = null, ITokenProvider tokenProvider = null, Func<HttpClient> httpClientFunc = null)
            : base(rootUrl, serializer, tokenProvider, httpClientFunc)
        { }

        public PortalGateway(string rootUrl, string username, string password, ISerializer serializer = null, Func<HttpClient> httpClientFunc = null)
            : this(rootUrl, serializer,
            (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                ? null
                : new TokenProvider(rootUrl, username, password, serializer),
            httpClientFunc)
        { }

        /// <summary>
        /// Request the health of a server
        /// </summary>
        /// <param name="ct">Optional cancellation token to cancel pending request</param>
        /// <returns>True if the server is available to accept requests, otherwise false</returns>
        public virtual Task<HealthCheckResponse> HealthCheck(CancellationToken ct = default(CancellationToken))
        {
            return Get<HealthCheckResponse, HealthCheck>(new HealthCheck(), ct);
        }

        /// <summary>
        /// Recursively parses an ArcGIS Server site and discovers the resources available
        /// </summary>
        /// <param name="ct">Optional cancellation token to cancel pending request</param>
        /// <returns>An ArcGIS Server site hierarchy</returns>
        public virtual async Task<SiteDescription> DescribeSite(CancellationToken ct = default(CancellationToken))
        {
            var result = new SiteDescription();

            result.Resources.AddRange(await DescribeEndpoint(new ArcGISServerOperation("/".AsEndpoint()), ct).ConfigureAwait(false));

            return result;
        }

        async Task<List<SiteFolderDescription>> DescribeEndpoint(ArcGISServerOperation operation, CancellationToken ct = default(CancellationToken))
        {
            SiteFolderDescription folderDescription = null;
            var result = new List<SiteFolderDescription>();
            try
            {
                folderDescription = await Get<SiteFolderDescription, ArcGISServerOperation>(operation, ct).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                // don't have access to the folder
                result.Add(new SiteFolderDescription
                {
                    Error = new ArcGISError
                    {
                        Message = "HttpRequestException for Get SiteFolderDescription at path " + operation.Endpoint.RelativeUrl,
                        Details = new[] { ex.ToString() }
                    }
                });
                return result;
            }
            catch (System.Runtime.Serialization.SerializationException ex)
            {
                // don't have access to the folder
                result.Add(new SiteFolderDescription
                {
                    Error = new ArcGISError
                    {
                        Message = "SerializationException for Get SiteFolderDescription at path " + operation.Endpoint.RelativeUrl,
                        Details = new[] { ex.ToString() }
                    }
                });
                return result;
            }
            catch (Exception ex)
            {
                result.Add(new SiteFolderDescription
                {
                    Error = new ArcGISError
                    {
                        Message = "Exception for Get SiteFolderDescription at path " + operation.Endpoint.RelativeUrl,
                        Details = new[] { ex.ToString() }
                    }
                });
                return result;
            }
            if (ct.IsCancellationRequested) return result;

            folderDescription.Path = operation.Endpoint.RelativeUrl;
            result.Add(folderDescription);

            if (folderDescription.Folders != null)
            {
                foreach (var folder in folderDescription.Folders)
                {
                    if (ct.IsCancellationRequested)
                    {
                        return result;
                    }

                    result.AddRange(await DescribeEndpoint(new ArcGISServerOperation((operation.Endpoint.RelativeUrl + folder).AsEndpoint()), ct).ConfigureAwait(false));
                }
            }

            return result;
        }

        /// <summary>
        /// Return the service description details for the matched services in the site description
        /// </summary>
        /// <param name="siteDescription"></param>
        /// <param name="ct">An optional cancellation token</param>
        /// <returns>A collection of service description details</returns>
        public virtual Task<List<ServiceDescriptionDetailsResponse>> DescribeServices(SiteDescription siteDescription, CancellationToken ct = default(CancellationToken))
        {
            if (siteDescription == null)
            {
                throw new ArgumentNullException(nameof(siteDescription));
            }

            if (siteDescription.Services == null)
            {
                throw new ArgumentNullException(nameof(siteDescription.Services));
            }
          
            return DescribeServices(siteDescription.Services.ToList(), ct);
        }

        /// <summary>
        /// Return the service description details for the matched services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="ct">An optional cancellation token</param>
        /// <returns>A collection of service description details</returns>
        public virtual async Task<List<ServiceDescriptionDetailsResponse>> DescribeServices(List<ServiceDescription> services, CancellationToken ct = default(CancellationToken))
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var result = new List<ServiceDescriptionDetailsResponse>();

            foreach (var serviceDescription in services)
            {
                result.Add(await DescribeService(serviceDescription.ArcGISServerEndpoint, ct).ConfigureAwait(false));
            }

            return result;
        }

        /// <summary>
        ///  Return the service description details for the requested endpoint
        /// </summary>
        /// <param name="serviceEndpoint"></param>
        /// <param name="ct">An optional cancellation token</param>
        /// <returns>The service description details</returns>
        public virtual Task<ServiceDescriptionDetailsResponse> DescribeService(IEndpoint serviceEndpoint, CancellationToken ct = default(CancellationToken))
        {
            if (serviceEndpoint == null)
            {
                throw new ArgumentNullException(nameof(serviceEndpoint));
            }

            return Get<ServiceDescriptionDetailsResponse, ServiceDescriptionDetails>(new ServiceDescriptionDetails(serviceEndpoint), ct);
        }

        /// <summary>
        /// Admin operation used to get all services for the ArcGIS Server and their reports
        /// </summary>
        /// <param name="ct">Optional cancellation token to cancel pending request</param>
        /// <param name="path">The starting path (folder). If omitted then this will start at the root and get all sub folders too</param>
        /// <returns>All discovered services for the site</returns>
        public virtual async Task<SiteReportResponse> SiteReport(string path = "", CancellationToken ct = default(CancellationToken))
        {
            var folders = new List<string>();

            if (string.IsNullOrWhiteSpace(path))
            {
                var folderDescription = await Get<SiteFolderDescription, ArcGISServerOperation>(new ArcGISServerOperation("/"), ct).ConfigureAwait(false);
                folders.Add("/");
                folders.AddRange(folderDescription.Folders);
            }
            else
                folders.Add(path);

            var result = new SiteReportResponse();
            foreach (var folder in folders)
            {
                var folderReport = await Get<FolderReportResponse, ServiceReport>(new ServiceReport(folder), ct).ConfigureAwait(false);

                result.Resources.Add(folderReport);

                if (ct.IsCancellationRequested) return result;
            }
            return result;
        }

        /// <summary>
        /// Returns the expected and actual status of a service
        /// </summary>
        /// <param name="serviceDescription">Service description usually generated from a previous call to DescribeSite</param>
        /// <param name="ct">Optional cancellation token to cancel pending request</param>
        /// <returns>The expected and actual status of the service</returns>
        public virtual Task<ServiceStatusResponse> ServiceStatus(ServiceDescription serviceDescription, CancellationToken ct = default(CancellationToken))
        {
            return Get<ServiceStatusResponse, ServiceStatus>(new ServiceStatus(serviceDescription), ct);
        }

        /// <summary>
        /// Start the service
        /// </summary>
        /// <param name="serviceDescription">Service description usually generated from a previous call to DescribeSite</param>
        /// <param name="ct">Optional cancellation token to cancel pending request</param>
        /// <returns>Standard response object</returns>
        public virtual Task<StartStopServiceResponse> StartService(ServiceDescription serviceDescription, CancellationToken ct = default(CancellationToken))
        {
            return Post<StartStopServiceResponse, StartService>(new StartService(serviceDescription), ct);
        }

        /// <summary>
        /// Stop the service
        /// </summary>
        /// <param name="serviceDescription">Service description usually generated from a previous call to DescribeSite</param>
        /// <param name="ct">Optional cancellation token to cancel pending request</param>
        /// <returns>Standard response object</returns>
        public virtual Task<StartStopServiceResponse> StopService(ServiceDescription serviceDescription, CancellationToken ct = default(CancellationToken))
        {
            return Post<StartStopServiceResponse, StopService>(new StopService(serviceDescription), ct);
        }

        /// <summary>
        /// Provides a view into the life cycle of all instances of the service on all server machines within the cluster.
        /// </summary>
        /// <param name="serviceDescription">Service description usually generated from a previous call to DescribeSite</param>
        /// <param name="ct">Optional cancellation token to cancel pending request</param>
        /// <returns>The statistics for the service</returns>
        public virtual Task<ServiceStatisticsResponse> ServiceStatistics(ServiceDescription serviceDescription, CancellationToken ct = default(CancellationToken))
        {
            return Get<ServiceStatisticsResponse, ServiceStatistics>(new ServiceStatistics(serviceDescription), ct);
        }

        /// <summary>
        /// Call the reverse geocode operation.
        /// </summary>
        /// <param name="reverseGeocode"></param>
        /// <param name="ct">Optional cancellation token to cancel pending request</param>
        /// <returns></returns>
        public virtual Task<ReverseGeocodeResponse> ReverseGeocode(ReverseGeocode reverseGeocode, CancellationToken ct = default(CancellationToken))
        {
            return Get<ReverseGeocodeResponse, ReverseGeocode>(reverseGeocode, ct);
        }

        /// <summary>
        /// Call the single line geocode operation.
        /// </summary>
        /// <param name="geocode"></param>
        /// <param name="ct">Optional cancellation token to cancel pending request</param>
        /// <returns></returns>
        public virtual Task<SingleInputGeocodeResponse> Geocode(SingleInputGeocode geocode, CancellationToken ct = default(CancellationToken))
        {            
            return Get<SingleInputGeocodeResponse, SingleInputGeocode>(geocode, ct);
        }

        /// <summary>
        /// The CustomGeocode (FindAddressCandidates) operation supports searching for places and addresses in single-field format.
        /// This method assumes the results are points.
        /// </summary>
        /// <param name="geocode"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public virtual Task<SingleInputCustomGeocodeResponse<Point>> CustomGeocode(SingleInputCustomGeocode geocode, CancellationToken ct = default(CancellationToken))
        {
            return CustomGeocode<Point>(geocode, ct);
        }

        /// <summary>
        /// The CustomGeocode (FindAddressCandidates) operation supports searching for places and addresses in single-field format
        /// </summary>
        /// <typeparam name="T">The geometry type for the result set</typeparam>
        /// <param name="geocode"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public virtual Task<SingleInputCustomGeocodeResponse<T>> CustomGeocode<T>(SingleInputCustomGeocode geocode, CancellationToken ct = default(CancellationToken))
            where T : IGeometry
        {
            return Get<SingleInputCustomGeocodeResponse<T>, SingleInputCustomGeocode>(geocode, ct);
        }

        /// <summary>
        /// Call the suggest geocode operation.
        /// </summary>
        /// <param name="suggestGeocode"></param>
        /// <param name="ct">Optional cancellation token to cancel pending request</param>
        /// <returns></returns>
        public virtual Task<SuggestGeocodeResponse> Suggest(SuggestGeocode suggestGeocode, CancellationToken ct = default(CancellationToken))
        {
            return Get<SuggestGeocodeResponse, SuggestGeocode>(suggestGeocode, ct);
        }

        /// <summary>
        /// Call the find operation, note that since this can return more than one geometry type you will need to deserialize
        /// the geometry string on the result set e.g.
        /// foreach (var result in response.Results.Where(r => r.Geometry != null))
        /// {
        ///     result.Geometry = JsonConvert.DeserializeObject(result.Geometry.ToString(), GeometryTypes.ToTypeMap[result.GeometryType]());
        /// }
        /// </summary>
        /// <param name="findOptions"></param>
        /// <param name="ct">Optional cancellation token to cancel pending request</param>
        /// <returns></returns>
        public virtual Task<FindResponse> Find(Find findOptions, CancellationToken ct = default(CancellationToken))
        {
            return Get<FindResponse, Find>(findOptions, ct);
        }
    }
}
