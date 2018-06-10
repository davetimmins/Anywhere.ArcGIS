using Anywhere.ArcGIS.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Anywhere.ArcGIS.Operation
{
    /// <summary>
    /// Calls findAddressCandidates for a locator service using the single line address field to search against.
    /// </summary>
    [DataContract]
    public class SingleInputCustomGeocode : ArcGISServerOperation
    {
        public SingleInputCustomGeocode(string relativeUrl, Action beforeRequest = null, Action afterRequest = null)
            : this(relativeUrl.AsEndpoint(), beforeRequest, afterRequest)
        { }

        public SingleInputCustomGeocode(ArcGISServerEndpoint endpoint, Action beforeRequest = null, Action afterRequest = null)
            : base(endpoint.RelativeUrl.Trim('/') + "/" + Operations.SingleInputCustomGeocode, beforeRequest, afterRequest)
        { }

        /// <summary>
        /// Specifies the location to be geocoded. This can be a street address, place-name, postal code, or POI. 
        /// The input address components need to be formatted as a single string
        /// </summary>
        [DataMember(Name = "singleLine")]
        public string Text { get; set; }

        /// <summary>
        /// The spatial reference of the x/y coordinates returned by a geocode request. 
        /// This is useful for applications using a map with a spatial reference different than that of the geocode service. 
        /// If the outSR is not specified, the spatial reference of the output locations is the same as that of the service. 
        /// The world geocoding service spatial reference is WGS84 (WKID = 4326). 
        /// </summary>
        [DataMember(Name = "outSR")]
        public SpatialReference OutputSpatialReference { get; set; }

        /// <summary>
        /// A set of bounding box coordinates that limit the search area to a specific region. 
        /// This is especially useful for applications in which a user will search for places and addresses only within the current map extent. 
        /// </summary>
        [DataMember(Name = "searchExtent")]
        public Extent SearchExtent { get; set; }

        /// <summary>
        ///  The field names of the attributes to be returned.
        /// </summary>
        [IgnoreDataMember]
        public List<string> OutFields { get; set; }

        [DataMember(Name = "outFields")]
        public string OutFieldsValue { get { return OutFields == null ? string.Empty : string.Join(",", OutFields); } }
    }
    
    [DataContract]
    public class SingleInputCustomGeocodeResponse<T> : PortalResponse where T : IGeometry
    {
        [DataMember(Name = "spatialReference")]
        public SpatialReference SpatialReference { get; set; }

        [DataMember(Name = "candidates")]
        public Candidate<T>[] Candidates { get; set; }
    }
    
    [DataContract]
    public class Candidate<T> where T : IGeometry
    {
        [DataMember(Name = "address")]
        public string Address { get; set; }

        [DataMember(Name = "location")]
        public T Location { get; set; }

        [DataMember(Name = "score")]
        public double Score { get; set; }

        [DataMember(Name = "attributes")]
        public Dictionary<string, object> Attributes { get; set; }

        [DataMember(Name = "extent")]
        public Extent Extent { get; set; }
    }
}
