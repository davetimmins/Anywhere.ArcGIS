using Anywhere.ArcGIS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Anywhere.ArcGIS.Operation
{
    /// <summary>
    /// This operation adds, updates and deletes features to the associated feature layer or table in a single call (POST only). 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public class ApplyEdits<T> : ArcGISServerOperation where T : IGeometry<T>
    {
        public ApplyEdits(ArcGISServerEndpoint endpoint, Action beforeRequest = null, Action afterRequest = null)
            : base(endpoint.RelativeUrl.Trim('/') + "/" + Operations.ApplyEdits, beforeRequest, afterRequest)
        {
            Adds = new List<Feature<T>>();
            Updates = new List<Feature<T>>();
            Deletes = new List<long>();
        }
                
        /// <summary>
        /// The array of features to be added.
        /// </summary>
        [DataMember(Name = "adds")]
        public List<Feature<T>> Adds { get; set; }

        /// <summary>
        /// The array of features to be updated.
        /// </summary>
        [DataMember(Name = "updates")]
        public List<Feature<T>> Updates { get; set; }

        /// <summary>
        ///  The object IDs of this layer / table to be deleted.
        /// </summary>
        [IgnoreDataMember]
        public List<long> Deletes { get; set; }

        [DataMember(Name = "deletes")]
        public string DeleteIds { get { return Deletes == null ? string.Empty : string.Join(",", Deletes); } }
    }

    /// <summary>
    ///  Results of the apply edits. The results are grouped by type of edit.
    /// </summary>
    [DataContract]
    public class ApplyEditsResponse : PortalResponse
    {
        [DataMember(Name = "addResults")]
        public List<ApplyEditResponse> Adds { get; set; }

        [DataMember(Name = "updateResults")]
        public List<ApplyEditResponse> Updates { get; set; }

        [DataMember(Name = "deleteResults")]
        public List<ApplyEditResponse> Deletes { get; set; }

        [IgnoreDataMember]
        public int ExpectedAdds { get; private set; }

        [IgnoreDataMember]
        public int ActualAdds { get { return Adds == null ? 0 : Adds.Count; } }

        [IgnoreDataMember]
        public int ActualAddsThatSucceeded { get { return AddsSuccessful.Count(); } }

        [IgnoreDataMember]
        public int ExpectedUpdates { get; private set; }

        [IgnoreDataMember]
        public int ActualUpdates { get { return Updates == null ? 0 : Updates.Count; } }

        [IgnoreDataMember]
        public int ActualUpdatesThatSucceeded { get { return UpdatesSuccessful.Count(); } }

        [IgnoreDataMember]
        public int ExpectedDeletes { get; private set; }

        [IgnoreDataMember]
        public int ActualDeletes { get { return Deletes == null ? 0 : Deletes.Count; } }

        [IgnoreDataMember]
        public int ActualDeletesThatSucceeded { get { return DeletesSuccessful.Count(); } }

        [IgnoreDataMember]
        public IEnumerable<ApplyEditResponse> AddsSuccessful { get { return CheckResults(Adds, true); } }

        [IgnoreDataMember]
        public IEnumerable<ApplyEditResponse> AddsFailed { get { return CheckResults(Adds, false); } }

        [IgnoreDataMember]
        public IEnumerable<ApplyEditResponse> UpdatesSuccessful { get { return CheckResults(Updates, true); } }

        [IgnoreDataMember]
        public IEnumerable<ApplyEditResponse> UpdatesFailed { get { return CheckResults(Updates, false); } }

        [IgnoreDataMember]
        public IEnumerable<ApplyEditResponse> DeletesSuccessful { get { return CheckResults(Deletes, true); } }

        [IgnoreDataMember]
        public IEnumerable<ApplyEditResponse> DeletesFailed { get { return CheckResults(Deletes, false); } }

        IEnumerable<ApplyEditResponse> CheckResults(List<ApplyEditResponse> results, bool success)
        {
            if (results == null || !results.Any()) return Enumerable.Empty<ApplyEditResponse>();

            return results.Where(r => r.Success == success);
        }

        public void SetExpected<T>(ApplyEdits<T> operation) where T : IGeometry<T>
        {
            if (operation == null) return;

            ExpectedAdds = operation.Adds == null ? 0 : operation.Adds.Count;
            ExpectedUpdates = operation.Updates == null ? 0 : operation.Updates.Count;
            ExpectedDeletes = operation.Deletes == null ? 0 : operation.Deletes.Count;
        }
    }

    /// <summary>
    /// Identifies a single feature and indicates if the edit was successful or not.
    /// </summary>
    [DataContract]
    public class ApplyEditResponse : PortalResponse
    {
        [DataMember(Name = "objectId")]
        public long ObjectId { get; set; }

        [DataMember(Name = "globalId")]
        public string GlobalId { get; set; }

        [DataMember(Name = "success")]
        public bool Success { get; set; }
    }
}
