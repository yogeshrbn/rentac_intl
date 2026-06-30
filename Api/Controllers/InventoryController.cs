using BAL.Common;
using BAL.DTO;
using BAL.DTO.View;
using BAL.Objects;
using BAL.Services;
using FarmaAPI.Helper;
using Microsoft.Azure.Amqp.Encoding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Windows.Input;
namespace FarmaAPI.Controllers
{
    [Authorize]
    public class InventoryController : BaseApiController
    {
        LoggingService logger = new LoggingService();


        public IHttpActionResult Save([FromBody] GRN objGRN)
        {
            var res = new ApiMessage();
            try
            {

                //  string dto = System.Web.HttpContext.Current.Request["dto"];
                // GRN objGRN = new GRN();
                //  JObject jsonObject = JObject.Parse(dto);
                var _user = new LoggedInUser();
                objGRN.FinYearId = _user.FinYearId;
                objGRN.CompanyId = _user.DefaultCompanyId;
                if (!string.IsNullOrEmpty(objGRN.ShipFrom))
                    objGRN.ShipFrom = HttpUtility.HtmlDecode(objGRN.ShipFrom);
                //  objGRN.GRN = Convert.ToString(jsonObject["GRN"]);
                //if (jsonObject["GRNId"] != null)
                //{
                //    objGRN.GRNId = Convert.ToInt32(jsonObject["GRNId"]);
                //}
                //if (jsonObject["WorkOrderId"] != null)
                //{
                //    objGRN.WorkOrderId = Convert.ToInt32(jsonObject["WorkOrderId"]);
                //}
                //if (jsonObject["LedgerSiteId"] != null)
                //{
                //    objGRN.LedgerSiteId = Convert.ToInt32(jsonObject["LedgerSiteId"]);
                //}
                //objGRN.SiteName = Convert.ToString(jsonObject["SiteName"]);
                //objGRN.JobNumber = Convert.ToString(jsonObject["JobNumber"]);
                //objGRN.Receiver = Convert.ToString(jsonObject["Receiver"]);
                //objGRN.Receiver = Convert.ToString(jsonObject["Receiver"]);
                //objGRN.Sender = Convert.ToString(jsonObject["Sender"]);
                //objGRN.LedgerId = Convert.ToInt16(jsonObject["LedgerId"]);
                //objGRN.ChallanType = Convert.ToInt16(jsonObject["ChallanType"]);
                //objGRN.JobCardId = Convert.ToInt32(jsonObject["JobCardId"]);
                //objGRN.Freight = Convert.ToDouble(jsonObject["Freight"]);
                //objGRN.VehicleNo = Convert.ToString(jsonObject["VehicleNo"]);
                //objGRN.Driver = Convert.ToString(jsonObject["Driver"]);
                //if (jsonObject["Tnc"] != null)
                //{

                //    objGRN.Tnc = HttpUtility.HtmlDecode(Convert.ToString(jsonObject.GetValue("Tnc")));
                //}
                //if (jsonObject["Remarks"] != null)
                //{
                //    objGRN.Remarks = HttpUtility.HtmlDecode(Convert.ToString(jsonObject.GetValue("Remarks")));
                //}



                //objGRN.ReceivingDate = Utils.FormatDate(Convert.ToString(jsonObject["ReceivingDate"]));
                //objGRN.RentStopDate = Utils.FormatDate(Convert.ToString(jsonObject["RentStopDate"]));

                //  objGRN.Remarks = Convert.ToString(jsonObject["Remarks"]);


                //var items = jsonObject["Items"];
                // List<WorkOrderItemDTO> wItems = new List<WorkOrderItemDTO>();
                if (objGRN.Items != null)
                {
                    foreach (var i in objGRN.Items)
                    {
                        //var itemObj = i["Item"];
                        //if (itemObj == null) continue;
                        // double qty = Convert.ToDouble(i["Quantity"]); // do not receive 0 quantity
                        //var grnItemId = Convert.ToInt32(i["GRNItemId"]);
                        //if (qty <= 0 && grnItemId == 0) continue;

                        WorkOrderItemDTO item = new WorkOrderItemDTO
                        {
                            //ProductId = Convert.ToInt32(i["ProductId"]),
                            //GroupItemId = Convert.ToInt32(i["GroupItemId"]),
                            //Deleted = Convert.ToByte(i["Deleted"]),
                            //GRNItemId = grnItemId,

                            //Quantity = Convert.ToDouble(i["Quantity"]),
                            //Remarks = Convert.ToString(i["Remarks"]),
                            //ExcessQty = Convert.ToDouble(i["ExcessQty"]),
                            //ShortQty = Convert.ToDouble(i["ShortQty"]),
                            //Breakage = Convert.ToDouble(i["Breakage"]),
                            //BreakageRate = Convert.ToDouble(i["BreakageRate"]),
                            //ReceivingQty = Convert.ToDouble(i["ReceivingQty"]),
                            //ConsiderFullReceive = Convert.ToBoolean(i["ConsiderFullReceive"]),

                            //ChargeReturnedDate = Convert.ToBoolean(i["ChargeReturnedDate"])

                        };
                        //if (item.ReceivingQty == 0)
                        //{
                        //    item.ReceivingQty = item.Quantity;
                        //}
                        //if (item.ReceivingQty > item.Quantity)
                        //{
                        //    item.ShortQty = item.ReceivingQty - item.Quantity;
                        //}
                        //if (item.ReceivingQty < item.Quantity)
                        //{
                        //    item.ExcessQty = item.Quantity - item.ReceivingQty;
                        //}
                        //if (i["ProductSizeId"] != null)
                        //{
                        //    item.ProductSizeId = Convert.ToInt32(i["ProductSizeId"]);
                        //}
                        //if (i["GRNItemId"] != null)
                        //{
                        //    item.GRNItemId = Convert.ToInt32(i["GRNItemId"]);
                        //}
                        //if (i["Rate"] != null)
                        //{
                        //    item.Rate = Convert.ToDouble(i["Rate"]);
                        //}
                        //wItems.Add(item);
                    }
                }
                // objGRN.Items = wItems;


                var ledger = new Ledger(objGRN.LedgerId);
                /*  var lastBill = ledger.GetLastBill(objGRN.LedgerId, objGRN.LedgerSiteId, 0);

                  if (lastBill != null)
                  {

                      if (objGRN.ReceivingDate <= Convert.ToDateTime(lastBill.To))
                      {
                          throw new Exception("Receiving date can not be on or before last bill date");
                      }
                  }
                */

                // var otherCharges = jsonObject["OtherCharges"];
                List<GRNChageDTO> grnCharges = new List<GRNChageDTO>();
                if (objGRN.OtherCharges != null)
                {
                    //foreach (var otherCharge in otherCharges)
                    //{
                    //    var amount = Convert.ToDouble(otherCharge["Amount"]);
                    //    var charge = new GRNChageDTO();
                    //    charge.Amount = amount;
                    //    charge.ChargeId = Convert.ToInt32(otherCharge["ChargeId"]);
                    //    charge.GRNChargeId = Convert.ToInt32(otherCharge["GRNChargeId"]);
                    //    grnCharges.Add(charge);
                    //}
                    objGRN.TotalOtherCharges = grnCharges.Sum(o => o.Amount);
                }
                //objGRN.OtherCharges = grnCharges;
                bool success = objGRN.Add();
                if (success)
                {
                    //CommHelpler com = new CommHelpler();
                    //EmailParameters p = new EmailParameters() { ChallanNumber = objGRN.GRN };
                    //Ledger ledger = new Ledger(objGRN.LedgerId);
                    //LedgerDTO lDto = ledger.GetDetails();
                    //com.sendSms(lDto.ContactPersonMobile, p, MessageEvent.RECEIVE_ITEM);
                }
                res.Code = ApiMessageCodes.SUCCESS;
                res.Message = "Items received successfully.";
                res.Data = objGRN;
                //return Request.CreateResponse(HttpStatusCode.OK, objGRN);
            }
            catch (Exception ex)
            {
                res.Code = ApiMessageCodes.ERROR;
                res.Message = ex.Message;
                res.Description = ex.StackTrace;
            }
            return Ok(res);
        }


        [System.Web.Mvc.ValidateInput(false)]
        [HttpPost]
        public IHttpActionResult AddGRN()
        {
            var res = new ApiMessage();
            try
            {

                string dto = System.Web.HttpContext.Current.Request["dto"];
                GRN objGRN = new GRN();
                JObject jsonObject = JObject.Parse(dto);
                var _user = new LoggedInUser();
                objGRN.FinYearId = _user.FinYearId;
                objGRN.CompanyId = _user.DefaultCompanyId;
                objGRN.GRN = Convert.ToString(jsonObject["GRN"]);
                if (jsonObject["GRNId"] != null)
                {
                    objGRN.GRNId = Convert.ToInt32(jsonObject["GRNId"]);
                }
                if (jsonObject["WorkOrderId"] != null)
                {
                    objGRN.WorkOrderId = Convert.ToInt32(jsonObject["WorkOrderId"]);
                }
                if (jsonObject["LedgerSiteId"] != null)
                {
                    objGRN.LedgerSiteId = Convert.ToInt32(jsonObject["LedgerSiteId"]);
                }
                objGRN.SiteName = Convert.ToString(jsonObject["SiteName"]);
                objGRN.JobNumber = Convert.ToString(jsonObject["JobNumber"]);
                objGRN.Receiver = Convert.ToString(jsonObject["Receiver"]);
                objGRN.Receiver = Convert.ToString(jsonObject["Receiver"]);
                objGRN.Sender = Convert.ToString(jsonObject["Sender"]);
                objGRN.LedgerId = Convert.ToInt16(jsonObject["LedgerId"]);
                objGRN.ChallanType = Convert.ToInt16(jsonObject["ChallanType"]);
                objGRN.JobCardId = Convert.ToInt32(jsonObject["JobCardId"]);
                objGRN.Freight = Convert.ToDouble(jsonObject["Freight"]);
                objGRN.VehicleNo = Convert.ToString(jsonObject["VehicleNo"]);
                objGRN.Driver = Convert.ToString(jsonObject["Driver"]);
                if (jsonObject["Tnc"] != null)
                {

                    objGRN.Tnc = HttpUtility.HtmlDecode(Convert.ToString(jsonObject.GetValue("Tnc")));
                }
                if (jsonObject["Remarks"] != null)
                {
                    objGRN.Remarks = HttpUtility.HtmlDecode(Convert.ToString(jsonObject.GetValue("Remarks")));
                }



                objGRN.ReceivingDate = Utils.FormatDate(Convert.ToString(jsonObject["ReceivingDate"]));
                objGRN.RentStopDate = Utils.FormatDate(Convert.ToString(jsonObject["RentStopDate"]));

                objGRN.Remarks = Convert.ToString(jsonObject["Remarks"]);


                var items = jsonObject["Items"];
                List<WorkOrderItemDTO> wItems = new List<WorkOrderItemDTO>();
                foreach (var i in items)
                {
                    var itemObj = i["Item"];
                    if (itemObj == null) continue;
                    double qty = Convert.ToDouble(i["Quantity"]); // do not receive 0 quantity
                    var grnItemId = Convert.ToInt32(i["GRNItemId"]);
                    //if (qty <= 0 && grnItemId == 0) continue;

                    WorkOrderItemDTO item = new WorkOrderItemDTO
                    {
                        ProductId = Convert.ToInt32(i["ProductId"]),
                        GroupItemId = Convert.ToInt32(i["GroupItemId"]),
                        Deleted = Convert.ToByte(i["Deleted"]),
                        GRNItemId = grnItemId,

                        Quantity = Convert.ToDouble(i["Quantity"]),
                        Remarks = Convert.ToString(i["Remarks"]),
                        ExcessQty = Convert.ToDouble(i["ExcessQty"]),
                        ShortQty = Convert.ToDouble(i["ShortQty"]),
                        Scrap = i["Scrap"] != null ? Convert.ToDouble(i["Scrap"]) : 0,
                        Breakage = Convert.ToDouble(i["Breakage"]),
                        BreakageRate = Convert.ToDouble(i["BreakageRate"]),
                        DamageComponent = i["DamageComponent"] != null ? Convert.ToString(i["DamageComponent"]) : null,
                        ReceivingQty = Convert.ToDouble(i["ReceivingQty"]),
                        ConsiderFullReceive = Convert.ToBoolean(i["ConsiderFullReceive"]),

                        ChargeReturnedDate = Convert.ToBoolean(i["ChargeReturnedDate"])

                    };
                    if (item.ReceivingQty == 0)
                    {
                        item.ReceivingQty = item.Quantity;
                    }
                    if (item.ReceivingQty > item.Quantity)
                    {
                        item.ShortQty = item.ReceivingQty - item.Quantity;
                    }
                    if (item.ReceivingQty < item.Quantity)
                    {
                        item.ExcessQty = item.Quantity - item.ReceivingQty;
                    }
                    if (i["ProductSizeId"] != null)
                    {
                        item.ProductSizeId = Convert.ToInt32(i["ProductSizeId"]);
                    }
                    if (i["GRNItemId"] != null)
                    {
                        item.GRNItemId = Convert.ToInt32(i["GRNItemId"]);
                    }
                    if (i["Rate"] != null)
                    {
                        item.Rate = Convert.ToDouble(i["Rate"]);
                    }
                    wItems.Add(item);
                }
                objGRN.Items = wItems;


                var ledger = new Ledger(objGRN.LedgerId);
                /* var lastBill = ledger.GetLastBill(objGRN.LedgerId, objGRN.LedgerSiteId, 0);

                 if (lastBill != null)
                 {

                     if (objGRN.ReceivingDate <= Convert.ToDateTime(lastBill.To))
                     {
                         throw new Exception("Receiving date can not be on or before last bill date");
                     }
                 }
                */
                var otherCharges = jsonObject["OtherCharges"];
                List<GRNChageDTO> grnCharges = new List<GRNChageDTO>();
                if (otherCharges != null)
                {
                    foreach (var otherCharge in otherCharges)
                    {
                        var amount = Convert.ToDouble(otherCharge["Amount"]);
                        var charge = new GRNChageDTO();
                        charge.Amount = amount;
                        charge.ChargeId = Convert.ToInt32(otherCharge["ChargeId"]);
                        charge.GRNChargeId = Convert.ToInt32(otherCharge["GRNChargeId"]);
                        grnCharges.Add(charge);
                    }
                    objGRN.TotalOtherCharges = grnCharges.Sum(o => o.Amount);
                }
                objGRN.OtherCharges = grnCharges;
                bool success = objGRN.Add();
                if (success)
                {
                    //CommHelpler com = new CommHelpler();
                    //EmailParameters p = new EmailParameters() { ChallanNumber = objGRN.GRN };
                    //Ledger ledger = new Ledger(objGRN.LedgerId);
                    //LedgerDTO lDto = ledger.GetDetails();
                    //com.sendSms(lDto.ContactPersonMobile, p, MessageEvent.RECEIVE_ITEM);
                }
                res.Code = ApiMessageCodes.SUCCESS;
                res.Message = "Items received successfully.";
                res.Data = objGRN;
                //return Request.CreateResponse(HttpStatusCode.OK, objGRN);
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
        public async Task<ApiMessage> GrnById([FromBody] GRNDTO dto)
        {
            GRN objGrn = new GRN();
            var user = new LoggedInUser();
            var res = new ApiMessage();
            try
            {


                var grnData = await objGrn.GrnById(dto.GRNId, user.DefaultCompanyId);
                grnData.GrnItems = objGrn.GetItemsByGrnId(dto.GRNId, user.DefaultCompanyId);
                res.Data = grnData;
                res.Code = ApiMessageCodes.SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.Code = ApiMessageCodes.ERROR;
                res.Message = ex.Message;
                logger.LogError(ex, ex.Message);
            }
            return res;
        }

        [HttpPost]
        public HttpResponseMessage GetItemsByGrnId([FromBody] GRNDTO dto)
        {
            GRN objGrn = new GRN();
            var user = new LoggedInUser();
            return Request.CreateResponse(HttpStatusCode.OK, objGrn.GetItemsByGrnId(dto.GRNId, user.DefaultCompanyId));
        }

        [HttpPost]
        public HttpResponseMessage ItemStock([FromBody] GRNDTO filter)
        {
            Inventory objInventory = new Inventory();
            ApiMessage message = new ApiMessage();
            try
            {
                message.Data = objInventory.ItemStock(new LoggedInUser().FinYearId, new LoggedInUser().DefaultCompanyId);
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
        public HttpResponseMessage StockInhand([FromBody] FilterCriteria dto)
        {
            try
            {
                int companyId = new LoggedInUser().DefaultCompanyId;
                Inventory objLedger = new Inventory();
                String onDate = "";
                if (!String.IsNullOrEmpty(dto.OnDate))
                    onDate = Utils.FormatDate(dto.OnDate).ToShortDateString();
                LoggedInUser user = new LoggedInUser();
                var lst = objLedger.StockInhand(companyId, onDate, dto.WarehouseId);
                //  List<LedgerbalanceDTO> lst = new Utils<LedgerbalanceDTO>().ConstructList(ds);
                return Request.CreateResponse(HttpStatusCode.OK, lst);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }

        [HttpPost]
        public HttpResponseMessage StockSummary([FromBody] FilterCriteria dto)
        {
            try
            {
                int companyId = new LoggedInUser().DefaultCompanyId;
                Inventory objLedger = new Inventory();
                String onDate = "";
                if (!String.IsNullOrEmpty(dto.OnDate))
                    onDate = Utils.FormatDate(dto.OnDate).ToShortDateString();
                LoggedInUser user = new LoggedInUser();
                var lst = objLedger.StockSummary(companyId, onDate, dto.WarehouseId);
                //  List<LedgerbalanceDTO> lst = new Utils<LedgerbalanceDTO>().ConstructList(ds);
                return Request.CreateResponse(HttpStatusCode.OK, lst);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }

        [HttpPost]
        public async Task<ApiMessage> GetOtherCharges([FromBody] GRNDTO obj)
        {
            var res = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                GRN objGRN = new GRN();
                objGRN.GRNId = obj.GRNId;
                objGRN.CompanyId = user.DefaultCompanyId;

                res.Data = await objGRN.GetOtherChages(obj.GRNId, user.DefaultCompanyId);
                res.Code = ApiMessageCodes.SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.Code = ApiMessageCodes.ERROR;
                logger.LogError(ex, ex.Message);
                return res;
            }

        }

        [HttpPost]
        public IHttpActionResult PostStockTxn([FromBody] StockTransactionHeaderViewDTO dto)
        {
            var res = new ApiMessage();
            try
            {
                int companyId = new LoggedInUser().DefaultCompanyId;
                var stdto = new StockTransactionHeaderDTO();
                LoggedInUser user = new LoggedInUser();
                stdto.InjectFrom(dto);
                stdto.PostingDate = dto.PostingDate;
                stdto.FinYear = user.FinYearId;
                stdto.CompanyId = user.DefaultCompanyId;
                stdto.CreatedOn = DateTime.Now;
                stdto.CreatedBy = user.UserId;
                stdto.StockTransactionHeaderId = dto.StockTransactionHeaderId;
                stdto.RbnClientId = user.RbnClientId;
                stdto.GuId = Guid.NewGuid().ToString();
                stdto.ModifiedBy = user.UserId;
                stdto.ModifiedOn = DateTime.Now;
                stdto.Items = (from d in dto.Items
                               select new StockTransactionDTO
                               {
                                   CompanyId = companyId,
                                   PostingDate = stdto.PostingDate,
                                   PostingType = stdto.PostingType,
                                   PostedBy = user.UserId,
                                   CreatedOn = DateTime.Now,
                                   Deleted = d.Deleted,
                                   Quantity = d.Quantity,
                                   Remarks = stdto.Remarks,
                                   VoucherId = stdto.VoucherId,
                                   ProductId = d.ProductId
                               }).ToList();
                ;
                Inventory objInventory = new Inventory();

                var lst = objInventory.PostStock(stdto);
                res.Data = lst;
                res.Code = ApiMessageCodes.SUCCESS;
                //  List<LedgerbalanceDTO> lst = new Utils<LedgerbalanceDTO>().ConstructList(ds);
                return Ok(res);
            }
            catch (Exception ex)
            {
                res.Code = ApiMessageCodes.ERROR;
                res.Description = "Could not save";
                return Ok(res);
                // return Request.CreateResponse(HttpStatusCode., ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }
        [HttpPost]
        public IHttpActionResult StockAdjustmentList([FromBody] StockAdjustmentListFilterDTO filter)
        {
            var res = new ApiMessage();
            try
            {
                int companyId = new LoggedInUser().DefaultCompanyId;
                var stdto = new StockTransactionHeaderDTO();
                LoggedInUser user = new LoggedInUser();

                Inventory objInventory = new Inventory();

                var lst = objInventory.StockAdjustmentList(user.DefaultCompanyId, Utils.FormatDate(filter.From), Utils.FormatDate(filter.To));
                //  List<LedgerbalanceDTO> lst = new Utils<LedgerbalanceDTO>().ConstructList(ds);
                res.Data = lst;
                res.Code = ApiMessageCodes.SUCCESS;
                //  List<LedgerbalanceDTO> lst = new Utils<LedgerbalanceDTO>().ConstructList(ds);
                return Ok(res);
            }
            catch (Exception ex)
            {
                res.Code = ApiMessageCodes.ERROR;
                res.Message = "Could not retrieve result";
                return Ok(res);
            }
        }
        [HttpPost]
        public IHttpActionResult StockAdjustmentDetails([FromBody] StockAdjustmentListFilterDTO filter)
        {
            var res = new ApiMessage();
            try
            {
                int companyId = new LoggedInUser().DefaultCompanyId;
                var stdto = new StockTransactionHeaderDTO();
                LoggedInUser user = new LoggedInUser();

                Inventory objInventory = new Inventory();

                var lst = objInventory.StockAdjustmentDetails(user.DefaultCompanyId, filter.StockTransactionHeaderId);
                //  List<LedgerbalanceDTO> lst = new Utils<LedgerbalanceDTO>().ConstructList(ds);
                res.Data = lst;
                res.Code = ApiMessageCodes.SUCCESS;
                //  List<LedgerbalanceDTO> lst = new Utils<LedgerbalanceDTO>().ConstructList(ds);
                return Ok(res);
            }
            catch (Exception ex)
            {
                res.Code = ApiMessageCodes.ERROR;
                res.Message = "Could not retrieve result";
                return Ok(res);
            }
        }

        [HttpPost]
        public IHttpActionResult DeleteStockAdjustment([FromBody] StockAdjustmentListFilterDTO filter)
        {
            var res = new ApiMessage();
            try
            {
                int companyId = new LoggedInUser().DefaultCompanyId;
                var stdto = new StockTransactionHeaderDTO();
                LoggedInUser user = new LoggedInUser();

                Inventory objInventory = new Inventory();

                var lst = objInventory.StockDelete(user.DefaultCompanyId, filter.StockTransactionHeaderId);
                //  List<LedgerbalanceDTO> lst = new Utils<LedgerbalanceDTO>().ConstructList(ds);
                res.Data = lst;
                res.Code = ApiMessageCodes.SUCCESS;
                //  List<LedgerbalanceDTO> lst = new Utils<LedgerbalanceDTO>().ConstructList(ds);
                return Ok(res);
            }
            catch (Exception ex)
            {
                res.Code = ApiMessageCodes.ERROR;
                res.Message = "Could not delete";
                return Ok(res);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteChallan([FromBody] GRNDTO filter)
        {
            var res = new ApiMessage();
            try
            {
                int companyId = new LoggedInUser().DefaultCompanyId;
                var stdto = new StockTransactionHeaderDTO();
                var user = new LoggedInUserInfo();

                var objInventory = new GRN();

                var ledger = new Ledger(filter.LedgerId);

                var lastBill = ledger.GetLastBill(filter.LedgerId, filter.LedgerSiteId, 0);

                if (lastBill != null)
                {

                    if (filter.ReceivingDate <= Convert.ToDateTime(lastBill.To))
                    {
                        throw new Exception("Bills has been generated, Can not delete");
                    }
                }
                var lst = await objInventory.DeleteChallan(companyId, filter.GRNId, user);

                res.Data = lst;
                res.Code = ApiMessageCodes.SUCCESS;

                return Ok(res);
            }
            catch (Exception ex)
            {
                res.Code = ApiMessageCodes.ERROR;
                res.Message = "Could not delete";
                return Ok(res);
            }
        }
        [HttpPost]
        public async Task<ApiMessage> InwardConfirm([FromBody] GRNDTO dto)
        {
            var response = new ApiMessage();
            try
            {
                if (dto == null || dto.GRNId <= 0)
                {
                    throw new Exception("Invalid GRN.");
                }

                var user = new LoggedInUser();
                dto.CompanyId = user.DefaultCompanyId;
                dto.ModifiedBy = user.UserId;
                dto.ModifiedOn = DateTime.Now;
                dto.StatusId = 1;

                var grn = new GRN();
                var data = await grn.InwardConfirm(dto);
                if (data > 0)
                {
                    if (Config.IsDispatchInwardNotificationEnabled(dto.CompanyId))
                    {
                        var grnData = await grn.GrnById(dto.GRNId, dto.CompanyId);
                        var recipient = ResolvePartyRecipientByLedgerId(grnData != null ? grnData.LedgerId : 0);
                        if (!String.IsNullOrEmpty(recipient))
                        {
                            var token = Request?.Headers?.Contains("Authorization") == true
                                ? Request.Headers.GetValues("Authorization").FirstOrDefault()
                                : null;
                            string apiKey = Company.GetRentacApiKeyByCompanyId(dto.CompanyId);

                            try
                            {
                                if (!String.IsNullOrEmpty(apiKey))
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
                                        MetaData = "1108," + dto.GRNId
                                    }, "contractreminders");
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex, "Failed to publish inward confirm queue message");
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
                logger.LogError(ex, "Could not update inward confirm status");
            }

            return response;
        }

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
    }
}
