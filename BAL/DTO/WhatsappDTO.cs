using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class WhatsappDTO : MasterDTO
    {
        public int WhatsappId { get; set; }
        public string Name { get; set; }
        public string App_Id { get; set; }
        public string LanguageCode { get; set; }
        public bool Live { get; set; }
        public string LiveTs { get; set; }
        public string Phone { get; set; }
        public bool Stopped { get; set; }
        public bool TemplateMessaging { get; set; }
        public string Type { get; set; }
        public int Version { get; set; }
        public string Embed_Link { get; set; }
        public string Callback_URL { get; set; }
        public bool IsDefault { get; set; }
        public bool Deleted { get; set; }
        public string AppToken { get; set; }
        public bool SubscriptionEnabled { get; set; }
    }


    public class GupShupToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ValidTill { get; set; }

        public string GuId { get; set; }
    }

    public class WhatsappTemplateMessage
    {
        [JsonIgnore]
        public string AppId { get; set; }
        [JsonIgnore]
        public int CompanyId { get; set; }
        [JsonIgnore]
        public int ClientId { get; set; }

        public string messaging_product { get; set; }
        public string to { get; set; }
        public string type { get; set; }
        public WhatsappTemplateMessageTemplate template { get; set; }
       
    }

    
    public class WhatsappTemplateMessageTemplate
    {
        public string name { get; set; }
        public WhatsappTemplateMessageLanguage language { get; set; }
        public List<WhatsappTemplateMessageComponent> components { get; set; }

    }
    public class WhatsappTemplateMessageLanguage
    {
        public string code { get; set; }
    }
    public class WhatsappTemplateMessageComponent
    {
        public string type { get; set; }
        public List<WhatsappTemplateMessageParamter> parameters { get; set; }
    }
    public class WhatsappTemplateMessageParamter
    {
        public string type { get; set; }
        public string parameter_name { get; set; }


    }

    public class WhatsappTemplateMessageTextParamter : WhatsappTemplateMessageParamter
    {

        public string text { get; set; }

    }
    public class WhatsappTemplateMessageDocumentParamter : WhatsappTemplateMessageParamter
    {
        public string fileName { get; set; }
        public WhatsappTemplateMessageDocumentLinkParamter document { get; set; }

    }
    public class WhatsappTemplateMessageDocumentLinkParamter
    {
      
        public string link { get; set; }

    }
}
