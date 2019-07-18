using Anywhere.ArcGIS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Anywhere.ArcGIS.Operation
{
    /// <summary>
    /// This operation adds, updates and deletes features to the associated feature layer or table in a single call (POST only).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public class ApplyEdits<T> : ArcGISServerOperation
        where T : IGeometry
    {
        public ApplyEdits(string relativeUrl, Action beforeRequest = null, Action afterRequest = null)
            : this(relativeUrl.AsEndpoint(), beforeRequest, afterRequest)
        { }

        public ApplyEdits(ArcGISServerEndpoint endpoint, Action beforeRequest = null, Action afterRequest = null)
            : base(endpoint.RelativeUrl.Trim('/') + "/" + Operations.ApplyEdits, beforeRequest, afterRequest)
        {
            Adds = new List<Feature<T>>();
            Updates = new List<Feature<T>>();
            Deletes = new List<long>();
            DeleteGlobalIds = new List<Guid>();
            RollbackOnFailure = true;
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

        /// <summary>
        ///  The Global IDs of this layer / table to be deleted. Use if useGlobalIds is true
        /// </summary>
        [IgnoreDataMember]
        public List<Guid> DeleteGlobalIds { get; set; }

        [DataMember(Name = "deletes")]
        public string DeleteIds
        {
            get
            {
                /* Return in form array of quoted, braced GUIDS - examples:
                []
                ['{509caea1-7a3f-444d-a0ef-81c942474624}']
                ['{509caea1-7a3f-444d-a0ef-81c942474624}','{701a68ab-86df-4244-a9cb-dda10028f528}']
                */
                if (UseGlobalIds)
                {
                    var deleteIds = new StringBuilder("[");
                    if (DeleteGlobalIds != null && DeleteGlobalIds.Any())
                    {
                        foreach (var deleteGlobalId in DeleteGlobalIds)
                        {
                            deleteIds.AppendFormat("'{0:B}',", deleteGlobalId);
                        }
                        deleteIds.Remove(deleteIds.Length - 1, 1);
                    }
                    deleteIds.Append("]");
                    return deleteIds.ToString();
                }
                else
                {
                    return Deletes == null ? string.Empty : string.Join(",", Deletes);
                }
            }
        }

        /// <summary>
        /// Geodatabase version to apply the edits. This parameter applies only if the isDataVersioned property of the layer is true.
        /// If the gdbVersion parameter is not specified, edits are made to the published map’s version.
        /// This option was added at 10.1.
        /// </summary>
        [DataMember(Name = "gdbVersion")]
        public string GeodatabaseVersion { get; set; }

        /// <summary>
        /// Optional parameter specifying whether the response will report the time features were deleted.
        /// If returnEditMoment = true, the server will report the time in the response's editMoment key.
        /// The default value is false.
        /// This option was added at 10.5 and works with ArcGIS Server services only.
        /// </summary>
        [DataMember(Name = "returnEditMoment")]
        public bool ReturnEditMoment { get; set; }

        /// <summary>
        /// Optional parameter to specify if the edits should be applied only if all submitted edits succeed.
        /// If false, the server will apply the edits that succeed even if some of the submitted edits fail.
        /// If true, the server will apply the edits only if all edits succeed. The default value is true.
        /// Not all data supports setting this parameter.
        /// Query the supportsRollbackonFailureParameter property of the layer to determine whether or not a layer supports setting this parameter.
        /// If supportsRollbackOnFailureParameter = false for a layer, then when editing this layer, rollbackOnFailure will always be true, regardless of how the parameter is set.
        /// However, if supportsRollbackonFailureParameter = true, this means the rollbackOnFailure parameter value will be honored on edit operations.
        /// This option was added at 10.1.
        /// </summary>
        [DataMember(Name = "rollbackOnFailure")]
        public bool RollbackOnFailure { get; set; }

        /// <summary>
        /// When set to true, the features and attachments in the adds, updates, deletes, and attachments parameters are identified by their globalIds.
        /// When true, the service adds the new features and attachments while preserving the globalIds submitted in the payload.
        /// If the globalId of a feature (or an attachment) collides with a pre-existing feature (or an attachment), that feature and/or attachment add fails.
        /// Other adds, updates, or deletes are attempted if rollbackOnFailure is false.
        /// If rollbackOnFailure is true, the whole operation fails and rolls back on any failure including a globalId collision.
        /// When useGlobalIds is true, updates and deletes are identified by each feature or attachment globalId rather than their objectId or attachmentId.
        /// This option was added at 10.4.
        /// </summary>
        [DataMember(Name = "useGlobalIds")]
        public bool UseGlobalIds { get; set; }
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

        public void SetExpected<T>(ApplyEdits<T> operation)
            where T : IGeometry
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

        [DataMember(Name = "editMoment")]
        public string EditMoment { get; set; }
    }
}
