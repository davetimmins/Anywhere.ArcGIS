using Anywhere.ArcGIS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Anywhere.ArcGIS.Operation
{
    [DataContract]
    public class DeleteAttachments : ArcGISServerOperation
    {        
        public DeleteAttachments(ArcGISServerEndpoint endpoint, long objectID, Action beforeRequest = null, Action afterRequest = null)
            : base(endpoint.RelativeUrl.Trim('/') + string.Format("/{0}/{1}", objectID, Operations.DeleteAttachments), beforeRequest, afterRequest)
        {
            AttachmentIds = new List<long>();
        }

        /// <summary>
        /// Gets or sets the IDs of the attachments to be deleted.
        /// </summary>
        [IgnoreDataMember]
        public List<long> AttachmentIds { get; set; }

        [DataMember(Name = "attachmentIds")]
        public string AttachmentIdsValue => AttachmentIds == null || !AttachmentIds.Any() ? string.Empty : string.Join(",", AttachmentIds);
    }

    [DataContract]
    public class DeleteAttachmentsResponse : PortalResponse
    {
        [DataMember(Name = "deleteAttachmentResults")]
        public PostAttachmentResult[] DeleteAttachmentResults { get; set; }
    }

    public interface IAttachment
    {
        byte[] Attachment { get; }

        string FileName { get; }

        string ContentType { get; }
    }

    public class AttachmentToPost : ArcGISServerOperation, IAttachment
    {
        public AttachmentToPost(ArcGISServerEndpoint endpoint, long objectID, string attachmentBase64Encoded, string fileName, string contentType, bool isUpdate = false)
            : this(endpoint, objectID, fileName, contentType, isUpdate)
        {
            if (string.IsNullOrWhiteSpace(attachmentBase64Encoded))
            {
                throw new ArgumentNullException(nameof(attachmentBase64Encoded));
            }

            AttachmentBase64Encoded = attachmentBase64Encoded;
            Attachment = Convert.FromBase64String(AttachmentBase64Encoded);
        }

        public AttachmentToPost(ArcGISServerEndpoint endpoint, long objectID, byte[] attachment, string fileName, string contentType, bool isUpdate = false)
            : this(endpoint, objectID, fileName, contentType, isUpdate)
        {
            Attachment = attachment ?? throw new ArgumentNullException(nameof(attachment));
            AttachmentBase64Encoded = Convert.ToBase64String(Attachment);
        }

        AttachmentToPost(ArcGISServerEndpoint endpoint, long objectID, string fileName, string contentType, bool isUpdate = false, Action beforeRequest = null, Action afterRequest = null)
            : base(endpoint.RelativeUrl.Trim('/') + string.Format("/{0}/{1}", objectID, isUpdate ? "updateAttachment" : "addAttachment"), beforeRequest, afterRequest)
        {
            ObjectID = objectID;
            FileName = fileName;
            ContentType = contentType;
        }

        public long ObjectID { get; private set; }

        public string AttachmentBase64Encoded { get; private set; }

        public byte[] Attachment { get; private set; }

        public string FileName { get; private set; }

        public string ContentType { get; private set; }
    }

    [DataContract]
    public class AddAttachmentResponse : PortalResponse
    {
        [DataMember(Name = "addAttachmentResult")]
        public PostAttachmentResult AddAttachmentResult { get; set; }
    }

    [DataContract]
    public class PostAttachmentResult : PortalResponse
    {
        [DataMember(Name = "objectId")]
        public long ObjectID { get; set; }

        [DataMember(Name = "globalId")]
        public string GlobalID { get; set; }

        [DataMember(Name = "success")]
        public bool Success { get; set; }
    }

    [DataContract]
    public class UpdateAttachmentResponse : PortalResponse
    {
        [DataMember(Name = "updateAttachmentResult")]
        public PostAttachmentResult UpdateAttachmentResult { get; set; }
    }
}
