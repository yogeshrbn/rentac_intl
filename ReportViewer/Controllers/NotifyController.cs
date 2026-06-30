using BAL.DTO;
using BAL.Objects;
using BAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Services.Description;
using BAL.Common;
using System.Data;
using BAL.Services.Contracts;
namespace ReportViewer.Controllers
{
    [CustomActionFilter]

    [System.Web.Mvc.Authorize]
    [System.Web.Http.RoutePrefix("api/[controller]")]
    public class NotifyController : ApiController
    {
        NotificationService service = new NotificationService();
        LoggingService logger = new LoggingService();
        AzureStorageService azService = new AzureStorageService();
        private readonly INotificationSenderService _notificationSenderService;

        public NotifyController(INotificationSenderService notificationSenderService)
        {
            _notificationSenderService = notificationSenderService;
        }

        [System.Web.Http.HttpPost]
        [RequireApiKeyHeader]
        public async Task<ApiResponseMessage> sendNotification([FromBody] NotificationDto dto)
        {
            var res = new ApiResponseMessage();

            try
            {
                var apiKey = Request?.Headers?
                    .Where(h => h.Key.Equals("ApiKey", StringComparison.OrdinalIgnoreCase))
                    .SelectMany(h => h.Value)
                    .FirstOrDefault();

                var compDto = new Company().ApiKeyExists(apiKey);
                if (compDto == null)
                {
                    res.Code = ApiResponseMessageCodes.ERROR;
                    res.Message = "Invalid ApiKey";
                    return res;
                }

                ApplyNotificationMode(dto, compDto.CompanyId);

                var user = new LoggedInUser();
                user.UserId = 0;
                user.DefaultCompanyId = compDto.CompanyId;
                user.FinYearId = 0;
                user.Phone = compDto.Phone1;

                // apiKey contains the request api-key header value.
                return await NotifyMessage(dto, user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message, ex.StackTrace);
                res.Code = ApiResponseMessageCodes.ERROR;
                res.Message = ex.Message;
                return res;
            }
        }

        private void ApplyNotificationMode(NotificationDto dto, int companyId)
        {
            if (dto == null || companyId <= 0)
            {
                return;
            }

            var cfg = new Config();
            var list = cfg.GetConfig(companyId, "general", "notifications");
            if (list == null || list.Count == 0)
            {
                return;
            }

            var mode = list.FirstOrDefault(o => o.Key != null &&
                o.Key.Equals("mode", StringComparison.OrdinalIgnoreCase))?.Value;
            if (!"Test".Equals(mode, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if ("whatsapp".Equals(dto.Type, StringComparison.OrdinalIgnoreCase))
            {
                var testPhone = list.FirstOrDefault(o => o.Key != null &&
                    o.Key.Equals("testPhoneMobile", StringComparison.OrdinalIgnoreCase))?.Value;
                if (!string.IsNullOrWhiteSpace(testPhone))
                {
                    dto.Receipients = testPhone;
                }
            }
            else if ("email".Equals(dto.Type, StringComparison.OrdinalIgnoreCase))
            {
                var testEmail = list.FirstOrDefault(o => o.Key != null &&
                    o.Key.Equals("testEmail", StringComparison.OrdinalIgnoreCase))?.Value;
                if (!string.IsNullOrWhiteSpace(testEmail))
                {
                    dto.Receipients = testEmail;
                }
            }
        }

        /// <summary>
        /// Sends a bill payment due reminder
        /// template name: bill_payment_due_reminder_today
        /// Code:1110
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        [RequireApiKeyHeader]
        public async Task<ApiResponseMessage> paymentDueTodayReminder([FromBody] NotificationDto dto)
        {
            var res = new ApiResponseMessage();

            try
            {
                var apiKey = Request?.Headers?
                    .Where(h => h.Key.Equals("ApiKey", StringComparison.OrdinalIgnoreCase))
                    .SelectMany(h => h.Value)
                    .FirstOrDefault();

                var compDto = new Company().ApiKeyExists(apiKey);

                var user = new LoggedInUser();
                user.UserId = 0;
                user.DefaultCompanyId = compDto.CompanyId;
                user.FinYearId = 0;
                user.Phone = compDto.Phone1;

                // apiKey contains the request api-key header value.
                return await NotifyMessage(dto, user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message, ex.StackTrace);
                res.Code = ApiResponseMessageCodes.ERROR;
                res.Message = ex.Message;
                return res;
            }
        }

        [System.Web.Http.HttpPost]
        [RequireApiKeyHeader]
        public async Task<ApiResponseMessage> balancePaymentReminder([FromBody] NotificationDto dto)
        {
            var res = new ApiResponseMessage();

            try
            {
                var apiKey = Request?.Headers?
                    .Where(h => h.Key.Equals("ApiKey", StringComparison.OrdinalIgnoreCase))
                    .SelectMany(h => h.Value)
                    .FirstOrDefault();

                var compDto = new Company().ApiKeyExists(apiKey);

                var user = new LoggedInUser();
                user.UserId = 0;
                user.DefaultCompanyId = compDto.CompanyId;
                user.FinYearId = 0;
                user.Phone = compDto.Phone1;



                // apiKey contains the request api-key header value.
                return await NotifyMessage(dto, user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message, ex.StackTrace);
                res.Code = ApiResponseMessageCodes.ERROR;
                res.Message = ex.Message;
                return res;
            }
        }


        [System.Web.Http.HttpPost]
        public async Task<ApiResponseMessage> Notify([FromBody] NotificationDto dto)
        {

            var user = new LoggedInUser();
            return await NotifyMessage(dto, user);

        }

        private async Task<ApiResponseMessage> NotifyMessage(NotificationDto dto,
            LoggedInUser user)
        {
            var res = new ApiResponseMessage();
            try
            {
                if (dto == null)
                {
                    res.Code = ApiResponseMessageCodes.BAD_REQUEST;
                    res.Message = "Invalid input request";
                    return res;
                }

                var noticationService = new NotificationService();


                dto.CompanyId = user.DefaultCompanyId;

                var company = new Company(dto.CompanyId);
                var cDto = company.GetDetails();
                dto.CreatedBy = user.UserId;
                dto.FinYearId = user.FinYearId;
                dto.Sender = user.Phone;
                dto.CreatedOn = DateTime.Now;
                dto.GuId = Guid.NewGuid().ToString();

                var _meta = dto.MetaData.Split(',');


                dto.TemplateCode = Convert.ToInt16(_meta[0]);

                var notiTemplate = await service.GetTemplateByCode(dto.TemplateCode, dto.CompanyId);
                if (notiTemplate == null)
                {
                    throw new Exception("Template not found or not configured");
                }
                if (!notiTemplate.Approved)
                {
                    throw new Exception("Template 1011 is not approved by Meta");
                }
                if (dto.Type == "whatsapp")
                {
                    dto.Name = dto.Subject = notiTemplate.Name;
                    dto.Body = notiTemplate.Body;
                }
                else if (dto.Type == "email")
                {
                    dto.Body = notiTemplate.EmailBody;
                }

                if (String.IsNullOrEmpty(dto.Subject))
                {
                    dto.Subject = notiTemplate.Subject;
                }
                dto.Name = notiTemplate.Name;

                //  dto.Type = "email";
                dto.Category = "utility";

                var result = await service.Add(dto);
                if (result)
                {
                    if (dto.Type == "whatsapp")
                    {
                        dto.Receipients = dto.Receipients;
                        var d = await SendWhatsapp(dto);
                        if (d == true)
                        {
                            res.Code = ApiResponseMessageCodes.SUCCESS;
                            res.Message = "Message Sent Successfully";

                        }
                    }
                    if (dto.Type == "email")
                    {
                        var d = await SendEmail(dto);
                        if (d == true)
                        {
                            res.Code = ApiResponseMessageCodes.SUCCESS;
                            res.Message = "Message Sent Successfully";

                        }
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                res.Code = ApiResponseMessageCodes.ERROR;
                res.Message = ex.Message;
                logger.LogError(ex, "Error while sending email", ex.Message);
                return res;
            }
        }

        /// <summary>
        /// Called from azure function
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ApiResponseMessage> SendNotificationV1([FromBody] NotificationDto dto)
        {
            var res = new ApiResponseMessage();
            try
            {
                return await NotifyMessage(dto, new LoggedInUser());
            }
            catch (Exception ex)
            {
                res.Code = ApiResponseMessageCodes.ERROR;
                res.Message = ex.Message;
                logger.LogError(ex, "Error while sending email", ex.Message);
                return res;
            }

        }

        #region Email 
        async Task<bool> SendEmail(NotificationDto dto)
        {
            switch (dto.TemplateCode)
            {
                case 1100:
                    return await SendEmailCode1101(dto);
                case 1101:
                    return await SendEmailCode1101(dto);
                case 1102:
                    return await SendEmailCode1102(dto);
                case 1103:
                    return await SendEmailCode1103(dto);
            }
            return false;
        }
        async Task<bool> SendEmailCode1101(NotificationDto dto)
        {
            Billing billing = new Billing();
            LoggedInUser user = new LoggedInUser();
            try
            {
                var invId = dto.MetaData.Split(',')[1];
                var bills = await billing.GetBillsByIds(invId, user.DefaultCompanyId);

                if (bills != null && bills.Count() > 0)
                {
                    var service = new NotificationService();
                    var bill = bills.First();
                    var company = new Company(bill.CompanyId);
                    var cdto = company.GetDetails();
                    dto.Body = dto.Body.Replace("{customer_name}", bill.Client);
                    dto.Body = dto.Body.Replace("{amount}", bill.Total.ToString());
                    dto.Body = dto.Body.Replace("{bill_period}", bill.From.ToString("dd MMM yyy") + " To " + bill.To.ToString("dd MMM yyy"));
                    dto.Body = dto.Body.Replace("{company}", bill.Company);
                    dto.Body = dto.Body.Replace("{invoice_number}", bill.InvoiceNumber);
                    dto.Body = dto.Body.Replace("{invoice_date}", bill.InvoiceDate.ToString("dd MMM yyy"));
                    dto.Body = dto.Body.Replace("{due_date}", bill.InvoiceDate.ToString("dd MMM yyy"));
                    dto.Body = dto.Body.Replace("{company_email}", cdto.Email);

                    dto.Body = dto.Body.Replace("{company_phone}", cdto.Phone1);
                    dto.Body = dto.Body.Replace("{link}", AzureStorageService.ContainerBaseUrl + bill.FinYearId + "/" + bill.CompanyId + "/bills/" + bill.FileName + ".pdf");


                    return await service.Send(dto);
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return false;
            }
        }
        async Task<bool> SendEmailCode1102(NotificationDto dto)
        {
            Billing billing = new Billing();
            LoggedInUser user = new LoggedInUser();
            try
            {
                var invId = dto.MetaData.Split(',')[1];
                var bill = await billing.QuotationById(Convert.ToInt32(invId), user.DefaultCompanyId);

                if (bill != null)
                {
                    var service = new NotificationService();

                    var company = new Company(bill.CompanyId);
                    var cdto = company.GetDetails();
                    var client = new Ledger(bill.LedgerId).GetDetails();
                    dto.Body = dto.Body.Replace("{customer_name}", client.Name);
                    dto.Body = dto.Body.Replace("{amount}", bill.Total.ToString());
                    dto.Body = dto.Body.Replace("{company}", cdto.Name);
                    dto.Body = dto.Body.Replace("{quotation_number}", bill.QuotationNumber);
                    dto.Body = dto.Body.Replace("{quotation_date}", bill.QuotationDate.ToString("dd MMM yyy"));

                    dto.Body = dto.Body.Replace("{company_email}", cdto.Email);

                    dto.Body = dto.Body.Replace("{company_phone}", cdto.Phone1);
                    string fileName = bill.QuotationNumber + bill.LedgerId.ToString() + ".pdf";
                    dto.Body = dto.Body.Replace("{link}", AzureStorageService.ContainerBaseUrl + bill.FinYearId + "/" + bill.CompanyId + "/quots/" + fileName);


                    return await service.Send(dto);
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return false;
            }
        }
        async Task<bool> SendEmailCode1103(NotificationDto dto)
        {
            Billing billing = new Billing();
            LoggedInUser user = new LoggedInUser();
            try
            {
                var workOrderId = Convert.ToInt32(dto.MetaData.Split(',')[1]);

                var wOrder = new WorkOrder(workOrderId);



                if (wOrder != null)
                {
                    var service = new NotificationService();
                    var cdto = new Company(wOrder.CompanyId).GetDetails();
                    var client = new Ledger(wOrder.LedgerId).GetDetails();


                    dto.Body = dto.Body.Replace("{client}", client.Name);
                    dto.Body = dto.Body.Replace("{challan_no}", wOrder.Number.ToString());
                    dto.Body = dto.Body.Replace("{challan_date}", wOrder.WorkOrderDate.ToString("dd MMM yyy"));
                    dto.Body = dto.Body.Replace("{ship_address}", wOrder.Site);
                    dto.Body = dto.Body.Replace("{company_name}", cdto.Name);

                    dto.Body = dto.Body.Replace("{company_email}", cdto.Email);

                    dto.Body = dto.Body.Replace("{company_phone}", cdto.Phone1);
                    dto.Body = dto.Body.Replace("{company_website}", cdto.Web);

                    string fileName = wOrder.Number + wOrder.LedgerId.ToString() + ".pdf";
                    dto.Body = dto.Body.Replace("{link}", AzureStorageService.ContainerBaseUrl + wOrder.FinYearId + "/" + wOrder.CompanyId + "/challan/" + fileName);


                    return await service.Send(dto);
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return false;
            }
        }
        #endregion
        #region whatsapp

        async Task<bool> SendWhatsapp(NotificationDto dto)
        {
            switch (dto.TemplateCode)
            {
                case 1100:
                    return await SendCode1100(dto);
                case 1101:
                    return await SendCode1101(dto);
                case 1102:
                    return await SendCode1102(dto);
                case 1103:
                    return await SendCode1103(dto);
                case 1104://scaffolding_installation_confirmation_v2
                    return await SendCode1104(dto);
                case 1105://installation_due_tomorrow_reminder_v3
                    return await SendCode1105(dto);
                case 1106://scaffolding_dismantle_reminder_v2
                    return await SendCode1106(dto);
                case 1107://scaffolding_dismantle_delay_reminder
                    return await SendCode1107(dto);
                case 1108: //inward confirmation
                    return await SendCode1108(dto);
                case 1109: //dispatched
                    return await SendCode1109(dto);
                case 1110://bill_payment_due_reminder_today
                    return await SendCode1110(dto);
            }
            return false;
        }
        /// <summary>
        /// whatsapp template bill_generated
        /// </summary>
        /// <returns></returns>
        async Task<bool> SendCode1100(NotificationDto dto)
        {
            Billing billing = new Billing();
            LoggedInUser user = new LoggedInUser();
            try
            {
                var invId = dto.MetaData.Split(',')[1];
                var bills = await billing.GetBillsByIds(invId, user.DefaultCompanyId);

                if (bills != null && bills.Count() > 0)
                {
                    var service = new NotificationService();
                    var bill = bills.First();
                    var parameters = new List<WhatsappTemplateMessageParamter>();
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "customer_name",
                        text = bill.Client
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "amount",
                        text = bill.Total.ToString()
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "bill_period",
                        text = bill.From.ToString("dd MMM yyy") + " To " + bill.To.ToString("dd MMM yyy")
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "customer_signature",
                        text = bill.Company
                    });
                    var components = new List<WhatsappTemplateMessageComponent>();


                    var documentParameter = new List<WhatsappTemplateMessageParamter>();
                    documentParameter.Add(new WhatsappTemplateMessageDocumentParamter
                    {
                        type = "document",
                        document = new WhatsappTemplateMessageDocumentLinkParamter
                        {
                            link = AzureStorageService.ContainerBaseUrl + bill.FinYearId + "/" + bill.CompanyId + "/bills/" + bill.FileName + ".pdf"
                        }
                    });
                    // link = AzureStorageService.ContainerBaseUrl + bill.FinYearId + "/" + bill.CompanyId + "/bills/" + bill.FileName + ".pdf"

                    components.Add(new WhatsappTemplateMessageComponent
                    {
                        type = "header",
                        parameters = documentParameter

                    });
                    components.Add(new WhatsappTemplateMessageComponent
                    {
                        type = "body",
                        parameters = parameters

                    });
                    var comp = new Company(dto.CompanyId).GetDetails();
                    if (comp.WhatsAppProvider != null)
                    {
                        dto.ws_source = comp.WhatsAppProvider;
                    }
                    return await _notificationSenderService.SendWhatsappAsync(dto, parameters);
                    // return await service.SendWhatsapp(dto, components);
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return false;
            }
        }
        /// <summary>
        /// whatsapp template bill_generated
        /// </summary>
        /// <returns></returns>
        async Task<bool> SendCode1101(NotificationDto dto)
        {
            Billing billing = new Billing();
            LoggedInUser user = new LoggedInUser();
            try
            {
                var invId = dto.MetaData.Split(',')[1];
                var bills = await billing.GetBillsByIds(invId, user.DefaultCompanyId);

                if (bills != null && bills.Count() > 0)
                {
                    var service = new NotificationService();
                    var bill = bills.First();
                    var parameters = new List<WhatsappTemplateMessageParamter>();
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "customer_name",
                        text = bill.Client
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "company_name",
                        text = bill.Company
                    });

                    return await service.SendWhatsapp(dto, parameters);
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// whatsapp template quotation
        /// </summary>
        /// <returns></returns>
        async Task<bool> SendCode1102(NotificationDto dto)
        {
            Billing billing = new Billing();
            LoggedInUser user = new LoggedInUser();
            try
            {
                var invId = dto.MetaData.Split(',')[1];


                var bill = await billing.QuotationById(Convert.ToInt32(invId), user.DefaultCompanyId);

                if (bill != null)
                {
                    var service = new NotificationService();

                    var company = new Company(bill.CompanyId);
                    var cdto = company.GetDetails();
                    var client = new Ledger(bill.LedgerId).GetDetails();
                    var parameters = new List<WhatsappTemplateMessageParamter>();

                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "customer_name",
                        text = client.Name
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "quotation_no",
                        text = bill.QuotationNumber
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "project_name",
                        text = bill.Project ?? bill.SiteAddress ?? string.Empty
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "amount",
                        text = bill.Total.ToString()
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "valid_until",
                        text = bill.ValidUntil.Year > 2000 ? bill.ValidUntil.ToShortDateString() : ""
                    });
                    var components = new List<WhatsappTemplateMessageComponent>();


                    string fileName = bill.QuotationNumber + bill.LedgerId.ToString() + ".pdf";
                    string link = AzureStorageService.ContainerBaseUrl + bill.FinYearId + "/" + bill.CompanyId + "/quots/" + fileName;


                    var documentParameter = new List<WhatsappTemplateMessageParamter>();
                    documentParameter.Add(new WhatsappTemplateMessageDocumentParamter
                    {
                        type = "document",
                        fileName = fileName,
                        document = new WhatsappTemplateMessageDocumentLinkParamter
                        {
                            link = link
                        }
                    });
                    // link = AzureStorageService.ContainerBaseUrl + bill.FinYearId + "/" + bill.CompanyId + "/bills/" + bill.FileName + ".pdf"

                    components.Add(new WhatsappTemplateMessageComponent
                    {
                        type = "header",
                        parameters = documentParameter

                    });
                    components.Add(new WhatsappTemplateMessageComponent
                    {
                        type = "body",
                        parameters = parameters

                    });

                    return await _notificationSenderService.SendWhatsappAsync(dto,components);
                    // return await service.SendWhatsapp(dto, components);
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return false;
            }
        }

        async Task<bool> SendCode1103(NotificationDto dto)
        {
            Billing billing = new Billing();
            LoggedInUser user = new LoggedInUser();
            try
            {
                var workOrderId = Convert.ToInt32(dto.MetaData.Split(',')[1]);

                var wOrder = new WorkOrder(workOrderId);


                if (wOrder != null)
                {
                    var service = new NotificationService();

                    var company = new Company(wOrder.CompanyId);
                    var cdto = company.GetDetails();
                    var client = new Ledger(wOrder.LedgerId).GetDetails();
                    var parameters = new List<WhatsappTemplateMessageParamter>();

                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "client",
                        text = client.Name
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "company",
                        text = cdto.Name
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "challanno",
                        text = wOrder.Number
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "challandate",
                        text = wOrder.WorkOrderDate.ToString("dd MMM yyy")
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "company_signature",
                        text = cdto.Name
                    });
                    var components = new List<WhatsappTemplateMessageComponent>();


                    string fileName = wOrder.Number + wOrder.LedgerId.ToString() + ".pdf";
                    string link = AzureStorageService.ContainerBaseUrl + wOrder.FinYearId + "/" + wOrder.CompanyId + "/challan/" + fileName;


                    var documentParameter = new List<WhatsappTemplateMessageParamter>();
                    documentParameter.Add(new WhatsappTemplateMessageDocumentParamter
                    {
                        type = "document",
                        document = new WhatsappTemplateMessageDocumentLinkParamter
                        {
                            link = link
                        }
                    });
                    // link = AzureStorageService.ContainerBaseUrl + bill.FinYearId + "/" + bill.CompanyId + "/bills/" + bill.FileName + ".pdf"

                    components.Add(new WhatsappTemplateMessageComponent
                    {
                        type = "header",
                        parameters = documentParameter

                    });
                    components.Add(new WhatsappTemplateMessageComponent
                    {
                        type = "body",
                        parameters = parameters

                    });


                    return await service.SendWhatsapp(dto, components);
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Installation Completed Template
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        async Task<bool> SendCode1104(NotificationDto dto)
        {
            Billing billing = new Billing();
            LoggedInUser user = new LoggedInUser();
            try
            {
                var contractId = Convert.ToInt32(dto.MetaData.Split(',')[1]);

                //var wOrder = new WorkOrder(contractId);
                var contract = new Contract();
                var contractDto = await contract.GetById(new ContractFilterDto { ContractId = contractId, CompanyId = user.DefaultCompanyId });
                if (contractDto != null)
                {


                    var service = new NotificationService();



                    var company = new Company(contractDto.CompanyId);
                    var cdto = company.GetDetails();
                    var client = new Ledger(contractDto.LedgerId).GetDetails();
                    var parameters = new List<WhatsappTemplateMessageParamter>();
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "customer_name",
                        text = client.Name
                    });
                    if (String.IsNullOrEmpty(contractDto.PONumber))
                    {
                        contractDto.PONumber = "NA";
                    }
                    if (String.IsNullOrEmpty(contractDto.ProjectOwnerName))
                    {
                        contractDto.ProjectOwnerName = "NA";
                    }

                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "site_address",
                        text = contractDto.SiteAddress
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "installation_date",
                        text = contractDto.InstalledDate.ToString()
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "po_number",
                        text = contractDto.PONumber
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "height",
                        text = contractDto.Height.ToString()
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "area",
                        text = contractDto.Area.ToString()
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "type",
                        text = "MS"
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "supervisor_name",
                        text = contractDto.ProjectOwnerName
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "supervisor_number",
                        text = cdto.Phone1
                    });
                    //parameters.Add(new WhatsappTemplateMessageTextParamter
                    //{
                    //    type = "text",
                    //    parameter_name = "Google Profile Link",
                    //    text = cdto.Web
                    //});
                    var components = new List<WhatsappTemplateMessageComponent>();


                    return await _notificationSenderService.SendWhatsappAsync(dto, parameters);

                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return false;
            }
        }


        /// <summary>
        /// Installation due tomorrow template
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        async Task<bool> SendCode1105(NotificationDto dto)
        {
            Billing billing = new Billing();
            LoggedInUser user = new LoggedInUser();
            try
            {
                var contractId = Convert.ToInt32(dto.MetaData.Split(',')[1]);

                //var wOrder = new WorkOrder(contractId);
                var contract = new Contract();
                var contractDto = await contract.GetById(new ContractFilterDto { ContractId = contractId, CompanyId = dto.CompanyId });
                if (contractDto != null)
                {


                    var service = new NotificationService();



                    var company = new Company(contractDto.CompanyId);
                    var cdto = company.GetDetails();
                    var client = new Ledger(contractDto.LedgerId).GetDetails();
                    var parameters = new List<WhatsappTemplateMessageParamter>();

                    if (String.IsNullOrEmpty(contractDto.PONumber))
                    {
                        contractDto.PONumber = "NA";
                    }
                    if (String.IsNullOrEmpty(contractDto.ProjectOwnerName))
                    {
                        contractDto.ProjectOwnerName = "NA";
                    }
                    if (contractDto.InstallDueDate < DateTime.Now)
                    {
                        throw new Exception("Invalid or installation due data expired");
                    }

                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "customer_name",
                        text = client.Name
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "site_address",
                        text = contractDto.SiteAddress
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "installation_date",
                        text = Utils.FormatDate(contractDto.InstallDueDate)
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "po_number",
                        text = contractDto.PONumber
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "height",
                        text = contractDto.Height.ToString()
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "area",
                        text = contractDto.Area.ToString()
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "type",
                        text = "MS"
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "supervisor_name",
                        text = contractDto.ProjectOwnerName
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "supervisor_number",
                        text = cdto.Phone1
                    });
                    var components = new List<WhatsappTemplateMessageComponent>();
                    return await _notificationSenderService.SendWhatsappAsync(dto, parameters);
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Dismantle due tomorrow template
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        async Task<bool> SendCode1106(NotificationDto dto)
        {
            Billing billing = new Billing();
            LoggedInUser user = new LoggedInUser();
            try
            {
                var contractId = Convert.ToInt32(dto.MetaData.Split(',')[1]);

                //var wOrder = new WorkOrder(contractId);
                var contract = new Contract();
                var contractDto = await contract.GetById(new ContractFilterDto { ContractId = contractId, CompanyId = dto.CompanyId });
                if (contractDto != null)
                {


                    var service = new NotificationService();



                    var company = new Company(contractDto.CompanyId);
                    var cdto = company.GetDetails();
                    var client = new Ledger(contractDto.LedgerId).GetDetails();
                    var parameters = new List<WhatsappTemplateMessageParamter>();
                    if (contractDto.DismantleDueDate < DateTime.Now)
                    {
                        throw new Exception("Invalid or dismantle due data expired");
                    }
                    var daysRemaining = (int)Math.Ceiling((contractDto.DismantleDueDate - DateTime.Now).TotalDays);
                    if (String.IsNullOrEmpty(contractDto.PONumber))
                    {
                        contractDto.PONumber = "NA";
                    }
                    if (String.IsNullOrEmpty(contractDto.ProjectOwnerName))
                    {
                        contractDto.ProjectOwnerName = "NA";
                    }

                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "number",
                        parameter_name = "1",
                        text = client.Name
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "number",
                        parameter_name = "2",
                        text = contractDto.PONumber
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "number",
                        parameter_name = "3",
                        text = contractDto.SiteAddress
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "number",
                        parameter_name = "4",
                        text = Utils.FormatDate(contractDto.DismantleDueDate)
                    });

                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "number",
                        parameter_name = "5",
                        text = daysRemaining.ToString()
                    });

                    var components = new List<WhatsappTemplateMessageComponent>();
                    return await _notificationSenderService.SendWhatsappAsync(dto, parameters);
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Dismantle delay reminder
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        async Task<bool> SendCode1107(NotificationDto dto)
        {
            Billing billing = new Billing();
            LoggedInUser user = new LoggedInUser();
            try
            {
                var contractId = Convert.ToInt32(dto.MetaData.Split(',')[1]);

                //var wOrder = new WorkOrder(contractId);
                var contract = new Contract();
                var contractDto = await contract.GetById(new ContractFilterDto { ContractId = contractId, CompanyId = dto.CompanyId });
                if (contractDto != null)
                {


                    var service = new NotificationService();



                    var company = new Company(contractDto.CompanyId);
                    var cdto = company.GetDetails();
                    var client = new Ledger(contractDto.LedgerId).GetDetails();
                    var parameters = new List<WhatsappTemplateMessageParamter>();
                    if (contractDto.DismantleDueDate < DateTime.Now)
                    {
                        throw new Exception("Invalid or dismantle due data expired");
                    }

                    if (String.IsNullOrEmpty(contractDto.PONumber))
                    {
                        contractDto.PONumber = "NA";
                    }
                    if (String.IsNullOrEmpty(contractDto.ProjectOwnerName))
                    {
                        contractDto.ProjectOwnerName = "NA";
                    }
                    if (contractDto.InstallDueDate < DateTime.Now)
                    {
                        throw new Exception("Invalid or installation due data expired");
                    }
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "customer_name",
                        text = client.Name
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "po_number",
                        text = contractDto.PONumber
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "site_address",
                        text = contractDto.SiteAddress
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "dismantle_date",
                        text = Utils.FormatDate(contractDto.DismantleDueDate)
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "new_dismantle_date",
                        text = contractDto.DismantleDueDate.ToString()
                    });

                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "delay_reason",
                        text = contractDto.Height.ToString()
                    });

                    var components = new List<WhatsappTemplateMessageComponent>();
                    return await _notificationSenderService.SendWhatsappAsync(dto, parameters);
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Material inward reminder confirmation
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        async Task<bool> SendCode1108(NotificationDto dto)
        {
            Billing billing = new Billing();
            LoggedInUser user = new LoggedInUser();
            try
            {
                var grnId = Convert.ToInt32(dto.MetaData.Split(',')[1]);

                //var wOrder = new WorkOrder(contractId);
                var grn = new GRN();
                var grnDto = await grn.GrnById(grnId, dto.CompanyId);
                if (grnDto != null)
                {

                    var service = new NotificationService();

                    var company = new Company(grnDto.CompanyId);
                    var cdto = company.GetDetails();
                    var client = new Ledger(grnDto.LedgerId).GetDetails();
                    var parameters = new List<WhatsappTemplateMessageParamter>();

                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "customer_name",
                        text = client.Name
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "inward_no",
                        text = grnDto.GRN
                    });

                    var components = new List<WhatsappTemplateMessageComponent>();
                    return await _notificationSenderService.SendWhatsappAsync(dto, parameters);
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Material dispatch confirmation
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        async Task<bool> SendCode1109(NotificationDto dto)
        {
            Billing billing = new Billing();
            LoggedInUser user = new LoggedInUser();
            try
            {
                var workOrderId = Convert.ToInt32(dto.MetaData.Split(',')[1]);

                var wOrder = new WorkOrder(workOrderId);

                // var grnDto = await grn.GrnById(grnId, dto.CompanyId);
                if (wOrder != null)
                {


                    var service = new NotificationService();
                    var company = new Company(wOrder.CompanyId);
                    var cdto = company.GetDetails();
                    var client = new Ledger(wOrder.LedgerId).GetDetails();
                    var parameters = new List<WhatsappTemplateMessageParamter>();

                    if (String.IsNullOrEmpty(wOrder.Driver))
                    {
                        wOrder.Driver = "NA";
                    }
                    if (String.IsNullOrEmpty(wOrder.ProjectOwnerPhone))
                    {
                        wOrder.ProjectOwnerPhone = "NA";
                    }
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "customer_name",
                        text = client.Name
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "challan_no",
                        text = wOrder.Number
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "driver_name",
                        text = wOrder.Driver
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "supervisor_number",
                        text = wOrder.ProjectOwnerPhone
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "support_number",
                        text = cdto.Phone1
                    });
                    var components = new List<WhatsappTemplateMessageComponent>();
                    return await _notificationSenderService.SendWhatsappAsync(dto, parameters);
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Bill payment due reminder today
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        async Task<bool> SendCode1110(NotificationDto dto)
        {
            Billing billing = new Billing();
            LoggedInUser user = new LoggedInUser();
            try
            {
                var invoiceId = Convert.ToInt32(dto.MetaData.Split(',')[1]);

                var bill = new Billing();
                var billdto = (await bill.GetBillsByIds(invoiceId.ToString(), dto.CompanyId)).FirstOrDefault();
                // var grnDto = await grn.GrnById(grnId, dto.CompanyId);
                if (billdto != null)
                {


                    var service = new NotificationService();



                    var company = new Company(dto.CompanyId);
                    var cdto = company.GetDetails();
                    var client = new Ledger(billdto.LedgerId).GetDetails();
                    var parameters = new List<WhatsappTemplateMessageParamter>();

                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "customer_name",
                        text = client.Name
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "bill_no",
                        text = billdto.InvoiceNumber
                    });
                    parameters.Add(new WhatsappTemplateMessageTextParamter
                    {
                        type = "text",
                        parameter_name = "amount",
                        text = billdto.Total.ToString()
                    });

                    var components = new List<WhatsappTemplateMessageComponent>();
                    return await _notificationSenderService.SendWhatsappAsync(dto, parameters);
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return false;
            }
        }

        #endregion


        #region alerts
        [System.Web.Http.HttpPost]

        public async Task<ApiResponseMessage> GetMyAlerts([FromBody] NotificationFilterDTO filter)
        {
            var ns = new NotificationService();

            var res = new ApiResponseMessage();
            try
            {
                if (filter == null)
                {
                    filter = new NotificationFilterDTO();
                    filter.From = DateTime.Today.AddDays(-1);
                    filter.To = DateTime.Now;

                }
                if (filter.From.Year < 2000)
                {
                    filter.From = DateTime.Today.AddDays(-1);
                    filter.To = DateTime.Now;
                }
                var user = new LoggedInUser();
                filter.CompanyId = user.DefaultCompanyId;
                filter.UserId = user.UserId;
                res.Data = await ns.GetMyAlerts(filter);
                res.Code = ApiResponseMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                res.Code = ApiResponseMessageCodes.ERROR;
                res.Message = ex.Message;
                logger.LogError(ex, "Error while getting my alerts", ex.Message);
            }
            return res;
        }

        [System.Web.Http.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ApiResponseMessage> CollectContractReminders()
        {
            var ns = new NotificationService();
            var res = new ApiResponseMessage();
            try
            {
                res.Data = await ns.CollectContractReminders();
                res.Code = ApiResponseMessageCodes.SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.Code = ApiResponseMessageCodes.ERROR;
                res.Message = ex.Message;
                logger.LogError(ex, "Error while getting my alerts", ex.Message);
            }
            return res;
        }
        [System.Web.Http.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ApiResponseMessage> CollectPickupReminders()
        {
            var ns = new NotificationService();
            var res = new ApiResponseMessage();
            try
            {
                res.Data = await ns.CollectPickupReminders();
                res.Code = ApiResponseMessageCodes.SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.Code = ApiResponseMessageCodes.ERROR;
                res.Message = ex.Message;
                logger.LogError(ex, "Error while getting my alerts", ex.Message);
            }
            return res;
        }

        [System.Web.Http.HttpPost]

        public async Task<ApiResponseMessage> UpdateStatus([FromBody] NotificationDto dto)
        {
            var ns = new NotificationService();
            var res = new ApiResponseMessage();
            var user = new LoggedInUser();
            try
            {
                if (dto == null)
                {
                    throw new Exception("Input is null or empty");
                }
                dto.ModifiedOn = DateTime.Now;
                res.Data = await ns.StatusUpdate(dto);
                res.Code = ApiResponseMessageCodes.SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.Code = ApiResponseMessageCodes.ERROR;
                res.Message = ex.Message;
                logger.LogError(ex, "Error while getting my alerts", ex.Message);
            }
            return res;
        }
        #endregion
    }
}