using Anywhere.ArcGIS.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Anywhere.ArcGIS.Operation
{
    /// <summary>
    /// Calls the find method for a locator service. This has the parameters used by the hosted world geocoding service at geocode.arcgis.com
    /// </summary>
    [DataContract]
    public class SingleInputGeocode : GeocodeOperation
    {
        public SingleInputGeocode(ArcGISServerEndpoint endpoint, Action beforeRequest = null, Action afterRequest = null)
            : base(new ArcGISServerEndpoint(endpoint.RelativeUrl.Trim('/') + "/" + Operations.SingleInputGeocode), beforeRequest, afterRequest)
        {
            MaxResults = 1;
            Distance = null;
        }

        /// <summary>
        /// Specifies the location to be geocoded. This can be a street address, place name, postal code, or POI. It is a required parameter
        /// </summary>
        [DataMember(Name = "text")]
        public string Text { get; set; }

        /// <summary>
        /// A value representing the country. Providing this value increases geocoding speed.
        /// Acceptable values include the full country name, the ISO 3166-1 2-digit country code, or the ISO 3166-1 3-digit country code.
        /// </summary>
        [DataMember(Name = "sourceCountry")]
        public string SourceCountry { get; set; }

        /// <summary>
        /// The maximum number of results to be returned by a search, up to the maximum number allowed by the service. 
        /// If not specified, then one location will be returned. 
        /// The world geocoding service allows up to 20 candidates to be returned for a single request. 
        /// Note that up to 50 POI candidates can be returned
        /// </summary>
        [DataMember(Name = "maxLocations")]
        public int MaxResults { get; set; }

        [DataMember(Name = "magicKey")]
        public string MagicKey { get; set; }

        /// <summary>
        /// A set of bounding box coordinates that limit the search area to a specific region. 
        /// This is especially useful for applications in which a user will search for places and addresses only within the current map extent. 
        /// </summary>
        [DataMember(Name = "bbox")]
        public Extent SearchExtent { get; set; }

        /// <summary>
        ///  The field names of the attributes to be returned.
        /// </summary>
        [IgnoreDataMember]
        public List<string> OutFields { get; set; }

        [DataMember(Name = "outFields")]
        public string OutFieldsValue { get { return OutFields == null ? string.Empty : string.Join(",", OutFields); } }

        /// <summary>
        /// The spatial reference of the x/y coordinates returned by a geocode request. 
        /// This is useful for applications using a map with a spatial reference different than that of the geocode service. 
        /// If the outSR is not specified, the spatial reference of the output locations is the same as that of the service. 
        /// The world geocoding service spatial reference is WGS84 (WKID = 4326). 
        /// </summary>
        [DataMember(Name = "outSR")]
        public SpatialReference OutputSpatialReference { get; set; }
    }

    [DataContract]
    public class SingleInputGeocodeResponse : PortalResponse
    {
        [DataMember(Name = "SpatialReference")]
        public SpatialReference SpatialReference { get; set; }

        [DataMember(Name = "locations")]
        public IEnumerable<Location> Results { get; set; }
    }

    [DataContract]
    public class Location
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "extent")]
        public Extent Extent { get; set; }

        [DataMember(Name = "feature")]
        public Feature<Point> Feature { get; set; }
    }

    /// <summary>
    /// Call the suggest method for a locator. This has the parameters used by the hosted world geocoding service at geocode.arcgis.com 
    /// </summary>
    [DataContract]
    public class SuggestGeocode : GeocodeOperation
    {
        public SuggestGeocode(ArcGISServerEndpoint endpoint, Action beforeRequest = null, Action afterRequest = null)
            : base(new ArcGISServerEndpoint(endpoint.RelativeUrl.Trim('/') + "/" + Operations.SuggestGeocode), beforeRequest, afterRequest)
        {
            Distance = null;
        }

        /// <summary>
        /// Specifies the location to be searched for.
        /// </summary>
        [DataMember(Name = "text")]
        public string Text { get; set; }
    }

    [DataContract]
    public class SuggestGeocodeResponse : PortalResponse
    {
        [DataMember(Name = "suggestions")]
        public Suggestion[] Suggestions { get; set; }
    }

    [DataContract]
    public class Suggestion
    {
        [DataMember(Name = "isCollection")]
        public bool IsCollection { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "magicKey")]
        public string MagicKey { get; set; }
    }
}
