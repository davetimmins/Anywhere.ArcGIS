using Anywhere.ArcGIS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Anywhere.ArcGIS.Operation
{
    /// <summary>
    /// Search for hosted feature services on ArcGIS Online
    /// </summary>
    [DataContract]
    public class SearchHostedFeatureServices : SearchArcGISOnline
    {
        /// <summary>
        /// Search for hosted feature services on ArcGIS Online
        /// </summary>
        /// <param name="username">The name of the user (owner) of the feature services</param>
        public SearchHostedFeatureServices(string username)
            : base(string.Format("owner:{0} AND (type:\"Feature Service\")", username))
        { }

        public SearchHostedFeatureServices()
            : base("type:\"Feature Service\"")
        { }
    }

    /// <summary>
    /// Search against ArcGISOnline / Portal
    /// </summary>
    [DataContract]
    public class SearchArcGISOnline : ArcGISServerOperation
    {
        /// <summary>
        /// SSearch against ArcGISOnline / Portal
        /// </summary>
        /// <param name="query">The search query to execute</param>
        public SearchArcGISOnline(string query, Action beforeRequest = null, Action afterRequest = null)
            : base(new ArcGISOnlineEndpoint(Operations.ArcGISOnlineSearch), beforeRequest, afterRequest)
        {
            Query = query;
            SortOrder = "asc";
            NumberToReturn = 10;
            StartIndex = 1;
            SortFields = new List<string>();
        }

        /// <summary>
        /// Search query to execute. See http://resources.arcgis.com/en/help/arcgis-rest-api/index.html#//02r3000000mn000000 for more information
        /// </summary>
        [DataMember(Name = "q")]
        public string Query { get; protected set; }

        /// <summary>
        /// The bounding box for a spatial search defined as minx, miny, maxx, or maxy. Search requires q, bbox, or both.
        /// Spatial search is an overlaps/intersects function of the query bbox and the extent of the document.
        /// Documents that have no extent (e.g., mxds, 3dds, lyr) will not be found when doing a bbox search.
        /// Document extent is assumed to be in the WGS84 geographic coordinate system.
        /// </summary>
        [IgnoreDataMember]
        public Extent BoundingBox { get; set; }

        /// <summary>
        /// The bounding box for a spatial search defined as minx, miny, maxx, or maxy. Search requires q, bbox, or both.
        /// Spatial search is an overlaps/intersects function of the query bbox and the extent of the document.
        /// Documents that have no extent (e.g., mxds, 3dds, lyr) will not be found when doing a bbox search.
        /// Document extent is assumed to be in the WGS84 geographic coordinate system.
        /// </summary>
        [DataMember(Name = "bbox")]
        public string BBox
        {
            get
            {
                return BoundingBox == null || BoundingBox.SpatialReference == null || BoundingBox.SpatialReference != SpatialReference.WGS84 ?
                    string.Empty :
                    string.Format("{0},{1},{2},{3}", BoundingBox.XMin, BoundingBox.YMin, BoundingBox.XMax, BoundingBox.YMax);
            }
        }

        /// <summary>
        /// Fields to sort results by.
        /// Valid fields are: title, created, type, owner, avgRating, numRatings, numComments and numViews
        /// </summary>
        [IgnoreDataMember]
        public List<string> SortFields { get; set; }

        /// <summary>
        /// The list of fields to sort results by. This list is a comma delimited list of field names.
        /// Field to sort results by.
        /// Valid fields are: title, created, type, owner, avgRating, numRatings, numComments and numViews
        /// </summary>
        /// <remarks>Default is 'created'</remarks>
        [DataMember(Name = "sortField")]
        public string SortFieldsValue { get { return SortFields == null || !SortFields.Any() ? "created" : string.Join(",", SortFields); } }

        /// <summary>
        /// Order results by desc or asc
        /// </summary>
        /// <remarks>Default is asc</remarks>
        [DataMember(Name = "sortOrder")]
        public string SortOrder { get; set; }

        /// <summary>
        /// The maximum number of results to be included in the result set response.
        /// The default value is 10 and the maximum allowed value is 100.
        /// The start parameter combined with the NumberToReturn parameter can be used to paginate the search results.
        /// Note that the actual number of returned results may be less than NumberToReturn if the number of
        /// results remaining after start is less than NumberToReturn
        /// </summary>
        /// <remarks>Default is 10</remarks>
        [DataMember(Name = "num")]
        public int NumberToReturn { get; set; }

        /// <summary>
        /// The number of the first entry in the result set response.
        /// The index number is 1-based. The StartIndex parameter, along with the NumberToReturn parameter
        /// can be used to paginate the search results.
        /// </summary>
        [DataMember(Name = "start")]
        public int StartIndex { get; set; }
    }

    [DataContract]
    public class SearchHostedFeatureServicesResponse : PortalResponse
    {
        [DataMember(Name = "results")]
        public HostedFeatureService[] Results { get; set; }
    }

    [DataContract]
    public class HostedFeatureService
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}
