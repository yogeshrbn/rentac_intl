using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BAL.DTO;
using BAL.Objects;
using BAL.Common;
using FarmaAPI.Helper;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Drawing;
using System.Globalization;
using System.Data;
using Microsoft.Reporting.WebForms;
using System.IO;
using BAL.Enums;
using System.Threading.Tasks;
using NLog;
using OfficeOpenXml.FormulaParsing.Utilities;
using BAL.Exceptions;
using Omu.ValueInjecter;
using System.Web;
using iTextSharp.tool.xml.html;
using System.Text.Encodings.Web;
using iTextSharp.text.html;



namespace FarmaAPI.Controllers
{
    [Authorize]
    public class WorkOrderController : BaseApiController
    {
        private static readonly string docsPath = Path.Combine(HttpRuntime.AppDomainAppPath, "docs");
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly string reportFilePath = Path.Combine(HttpRuntime.AppDomainAppPath, "rpts") + @"\";// System.Web.HttpContext.Current.Server.MapPath("~/rpts/");

        /// <summary>
        ///Actiates and de-activates an account group
        /// </summary>
        /// <param name="dto">AccountGroupDTO</param>
        /// <returns>true/false</returns>
        [System.Web.Mvc.ValidateInput(false)]

        [HttpPost]
        public async Task<IHttpActionResult> SaveWorkOrder()
        {
            var res = new ApiMessage();
            WorkOrder wOrder = new WorkOrder(0);
            //  DataCopier<WorkOrderDTO, WorkOrder> dcopier = new DataCopier<WorkOrderDTO, WorkOrder>();
            // dcopier.CopyData(dto, leder);
            string dto = System.Web.HttpContext.Current.Request["dto"];
            System.Web.HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            JObject jsonObject = new JObject();
            BAL.Objects.Config cnfig = new BAL.Objects.Config();
            try
            {

                jsonObject = JObject.Parse(dto);
                wOrder.WorkOrderId = Convert.ToInt32(jsonObject.GetValue("WorkOrderId"));
                Int16 challanType = 0;
                if (jsonObject["ChallanType"] != null)
                {
                    challanType = Convert.ToInt16(jsonObject["ChallanType"]);
                    if (challanType == 7) //if adding site bill for fixed amount type workorder
                    {
                        wOrder.ParentWorkOrderId = wOrder.WorkOrderId;
                    }
                }
                bool edit = wOrder.WorkOrderId > 0;


                //create new workorder/jobnumber 
                /*
                 * Now next number will be visibule to the user. He may edit it.
                 * if (wOrder.WorkOrderId == 0)
                 {
                     NextId n = new NextId();
                     wOrder.Number = n.GetNextId(BAL.Enums.NextIDTables.WorkOrder, new LoggedInUser().FinYearId.ToString(), new LoggedInUser().DefaultCompanyId);

                 }
                 else 
                 * */
                // edit mode or adding new sitebill for fixed amount workorder. Then same number will be used 
                string prefix = "";
                LoggedInUser user = new LoggedInUser();
                string number = Convert.ToString(jsonObject.GetValue("ChallanNumber"));
                wOrder.Number = number;
                //if (!edit)
                //{
                //    prefix = cnfig.GetKeyValue(ConfigCategory.ISSUECHALLAN, ConfigCategory.ISSUECHALLAN, ConfigKey.Prefix, user.DefaultCompanyId, null);
                //    wOrder.Number = prefix + number;
                //}
                wOrder.CompanyId = user.DefaultCompanyId;// Convert.ToInt32(jsonObject.GetValue("CompanyId"));
                wOrder.LedgerId = Convert.ToInt32(jsonObject.GetValue("LedgerId"));
                wOrder.JobCardId = Convert.ToInt32(jsonObject.GetValue("JobCardId"));
                wOrder.Remarks = HttpUtility.HtmlDecode(Convert.ToString(jsonObject.GetValue("Remarks")));
                wOrder.Tnc = HttpUtility.HtmlDecode(Convert.ToString(jsonObject.GetValue("Tnc")));
                if (jsonObject["SezDescription"] != null)
                    wOrder.SezDescription = HttpUtility.HtmlDecode(Convert.ToString(jsonObject["SezDescription"]));
                if (jsonObject["ShipFrom"] != null)
                    wOrder.ShipFrom = HttpUtility.HtmlDecode(Convert.ToString(jsonObject["ShipFrom"]));
                wOrder.RefNo = Convert.ToString(jsonObject.GetValue("RefNo"));
                if (jsonObject.GetValue("WarehouseId") != null)
                {
                    wOrder.WarehouseId = Convert.ToInt32(jsonObject.GetValue("WarehouseId"));
                }
                if (!String.IsNullOrEmpty(wOrder.Tnc))
                {
                    wOrder.Tnc = wOrder.Tnc.Replace("<br/>", "");
                    wOrder.Tnc = wOrder.Tnc.Replace("<br />", "");

                }
                if (!String.IsNullOrEmpty(wOrder.Tnc))
                {
                    wOrder.Tnc = wOrder.Tnc.Replace("<br/>", "");
                    wOrder.Tnc = wOrder.Tnc.Replace("<br />", "");

                }

                if (jsonObject["Taxes"] != null)
                {
                    if (jsonObject["Taxes"].HasValues)
                    {
                        var taxes = JObject.Parse(jsonObject["Taxes"].ToString());
                        wOrder.IGSTAmount = Convert.ToDouble(taxes.GetValue("IGSTAmount"));
                        wOrder.SGSTAmount = Convert.ToDouble(taxes.GetValue("SGSTAmount"));
                        wOrder.CGSTAmount = Convert.ToDouble(taxes.GetValue("CGSTAmount"));

                        wOrder.IGSTRate = Convert.ToDouble(taxes.GetValue("IGST"));
                        wOrder.SGSTRate = Convert.ToDouble(taxes.GetValue("SGST"));
                        wOrder.CGSTRate = Convert.ToDouble(taxes.GetValue("CGST"));
                    }
                }
                //        wOrder.WorkOrderDate = Utils.FormatDate(Convert.ToString(jsonObject.GetValue("WorkOrderDate")));
                wOrder.WorkOrderDate = Convert.ToDateTime(jsonObject.GetValue("WorkOrderDate"));
                wOrder.RentStartDate = wOrder.WorkOrderDate;
                if (jsonObject.GetValue("RentStartDate") != null)
                {
                    //wOrder.RentStartDate = Utils.FormatDate(Convert.ToString(jsonObject.GetValue("RentStartDate")));
                    wOrder.RentStartDate = Convert.ToDateTime(jsonObject.GetValue("RentStartDate"));
                }

                if (jsonObject["PODate"] != null && !String.IsNullOrEmpty(Convert.ToString(jsonObject["PODate"])))
                {
                    wOrder.PODate = Convert.ToDateTime(jsonObject["PODate"]);

                }
                if (jsonObject["PONumber"] != null)
                {
                    wOrder.PONumber = Convert.ToString(jsonObject["PONumber"]);
                }
                wOrder.ClientAmount = 0;// Convert.ToDouble(jsonObject.GetValue("ClientAmount"));
                wOrder.RbnClientId = user.RbnClientId;
                wOrder.FinYearId = user.FinYearId;

                //if (wOrder.WorkOrderDate.Date > DateTime.Today)
                //{
                //    throw new Exception("Issue date can not be future date");
                //}
                if (wOrder.WorkOrderDate.Date < user.FinYearStart && challanType == (int)ChallanType.RENT_DELIVERY)
                {
                    throw new Exception("Issue date can not be before Financial year start date");
                }

                var ledger = new Ledger(wOrder.LedgerId);
                // var lastBill = ledger.GetLastBill(wOrder.LedgerId, wOrder.SiteId, 0);


                if (challanType == 0)
                {
                    wOrder.ChallanType = BAL.Enums.ChallanType.WORKORDER;
                }
                else
                {
                    wOrder.ChallanType = (BAL.Enums.ChallanType)Enum.Parse(typeof(BAL.Enums.ChallanType), challanType.ToString());
                }
                wOrder.CreatedBy = user.UserId;
                //   wOrder.SubTotal = Convert.ToDouble(jsonObject.GetValue("SubTotal"));
                // wOrder.Site = Convert.ToString(jsonObject.GetValue("Site"));
                //if (wOrder.WorkOrderId == 0)
                //{
                AddSiteToObject(wOrder, jsonObject);
                addOtherCharges(wOrder, jsonObject);
                if (wOrder.Sites.Count > 0)
                {
                    wOrder.Sites[0].StartDate = wOrder.WorkOrderDate;
                    wOrder.Sites[0].ChallanNumber = wOrder.Number;
                    wOrder.Sites[0].JobNumber = wOrder.Number;
                }
                if (challanType == (int)ChallanType.RENT_DELIVERY)
                {
                    //var lastBill = ledger.GetLastBill(wOrder.LedgerId, wOrder.Sites[0].LedgerSiteId, 0);
                    //if (lastBill != null)
                    //{

                    //    if (wOrder.WorkOrderDate <= Convert.ToDateTime(lastBill.To))
                    //    {
                    //        throw new Exception("Issue date can not be on or before last bill date");
                    //    }
                    //}
                }

                if (wOrder.Sites[0].RecoveryDate.Year > 2000 &&
                    wOrder.Sites[0].RecoveryDate < wOrder.RentStartDate)
                {
                    throw new Exception("Recovery date must be future date");
                }

                //  }
                wOrder.Sites[0].AppliedTaxes = ParseWorkOrderTaxList(jsonObject["AppliedTaxes"]);
                int workOrderId = wOrder.Save();
                if (workOrderId > 0)
                {
                    // AddTransaction(wOrder, jsonObject);

                    //-- Update last Number
                    //NextId n = new NextId();
                    //n.UpdateId(NextIDTables.WorkOrder, user.FinYearId.ToString(), user.DefaultCompanyId, number, prefix);

                    //--send sms here
                    //DataSet ds = wOrder.GetChallanReportHeader(workOrderId, 1);
                    //CommHelpler com = new CommHelpler();
                    //if (ds.Tables.Count > 0)
                    //{

                    //WorkOrderDTO siteDTO = new Utils<WorkOrderDTO>().ConstructList(ds).FirstOrDefault();
                    //EmailParameters p = new EmailParameters() { ChallanNumber = siteDTO.Number, Driver = siteDTO.Driver, Vehicle = siteDTO.Vehicle };

                    //LedgerDTO lDto = ledger.GetDetails();
                    //  com.sendSms(lDto.ContactPersonMobile, p, MessageEvent.ISSUE_CHALLAN);
                    //var param = new Dictionary<string, string>();
                    //param.Add("otp", "335553");
                    //string phone = lDto.ContactPersonMobile;
                    //if (String.IsNullOrEmpty(phone))
                    //{
                    //    phone = lDto.ContactPersonOffPhone;
                    //}
                    //if (!String.IsNullOrEmpty(phone))
                    //{
                    //    if (phone.Length >= 10)
                    //    {
                    //        phone = phone.Substring(phone.Length - 10, 10);
                    //        phone = "91" + phone;
                    //        com.sendSms(phone, SMSTemplates.ISSUE_CHALLAN, MessageEvent.ISSUE_CHALLAN, param);

                    //    }



                    //}
                    //}
                    //try
                    //{
                    //   // ReportUtility rep = new ReportUtility();
                    //    //Task.Factory.StartNew(() =>
                    //    //{
                    //    //    rep.EmailRentIssueReceipt(workOrderId, true);
                    //    //});
                    //   // var x = rep.EmailRentIssueReceipt(workOrderId, false);
                    //    // x.Start();
                    //}
                    //catch (Exception ex)
                    //{
                    //    logger.Error("Error while sending  challan on email: ", ex);
                    //}

                }
                res.Code = ApiMessageCodes.SUCCESS;
                res.Message = "Items issued successfully.";
                res.Data = wOrder;
                res.Data = wOrder;

            }
            catch (Exception ex)
            {
                res.Code = ApiMessageCodes.ERROR;
                res.Message = ex.Message;
                res.Description = ex.StackTrace;
                logger.Error(ex.StackTrace, ex);
                logger.Error("Error while creating work order: ", ex);
            }
            return Ok(res);
        }

        [HttpPost]
        public async Task<IHttpActionResult> InitChallan()
        {
            var res = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                var comp = new Company(user.DefaultCompanyId).GetDetails();
                res.Data = new { company = comp };
                res.Code = ApiMessageCodes.SUCCESS;
                return Ok(res);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void addOtherCharges(WorkOrderDTO wDto, JObject jsonObject)
        {
            var charges = jsonObject["OtherCharges"];
            if (charges != null)
            {
                wDto.OtherCharges = new List<WorkOrderChageDTO>();
                foreach (var dto in charges)
                {
                    WorkOrderChageDTO chDto = new WorkOrderChageDTO();
                    chDto.ChargeId = Convert.ToInt32(dto["ChargeId"]);
                    chDto.Amount = Convert.ToDouble(dto["Amount"]);
                    if (dto["WorkOrderChargeId"] != null)
                    {
                        chDto.WorkOrderChargeId = Convert.ToInt32(dto["WorkOrderChargeId"]);
                    }
                    if (chDto.Amount > 0 || chDto.WorkOrderChargeId > 0)
                    {
                        wDto.OtherCharges.Add(chDto);
                    }
                }
            }
        }

        void AddTransaction(WorkOrderDTO wDto, JObject jsonObject)
        {
            var transaction = jsonObject["Transaction"];
            LedgerTransactionDTO ldto = new LedgerTransactionDTO();
            if (transaction != null)
            {
                ldto.TransactionAmount = Convert.ToDouble(transaction["TransactionAmount"]);
                ldto.TranRefNumber = Convert.ToString(transaction["TranRefNumber"]);
                ldto.EntryType = Convert.ToInt16(transaction["EntryType"]);
                ldto.Description = Convert.ToString(transaction["Description"]);
            }
            ldto.TransactionDate = wDto.WorkOrderDate;//.ToShortDateString();
            ldto.CrLedgerId = wDto.LedgerId;
            ldto.WorkOrderId = wDto.WorkOrderId;
            ldto.CreatedBy = new LoggedInUser().UserId;

            if (ldto.TransactionAmount > 0)
            {
                Ledger ledger = new Ledger();
                ledger.CreateTransactions(ldto);
            }
        }

        string SaveFile(System.Web.HttpPostedFile file, string fileName)
        {
            fileName = String.IsNullOrEmpty(fileName) ? file.FileName : fileName;
            string virtualPath = @"../docs/wo/";// +Guid.NewGuid().ToString() + ".jpg";
            String fullPath = System.Web.HttpContext.Current.Server.MapPath(@"../" + virtualPath);
            string vPath = virtualPath + fileName;
            fullPath = fullPath + fileName;
            file.SaveAs(fullPath);
            return vPath.Remove(0, 2).Replace("/api/", "/");
        }

        void GetImage(String filePath, String base64String)
        {
            //Image imgToReturn;
            try
            {
                byte[] bytes = Convert.FromBase64String(base64String.Split(',')[1]);
                using (var imageFile = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }

            }
            catch (Exception ex)
            {

            }
        }

        [HttpPost]
        public HttpResponseMessage GetAll([FromBody] SiteDTO obj)
        {


            WorkOrder objWorkOrder = new WorkOrder(0);
            List<SiteDTO> listWorkOrder = objWorkOrder.GetAll(obj.Number, obj.JobNumber, obj.Site, obj.Client, obj.Closed, new LoggedInUser().DefaultCompanyId, obj.SiteEng);
            return Request.CreateResponse(HttpStatusCode.OK, listWorkOrder);
        }

        [HttpPost]
        public HttpResponseMessage GetByCompany([FromBody] SiteDTO obj)
        {


            WorkOrder objWorkOrder = new WorkOrder(0);
            int companyId = new LoggedInUser().DefaultCompanyId;
            List<SiteDTO> listWorkOrder = objWorkOrder.GetByCompany(companyId, obj.FinYearId, obj.Code);
            return Request.CreateResponse(HttpStatusCode.OK, listWorkOrder);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetById([FromBody] WorkOrderDTO obj)
        {
            try
            {
                WorkOrder objWorkOrder = new WorkOrder(obj.WorkOrderId);
                objWorkOrder.Items = objWorkOrder.GetItems();

                if (objWorkOrder != null)
                {
                    var ledger = new Ledger(objWorkOrder.LedgerId);
                    var ldto = ledger.GetDetails();
                    objWorkOrder.ClientDetails = ldto;
                }


                objWorkOrder.Operations = (await objWorkOrder.GetOperations(obj.WorkOrderId, objWorkOrder.CompanyId)).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, objWorkOrder);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public async Task<HttpResponseMessage> JobWoById([FromBody] WorkOrderDTO obj)
        {
            try
            {
                var user = new LoggedInUser();
                WorkOrder objWorkOrder = new WorkOrder(0);
                obj = await objWorkOrder.JobWoById(obj.WorkOrderId, user.DefaultCompanyId);
                obj.Items = objWorkOrder.GetItems();
                obj.Operations = (await objWorkOrder.GetOperations(obj.WorkOrderId, objWorkOrder.CompanyId)).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public HttpResponseMessage GetItems([FromBody] WorkOrderDTO obj)
        {
            WorkOrder objWorkOrder = new WorkOrder(obj.WorkOrderId);
            return Request.CreateResponse(HttpStatusCode.OK, objWorkOrder.GetItems());
        }
        [HttpPost]
        public HttpResponseMessage GetSites([FromBody] WorkOrderDTO obj)
        {
            WorkOrder objWorkOrder = new WorkOrder(0);
            return Request.CreateResponse(HttpStatusCode.OK, objWorkOrder.GetSites(obj.WorkOrderId));
        }
        [HttpPost]
        public HttpResponseMessage GetClientSites([FromBody] FilterCriteria obj)
        {
            WorkOrder objWorkOrder = new WorkOrder(0);
            return Request.CreateResponse(HttpStatusCode.OK, objWorkOrder.GetClientSites(obj.LedgerId));
        }
        [HttpPost]
        public IHttpActionResult AddSite()
        {
            WorkOrder wOrder = new WorkOrder(0);
            var res = new ApiMessage();
            try
            {

                //  DataCopier<WorkOrderDTO, WorkOrder> dcopier = new DataCopier<WorkOrderDTO, WorkOrder>();
                // dcopier.CopyData(dto, leder);
                string dto = System.Web.HttpContext.Current.Request["dto"];
                System.Web.HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                JObject jsonObject = new JObject();
                jsonObject = JObject.Parse(dto);
                int workOrderId = Convert.ToInt32(jsonObject.GetValue("WorkOrderId"));
                LoggedInUser user = new LoggedInUser();



                wOrder = new WorkOrder(workOrderId);
                //  wOrder.WorkOrderDate = Utils.FormatDate(Convert.ToString(jsonObject.GetValue("WorkOrderDate")));
                wOrder.WorkOrderDate = Convert.ToDateTime(jsonObject.GetValue("WorkOrderDate"));
                wOrder.Number = Convert.ToString(jsonObject.GetValue("ChallanNumber"));
                wOrder.LedgerId = Convert.ToInt32(jsonObject.GetValue("LedgerId"));
                //  wOrder.RentStartDate = Utils.FormatDate(Convert.ToString(jsonObject.GetValue("RentStartDate")));
                wOrder.RentStartDate = Convert.ToDateTime(jsonObject.GetValue("RentStartDate"));
                wOrder.Remarks = Convert.ToString(jsonObject.GetValue("Remarks"));
                wOrder.Tnc = Convert.ToString(jsonObject.GetValue("Tnc"));

                AddSiteToObject(wOrder, jsonObject);

                //if (wOrder.WorkOrderDate > DateTime.Today)
                //{
                //    throw new Exception("Issue date can not be future date");
                //}
                if (wOrder.WorkOrderDate < user.FinYearStart)
                {
                    throw new Exception("Issue date can not be before Financial year start date");
                }

                //var ledger = new Ledger(wOrder.LedgerId);
                //var lastBill = ledger.GetLastBill(wOrder.LedgerId, wOrder.Sites[0].LedgerSiteId, 0);

                //if (lastBill != null)
                //{

                //    if (wOrder.WorkOrderDate <= Convert.ToDateTime(lastBill.To))
                //    {
                //        throw new Exception("Issue date can not be on or before last bill date");
                //    }
                //}


                wOrder.Sites[0].StartDate = wOrder.WorkOrderDate;
                wOrder.ApplicableTaxes = new List<TaxDTO>();
                addOtherCharges(wOrder, jsonObject);
                wOrder.Sites[0].ChallanNumber = wOrder.Number;
                wOrder.Sites[0].JobNumber = wOrder.Number;
                wOrder.Sites[0].Remarks = wOrder.Remarks;
                wOrder.Sites[0].Tnc = wOrder.Tnc;
                wOrder.Sites[0].CompanyId = user.DefaultCompanyId;
                //create a new site
                //    wOrder.Sites[0].ParentSiteId = wOrder.Sites[0].SiteId;
                var x = wOrder.AddSite(wOrder.Sites, wOrder.WorkOrderId);
                if (x)
                {
                    wOrder.AddOtherCharges(workOrderId, wOrder.OtherCharges);
                }
                //if (wOrder.Sites[0].SiteId == 0)
                //{
                //    wOrder.AddSite(wOrder.Sites, wOrder.WorkOrderId);
                //}
                //else
                //{ //add new items to the exsting site.
                //    wOrder.AddSiteItems(wOrder.Sites[0]);
                //}
                //LoggedInUser user = new LoggedInUser();
                //string prefix = new Config().GetKeyValue(ConfigCategory.ISSUECHALLAN, ConfigCategory.ISSUECHALLAN, ConfigKey.Prefix, user.DefaultCompanyId, null);

                //NextId n = new NextId();
                //n.UpdateId(NextIDTables.WorkOrder, user.FinYearId.ToString(), user.DefaultCompanyId, wOrder.Number, prefix);
                res.Code = ApiMessageCodes.SUCCESS;
                res.Message = "Items issued successfully.";
                res.Data = wOrder;
                //updtes the  challan after change
                // var rep = new ReportUtility();
                //  var x = rep.EmailRentIssueReceipt(workOrderId, false);
            }
            catch (Exception ex)
            {
                res.Code = ApiMessageCodes.ERROR;
                res.Message = ex.Message;
                res.Description = ex.StackTrace;

            }
            return Ok(res);

        }

        [HttpPost]
        public HttpResponseMessage AddItemsToSite()
        {
            try
            {
                return SiteRequest(BAL.Enums.SiteItemType.DELIVERED);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult AddProductionWO([FromBody] WorkOrderDTO dto)
        {
            var res = new ApiMessage();
            try
            {
                if (dto == null)
                {
                    throw new Exception("Input is empty");
                }
                var user = new LoggedInUser();
                var wo = new WorkOrder(0);
                wo.InjectFrom(dto);

                wo.Operations = dto.Operations;
                wo.Items = dto.Items;
                wo.CreatedBy = user.UserId;
                wo.CreatedOn = DateTime.Now;
                wo.GuId = Guid.NewGuid().ToString();
                wo.ModifiedBy = user.UserId;
                wo.ModifiedOn = DateTime.Now;
                wo.CompanyId = user.DefaultCompanyId;



                var siteObj = new Site();
                siteObj.StartDate = wo.WorkOrderDate;
                siteObj.LedgerId = dto.LedgerId;
                siteObj.LedgerSiteId = dto.LedgerSiteId;
                siteObj.Items = dto.Items;
                siteObj.RentStartDate = dto.WorkOrderDate;
                wo.Sites.Add(siteObj);

                dto.WorkOrderId = wo.Save();

                res.Code = ApiMessageCodes.SUCCESS;
                res.Data = dto.WorkOrderId;

                return Ok(res);
            }
            catch (Exception ex)
            {
                res.Code = ApiMessageCodes.ERROR;
                res.Message = ex.Message;
                return Ok(res);
            }
        }



        private HttpResponseMessage SiteRequest(BAL.Enums.SiteItemType itemTypes)
        {
            string dto = System.Web.HttpContext.Current.Request["dto"];
            JObject jsonObject = new JObject();
            jsonObject = JObject.Parse(dto);
            var items = jsonObject["Items"];
            WorkOrder wOrder = new WorkOrder(0);
            SiteDTO siteDto = BuildSiteObject(jsonObject, items);
            wOrder.AddSiteItems(siteDto, itemTypes);
            return Request.CreateResponse(HttpStatusCode.OK, "Saved");
        }
        [HttpPost]
        public HttpResponseMessage ReceiveItems()
        {
            return SiteRequest(BAL.Enums.SiteItemType.RECIEVED);
        }
        private void AddSiteToObject(WorkOrder wOrder, JObject jsonObject)
        {
            var items = jsonObject["Items"];

            SiteDTO siteDto = BuildSiteObject(jsonObject, items);
            if (String.IsNullOrEmpty(siteDto.JobNumber))
            {
                siteDto.JobNumber = wOrder.Number;
            }


            siteDto.StartDate = wOrder.WorkOrderDate;
            siteDto.RentStartDate = wOrder.RentStartDate;
            if (siteDto.SiteId == 0)
            {
                if (System.Web.HttpContext.Current.Request.Files.Count >= 1)
                {
                    siteDto.Doc1 = SaveFile(System.Web.HttpContext.Current.Request.Files[0], "");
                }
                if (System.Web.HttpContext.Current.Request.Files.Count >= 2)
                {
                    siteDto.Doc2 = SaveFile(System.Web.HttpContext.Current.Request.Files[1], "");
                }
                if (System.Web.HttpContext.Current.Request.Files.Count >= 3)
                {
                    siteDto.Doc3 = SaveFile(System.Web.HttpContext.Current.Request.Files[2], "");
                }
            }
            wOrder.Sites.Add(siteDto);
        }

        private static SiteDTO BuildSiteObject(JObject jsonObject, JToken items)
        {
            List<WorkOrderItemDTO> woItems = AddItemsToSite(items);
            var siteInfo = jsonObject["SiteInfo"];

            SiteDTO siteDto = new SiteDTO();
            siteDto.JobNumber = Convert.ToString(siteInfo["JobNumber"]);
            siteDto.ChallanNumber = Convert.ToString(siteInfo["ChallanNumber"]);

            siteDto.Site = Convert.ToString(siteInfo["Site"]);
            siteDto.ShaftSize = Convert.ToString(siteInfo["ShaftSize"]);
            siteDto.ShaftHeight = Convert.ToString(siteInfo["ShaftHeight"]);
            siteDto.SiteEng = Convert.ToString(siteInfo["SiteEng"]);
            if (siteInfo["Vehicle"] != null)
            {
                siteDto.Vehicle = Convert.ToString(siteInfo["Vehicle"]);
            }
            if (siteInfo["Driver"] != null)
            {
                siteDto.Driver = Convert.ToString(siteInfo["Driver"]);
            }
            if (siteInfo["VehicleId"] != null)
            {
                siteDto.VehicleId = Convert.ToInt16(siteInfo["VehicleId"]);
            }
            if (siteInfo["DriverId"] != null)
            {
                siteDto.DriverId = Convert.ToInt16(siteInfo["DriverId"]);
            }
            if (siteInfo["LedgerSiteId"] != null)
            {
                siteDto.LedgerSiteId = Convert.ToInt32(siteInfo["LedgerSiteId"]);
            }
            if (siteInfo["TransporterId"] != null)
            {
                siteDto.TransporterId = Convert.ToInt32(siteInfo["TransporterId"]);
            }
            if (siteInfo["TeamId"] != null)
            {
                siteDto.TeamId = Convert.ToInt32(siteInfo["TeamId"]);
            }
            if (siteInfo["RecoveryDate"] != null && !String.IsNullOrEmpty(Convert.ToString(siteInfo["RecoveryDate"])))
            {
                siteDto.RecoveryDate = Convert.ToDateTime(siteInfo["RecoveryDate"]);

            }

            if (siteInfo["RecoveryDate"] != null && !String.IsNullOrEmpty(Convert.ToString(siteInfo["RecoveryDate"])))
            {
                siteDto.RecoveryDate = Convert.ToDateTime(siteInfo["RecoveryDate"]);

            }



            siteDto.StartDate = DateTime.Now;
            siteDto.Duration = siteInfo["Duration"] != null ? Convert.ToInt16(siteInfo["Duration"]) : 0;
            siteDto.State = siteInfo["State"] != null ? Convert.ToString(siteInfo["State"]) : null;
            siteDto.Freight = siteInfo["Freight"] != null ? Convert.ToDouble(siteInfo["Freight"]) : 0;
            siteDto.FreightTax = siteInfo["FreightTax"] != null ? Convert.ToDouble(siteInfo["FreightTax"]) : 0;
            siteDto.SubTotal = siteInfo["SubTotal"] != null ? Convert.ToDouble(siteInfo["SubTotal"]) : 0;
            siteDto.TaxAmount = siteInfo["TaxAmount"] != null ? Convert.ToDouble(siteInfo["TaxAmount"]) : 0;
            siteDto.Total = siteInfo["Total"] != null ? Convert.ToDouble(siteInfo["Total"]) : 0;
            siteDto.SiteId = siteInfo["SiteId"] != null ? Convert.ToInt32(siteInfo["SiteId"]) : 0;
            siteDto.Weight = siteInfo["Weight"] != null ? Convert.ToDecimal(siteInfo["Weight"]) : 0;
            siteDto.ApproximateValue = siteInfo["ApproximateValue"] != null ? Convert.ToDouble(siteInfo["ApproximateValue"]) : 0;
            if (siteInfo["LRNumber"] != null)
                siteDto.LRNumber = Convert.ToString(siteInfo["LRNumber"]);
            if (siteInfo["CRNumber"] != null)
                siteDto.CRNumber = Convert.ToString(siteInfo["CRNumber"]);
            if (siteInfo["GRNumber"] != null)
                siteDto.GRNumber = Convert.ToString(siteInfo["GRNumber"]);

            siteDto.Items = woItems;
            siteDto.AppliedTaxes = ParseWorkOrderTaxList(jsonObject["AppliedTaxes"]);

            //var taxInfo = jsonObject["Taxes"];
            //siteDto.Taxes = new List<TaxDTO>();
            //foreach (var tax in taxInfo)
            //{
            //    TaxDTO taxDto = new TaxDTO();
            //    taxDto.TaxId = Convert.ToInt16(tax["TaxId"]);
            //    taxDto.Rate = Convert.ToDouble(tax["Rate"]);
            //    bool applicable = Convert.ToBoolean(tax["Applicable"]);
            //    //taxDto.TaxAmount= Convert.ToInt16(tax["TaxId"]);
            //    if (applicable)
            //        siteDto.Taxes.Add(taxDto);
            //}
            return siteDto;
        }

        private static List<WorkOrderTaxDTO> ParseWorkOrderTaxList(JToken token)
        {
            var taxes = new List<WorkOrderTaxDTO>();
            if (token == null)
            {
                return taxes;
            }

            foreach (var tax in token)
            {
                if (tax == null || tax["TaxId"] == null)
                {
                    continue;
                }

                taxes.Add(new WorkOrderTaxDTO
                {
                    TaxId =  Convert.ToInt32(tax["TaxId"]),
                    TaxCategoryId = tax["TaxCategoryId"] != null ? Convert.ToInt32(tax["TaxCategoryId"]) : 0,
                    TaxName = Convert.ToString(tax["TaxName"]),
                    TaxCode = Convert.ToString(tax["TaxCode"]),
                    Rate = tax["Rate"] != null ? Convert.ToDecimal(tax["Rate"]) : 0,
                    RateType = tax["RateType"] != null ? Convert.ToString(tax["RateType"]) : "Percentage",
                    Amount = tax["Amount"] != null ? Convert.ToDecimal(tax["Amount"]) : 0,
                    WorkOrderItemId = tax["WorkOrderItemId"] != null ? Convert.ToInt32(tax["WorkOrderItemId"]) : 0,
                    ProductId = tax["ProductId"] != null ? Convert.ToInt32(tax["ProductId"]) : 0,
                    SiteId = tax["SiteId"] != null ? Convert.ToInt32(tax["SiteId"]) : 0
                });
            }

            return taxes;
        }

        private static List<WorkOrderItemDTO> AddItemsToSite(JToken items)
        {
            List<WorkOrderItemDTO> woItems = new List<WorkOrderItemDTO>();
            foreach (var item in items)
            {
                //  var itemObj = item["Item"];
                // if (itemObj == null) continue;
                WorkOrderItemDTO itemDto = new WorkOrderItemDTO();
                itemDto.ProductId = Convert.ToInt32(item["ProductId"]);
                itemDto.WorkOrderItemId = Convert.ToInt32(item["WorkOrderItemId"]);
                itemDto.Deleted = Convert.ToByte(item["Deleted"]);
                if (item["ProductSizeId"] != null)
                {
                    itemDto.ProductSizeId = 0;// Convert.ToInt32(item["ProductSizeId"]);
                }
                if (item["GroupItemId"] != null)
                {
                    itemDto.GroupItemId = Convert.ToInt32(item["GroupItemId"]);
                }
                itemDto.Size = Convert.ToString(item["Size"]);
                itemDto.Rate = Convert.ToDouble(item["Rate"]);
                itemDto.PurchaseQty = Convert.ToDouble(item["SentQty"]);
                itemDto.SubTotal = itemDto.Rate * itemDto.PurchaseQty;

                if (item["TaxCategoryId"] != null)
                {
                    itemDto.TaxCategoryId = Convert.ToInt32(item["TaxCategoryId"]);
                }

                itemDto.LineTaxes = ParseWorkOrderTaxList(item["LineTaxes"]);
                woItems.Add(itemDto);

            }
            return woItems;
        }

        [HttpPost]
        public HttpResponseMessage GetSiteItems([FromBody] SiteDTO obj)
        {
            WorkOrder objWorkOrder = new WorkOrder(0);
            return Request.CreateResponse(HttpStatusCode.OK, objWorkOrder.GetSiteItems(obj.SiteId));
        }
        [HttpPost]
        public HttpResponseMessage GetSiteTaxes([FromBody] SiteDTO obj)
        {
            WorkOrder objWorkOrder = new WorkOrder(0);
            return Request.CreateResponse(HttpStatusCode.OK, objWorkOrder.GetSiteTaxes(obj.SiteId));
        }
        [HttpPost]
        public async Task<ApiMessage> DeleteChallan([FromBody] SiteDTO obj)
        {
            var msg = new ApiMessage();
            try
            {
                var ledger = new Ledger(obj.LedgerId);
                var wo = new WorkOrder(obj.WorkOrderId);
                //if (wo.LedgerId == 0 || obj.LedgerSiteId == 0)
                //{
                //    throw new Exception("Invalid input");
                //}


                //var lastBill = ledger.GetLastBill(wo.LedgerId, obj.LedgerSiteId, 0);

                //if (lastBill != null)
                //{

                //    if (wo.WorkOrderDate <= Convert.ToDateTime(lastBill.To))
                //    {
                //        throw new Exception("Bills has been generated, Can not delete");
                //    }
                //}

                var user = new LoggedInUserInfo();
                WorkOrder objWorkOrder = new WorkOrder(0);
                msg.Data = await objWorkOrder.DeleteChallan(obj.WorkOrderId, obj.SiteId, user);
                msg.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                msg.Message = ex.Message;
                msg.Code = ApiMessageCodes.ERROR;
                logger.Error("Could not delete challan", ex);
            }
            return msg;
        }

        [HttpPost]
        public HttpResponseMessage UpdateSiteInfo([FromBody] SiteDTO obj)
        {
            try
            {
                CultureInfo ci = CultureInfo.CreateSpecificCulture("en-US");
                String d = obj.StartDate.ToString(ci);

                obj.StartDate = Convert.ToDateTime(d, ci);
                WorkOrder objWorkOrder = new WorkOrder(0);
                return Request.CreateResponse(HttpStatusCode.OK, objWorkOrder.UpdateSiteInfo(obj));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public async Task<ApiMessage> UpdateStatus([FromBody] WorkOrderDTO dto)
        {
            var response = new ApiMessage();
            try
            {
                if (dto == null || dto.WorkOrderId <= 0)
                {
                    throw new Exception("Invalid work order.");
                }
                var user = new LoggedInUser();
                dto.CompanyId = user.DefaultCompanyId;
                dto.ModifiedBy = user.UserId;
                dto.ModifiedOn = DateTime.Now;
                dto.StatusId = 1;

                var workOrder = new WorkOrder(0);
                var data = await workOrder.UpdateStatus(dto);
                if (data > 0)
                {
                    if (Config.IsDispatchInwardNotificationEnabled(dto.CompanyId))
                    {
                        string apiKey = Company.GetRentacApiKeyByCompanyId(dto.CompanyId);
                        if (!String.IsNullOrEmpty(apiKey))
                        {


                            var recipient = ResolvePartyRecipientByLedgerId(new WorkOrder(dto.WorkOrderId).LedgerId);
                            if (!String.IsNullOrEmpty(recipient))
                            {
                                var token = Request?.Headers?.Contains("Authorization") == true
                                    ? Request.Headers.GetValues("Authorization").FirstOrDefault()
                                    : null;

                                try
                                {
                                    var queuePublisher = new ServiceBusQueuePublisher();
                                    await queuePublisher.PublishContractReminderAsync(new NotificationDto
                                    {
                                        CompanyId = dto.CompanyId,
                                        StatusId = dto.StatusId,
                                        ModifiedBy = dto.ModifiedBy,
                                        ModifiedOn = dto.ModifiedOn,
                                        RentacApiKey = apiKey,
                                        Token = token,
                                        Type = "whatsapp",
                                        Receipients = recipient,
                                        MetaData = "1109," + dto.WorkOrderId
                                    }, "contractreminders");
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex, "Failed to publish contract reminder queue message for WorkOrderId {0}", dto.WorkOrderId);
                                }
                            }
                        }
                    }
                }

                response.Data = data;
                response.Code = ApiMessageCodes.SUCCESS;
                response.Message = ApiMessage.SUCCESS;
            }
            catch (Exception ex)
            {
                response.Code = ApiMessageCodes.ERROR;
                response.Message = ex.Message;
                logger.Error(ex, "Could not update work order dispatch status");
            }

            return response;
        }
        [HttpPost]
        public async Task<IHttpActionResult> ItemIssued([FromBody] BillingDTO obj)
        {
            if (obj == null)
                return BadRequest("Invalid request");
            WorkOrder objWorkOrder = new WorkOrder(0);
            var allData = (await objWorkOrder.ItemIssued(obj.From.ToShortDateString(), obj.To.ToShortDateString(),
                obj.ChallanType, obj.LedgerId, obj.LedgerSiteId, new LoggedInUser().DefaultCompanyId, obj.ChallanNo ?? string.Empty,
                obj.IssuedListStatusFilter)).ToList();
            // Group by WorkOrderId for list view (one row per challan)
            var grouped = allData.GroupBy(x => x.WorkOrderId).Select(g =>
            {
                var first = g.First();
                first.SentQty = g.Sum(x => x.SentQty);
                return first;
            }).ToList();
            var totalCount = grouped.Count;
            if (obj.PageSize > 0 && obj.PageIndex >= 1)
            {
                var pageData = grouped.Skip((obj.PageIndex - 1) * obj.PageSize).Take(obj.PageSize).ToList();
                return Ok(new { Data = pageData, TotalCount = totalCount });
            }
            return Ok(new { Data = grouped, TotalCount = totalCount });
        }

        [HttpPost]
        public HttpResponseMessage ItemReceived([FromBody] BillingDTO obj)
        {
            WorkOrder objWorkOrder = new WorkOrder(0);
            var list = objWorkOrder.ItemReceived(obj.From.ToShortDateString(),
                obj.To.ToShortDateString(), obj.ChallanType, obj.LedgerId, obj.LedgerSiteId, new LoggedInUser().DefaultCompanyId, obj.ChallanNo,
                obj.ReceivedListStatusFilter);
            var totalCount = list.Count;
            if (obj.PageSize > 0 && obj.PageIndex >= 1)
            {
                var pageData = list.Skip((obj.PageIndex - 1) * obj.PageSize).Take(obj.PageSize).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new { Data = pageData, TotalCount = totalCount });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { Data = list, TotalCount = totalCount });
        }
        [HttpPost]
        public async Task<HttpResponseMessage> ItemReceived_rpt([FromBody] List<GRNDTO> list)
        {


            //   String fileName = rep.PrintAndEmailReceiveReceipt(obj.GRNId, false);

            ReportUtility rep = new ReportUtility();
            List<String> files = new List<String>();
            foreach (GRNDTO w in list)
            {
                string fileName = await rep.PrintAndEmailReceiveReceipt(w.GRNId, false);
                files.Add(fileName);
            }
            string outPutFIle = "";
            if (files.Count > 0)
            {
                outPutFIle = rep.MergeFiles(files, Guid.NewGuid().ToString() + ".pdf");
            }
            return Request.CreateResponse(HttpStatusCode.OK, outPutFIle);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> EmailReceivedReceipt([FromBody] GRNDTO dto)
        {
            ApiMessage message = new ApiMessage();
            try
            {

                ReportUtility rep = new ReportUtility();
                string fileName = await rep.PrintAndEmailReceiveReceipt(dto.GRNId, true);
                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        [HttpPost]
        public HttpResponseMessage SMSReceivedReceipt([FromBody] GRNDTO dto)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                GRN objGRN = new GRN();
                DataSet ds = objGRN.GRNHeader(dto.GRNId);
                if (ds.Tables.Count > 0)
                {

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        string grn = Convert.ToString(ds.Tables[0].Rows[0]["GRN"]);
                        int ledgerId = Convert.ToInt32(ds.Tables[0].Rows[0]["LedgerId"]);
                        CommHelpler com = new CommHelpler();
                        EmailParameters p = new EmailParameters() { ChallanNumber = grn };
                        Ledger ledger = new Ledger(ledgerId);
                        LedgerDTO lDto = ledger.GetDetails();
                        com.sendSms(lDto.ContactPersonMobile, p, MessageEvent.RECEIVE_ITEM);
                        message.Code = ApiMessageCodes.SUCCESS;
                    }
                    else
                    {
                        message.Code = ApiMessageCodes.ERROR;
                        message.Message = "Not Found";
                    }
                }
                else
                {
                    message.Code = ApiMessageCodes.ERROR;
                    message.Message = "Not Found";
                }


            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> PrintIssueReceipt([FromBody] List<WorkOrderDTO> dto)
        {
            try
            {
                var user = new LoggedInUser();
                WorkOrder wOrder = new WorkOrder(0);
                ReportUtility rep = new ReportUtility();
                List<String> files = new List<String>();
                foreach (WorkOrderDTO w in dto)
                {
                    string fileName = await rep.EmailRentIssueReceipt(w.WorkOrderId, user.DefaultCompanyId, false);
                    files.Add(fileName);
                }
                string outPutFIle = "";
                if (files.Count > 0)
                {
                    outPutFIle = rep.MergeFiles(files, Guid.NewGuid().ToString() + ".pdf");
                }
                return Request.CreateResponse(HttpStatusCode.OK, outPutFIle);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                logger.Error(ex.Message, ex.StackTrace);
                if (ex.InnerException != null)
                    logger.Error(ex.Message, ex.InnerException);

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> UpdateParty([FromBody] List<ChallanChangePartyDto> dto)
        {
            var res = new ApiMessage();
            try
            {
                WorkOrder wOrder = new WorkOrder(0);
                ReportUtility rep = new ReportUtility();
                List<String> files = new List<String>();


                var wo = new WorkOrder(0);
                var user = new LoggedInUser();
                var userInfo = new LoggedInUserInfo();
                userInfo.InjectFrom(user);
                res.Data = await wo.ChangeChallanParty(dto, userInfo);
                res.Code = ApiMessageCodes.SUCCESS;
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                logger.Error(ex.Message, ex.StackTrace);
                if (ex.InnerException != null)
                    logger.Error(ex.Message, ex.InnerException);
                res.Code = ApiMessageCodes.ERROR;
                res.Message = ex.Message;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, res);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> EmailIssueChallan([FromBody] WorkOrderDTO dto)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                WorkOrder wOrder = new WorkOrder(0);
                ReportUtility rep = new ReportUtility();
                var x = await rep.EmailRentIssueReceipt(dto.WorkOrderId, user.DefaultCompanyId, true);
                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        [HttpPost]
        public HttpResponseMessage SmsIssueChallan([FromBody] WorkOrderDTO dto)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                WorkOrder wOrder = new WorkOrder(dto.WorkOrderId);
                DataSet ds = wOrder.GetChallanReportHeader(dto.WorkOrderId, 1);
                CommHelpler com = new CommHelpler();
                if (ds.Tables.Count > 0)
                {
                    WorkOrderDTO siteDTO = new Utils<WorkOrderDTO>().ConstructList(ds).FirstOrDefault();
                    EmailParameters p = new EmailParameters() { ChallanNumber = siteDTO.Number, Driver = siteDTO.Driver, Vehicle = siteDTO.Vehicle };
                    Ledger ledger = new Ledger(wOrder.LedgerId);
                    LedgerDTO lDto = ledger.GetDetails();
                    com.sendSms(lDto.ContactPersonMobile, p, MessageEvent.ISSUE_CHALLAN);
                    message.Code = ApiMessageCodes.SUCCESS;
                }
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError, message);
        }


        [HttpPost]
        public HttpResponseMessage PrintDeliveryChallanReceipt([FromBody] WorkOrderDTO dto)
        {
            try
            {
                WorkOrder wOrder = new WorkOrder(0);

                int partyId = 0;
                int companyId = new LoggedInUser().DefaultCompanyId;
                BAL.Objects.Report objReport = new BAL.Objects.Report();

                DataSet mainDS = wOrder.ItemIssuedForPrint(dto.WorkOrderId, companyId);
                if (mainDS.Tables.Count > 0)
                {
                    if (mainDS.Tables[0].Rows.Count > 0)
                    {
                        partyId = Convert.ToInt32(mainDS.Tables[0].Rows[0]["LedgerId"]);
                    }
                }
                DataSet headerDataSet = wOrder.GetChallanReportHeader(dto.WorkOrderId, dto.ChallanHeaderType);

                string fileName = CreateReportFile(dto.WorkOrderId.ToString(), "deliveryChallanReceipt.rdlc", headerDataSet, mainDS, BAL.Enums.ExportFormat.PDF);
                return Request.CreateResponse(HttpStatusCode.OK, fileName);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage GetWorkOrderBalance([FromBody] WorkOrderDTO obj)
        {
            WorkOrder objWorkOrder = new WorkOrder(0);
            //obj.From = Utils.FormatDate(obj.From).ToShortDateString();
            //obj.To = Utils.FormatDate(obj.To).ToShortDateString();
            return Request.CreateResponse(HttpStatusCode.OK, objWorkOrder.GetWorkOrderBalance(obj.LedgerId, new LoggedInUser().DefaultCompanyId));
        }
        [HttpPost]
        public HttpResponseMessage WorkOrderDueDateReminder([FromBody] WorkOrderDTO obj)
        {
            WorkOrder objWorkOrder = new WorkOrder(0);

            return Request.CreateResponse(HttpStatusCode.OK, objWorkOrder.WorkOrderDueDateReminder(obj.LedgerId, new LoggedInUser().DefaultCompanyId));
        }
        [HttpPost]
        public HttpResponseMessage WorkOrderOverDuesReminder([FromBody] WorkOrderDTO obj)
        {
            WorkOrder objWorkOrder = new WorkOrder(0);
            //obj.From = Utils.FormatDate(obj.From).ToShortDateString();
            //obj.To = Utils.FormatDate(obj.To).ToShortDateString();
            return Request.CreateResponse(HttpStatusCode.OK, objWorkOrder.WorkOrderOverDuesReminder(obj.LedgerId, new LoggedInUser().DefaultCompanyId));
        }

        [HttpPost]
        public HttpResponseMessage GetOtherCharges([FromBody] WorkOrderDTO obj)
        {
            WorkOrder objWorkOrder = new WorkOrder(obj.WorkOrderId);
            //obj.From = Utils.FormatDate(obj.From).ToShortDateString();
            //obj.To = Utils.FormatDate(obj.To).ToShortDateString();
            return Request.CreateResponse(HttpStatusCode.OK, objWorkOrder.GetOtherChages(obj.WorkOrderId));
        }
        [HttpPost]
        public HttpResponseMessage LederSiteCharges([FromBody] ClientSiteDTO obj)
        {
            WorkOrder objWorkOrder = new WorkOrder(0);
            var user = new LoggedInUser();
            //obj.From = Utils.FormatDate(obj.From).ToShortDateString();
            //obj.To = Utils.FormatDate(obj.To).ToShortDateString();
            return Request.CreateResponse(HttpStatusCode.OK, objWorkOrder.GetSiteOtherCharges(obj.LedgerSiteId, user.DefaultCompanyId));
        }
        [HttpPost]
        public HttpResponseMessage DeleteWorkOrderItem([FromBody] WorkOrderItemDTO obj)
        {
            WorkOrder objWorkOrder = new WorkOrder(0);
            //obj.From = Utils.FormatDate(obj.From).ToShortDateString();
            //obj.To = Utils.FormatDate(obj.To).ToShortDateString();
            return Request.CreateResponse(HttpStatusCode.OK, objWorkOrder.DeleteChallanItem(obj.WorkOrderItemId));
        }
        [HttpPost]
        public HttpResponseMessage AddChallanDocument()
        {
            ApiMessage message = new ApiMessage();
            try
            {
                string dto = System.Web.HttpContext.Current.Request["dto"];
                JObject jsonObject = JObject.Parse(dto);
                int workOrderId = Convert.ToInt32(jsonObject.GetValue("WorkOrderId"));
                System.Web.HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                WorkOrder objWorkOrder = new WorkOrder(0);
                LoggedInUser user = new LoggedInUser();
                ChallanDocumentDTO chDto = new ChallanDocumentDTO();
                chDto.WorkOrderId = workOrderId;
                chDto.UploadedBy = user.UserId;

                if (files.Count > 0)
                {
                    string fileName = "CH-DOC-" + workOrderId + "-" + files[0].FileName;
                    string filePath = SaveFile(files[0], fileName);
                    chDto.FilePath = filePath;
                    ChallanDocumentDTO newCH = objWorkOrder.AddChallanDocument(chDto);
                    if (newCH != null)
                    {
                        message.Code = ApiMessageCodes.SUCCESS;
                        message.Data = newCH.ChallanDocumentId;
                    }
                }
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }
        [HttpPost]
        public HttpResponseMessage GetChallanDocument([FromBody] ChallanDocumentDTO dto)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                WorkOrder worder = new WorkOrder(0);

                message.Data = worder.GetChallanDocuments(dto.WorkOrderId);
                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);

        }
        [HttpPost]
        public HttpResponseMessage DeleteChallanDocument([FromBody] ChallanDocumentDTO dto)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                WorkOrder worder = new WorkOrder(0);

                message.Data = worder.DeleteChallanDocument(dto.ChallanDocumentId);
                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);

        }

        [HttpPost]
        public HttpResponseMessage PendingChallanAcknowledgements([FromBody] ChallanDocumentDTO dto)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                WorkOrder worder = new WorkOrder(0);
                LoggedInUser user = new LoggedInUser();
                DataSet ds = worder.PendingChallanAcknoledgements(user.FinYearId, user.DefaultCompanyId);
                List<SiteDTO> lst = (new Utils<SiteDTO>()).ConstructList(ds);
                message.Data = lst;
                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);

        }
        [HttpPost]
        public async Task<HttpResponseMessage> TransferMaterial()
        {

            ApiMessage message = new ApiMessage();
            try
            {
                string dto = System.Web.HttpContext.Current.Request["dto"];

                GRN objGRN = new GRN();
                JObject jsonObject = JObject.Parse(dto);
                LoggedInUser user = new LoggedInUser();
                objGRN.FinYearId = user.FinYearId;
                int createdBy = user.UserId;
                if (jsonObject["SourceLedgerSiteId"] != null)
                {
                    objGRN.LedgerSiteId = Convert.ToInt32(jsonObject["SourceLedgerSiteId"]);
                }
                objGRN.LedgerId = Convert.ToInt16(jsonObject["SourceLedgerId"]);
                objGRN.ReceivingDate = objGRN.RentStopDate = Convert.ToDateTime(jsonObject["ReceivingDate"]);
                objGRN.Remarks = Convert.ToString(jsonObject["Remarks"]);
                objGRN.CompanyId = user.DefaultCompanyId;
                objGRN.FinYearId = user.FinYearId;
                int driverId = 0, vehicleId = 0, ledgerSiteId = 0;
                WorkOrder wo = new WorkOrder(0);
                if (jsonObject["Driver"] != null)
                {
                    wo.Driver = Convert.ToString(jsonObject["Driver"]);
                }
                if (jsonObject["Vehicle"] != null)
                {
                    wo.Vehicle = Convert.ToString(jsonObject["Vehicle"]);
                }
                if (jsonObject["DestLedgerSiteId"] != null)
                {
                    ledgerSiteId = Convert.ToInt32(jsonObject["DestLedgerSiteId"]);
                    wo.LedgerSiteId = ledgerSiteId;
                }

                if (ledgerSiteId == 0 || objGRN.LedgerSiteId == 0)
                {
                    throw new Exception("Please select the source and destination sites");
                }
                if (ledgerSiteId == objGRN.LedgerSiteId)
                {
                    throw new Exception("Source and destination sites must not be same");
                }
                if (jsonObject["ChallanNumber"] != null)
                {
                    wo.Number = wo.WorkOrderNumber = Convert.ToString(jsonObject["ChallanNumber"]);
                }
                if (jsonObject["Weight"] != null)
                {
                    wo.Weight = Convert.ToDecimal(jsonObject["Weight"]);
                }
                if (jsonObject["Freight"] != null)
                {
                    wo.Freight = Convert.ToDouble(jsonObject["Freight"]);
                }

                var items = jsonObject["Items"];
                List<WorkOrderItemDTO> wItems = new List<WorkOrderItemDTO>();
                foreach (var i in items)
                {
                    if (i["ProductId"] == null || i["ProductId"].Type == JTokenType.Null)
                    {
                        continue;
                    }
                    double qty = Convert.ToDouble(i["Quantity"]);
                    if (qty == 0) continue;
                    WorkOrderItemDTO item = new WorkOrderItemDTO
                    {
                        ProductId = Convert.ToInt32(i["ProductId"]),
                        Quantity = Convert.ToDouble(i["Quantity"]),
                    };
                    if (i["ProductSizeId"] != null && i["ProductSizeId"].Type != JTokenType.Null)
                    {
                        item.ProductSizeId = Convert.ToInt32(i["ProductSizeId"]);
                    }
                    if (i["Rate"] != null && i["Rate"].Type != JTokenType.Null)
                    {
                        item.Rate = Convert.ToDouble(i["Rate"]);
                    }

                    wItems.Add(item);
                }
                objGRN.GRN = "";
                objGRN.Items = wItems;
                var otherCharges = jsonObject["OtherCharges"];

                wo.OtherCharges = new List<WorkOrderChageDTO>();
                if (otherCharges != null)
                {
                    foreach (var ch in otherCharges)
                    {
                        WorkOrderChageDTO chDto = new WorkOrderChageDTO();
                        chDto.ChargeId = Convert.ToInt32(ch["ChargeId"]);
                        chDto.Amount = Convert.ToDouble(ch["Amount"]);
                        if (ch["WorkOrderChargeId"] != null)
                        {
                            chDto.WorkOrderChargeId = Convert.ToInt32(ch["WorkOrderChargeId"]);
                        }
                        if (chDto.Amount > 0 || chDto.WorkOrderChargeId > 0)
                        {
                            wo.OtherCharges.Add(chDto);
                        }

                    }
                }

                var ledger = new Ledger(objGRN.LedgerId);
                //var lastBill = ledger.GetLastBill(objGRN.LedgerId, objGRN.LedgerSiteId, 0);

                //if (lastBill != null)
                //{

                //    if (objGRN.ReceivingDate <= Convert.ToDateTime(lastBill.To))
                //    {
                //        throw new Exception("Receiving date can not be on or before last bill date");
                //    }
                //}

                wo.CompanyId = user.DefaultCompanyId;
                wo.FinYearId = user.FinYearId;
                wo.LedgerSiteId = ledgerSiteId;
                wo.CreatedBy = user.UserId;
                wo.WorkOrderDate = objGRN.ReceivingDate;
                wo.RentStartDate = objGRN.ReceivingDate;
                if (jsonObject["DestLedgerId"] != null)
                {
                    wo.LedgerId = Convert.ToInt32(jsonObject["DestLedgerId"]);
                }
                //objGRN.Freight = wo.Freight;
                objGRN.Driver = wo.Driver ?? "";
                objGRN.VehicleNo = wo.Vehicle ?? "";
                objGRN.Weight = wo.Weight;

                int workOrderId = 0;
                if (jsonObject["WorkOrderId"] != null && jsonObject["WorkOrderId"].Type != JTokenType.Null)
                {
                    workOrderId = Convert.ToInt32(jsonObject["WorkOrderId"]);
                }
                int grnId = 0;
                if (jsonObject["GRNId"] != null && jsonObject["GRNId"].Type != JTokenType.Null)
                {
                    grnId = Convert.ToInt32(jsonObject["GRNId"]);
                }

                if (workOrderId > 0 && grnId > 0)
                {
                    wo.WorkOrderId = workOrderId;
                    objGRN.GRNId = grnId;
                    message.Data = await wo.UpdateTransferChallan(wo, objGRN);
                }
                else
                {
                    message.Data = await wo.TransferChallan(wo, objGRN);
                }
                message.Code = ApiMessageCodes.SUCCESS;


            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }
        [HttpPost]
        public HttpResponseMessage AdjustMaterial()
        {

            ApiMessage message = new ApiMessage();
            try
            {



                string dto = System.Web.HttpContext.Current.Request["dto"];
                JObject jsonObject = JObject.Parse(dto);
                var issueList = jsonObject["IssueList"];
                var recList = jsonObject["ReceiveList"];
                byte adjType = 1;
                if (jsonObject["AdjType"] != null && jsonObject["AdjType"].Type != JTokenType.Null)
                {
                    adjType = Convert.ToByte(jsonObject["AdjType"]);
                    if (adjType < 1 || adjType > 2)
                    {
                        adjType = 1;
                    }
                }
                LoggedInUser user = new LoggedInUser();
                NextId n = new NextId();
                string number = "";
                if (jsonObject["Number"] == null)
                {
                    n.GetNextKeyWithPrefix(BAL.Enums.NextIDTables.WorkOrder, ConfigCategory.ISSUECHALLAN, ConfigCategory.ISSUECHALLAN, user.DefaultCompanyId, user.FinYearId);
                }
                else
                {
                    number = Convert.ToString(jsonObject["Number"]);
                }
                TransactionDTO trDto = new TransactionDTO();
                trDto.Number = number;
                trDto.TransactionType = Convert.ToInt16(TransactionTypes.MAT_ADJUST);
                trDto.UserId = user.UserId;
                trDto.FinYearId = user.FinYearId;
                trDto.CompanyId = user.DefaultCompanyId;
                trDto.LedgerId = Convert.ToInt16(jsonObject["LedgerId"]); ;
                trDto.LedgerSiteId = Convert.ToInt16(jsonObject["LedgerSiteId"]); ;
                trDto.TransactionDate = Convert.ToDateTime(jsonObject["WorkOrderDate"]);
                Transaction tr = new Transaction();
                int transactionId = 0;
                /* if (jsonObject["TransactionId"] != null)
                 {
                     transactionId = Convert.ToInt32(jsonObject["TransactionId"]);
                 }
                 else
                 {
                     bool result = tr.Add(trDto);

                     if (!result)
                     {
                         message.Code = ApiMessageCodes.ERROR;
                         message.Message = "Transaction failed";
                         return Request.CreateResponse(HttpStatusCode.OK, message);
                     }
                     transactionId = trDto.TransactionId;
                 } */

                //------ISSUE
                WorkOrder wo = new WorkOrder(0);
                wo.TransactionId = transactionId;
                wo.Number = number;
                wo.LedgerId = Convert.ToInt16(jsonObject["LedgerId"]);
                wo.CompanyId = user.DefaultCompanyId;
                wo.FinYearId = user.FinYearId;
                wo.WorkOrderDate = trDto.TransactionDate;
                wo.RentStartDate = trDto.TransactionDate;
                wo.CreatedBy = user.UserId;
                wo.ChallanType = ChallanType.MAT_ADJUST;
                wo.RbnClientId = user.RbnClientId;
                wo.AdjType = adjType;

                wo.Sites.Add(new SiteDTO());
                wo.Sites[0].Items = AddItemsToSite(issueList);
                wo.Sites[0].LedgerSiteId = Convert.ToInt16(jsonObject["LedgerSiteId"]);
                wo.Sites[0].StartDate = wo.Sites[0].RentStartDate = wo.WorkOrderDate;
                if (jsonObject["WorkOrderId"] != null)
                {
                    wo.WorkOrderId = Convert.ToInt32(jsonObject["WorkOrderId"]);
                }
                if (jsonObject["SiteId"] != null)
                {
                    wo.Sites[0].SiteId = Convert.ToInt32(jsonObject["SiteId"]);
                    wo.Sites[0].WorkOrderId = wo.WorkOrderId;
                }
                //wo.Sites[0].LedgerId = Convert.ToInt16(jsonObject["LedgerId"]);
                //wo.Sites[0].CompanyId = wo.CompanyId;
                //--RECEIVE
                GRN grn = new GRN();
                grn.LedgerId = wo.LedgerId;
                grn.FinYearId = wo.FinYearId;
                grn.CreatedBy = wo.CreatedBy;
                grn.CompanyId = wo.CompanyId;
                grn.ReceivingDate = trDto.TransactionDate;
                grn.RentStopDate = trDto.TransactionDate;
                grn.LedgerSiteId = wo.Sites[0].LedgerSiteId;
                grn.TransactionId = trDto.TransactionId;
                grn.ChallanType = Convert.ToInt16(ChallanType.MAT_ADJUST);
                grn.AdjType = adjType;

                List<WorkOrderItemDTO> wItems = new List<WorkOrderItemDTO>();
                var ledger = new Ledger(grn.LedgerId);
                //var lastBill = ledger.GetLastBill(grn.LedgerId, grn.LedgerSiteId, 0);

                //if (lastBill != null)
                //{

                //    if (grn.ReceivingDate <= Convert.ToDateTime(lastBill.To))
                //    {
                //        throw new Exception("Receiving date can not be on or before last bill date");
                //    }
                //}
                foreach (var i in recList)
                {

                    double qty = Convert.ToDouble(i["Quantity"]); // do not receive 0 quantity
                    if (qty <= 0) continue;
                    WorkOrderItemDTO item = new WorkOrderItemDTO
                    {
                        ProductId = Convert.ToInt32(i["ProductId"]),
                        Quantity = Convert.ToDouble(i["Quantity"])
                    };
                    //if (i["ProductSizeId"] != null)
                    //{
                    //    item.ProductSizeId = Convert.ToInt32(i["ProductSizeId"]);
                    //}
                    if (i["GRnId"] != null)
                    {
                        item.GRNId = Convert.ToInt32(i["GRnId"]);
                    }
                    if (i["GRNItemId"] != null)
                    {
                        item.GRNItemId = Convert.ToInt32(i["GRNItemId"]);
                    }

                    wItems.Add(item);
                }
                grn.Items = wItems;

                bool adjResult = wo.ItemAdjustment(wo, grn);
                if (adjResult)
                {
                    message.Code = ApiMessageCodes.SUCCESS;
                    message.Data = adjResult;
                }

            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }

            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> MatTransferDetailsById([FromBody] WorkOrderDTO obj)
        {
            WorkOrder objWorkOrder = new WorkOrder(0);
            ApiMessage message = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                MaterialTransferDto ds = await objWorkOrder.MatTransferById(obj.WorkOrderId, user.DefaultCompanyId);
                if (ds != null)
                {
                    message.Code = ApiMessageCodes.SUCCESS;
                    message.Data = ds;
                }
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        [HttpPost]
        public HttpResponseMessage MatAjustList([FromBody] BillingDTO obj)
        {
            WorkOrder objWorkOrder = new WorkOrder(0);
            //obj.From = Utils.FormatDate(obj.From).ToShortDateString();
            //obj.To = Utils.FormatDate(obj.To).ToShortDateString();
            DataSet ds = objWorkOrder.MatAdjustList(obj.From.ToShortDateString(), obj.To.ToShortDateString(), obj.LedgerId, obj.LedgerSiteId, new LoggedInUser().DefaultCompanyId);
            DataTable table = new DataTable();
            if (ds != null && ds.Tables.Count > 0)
            {
                table = ds.Tables[0];
            }
            return Request.CreateResponse(HttpStatusCode.OK, table);
        }
        [HttpPost]
        public HttpResponseMessage MatAjusDetailsById([FromBody] WorkOrderDTO obj)
        {
            WorkOrder objWorkOrder = new WorkOrder(0);
            ApiMessage message = new ApiMessage();
            try
            {
                DataSet ds = objWorkOrder.MatAdjustById(obj.WorkOrderId);
                DataTable table = new DataTable();
                if (ds != null && ds.Tables.Count > 0)
                {
                    message.Code = ApiMessageCodes.SUCCESS;
                    message.Data = ds.Tables[0];
                }
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        [HttpPost]
        public async Task<ApiMessage> SaveMatLoss([FromBody] MatLossDTO dto)
        {
            Billing billing = new Billing();
            ApiMessage message = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                dto.CreatedBy = dto.ModifiedBy = user.UserId;
                dto.CreatedOn = dto.ModifiedOn = DateTime.Now;
                dto.CompanyId = user.DefaultCompanyId;
                dto.GuId = Guid.NewGuid().ToString();
                dto.FinYearId = user.FinYearId;
                var d = await billing.SaveMatLoss(dto);
                message.Code = ApiMessageCodes.SUCCESS;
                message.Data = true;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return message;
        }

        [HttpPost]
        public async Task<ApiMessage> MatLossList([FromBody] MatLossFilterDTO dto)
        {
            Billing billing = new Billing();
            ApiMessage message = new ApiMessage();
            try
            {
                var user = new LoggedInUser();

                if (dto.From.Year <= 2000 || dto.To.Year <= 2000)
                {
                    throw new UDFException("Please enter valid from and to date", ErrorCodes.MATLOSS_INVALID_FROM_TO_DATE);
                }

                dto.CompanyId = user.DefaultCompanyId;

                var d = await billing.MatLossList(dto);
                message.Code = ApiMessageCodes.SUCCESS;
                message.Data = d;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return message;
        }
        [HttpPost]
        public async Task<ApiMessage> MatLossById([FromBody] MatLossFilterDTO dto)
        {
            Billing billing = new Billing();
            ApiMessage message = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                if (dto.MatLossId <= 0)
                {
                    throw new UDFException("Please provide mat loss Id", ErrorCodes.MATLOSS_INVALID_KEY_BY_ID);
                }
                dto.CompanyId = user.DefaultCompanyId;

                var d = await billing.MatLossById(dto);
                message.Code = ApiMessageCodes.SUCCESS;
                message.Data = d;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return message;
        }
        [HttpPost]
        public async Task<ApiMessage> DeleteMatLossId([FromBody] MatLossFilterDTO dtoFilter)
        {
            Billing billing = new Billing();
            ApiMessage message = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                if (dtoFilter == null || dtoFilter.MatLossId <= 0)
                {
                    throw new UDFException("Please provide mat loss Id", ErrorCodes.MATLOSS_INVALID_KEY_BY_ID);
                }
                var dto = new MatLossDTO();
                dto.CompanyId = user.DefaultCompanyId;
                dto.MatLossId = dtoFilter.MatLossId;
                var d = await billing.DeleteMatLoss(dto);
                if (!d)
                {
                    message.Code = ApiMessageCodes.ERROR;
                    message.Data = false;
                    message.Message = "Could not delete record";
                    return message;
                }
                message.Code = ApiMessageCodes.SUCCESS;
                message.Data = d;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return message;
        }
        [HttpPost]
        public async Task<ApiMessage> GetLastChallanNo([FromBody] WorkOrderDTO dto)
        {
            var wo = new WorkOrder(0);
            ApiMessage message = new ApiMessage();
            try
            {
                var user = new LoggedInUser();

                dto.CompanyId = user.DefaultCompanyId;
                dto.FinYearId = user.FinYearId;
                var d = await wo.GetLastChallanNumber(dto);
                if (String.IsNullOrEmpty(d))
                {
                    message.Code = ApiMessageCodes.ERROR;
                    message.Data = false;
                    message.Message = "Could not delete record";
                    return message;
                }
                message.Code = ApiMessageCodes.SUCCESS;
                message.Data = d;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return message;
        }

        [HttpPost]
        public async Task<ApiMessage> GetNextChallanNumberPreview([FromBody] WorkOrderDTO dto)
        {
            var wo = new WorkOrder(0);
            ApiMessage message = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                dto.CompanyId = user.DefaultCompanyId;
                dto.FinYearId = user.FinYearId;
                var d = await wo.GetNextChallanNumberPreview(dto);
                message.Code = ApiMessageCodes.SUCCESS;
                message.Data = d ?? string.Empty;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return message;
        }

        [HttpPost]
        public async Task<ApiMessage> GetLastReturnChallanNo([FromBody] GRNDTO dto)
        {
            var wo = new GRN();
            ApiMessage message = new ApiMessage();
            try
            {
                var user = new LoggedInUser();

                dto.CompanyId = user.DefaultCompanyId;
                dto.FinYearId = user.FinYearId;
                var d = await wo.LastChallanNumber(dto.CompanyId, dto.FinYearId, Convert.ToInt16(dto.ChallanType));

                message.Code = ApiMessageCodes.SUCCESS;
                message.Data = d;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return message;
        }

        [HttpPost]
        public async Task<ApiMessage> GetNextReceivingChallanNumberPreview([FromBody] GRNDTO dto)
        {
            var grn = new GRN();
            ApiMessage message = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                dto.CompanyId = user.DefaultCompanyId;
                dto.FinYearId = user.FinYearId;
                var d = await grn.GetNextReceivingChallanNumberPreview(dto.CompanyId, dto.FinYearId, dto.ChallanType);
                message.Code = ApiMessageCodes.SUCCESS;
                message.Data = d ?? string.Empty;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return message;
        }

        #region WorkOrders

        private string ResolvePartyRecipientByLedgerId(int ledgerId)
        {
            if (ledgerId <= 0)
            {
                return null;
            }
            var details = new Ledger(ledgerId).GetDetails();
            if (details == null)
            {
                return null;
            }
            return NormalizeIndianMobile(details.ContactPersonMobile)
                   ?? NormalizeIndianMobile(details.ContactPersonOffPhone)
                   ?? NormalizeIndianMobile(details.Phone1);
        }

        private string NormalizeIndianMobile(string input)
        {
            if (String.IsNullOrWhiteSpace(input))
            {
                return null;
            }
            var digits = new string(input.Where(char.IsDigit).ToArray());
            if (digits.Length < 10)
            {
                return null;
            }
            digits = digits.Substring(digits.Length - 10, 10);
            return "91" + digits;
        }

        [HttpPost]
        public async Task<IHttpActionResult> WorkOrders([FromBody] WorkOrderDTO obj)
        {
            WorkOrder objWorkOrder = new WorkOrder(0);
            try
            {
                var user = new LoggedInUser();
                var msg = new ApiMessage();
                obj.CompanyId = user.DefaultCompanyId;
                msg.Data = await objWorkOrder.WorkOrders(obj);
                msg.Code = ApiMessageCodes.SUCCESS;

                return Ok(msg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

}
