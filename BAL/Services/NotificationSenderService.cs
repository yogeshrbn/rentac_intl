using BAL.DAL;
using BAL.DTO;
using BAL.Objects;
using BAL.Services.Contracts;
using BAL.Services.Integrations;
using BAL.Services.Integrations.Whatsapp;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services
{
    public class NotificationSenderService : INotificationSenderService
    {
        private const string Msg91FlowUrl = "https://control.msg91.com/api/v5/flow";

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly SendEmails _emailSender = new SendEmails();
        private readonly WhatsappService _whatsappService = new WhatsappService();
        private readonly IWhatsappTemplateProvider _gupshupWhatsapp;
        private readonly IWhatsappTemplateProvider _doubleTickWhatsapp;
        private readonly NotificationDAL _notificationDal = new NotificationDAL();
        private readonly LoggingService _legacyLog = new LoggingService();
        private readonly CompanyDAL companyService = new CompanyDAL();
        public NotificationSenderService()
        {
            _gupshupWhatsapp = new GupshupWhatsappTemplateProvider(_whatsappService);
            _doubleTickWhatsapp = new DoubleTickWhatsappTemplateProvider();
            companyService = new CompanyDAL();
        }

        public async Task<bool> SendEmailAsync(NotificationDto dto)
        {
            try
            {
                return await _emailSender.SendEmail(dto).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error sending email notification: {@Dto}", dto);
                throw;
            }
        }

        public async Task<bool> SendSmsAsync(NotificationDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Receipients))
                return false;

            var templateId = dto.Subject;
            if (string.IsNullOrWhiteSpace(templateId))
            {
                _logger.Warn("SMS skipped: NotificationDto.Subject must contain Msg91 template id.");
                return false;
            }

            IDictionary<string, string> variables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(dto.MetaData))
            {
                try
                {
                    var parsed = JsonConvert.DeserializeObject<Dictionary<string, string>>(dto.MetaData);
                    if (parsed != null)
                    {
                        foreach (var kv in parsed)
                            variables[kv.Key] = kv.Value;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "SMS MetaData must be JSON key/value pairs for Msg91 variables.");
                    return false;
                }
            }

            return await SendSmsAsync(dto.Receipients, templateId, variables).ConfigureAwait(false);
        }

        public async Task<bool> SendSmsAsync(string recipients, string templateId, IDictionary<string, string> variables)
        {
            if (string.IsNullOrWhiteSpace(recipients) || string.IsNullOrWhiteSpace(templateId))
                return false;

            variables = variables ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                var key = ConfigurationManager.AppSettings["msg91AuthKey"];
                if (string.IsNullOrWhiteSpace(key))
                {
                    _logger.Error("msg91AuthKey is missing in configuration.");
                    return false;
                }

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var mobiles = NormalizeIndianMobiles(recipients);
                if (string.IsNullOrEmpty(mobiles))
                {
                    _logger.Warn("No valid phone numbers after normalization: {Recipients}", recipients);
                    return false;
                }

                var recipientRow = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["mobiles"] = mobiles
                };
                foreach (var kv in variables)
                    recipientRow[kv.Key] = kv.Value;

                var payload = new
                {
                    template_id = templateId,
                    short_url = "0",
                    short_url_expiry = "seconds",
                    recipients = new[] { recipientRow },
                    realTimeResponse = 1
                };

                var json = JsonConvert.SerializeObject(payload);
                using (var client = new HttpClient())
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.TryAddWithoutValidation("authkey", key);

                    var response = await client.PostAsync(Msg91FlowUrl, content).ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                    {
                        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        _logger.Error("Msg91 SMS failed: {Status} {Body}", response.StatusCode, body);
                    }
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error sending SMS via Msg91 for template {TemplateId}", templateId);
                throw;
            }
        }

        public async Task<bool> SendWhatsappAsync(NotificationDto dto, List<WhatsappTemplateMessageParamter> parameters)
        {
            try
            {
                var message = new WhatsappTemplateMessage
                {
                    to = dto.Receipients,
                    type = "template",
                    template = new WhatsappTemplateMessageTemplate
                    {
                        name = dto.Subject,
                        language = new WhatsappTemplateMessageLanguage { code = "en_US" },
                        components = new List<WhatsappTemplateMessageComponent>
                        {
                            new WhatsappTemplateMessageComponent
                            {
                                type = "body",
                                parameters = parameters
                            }
                        }
                    },
                    CompanyId = dto.CompanyId,
                    ClientId = dto.RbnClientId
                };

                return await SendWhatsappWithProviderAsync(dto, message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error sending WhatsApp (parameter template): {@Dto}", dto);
                throw;
            }
        }

        public async Task<bool> SendWhatsappAsync(NotificationDto dto, List<WhatsappTemplateMessageComponent> components)
        {
            try
            {
                var message = new WhatsappTemplateMessage
                {
                    to = dto.Receipients,
                    type = "template",
                    template = new WhatsappTemplateMessageTemplate
                    {
                        name = dto.Name,
                        language = new WhatsappTemplateMessageLanguage { code = "en" },
                        components = components
                    },
                    CompanyId = dto.CompanyId,
                    ClientId = dto.RbnClientId
                };

                return await SendWhatsappWithProviderAsync(dto, message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error sending WhatsApp (component template): {@Dto}", dto);
                throw;
            }
        }

        private IWhatsappTemplateProvider ResolveWhatsappProvider(NotificationDto dto)
        {
            var comp = new Company(dto.CompanyId).GetDetails();
            if (String.IsNullOrEmpty(comp.GupShupKey) && String.IsNullOrEmpty(comp.DoubleTickKey))
            {
                string className = this.GetType().Name;
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Exception($"[{className}.{methodName}] Whatsapp Provider not configured");
            }
            //var raw = string.IsNullOrWhiteSpace(dto?.ws_source)
            //    ? ConfigurationManager.AppSettings["defaultWhatsappProvider"]
            //    : dto.ws_source;

            //if (string.IsNullOrWhiteSpace(raw))
            //    return _gupshupWhatsapp;

            //var key = raw.Trim().ToLowerInvariant();
            if (comp.WhatsAppProvider == "doubletick")
            {
                if (String.IsNullOrEmpty(comp.DoubleTickKey))
                {
                    string className = this.GetType().Name;
                    string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    throw new Exception($"[{className}.{methodName}] Whatsapp Provider Key not configured");
                }

                _doubleTickWhatsapp.ApiKey = comp.DoubleTickKey;
                return _doubleTickWhatsapp;
            }

            return _gupshupWhatsapp;
        }

        private async Task<bool> SendWhatsappWithProviderAsync(NotificationDto dto, WhatsappTemplateMessage message)
        {
            var provider = ResolveWhatsappProvider(dto);
            dto.ws_source = provider.Name;

            var result = await provider.SendTemplateMessageAsync(message).ConfigureAwait(false);
            if (result != null && result.Count > 0)
            {
                dto.gsId = string.Join(",", result);
                dto.wsAppId = message.AppId;
                var updResult = await _notificationDal.UpdateGSId(dto).ConfigureAwait(false);
                if (!updResult)
                    _legacyLog.LogError("Could not update gsId in whatsapp notification: ", result.ToArray());
                return true;
            }
            return false;
        }

        private static string NormalizeIndianMobiles(string recipients)
        {
            var phones = recipients.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            var mobiles = new List<string>();
            foreach (var phone in phones)
            {
                var ph = phone.Trim();
                if (ph.Length >= 10)
                {
                    ph = ph.Substring(ph.Length - 10, 10);
                    mobiles.Add("91" + ph);
                }
            }
            return mobiles.Count > 0 ? string.Join(",", mobiles) : null;
        }
    }
}
