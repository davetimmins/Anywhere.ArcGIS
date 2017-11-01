using Anywhere.ArcGIS.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace Anywhere.ArcGIS.Operation
{
    /// <summary>
    /// The Query Attachments operation is performed on a feature service layer resource. 
    /// The result of this operation are attachments grouped by the source feature object Ids and global ids (if they exist).
    /// </summary>
    [DataContract]
    public class QueryAttachments : ArcGISServerOperation
    {
        public QueryAttachments(ArcGISServerEndpoint endpoint, Action beforeRequest = null, Action afterRequest = null)
            : base(endpoint.RelativeUrl.Trim('/') + "/" + Operations.QueryAttachments, beforeRequest, afterRequest)
        {
            ObjectIds = new List<long>();
            GlobalIds = new List<string>();
            SizeRange = new List<int>();
        }

        /// <summary>
        /// The definition expression to be applied to the related layer/table. 
        /// From the list of records that are related to the specified objectIds,
        /// only those records that conform to this expression will be returned.
        /// </summary>
        [DataMember(Name = "definitionExpression")]
        public string DefinitionExpression { get; set; }

        /// <summary>
        /// The object IDs of this layer/table to be queried.
        /// </summary>
        [IgnoreDataMember]
        public List<long> ObjectIds { get; set; }

        /// <summary>
        /// The list of object Ids to be queried. This list is a comma delimited list of field names.
        /// </summary>
        [DataMember(Name = "objectIds")]
        public string ObjectIdsValue { get { return ObjectIds == null || !ObjectIds.Any() ? null : string.Join(",", ObjectIds); } }

        /// <summary>
        /// The global IDs of this layer/table to be queried
        /// </summary>
        [IgnoreDataMember]
        public List<string> GlobalIds { get; set; }

        /// <summary>
        /// The list of global Ids to be queried. This list is a comma delimited list of field names.
        /// </summary>
        [DataMember(Name = "globalIds")]
        public string GlobalIdsValue { get { return GlobalIds == null || !GlobalIds.Any() ? null : string.Join(",", GlobalIds); } }

        /// <summary>
        /// The file format that is supported by query attachment.
        /// </summary>
        [DataMember(Name = "attachmentTypes")]
        public string AttachmentTypes { get; set; }

        /// <summary>
        /// The file size of the attachment is specified in bytes. 
        /// You can enter a file size range (1000,15000) to query for attachments with the specified range.
        /// </summary>
        [IgnoreDataMember]
        public List<int> SizeRange { get; set; }

        /// <summary>
        /// The list of object Ids to be queried. This list is a comma delimited list of field names.
        /// </summary>
        [DataMember(Name = "size")]
        public string SizeRangeValue
        {
            get
            {
                if (SizeRange?.Count > 0 && SizeRange?.Count != 2)
                {
                    throw new ArgumentOutOfRangeException(nameof(SizeRange), "If you want to use the SizeRange parameter then you need to supply exactly 2 values: the lower and upper range");
                }

                return SizeRange == null || !SizeRange.Any() ? null : string.Join(",", SizeRange);
            }
        }

        /// <summary>
        /// This option fetches query results by skipping a specified number of records. 
        /// The query results start from the next record (i.e., resultOffset + 1). 
        /// The default value is 0.
        ///This parameter only applies when supportPagination is true.
        ///You can use this option to fetch records that are beyond maxRecordCount.
        /// </summary>
        [DataMember(Name = "resultOffset")]
        public int? ResultOffset { get; set; }

        /// <summary>
        /// This option fetches query results up to the resultRecordCount specified. 
        /// When resultOffset is specified and this parameter is not, the feature service defaults to the maxRecordCount. 
        /// The maximum value for this parameter is the value of the layer's maxRecordCount property.
        /// This parameter only applies if supportPagination is true.
        /// </summary>
        [DataMember(Name = "resultRecordCount")]
        public int? ResultRecordCount { get; set; }
    }

    [DataContract]
    public class QueryAttachmentsResponse : PortalResponse
    {
        [DataMember(Name = "fields")]
        public List<Field> Fields { get; set; }

        [DataMember(Name = "attachmentGroups")]
        public List<AttachmentGroup> AttachmentGroups { get; set; }
    }

    [DataContract]
    public class AttachmentGroup
    {
        [DataMember(Name = "parentObjectId")]
        public long ParentObjectId { get; set; }

        [DataMember(Name = "parentGlobalId")]
        public string ParentGlobalId { get; set; }

        [DataMember(Name = "attachmentInfos")]
        public List<AttachmentInfo> AttachmentInfos { get; set; }
    }

    [DataContract]
    public class AttachmentInfo
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "globalId")]
        public string GlobalId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "contentType")]
        public string ContentType { get; set; }

        [DataMember(Name = "size")]
        public long Size { get; set; }

        [IgnoreDataMember]
        public string SafeFileName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Name))
                {
                    return string.Empty;
                }

                var r = new Regex(string.Format("[{0}]", Regex.Escape(new string(Path.GetInvalidFileNameChars()))));
                return r.Replace(Name, string.Empty);
            }
        }
    }
}
