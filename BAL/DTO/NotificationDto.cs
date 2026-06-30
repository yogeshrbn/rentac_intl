using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class NotificationDto : MasterDTO
    {
        public int NotificationId { get; set; }
        public string Type { get; set; }
        public string Body { get; set; }
        public string EmailBody { get; set; }
        public string SMSBody { get; set; }

        public string Receipients { get; set; }
        public string CopyTo { get; set; }
        public string Sender { get; set; }
        public string Subject { get; set; }
        public string Name { get; set; }

        public string Category { get; set; }
        public DateTime SentOn { get; set; }
        public DateTime DeliveredOn { get; set; }
        public DateTime FailedOn { get; set; }
        public string Status { get; set; }
        public string DeliveryMessage { get; set; }
        public List<string> Attachment { get; set; }
        /// <summary>
        /// information about what to send, attachment etc.
        /// </summary>
        public string MetaData { get; set; }

        public int TemplateCode { get; set; }
        public List<AttachmentDoc> AttachmentDocs { get; set; }

        public string wsAppId { get; set; }
        public string gsId { get; set; }
        public string wsId { get; set; }
        public string conversationId { get; set; }
        public bool billable { get; set; }
        public string ws_msgType { get; set; }
        public string ws_model { get; set; }
        public string ws_source { get; set; }

        public string ReceipientUserId { get; set; }
        public string route { get; set; }

        public string Token { get; set; }
        public string RentacApiKey { get; set; }

    }

    public class AttachmentDoc
    {
        public byte[] Buffer { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
    }

    public class NotificationFilterDTO
    {
      public int ReceipientUserId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
         
    }

    public class NotificationTemplate : MasterDTO
    {
        public string Subject { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }

        public bool IsActive { get; set; }
        public bool Approved { get; set; }
        public string Body { get; set; }
        public string EmailBody { get; set; }
        public string SmsBody { get; set; }


    }


}
