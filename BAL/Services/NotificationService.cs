using BAL.DAL;
using BAL.DTO;
using BAL.Objects;
using BAL.Services.Integrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BAL.Services
{
    public class NotificationService
    {
        SendEmails emails = new SendEmails();
        LoggingService logger = new LoggingService();
        public async Task<bool> Add(NotificationDto dto)
        {
            var dal = new NotificationDAL();
            return await dal.Add(dto);
        }
        public async Task<bool> StatusUpdate(NotificationDto dto)
        {
            var dal = new NotificationDAL();
            return await dal.StatusUpdate(dto);
        }
        public async Task<bool> StatusUpdateByGsId(NotificationDto dto)
        {
            var dal = new NotificationDAL();
            return await dal.StatusUpdateByGsId(dto);
        }
        public async Task<bool> Send(NotificationDto dto)
        {
            return await emails.SendEmail(dto);
        }
        public async Task<bool> UpdateGSId(NotificationDto dto)
        {
            var dal = new NotificationDAL();
            return await dal.UpdateGSId(dto);
        }
        public async Task<NotificationTemplate> GetTemplateByCode(int code, int companyId)
        {
            var dal = new NotificationDAL();
            return await dal.GetTemplateByCode(code, companyId);
        }
        public async Task<IEnumerable<NotificationDto>> GetMyAlerts(NotificationFilterDTO dto)
        {
            var dal = new NotificationDAL();
            return await dal.GetMyAlerts(dto);
        }
        public async Task<bool> SendWhatsapp(NotificationDto dto,
            List<WhatsappTemplateMessageParamter> parameters)
        {


            var wsService = new WhatsappService();
            var message = new WhatsappTemplateMessage();

            message.to = dto.Receipients;
            message.type = "template";

            var template = new WhatsappTemplateMessageTemplate();
            template.name = dto.Subject;
            template.language = new WhatsappTemplateMessageLanguage { code = "en_US" };

            var components = new List<WhatsappTemplateMessageComponent>();
            components.Add(new WhatsappTemplateMessageComponent
            {
                type = "body",
                parameters = parameters
            });

            template.components = components;
            message.template = template;
            message.CompanyId = dto.CompanyId;
            message.ClientId = dto.RbnClientId;

            var result = await wsService.SendTemplateMessage(message);
            if (result != null && result.Count > 0)
            {
                //update gupshup message Id
                dto.gsId = String.Join(",", result);
                dto.wsAppId = message.AppId;
                var updResult = await UpdateGSId(dto);
                if (!updResult)
                {
                    logger.LogError("Could not update gsId in whatsapp notification: ", result.ToArray());
                }
                return true;
            }
            return false;
        }
        public async Task<bool> SendWhatsapp(NotificationDto dto,
            List<WhatsappTemplateMessageComponent> components)
        {


            var wsService = new WhatsappService();
            var message = new WhatsappTemplateMessage();

            message.to = dto.Receipients;
            message.type = "template";

            var template = new WhatsappTemplateMessageTemplate();
            template.name = dto.Name;
            template.language = new WhatsappTemplateMessageLanguage { code = "en" };


            template.components = components;
            message.template = template;
            message.CompanyId = dto.CompanyId;
            message.ClientId = dto.RbnClientId;

            var result = await wsService.SendTemplateMessage(message);
            if (result != null && result.Count > 0)
            {
                //update gupshup message Id
                dto.gsId = String.Join(",", result);
                dto.wsAppId = message.AppId;
                var updResult = await UpdateGSId(dto);
                if (!updResult)
                {
                    logger.LogError("Could not update gsId in whatsapp notification: ", result.ToArray());
                }
                return true;
            }
            return false;
        }


        public async Task<bool> CollectContractReminders()
        {
            var dal = new NotificationDAL();
            return await dal.CollectContractReminders();
        }

        public async Task<bool> CollectPickupReminders()
        {
            var dal = new NotificationDAL();
            var _pickups = await dal.CollectPickkupReminders();
            foreach (var ct in _pickups)
            {
                var notification = new NotificationDto();



                if (ct.RecoveryDate == DateTime.Today)
                {
                    notification.Body = notification.Subject = $" Pick-up from {ct.Site} of the party {ct.Client}  is due today.";
                }
                else
                {
                    notification.Body = notification.Subject = $" Pick-up from {ct.Site} of the party {ct.Client}  is due on {Utils.FormatDate(ct.RecoveryDate)}.";
                }

                notification.Type = "al";
                notification.Receipients = "NA";
                notification.CompanyId = ct.CompanyId;
                notification.Sender = "0";
                notification.Category = "pickups";
                notification.CreatedOn = DateTime.Now;
                notification.CreatedBy = 0;
                notification.GuId = Guid.NewGuid().ToString();

                await this.Add(notification);

            }
            return true;
        }
    }
}

