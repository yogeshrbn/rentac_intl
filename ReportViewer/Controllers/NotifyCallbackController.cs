using BAL.DTO;
using BAL.Objects;
using BAL.Services;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;

namespace ReportViewer.Controllers
{
    [System.Web.Mvc.AllowAnonymous]
    [System.Web.Http.RoutePrefix("api/[controller]")]
    public class NotifyCallbackController : Controller
    {
        LoggingService logger = new LoggingService();
        [System.Web.Mvc.HttpPost]
        public async Task<bool> Notify()
        {
            try
            {
                var req = Request.InputStream;
                req.Seek(0, System.IO.SeekOrigin.Begin);
                string json = new StreamReader(req).ReadToEnd();
                if (String.IsNullOrEmpty(json))
                {
                    logger.LogError("Empty or invalid whatsapp callback response: " + json);
                    return false;
                }
                var jObject = JObject.Parse(json);
                if (jObject == null)
                {
                    logger.LogError("Empty or invalid whatsapp callback response: " + json);
                    return false;
                }
                var _eventType = Convert.ToString(jObject["type"]);
                bool eventResult = false;
                switch (_eventType)
                {
                    case "message-event":
                        eventResult = await processMessageEvent(jObject);
                        break;
                    case "billing-event":
                        eventResult = await processBillingEvent(jObject);
                        break;
                    case "user-event":
                        eventResult = await processUserEvent(jObject);
                        break;
                    case "account-event":
                        eventResult = await processAccountEvent(jObject);
                        break;
                }


                return eventResult;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return false;
            }
        }

        async Task<bool> processMessageEvent(JObject jsonData)
        {
            var payLoad = jsonData["payload"];
            if (payLoad == null)
            {
                logger.LogError("Payload data is missing-> processMessageEvent");
                logger.LogError(jsonData.ToString());
            }

            var dto = new NotificationDto();
            dto.gsId = Convert.ToString(payLoad["id"]);
            dto.Status = Convert.ToString(payLoad["type"]);
            dto.ModifiedOn = DateTime.Now;
            dto.DeliveryMessage = dto.Status;
            var wsPayLoad = payLoad["payload"];
            if (wsPayLoad != null)
            {
                dto.wsId = Convert.ToString(wsPayLoad["whatsappMessageId"]);

            }
            var service = new NotificationService();
            return await service.StatusUpdateByGsId(dto);

        }
        async Task<bool> processUserEvent(JObject jsonData)
        {

            return true;
        }

        async Task<bool> processAccountEvent(JObject jsonData)
        {

            return true;
        }
        async Task<bool> processBillingEvent(JObject jsonData)
        {

            return true;
        }

        [System.Web.Mvc.HttpPost]
        public async Task<bool> pravahcrmhk()
        {
            try
            {
                var req = Request.InputStream;
                req.Seek(0, System.IO.SeekOrigin.Begin);
                string json = new StreamReader(req).ReadToEnd();
                logger.LogInfo("CRM Client: " + json);

                if (String.IsNullOrEmpty(json))
                {
                    logger.LogError("Empty or invalid whatsapp callback response: " + json);
                    return false;
                }
                var jObject = JObject.Parse(json);
                if (jObject == null)
                {
                    logger.LogError("Empty or invalid whatsapp callback response: " + json);
                    return false;
                }
                Ledger ledger = new Ledger();
                if (jObject["CompanyId"] != null)
                {
                    ledger.CompanyId = Convert.ToInt32(jObject["CompanyId"]);

                }
                if (jObject["FirstName"] != null)
                {
                    ledger.ContactPersonName = Convert.ToString(jObject["FirstName"]);
                }
                if (jObject["LastName"] != null)
                {
                    ledger.ContactPersonName = ledger.ContactPersonName + " " + Convert.ToString(jObject["LastName"]);
                }

                if (jObject["Email"] != null)
                {
                    ledger.Email = Convert.ToString(jObject["Email"]);

                }
                if (jObject["Phone"] != null)
                {
                    ledger.Phone1 = ledger.ContactPersonMobile = Convert.ToString(jObject["Phone"]);

                }
                if (jObject["City"] != null)
                {
                    ledger.City = Convert.ToString(jObject["City"]);

                }
                if (jObject["StateId"] != null)
                {
                    ledger.StateId = Convert.ToInt32(jObject["StateId"]);

                }
                if (jObject["company_name"] != null)
                {
                    ledger.Name = ledger.TradeName = Convert.ToString(jObject["company_name"]);

                }
                if (jObject["Website"] != null)
                {
                    ledger.Web = Convert.ToString(jObject["Website"]);

                }
                if (jObject["Locality"] != null)
                {
                    ledger.Address1 = Convert.ToString(jObject["Locality"]);

                }
                if (jObject["ZipCode"] != null)
                {
                    ledger.ZipCode = Convert.ToString(jObject["ZipCode"]);

                }

                var address = new AddressDTO();
                address.Address1 = ledger.Address1;
                address.Address2 = ""; ;
                address.City = ledger.City;
                address.StateId = ledger.StateId;
                address.State= ledger.StateId.ToString();
                address.ZipCode = ledger.ZipCode;
                address.Email = ledger.Email;
                address.Phone1 = ledger.Phone1;
                address.AddressTypeId = 1;
                ledger.BillingAddress = address;

                ledger.CreatedBy = 0;
                ledger.CreatedOn = DateTime.Now;


                if (ledger.CompanyId == 0)
                {
                    logger.LogError("CompanyId must exist in Rentac " + json);
                    return false;
                }
                if (String.IsNullOrEmpty(ledger.Name))
                {
                    logger.LogError("Name must be non empty" + json);
                    return false;
                }

                var company = new Company(ledger.CompanyId);
                var cdto = company.GetDetails();
                if (cdto == null)
                {
                    logger.LogError("Company not found in rentac: " + json);
                    return false;

                }
                ledger.Addresses = new List<AddressDTO>();
                ledger.Addresses.Add(address);

                ledger.Source = "crm";
                ledger.RbnClientId = cdto.RbnClientId;
                var l = ledger.Save();
                if (l >0)
                {
                    return true;
                }
                else
                {
                    logger.LogError("Could not create ledger account in rentac via crm callback: " + json);
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error occurred with create client via crm callback " + ex.Message);
                logger.LogError("Error occurred with create client via crm callback " + ex.StackTrace);

                return false;
            }
        }
    }
}