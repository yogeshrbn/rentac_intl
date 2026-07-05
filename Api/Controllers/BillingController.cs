using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BAL.DTO;
using BAL.Objects;
using Microsoft.Reporting.WebForms;
using System.Data;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using FarmaAPI.Helper;
using BAL.Enums;
using BAL.Common;
using Omu.ValueInjecter;
using NLog;
using System.Threading.Tasks;
using QRCoder;
using System.Text;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System.Net.NetworkInformation;
using System.Web.Razor.Generator;
using System.Web;
using System.Web.Services.Description;
using BAL.Services;
using System.Diagnostics;
namespace FarmaAPI.Controllers
{
    [Authorize]
    public class BillingController : BaseApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly string docsPath = Path.Combine(HttpRuntime.AppDomainAppPath, "docs");
        [HttpPost]
        public async Task<HttpResponseMessage> GenerateBill([FromBody] BillingDTO dto)
        {
            try
            {
                Billing objBilling = new Billing();
                //dto.From = Utils.FormatDate(dto.From).ToShortDateString();
                //dto.To = Utils.FormatDate(dto.To).ToShortDateString();
                //check if bill is already generated for the given period. Commented on 27th Nov 18
                //if (objBilling.CheckForBilling(dto.LedgerId, dto.From, dto.To, dto.WorkOrderNumber).Count > 0)
                //{

                //    return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.BILLING_GENERATED));
                //}
                //else
                //{
                var userInfo = new LoggedInUser();
                // userInfo.InjectFrom(new LoggedInUser());
                var billingData = await objBilling.GenerateBill(dto, userInfo);

                // var cahllans = billingData.Select(o => new BillChallanDto {
                if (billingData.BillingItems != null)
                {
                    billingData.StockBalanceAfterBill = (from d in billingData.BillingItems
                                                         group d by new { ProductId = d.ProductId } into g

                                                         select new InvoiceItemDTO
                                                         {
                                                             ProductId = g.Key.ProductId,
                                                             Item = g.Last().Item,
                                                             ClosingBalance = g.Last().ClosingBalance
                                                         }
                                                         ).Where(o => o.ClosingBalance > 0).ToList();
                }

                // }).Distinct();
                //   var _billData = new { items = billingData, challans = cahllans };
                return Request.CreateResponse(HttpStatusCode.OK, billingData);
                //}
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
                // return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ApiMessage> ById([FromBody] BillingDTO dto)
        {
            ApiMessage msg = new ApiMessage();
            try
            {
                if (dto.InvoiceId <= 0)
                {
                    throw new Exception("InvoiceId not found");
                }

                Billing objBill = new Billing();
                var usr = new LoggedInUser();
                dto.CompanyId = usr.DefaultCompanyId;
                msg.Data = await objBill.GetByIdForEdit(dto);
                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        [HttpPost]
        public async Task<ApiMessage> SettleBill([FromBody] InvoiceDTO dto)
        {
            ApiMessage msg = new ApiMessage();
            try
            {
                if (dto.InvoiceId <= 0)
                {
                    throw new Exception("InvoiceId not found");
                }
                if (String.IsNullOrEmpty(dto.SettlementRemarks))
                {
                    throw new Exception("Please provide remarks");
                }
                Billing objBill = new Billing();
                var usr = new LoggedInUser();
                dto.CompanyId = usr.DefaultCompanyId;
                dto.SettlementDate = DateTime.Now;
                dto.SettledBy = usr.UserId;
                msg.Data = await objBill.SettleBill(dto);
                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = ex.Message;
                return msg;

            }


        }
        // [HttpPost]
        //public HttpResponseMessage GetBreakageForBill([FromBody] FilterCriteria dto)
        //{
        //    Billing objBill = new Billing();

        //    return Request.CreateResponse(HttpStatusCode.OK, objBill.GetBreakageForBill(dto.LedgerId, dto.InvoiceId, dto.LedgerSiteId, Convert.ToDateTime(dto.From), Convert.ToDateTime(dto.To), new LoggedInUser().FinYearId));

        //}

        [HttpPost]
        public async Task<IHttpActionResult> GetLostItemsToBill([FromBody] BillingDTO bdto)
        {
            Billing objBill = new Billing();
            var res = new ApiMessage();
            try
            {
                var userInfo = new LoggedInUserInfo();
                bdto.CompanyId = userInfo.DefaultCompanyId;
                res.Data = await objBill.GetLostItemsToBill(bdto, userInfo);
                res.Code = ApiMessageCodes.SUCCESS;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                res.Code = ApiMessageCodes.ERROR;
                res.Message = ex.Message;
                return Ok(res);
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetBreakageItemsToBill([FromBody] BillingDTO bdto)
        {
            Billing objBill = new Billing();
            var res = new ApiMessage();
            try
            {

                var userInfo = new LoggedInUserInfo();
                bdto.CompanyId = userInfo.DefaultCompanyId;
                if (bdto.LedgerId > 0 && bdto.LedgerSiteId > 0)
                    res.Data = await objBill.GetBreakageForBill(bdto, userInfo);

                res.Code = ApiMessageCodes.SUCCESS;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                res.Code = ApiMessageCodes.ERROR;
                res.Message = ex.Message;
                return Ok(res);
            }
        }
        //[HttpPost]
        //public HttpResponseMessage SaveBill([FromBody] BillingDTO dto)
        //{
        //    try
        //    {
        //        Billing objBilling = new Billing();
        //        dto.From = Utils.FormatDate(dto.From).ToShortDateString();
        //        dto.To = Utils.FormatDate(dto.To).ToShortDateString();
        //        return Request.CreateResponse(HttpStatusCode.OK, objBilling.SaveBill(dto.LedgerId, dto.From, dto.To, dto.WorkOrderNumber));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
        //    }
        //}
        [HttpPost]
        [System.Web.Mvc.ValidateInput(false)]
        public async Task<IHttpActionResult> SaveBill()
        {
            var apiMessage = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                var config = new Config();
                var billConfig = config.GetBillingConfig(user.DefaultCompanyId);
                ConfigDTO brekageConfig = null;
                if (billConfig != null)
                {
                    brekageConfig = billConfig.Where(o => o.Key.ToLower() == "breakagebill").FirstOrDefault();
                }

                //  DataCopier<WorkOrderDTO, WorkOrder> dcopier = new DataCopier<WorkOrderDTO, WorkOrder>();
                // dcopier.CopyData(dto, leder);
                string dto = System.Web.HttpContext.Current.Request["dto"];
                System.Web.HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                JObject jsonObject = new JObject();

                BillingDTO billingDTO = new BillingDTO();
                Billing objBilling = new Billing();
                jsonObject = JObject.Parse(dto);
                NextId n = new NextId();

                billingDTO.InvoiceId = Convert.ToInt16(jsonObject.GetValue("InvoiceId"));

                billingDTO.InvoiceType = Convert.ToInt16(jsonObject.GetValue("InvoiceType")); // bill
                if (billingDTO.InvoiceType == 0)
                {
                    throw new Exception("Invalid invoice type");
                }
                billingDTO.WorkOrderNumber = Convert.ToString(jsonObject.GetValue("WorkOrderNumber"));

                //   billingDTO.From = Utils.FormatDate(Convert.ToString(jsonObject.GetValue("From"))).ToShortDateString();
                //  billingDTO.To = Utils.FormatDate(Convert.ToString(jsonObject.GetValue("To"))).ToShortDateString();
                if (jsonObject.GetValue("To") != null
                    && !String.IsNullOrEmpty(Convert.ToString(jsonObject.GetValue("To")))
                    )
                {
                    billingDTO.To = Convert.ToDateTime(jsonObject.GetValue("To"));
                }
                //if (jsonObject.GetValue("From") != null
                //    && !String.IsNullOrEmpty(Convert.ToString(jsonObject.GetValue("From"))))
                //{
                //    billingDTO.To = Convert.ToDateTime(jsonObject.GetValue("From"));
                //}

                billingDTO.LedgerId = Convert.ToInt16(jsonObject.GetValue("LedgerId"));
                billingDTO.SubTotal = Convert.ToDouble(jsonObject.GetValue("SubTotal"));
                if (jsonObject.GetValue("Taxable") != null)
                    billingDTO.Taxable = Convert.ToDouble(jsonObject.GetValue("Taxable"));
                billingDTO.TaxAmount = Convert.ToDouble(jsonObject.GetValue("TaxAmount"));
                billingDTO.FreightTax = Convert.ToDouble(jsonObject.GetValue("FreightTax"));
                billingDTO.Discount = Convert.ToDouble(jsonObject.GetValue("Discount"));
                billingDTO.ChargeReturnDay = Convert.ToByte(jsonObject.GetValue("ChargeReturnDay"));
                billingDTO.RateCalcType = Convert.ToByte(jsonObject.GetValue("RateCalcType"));
                double BreakageTax = Convert.ToDouble(jsonObject.GetValue("BreakageTax"));

                billingDTO.Discount = Convert.ToDouble(jsonObject.GetValue("Discount"));
                billingDTO.DiscountPercent = Convert.ToDouble(jsonObject.GetValue("DiscountPercent"));

                billingDTO.BreakageDiscountPercent = Convert.ToDouble(jsonObject.GetValue("BrekageDiscountPercent"));
                billingDTO.LossDiscountPercent = Convert.ToDouble(jsonObject.GetValue("LossDiscountPercent"));
                billingDTO.BreakageDiscount = Convert.ToDouble(jsonObject.GetValue("BreakageDiscount"));
                billingDTO.LossDiscount = Convert.ToDouble(jsonObject.GetValue("LossDiscount"));


                if (jsonObject.GetValue("PODate") != null
                 && !String.IsNullOrEmpty(Convert.ToString(jsonObject.GetValue("PODate"))))
                {
                    billingDTO.PODate = Convert.ToDateTime(jsonObject.GetValue("PODate"));
                }

                if (jsonObject.GetValue("PONumber") != null
                 && !String.IsNullOrEmpty(Convert.ToString(jsonObject.GetValue("PONumber"))))
                {
                    billingDTO.PONumber = Convert.ToString(jsonObject.GetValue("PONumber"));
                }

                if (!String.IsNullOrEmpty(billingDTO.PONumber) && billingDTO.PODate == DateTime.MinValue)
                {
                    throw new Exception("PO date is required");
                }
                if (String.IsNullOrEmpty(billingDTO.PONumber) && billingDTO.PODate != DateTime.MinValue)
                {
                    throw new Exception("PO number is required");
                }

                if (jsonObject.GetValue("Tnc") != null)
                {
                    billingDTO.Tnc = HttpUtility.HtmlDecode(Convert.ToString(jsonObject.GetValue("Tnc")));
                }
                //check if bill is already generated for the given period. Commented on 27th Nov 18
                //if (objBilling.CheckForBilling(billingDTO.LedgerId, billingDTO.From, billingDTO.To, billingDTO.WorkOrderNumber).Count > 0)
                //{
                //    return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.BILLING_GENERATED));

                //}
                if (jsonObject.GetValue("OutStanding") != null)
                {
                    billingDTO.OutStanding = Convert.ToDouble(jsonObject.GetValue("OutStanding"));
                    billingDTO.OutStandingType = (Int16)(Convert.ToString(jsonObject.GetValue("OutStandingType")) == "Dr" ? 1 : 2);
                }
                if (jsonObject.GetValue("LedgerSiteId") != null)
                {
                    billingDTO.LedgerSiteId = Convert.ToInt32(jsonObject.GetValue("LedgerSiteId"));

                }
                if (Convert.ToDouble(jsonObject.GetValue("RoundOff")) != null)
                {
                    billingDTO.RoundOff = Convert.ToBoolean(jsonObject.GetValue("RoundOff"));
                }
                var isCashBillToken = jsonObject["IsCashBill"] ?? jsonObject["isCashBill"];
                if (isCashBillToken != null && isCashBillToken.Type != JTokenType.Null)
                {
                    billingDTO.IsCashBill = isCashBillToken.Value<bool>();
                }
                billingDTO.Freight = Convert.ToDouble(jsonObject.GetValue("Freight"));
                billingDTO.CompanyId = user.DefaultCompanyId;
                //   billingDTO.InvoiceNumber = nextId;
                billingDTO.CreatedBy = user.UserId;

                billingDTO.FinYearId = user.FinYearId;
                billingDTO.InvoiceDate = DateTime.Today;

                //invoice date and number is editable only on rent and breakage bill, remove
                //this condition once implemented on all bill types
                if (billingDTO.InvoiceType == 1 || billingDTO.InvoiceType == 2)
                {
                    billingDTO.InvoiceDate = Convert.ToDateTime(jsonObject.GetValue("InvoiceDate"));
                    billingDTO.InvoiceNumber = Convert.ToString(jsonObject.GetValue("InvoiceNumber"));
                }
                var items = jsonObject["Items"];
                //var taxInfo = jsonObject["AppliedTaxes"]; modified on 10 march 19. Now we will apply tax on whole bill not on individual items
                var taxInfo = jsonObject["Taxes"];
                billingDTO.ApplicableTaxes = new List<TaxDTO>();
                billingDTO.BillableItems = AddItems(items);
                // billingDTO.From = billingDTO.BillableItems.Min(o => Convert.ToDateTime(o.From)).ToShortDateString();
                billingDTO.From = billingDTO.BillableItems.Min(o => Convert.ToDateTime(o.From));

                Billing objBill = new Billing();
                List<BillingItemDTO> breakageItems = new List<BillingItemDTO>();// objBill.GetBreakageForBill(billingDTO.LedgerId, billingDTO.From, billingDTO.To, new LoggedInUser().FinYearId);
                var breakages = jsonObject["Breakage"];
                billingDTO.BreakageItems = breakageItems;
                double breakageAmount = 0;
                if (breakages != null)
                {
                    foreach (var br in breakages)
                    {
                        BillingItemDTO b = new BillingItemDTO();
                        b.ProductId = Convert.ToInt32(br["ProductId"]);
                        b.Quantity = Convert.ToDouble(br["Quantity"]);
                        b.Rate = Convert.ToDouble(br["Rate"]);
                        b.SubTotal = b.Quantity * b.Rate;
                        b.IGST = Convert.ToDouble(br["IGST"]);
                        b.CGST = Convert.ToDouble(br["CGST"]);
                        b.SGST = Convert.ToDouble(br["SGST"]);
                        b.IGSTRate = Convert.ToDouble(br["IGSTRate"]);
                        b.CGSTRate = Convert.ToDouble(br["CGSTRate"]);
                        b.SGSTRate = Convert.ToDouble(br["SGSTRate"]);
                        b.ChallanId = Convert.ToInt32(br["ChallanId"]);
                        if (br["TaxCategoryId"] != null)
                        {
                            b.TaxCategoryId = Convert.ToInt16(br["TaxCategoryId"]);
                        }
                        b.LineTaxes = ParseInvoiceLineTaxes(br["LineTaxes"]);
                        // b.BreakageAmount = b.SubTotal * b.Rate;
                        breakageItems.Add(b);
                    }


                    if (breakageItems != null && (brekageConfig == null || brekageConfig.Value.ToLower() == "false"))
                    {
                        breakageAmount = breakageItems.Sum(o => o.SubTotal);
                        billingDTO.BreakageAmount = breakageAmount;
                        billingDTO.BreakageTax = BreakageTax;
                    }
                }
                billingDTO.BreakageDamageDetails = new List<BreakageDamageDetailDTO>();
                var damageArr = jsonObject["BreakageDamageDetails"];
                if (damageArr != null && damageArr.Type == JTokenType.Array)
                {
                    foreach (var row in damageArr)
                    {
                        var d = new BreakageDamageDetailDTO();
                        if (row["GRNItemId"] != null && row["GRNItemId"].Type != JTokenType.Null)
                            d.GRNItemId = Convert.ToInt32(row["GRNItemId"]);
                        if (row["ProductId"] != null && row["ProductId"].Type != JTokenType.Null)
                            d.ProductId = Convert.ToInt32(row["ProductId"]);
                        var pt = row["ParentItem"] ?? row["parentItem"];
                        d.ParentItem = pt == null || pt.Type == JTokenType.Null ? null : Convert.ToString(pt);
                        var grnTok = row["GRN"] ?? row["grn"];
                        d.GRN = grnTok == null || grnTok.Type == JTokenType.Null ? null : Convert.ToString(grnTok);
                        var compTok = row["ComponentName"] ?? row["componentName"];
                        d.ComponentName = compTok == null || compTok.Type == JTokenType.Null ? null : Convert.ToString(compTok);
                        if (row["Quantity"] != null && row["Quantity"].Type != JTokenType.Null)
                            d.Quantity = Convert.ToDecimal(row["Quantity"] ?? row["quantity"] ?? 0);
                        if (row["Rate"] != null && row["Rate"].Type != JTokenType.Null)
                            d.Rate = Convert.ToDecimal(row["Rate"] ?? row["rate"] ?? 0);
                        d.Cost = Convert.ToDecimal(row["Cost"] ?? row["cost"] ?? 0);
                        var recv = row["ReceivingDate"] ?? row["receivingDate"];
                        if (recv != null && recv.Type != JTokenType.Null && !string.IsNullOrWhiteSpace(recv.ToString()))
                            d.ReceivingDate = Convert.ToDateTime(recv);
                        d.IGST = Convert.ToDouble(row["IGST"] ?? row["igst"] ?? 0);
                        d.CGST = Convert.ToDouble(row["CGST"] ?? row["cgst"] ?? 0);
                        d.SGST = Convert.ToDouble(row["SGST"] ?? row["sgst"] ?? 0);
                        d.IGSTRate = Convert.ToDouble(row["IGSTRate"] ?? row["igstRate"] ?? 0);
                        d.CGSTRate = Convert.ToDouble(row["CGSTRate"] ?? row["cgstRate"] ?? 0);
                        d.SGSTRate = Convert.ToDouble(row["SGSTRate"] ?? row["sgstRate"] ?? 0);
                        billingDTO.BreakageDamageDetails.Add(d);
                    }
                }
                var challans = jsonObject["Challans"];
                billingDTO.Challans = new List<BillChallanDto>();
                if (challans != null)
                {
                    foreach (var ch in challans)
                    {
                        var bchDto = new BillChallanDto();
                        bchDto.Type = Convert.ToInt16(ch["Type"]);
                        bchDto.ChallanId = Convert.ToInt32(ch["ChallanId"]);
                        bchDto.ChallanNumber = Convert.ToString(ch["ChallanNumber"]);
                        bchDto.CreationDate = DateTime.Now;
                        bchDto.GuId = Guid.NewGuid().ToString();
                        bchDto.LedgerId = billingDTO.LedgerId;
                        bchDto.CompanyId = billingDTO.CompanyId;
                        billingDTO.Challans.Add(bchDto);
                    }

                }
                var po = jsonObject["PO"];
                if (jsonObject["PrintAllPO"] != null)
                {
                    billingDTO.PrintAllPO = Convert.ToBoolean(jsonObject["PrintAllPO"]);
                }
                if (po != null)
                {
                    billingDTO.PO = new List<BillPODto>();
                    foreach (var p in po)
                    {
                        billingDTO.PO.Add(new BillPODto { PONumber = Convert.ToString(p["PONumber"]) });
                    }
                }
                if (taxInfo != null)
                {
                    foreach (var tax in taxInfo)
                    {
                        TaxDTO taxDto = new TaxDTO();
                        taxDto.TaxId = Convert.ToInt16(tax["TaxId"]);
                        taxDto.ItemValue = 0;// Convert.ToInt16(tax["ProductId"]); changed on 10/03/19 to apply tax on whole bill
                        taxDto.Rate = Convert.ToDouble(tax["Rate"]);// Convert.ToDouble(tax["TaxRate"]); changed on 10/03/19  to apply tax on whole bill
                        bool applicable = Convert.ToBoolean(tax["Applicable"]);
                        taxDto.TaxAmount = Convert.ToDouble(tax["TaxAmount"]);// (billingDTO.SubTotal + billingDTO.Freight + breakageAmount);

                        billingDTO.ApplicableTaxes.Add(taxDto);
                    }
                }
                var lossItems = jsonObject["LossItems"];
                billingDTO.LostItems = new List<LostItemDTO>();
                if (lossItems != null)
                {
                    foreach (var item in lossItems)
                    {
                        LostItemDTO lstDTO = new LostItemDTO();
                        lstDTO.ProductId = Convert.ToInt32(item["ProductId"]);
                        lstDTO.CompanyId = billingDTO.CompanyId;
                        lstDTO.LedgerId = billingDTO.LedgerId;
                        lstDTO.FinYearId = new LoggedInUser().FinYearId;
                        lstDTO.ChallanId = Convert.ToInt32(item["ProductId"]);
                        lstDTO.Quantity = Convert.ToDouble(item["Quantity"]);
                        lstDTO.Rate = Convert.ToDouble(item["Rate"]);
                        lstDTO.IGST = Convert.ToDouble(item["IGST"]);
                        lstDTO.CGST = Convert.ToDouble(item["CGST"]);
                        lstDTO.SGST = Convert.ToDouble(item["SGST"]);
                        lstDTO.IGSTRate = Convert.ToDouble(item["IGSTRate"]);
                        lstDTO.CGSTRate = Convert.ToDouble(item["CGSTRate"]);
                        lstDTO.SGSTRate = Convert.ToDouble(item["SGSTRate"]);
                        if (item["TaxCategoryId"] != null)
                        {
                            lstDTO.TaxCategoryId = Convert.ToInt32(item["TaxCategoryId"]);
                        }
                        lstDTO.LineTaxes = ParseInvoiceLineTaxes(item["LineTaxes"]);

                        lstDTO.Amount = lstDTO.Quantity * lstDTO.Rate;
                        //lstDTO.LostDate = Utils.FormatDate(Convert.ToString(jsonObject.GetValue("To")));
                        lstDTO.ChallanId = Convert.ToInt32(item["ChallanId"]);
                        billingDTO.LostItems.Add(lstDTO);

                    }

                }

                var payments = jsonObject["Payments"];
                billingDTO.Payments = new List<LedgerTransactionDTO>();
                if (payments != null)
                {
                    foreach (var item in payments)
                    {
                        LedgerTransactionDTO lstDTO = new LedgerTransactionDTO();
                        lstDTO.LedgerTransactionId = Convert.ToInt32(item["LedgerTransactionId"]);
                        lstDTO.TransactionAmount = Convert.ToDouble(item["TransactionAmount"]);
                        billingDTO.Payments.Add(lstDTO);
                    }
                }

                //billingDTO.LostItems = new List<LostItemDTO>();
                //if (lossItems != null)
                //{
                //    foreach (var item in lossItems)
                //    {
                //        LostItemDTO lstDTO = new LostItemDTO();
                //        lstDTO.ProductId = Convert.ToInt32(item["ProductId"]);
                //        lstDTO.CompanyId = billingDTO.CompanyId;
                //        lstDTO.LedgerId = billingDTO.LedgerId;
                //        lstDTO.FinYearId = new LoggedInUser().FinYearId;
                //        lstDTO.ChallanId = Convert.ToInt32(item["ProductId"]);
                //        lstDTO.Quantity = Convert.ToDouble(item["Quantity"]);
                //        lstDTO.Rate = Convert.ToDouble(item["Rate"]);
                //        lstDTO.IGST = Convert.ToDouble(item["IGST"]);
                //        lstDTO.CGST = Convert.ToDouble(item["CGST"]);
                //        lstDTO.SGST = Convert.ToDouble(item["SGST"]);
                //        lstDTO.IGSTRate = Convert.ToDouble(item["IGSTRate"]);
                //        lstDTO.CGSTRate = Convert.ToDouble(item["CGSTRate"]);
                //        lstDTO.SGSTRate = Convert.ToDouble(item["SGSTRate"]);

                //        lstDTO.Amount = lstDTO.Quantity * lstDTO.Rate;
                //        lstDTO.LostDate = Utils.FormatDate(Convert.ToString(jsonObject.GetValue("To")));

                //        billingDTO.LostItems.Add(lstDTO);

                //    }

                //}
                billingDTO.OtherCharges = new List<InvoiceChargeDTO>();
                var otherCharges = jsonObject["OtherCharges"];
                if (otherCharges != null)
                {

                    foreach (var item in otherCharges)
                    {
                        InvoiceChargeDTO lstDTO = new InvoiceChargeDTO();
                        lstDTO.ChargeId = Convert.ToInt32(item["ChargeId"]);
                        lstDTO.Amount = Convert.ToDouble(item["Amount"]);
                        billingDTO.OtherCharges.Add(lstDTO);

                    }
                    billingDTO.ChargesTax = Convert.ToDouble(jsonObject["ChargesTax"]);
                }

                billingDTO.Total += billingDTO.OtherCharges.Sum(o => o.Amount);

                billingDTO.Total -= billingDTO.Discount;
                bool result = await objBilling.CreateInvoice(billingDTO);
                int invoiceId = 0;
                if (result)
                {
                    invoiceId = billingDTO.InvoiceId;
                    billingDTO.TaxAmount = BreakageTax;
                    billingDTO.BreakageTax = 0;
                    billingDTO.Freight = 0;
                    billingDTO.OtherCharges = new List<InvoiceChargeDTO>();
                    billingDTO.LostItems = new List<LostItemDTO>();

                    //result = await AddBreakageBill(billingDTO, billingDTO.From, billingDTO.To, billingDTO.LedgerId);
                    //billingDTO.Freight = 0;
                }


                billingDTO.InvoiceId = invoiceId;// parent invoice
                if (invoiceId > 0)
                {

                    //CommHelpler com = new CommHelpler();

                    //var billInfo = objBilling.SelInvoiceHeader(invoiceId, user.DefaultCompanyId);
                    //if (billInfo != null && billInfo.Tables.Count > 0 && billInfo.Tables[0].Rows.Count > 0)
                    //{
                    //    var dr = billInfo.Tables[0].Rows[0];
                    //    var param = new Dictionary<string, string>();
                    //    param.Add("party", Convert.ToString(dr["PartyName"]));
                    //    param.Add("billAmount", Convert.ToString(dr["Total"]));
                    //    param.Add("company", Convert.ToString(dr["CompanyName"]));

                    //    // com.sendSms(Convert.ToString(dr["ContactPersonMobile"]), SMSTemplates.BILL_GENERATED, MessageEvent.BILL_GENRATED, param);
                    //}
                    //EmailParameters p = new EmailParameters()
                    //{
                    //    InvoiceNumber = billingDTO.InvoiceNumber,
                    //    InvoiceAmount = billingDTO.Total,
                    //    From = billingDTO.From,
                    //    To = billingDTO.To,
                    //    PartyName = billingDTO.Party,
                    //    Company = billingDTO.Company

                    //};
                    //Ledger ledger = new Ledger(billingDTO.LedgerId);
                    //LedgerDTO lDto = ledger.GetDetails();

                    // com.sendSms(lDto.ContactPersonMobile, p, MessageEvent.BILL_GENRATED);
                }
                apiMessage.Data = billingDTO;
                apiMessage.Code = ApiMessageCodes.SUCCESS;
                return Ok(apiMessage);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                logger.Error(ex);
                //   return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);
                //  apiMessage.Data = billingDTO;
                apiMessage.Message = ex.Message;
                apiMessage.Code = ApiMessageCodes.ERROR;
                return Ok(apiMessage);
            }
        }

        List<BillingItemDTO> AddItems(JToken items)
        {
            List<BillingItemDTO> billItems = new List<BillingItemDTO>();
            foreach (var item in items)
            {
                BillingItemDTO b = new BillingItemDTO();
                b.ProductId = Convert.ToInt32(item["ProductId"]);
                b.Rate = Convert.ToDouble(item["Rate"]);
                b.Quantity = Convert.ToDouble(item["Quantity"]);
                b.SubTotal = Convert.ToDouble(item["Amount"]);
                if (!String.IsNullOrEmpty(Convert.ToString(item["From"])))
                {
                    b.From = Convert.ToDateTime(item["From"]);
                    b.To = Convert.ToDateTime(item["To"]);
                }
                b.Days = Convert.ToDouble(item["Days"]);
                b.ClosingBalance = Convert.ToDouble(item["ClosingBalance"]);
                if (item["OPB"] != null)
                {
                    b.OPB = Convert.ToDouble(item["OPB"]);
                }
                b.ChargeReturnedDate = Convert.ToBoolean(item["ChargeReturnedDate"]);
                if (item["ProductSizeId"] != null)
                {
                    b.ProductSizeId = Convert.ToInt16(item["ProductSizeId"]);
                }

                if (item["QtyCalculation"] != null)
                {
                    b.QtyCalculation = Convert.ToString(item["QtyCalculation"]);
                }

                if (b.ChargeReturnedDate)
                {
                    b.Days += 1;
                }
                if (b.ItemCategory == 1013)
                {
                    b.SubTotal = (b.ClosingBalance * b.Rate * b.Days);
                }
                //item wise gst is 0 for rent bills
                //b.IGST = 0;
                //b.IGST = 0;
                //b.IGST = 0;
                b.DiscountPercent = Convert.ToDouble(item["DiscountPercent"]);
                b.Discount = Convert.ToDouble(item["Discount"]);


                b.IGST = Convert.ToDouble(item["IGST"]);
                b.CGST = Convert.ToDouble(item["CGST"]);
                b.SGST = Convert.ToDouble(item["SGST"]);
                b.IGSTRate = Convert.ToDouble(item["IGSTRate"]);
                b.CGSTRate = Convert.ToDouble(item["CGSTRate"]);
                b.SGSTRate = Convert.ToDouble(item["SGSTRate"]);

                if (item["OPB"] != null)
                {
                    b.ExcessQty = Convert.ToDouble(item["ExcessQty"]);
                }

                b.Total = b.SubTotal;
                if (item["TaxCategoryId"] != null && item["TaxCategoryId"].Type != JTokenType.Null)
                {
                    b.TaxCategoryId = Convert.ToInt16(item["TaxCategoryId"]);
                }
                if (item["ChallanId"] != null && item["ChallanId"].Type != JTokenType.Null)
                {
                    b.ChallanId = Convert.ToInt32(item["ChallanId"]);
                }
                b.LineTaxes = ParseInvoiceLineTaxes(item["LineTaxes"]);

                billItems.Add(b);
            }
            return billItems;
        }

        static List<InvoiceTaxDTO> ParseInvoiceLineTaxes(JToken lineTaxesToken)
        {
            if (lineTaxesToken == null || lineTaxesToken.Type != JTokenType.Array)
            {
                return null;
            }

            var lineTaxes = new List<InvoiceTaxDTO>();
            foreach (var lt in lineTaxesToken)
            {
                var tax = new InvoiceTaxDTO();
                tax.TaxId = Convert.ToInt32(lt["TaxId"]);
                tax.Rate = Convert.ToDouble(lt["Rate"]);
                tax.Amount = Convert.ToDouble(lt["Amount"]);
                tax.TaxAmount = tax.Amount;
                if (lt["TaxCategoryId"] != null && lt["TaxCategoryId"].Type != JTokenType.Null)
                {
                    tax.TaxCategoryId = Convert.ToInt32(lt["TaxCategoryId"]);
                }
                if (lt["TaxCode"] != null)
                {
                    tax.TaxCode = Convert.ToString(lt["TaxCode"]);
                }
                if (lt["RateType"] != null)
                {
                    tax.RateType = Convert.ToString(lt["RateType"]);
                }
                var taxName = lt["TaxName"] ?? lt["Name"];
                if (taxName != null)
                {
                    tax.Name = Convert.ToString(taxName);
                }
                lineTaxes.Add(tax);
            }
            return lineTaxes.Count > 0 ? lineTaxes : null;
        }

        public async Task<bool> AddBreakageBill(BillingDTO dto, DateTime from, DateTime to, int ledgerId)
        {
            if (dto.InvoiceId > 0)
            {
                var billDto = new BillingDTO();
                billDto.InjectFrom(dto);
                Billing objBill = new Billing();
                //   List<BillingItemDTO> breakageItems =  objBill.GetBreakageForBill(ledgerId, from, to, new LoggedInUser().FinYearId);
                if (billDto.BreakageItems.Count > 0)
                {

                    //dataObject.InvoiceDate = DateTime.Today;
                    billDto.BillableItems = billDto.BreakageItems;
                    //dataObject.InvoiceNumber = billDto.BillNumber;
                    billDto.From = from;
                    billDto.To = to;
                    billDto.LedgerId = ledgerId;
                    billDto.ParentInvoiceId = billDto.InvoiceId;
                    billDto.SubTotal = billDto.BreakageItems.Sum(o => o.Quantity * o.Rate);
                    // billDto.TaxAmount = 0;
                    billDto.Total = billDto.SubTotal;
                    billDto.ApplicableTaxes = new List<TaxDTO>(); //no taxes as of now
                    billDto.InvoiceType = 2;// - breakage bill
                    billDto.Freight = 0;// no freight for breakage items
                    billDto.FreightTax = 0;
                    billDto.TaxAmount = 0;
                    billDto.OtherCharges = null;
                    billDto.OtherChargeAmount = 0;
                    billDto.InvoiceId = 0;
                    return await objBill.CreateInvoice(billDto);
                }
            }
            return false;
        }


        [HttpPost]
        public async Task<HttpResponseMessage> SaveBreakageBill()
        {
            try
            {
                //  DataCopier<WorkOrderDTO, WorkOrder> dcopier = new DataCopier<WorkOrderDTO, WorkOrder>();
                // dcopier.CopyData(dto, leder);
                string dto = System.Web.HttpContext.Current.Request["dto"];
                System.Web.HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                JObject jsonObject = new JObject();

                BillingDTO billingDTO = new BillingDTO();
                billingDTO.FinYearId = new LoggedInUser().FinYearId;
                jsonObject = JObject.Parse(dto);
                NextId n = new NextId();
                //   String nextId = n.GetNextId(BAL.Enums.NextIDTables.Invoice, new LoggedInUser().FinYearId.ToString(), new LoggedInUser().DefaultCompanyId);
                billingDTO.WorkOrderNumber = Convert.ToString(jsonObject.GetValue("WorkOrderNumber"));
                billingDTO.InvoiceDate = Utils.FormatDate(Convert.ToString(jsonObject.GetValue("InvoiceDate")));

                billingDTO.LedgerId = Convert.ToInt16(jsonObject.GetValue("LedgerId"));
                billingDTO.SubTotal = Convert.ToDouble(jsonObject.GetValue("SubTotal"));
                if (jsonObject.GetValue("Taxable") != null)
                    billingDTO.Taxable = Convert.ToDouble(jsonObject.GetValue("Taxable"));
                billingDTO.TaxAmount = Convert.ToDouble(jsonObject.GetValue("TaxAmount"));
                billingDTO.FreightTax = Convert.ToDouble(jsonObject.GetValue("FreightTax"));


                double BreakageTax = Convert.ToDouble(jsonObject.GetValue("BreakageTax"));

                if (jsonObject.GetValue("SiteAddress") != null)
                {
                    billingDTO.SiteAddress = Convert.ToString(jsonObject.GetValue("SiteAddress"));
                }
                billingDTO.Freight = Convert.ToDouble(jsonObject.GetValue("Freight"));
                billingDTO.WorkOrderId = jsonObject.GetValue("WorkOrderId") != null ? Convert.ToInt32(jsonObject.GetValue("WorkOrderId")) : 0;

                billingDTO.CompanyId = new LoggedInUser().DefaultCompanyId;
                billingDTO.InvoiceNumber = Convert.ToString(jsonObject.GetValue("InvoiceNumber")); //nextId;
                billingDTO.CreatedBy = new LoggedInUser().UserId;

                if (Convert.ToDouble(jsonObject.GetValue("RoundOff")) != null)
                {
                    billingDTO.RoundOff = Convert.ToBoolean(jsonObject.GetValue("RoundOff"));
                }

                Int16 invoiceType = 2;
                if (Convert.ToDouble(jsonObject.GetValue("InvoiceType")) != null)
                {
                    invoiceType = Convert.ToInt16(jsonObject.GetValue("InvoiceType"));
                }
                if (Convert.ToString(jsonObject.GetValue("Category")) != null)
                {
                    billingDTO.Category = Convert.ToString(jsonObject.GetValue("Category"));
                }
                if (Convert.ToString(jsonObject.GetValue("BranchCode")) != null)
                {
                    billingDTO.BranchCode = Convert.ToString(jsonObject.GetValue("BranchCode"));
                }
                if (Convert.ToString(jsonObject.GetValue("ContractorCode")) != null)
                {
                    billingDTO.ContractorCode = Convert.ToString(jsonObject.GetValue("ContractorCode"));
                }
                billingDTO.InvoiceType = Convert.ToInt16(Enum.ToObject(typeof(InvoiceTypes), invoiceType));

                Billing objBilling = new Billing();
                var items = jsonObject["Items"];
                var taxInfo = jsonObject["AppliedTaxes"];
                billingDTO.ApplicableTaxes = new List<TaxDTO>();
                billingDTO.BillableItems = AddItems(items);
                Billing objBill = new Billing();

                foreach (var tax in taxInfo)
                {
                    TaxDTO taxDto = new TaxDTO();
                    taxDto.TaxId = Convert.ToInt16(tax["TaxId"]);
                    taxDto.ItemValue = Convert.ToInt16(tax["ProductId"]);
                    taxDto.Rate = Convert.ToDouble(tax["TaxRate"]);
                    bool applicable = Convert.ToBoolean(tax["Applicable"]);
                    taxDto.TaxAmount = Convert.ToDouble(tax["TaxAmount"]);// (billingDTO.SubTotal + billingDTO.Freight + breakageAmount);

                    billingDTO.ApplicableTaxes.Add(taxDto);
                }
                //to calculate the subtotal of individual items. As above addItem calculates the subtotal on the basis of closing balance and days.
                //days and closing balance will not exist in case of breakage/loss billing.
                foreach (BillingItemDTO item in billingDTO.BillableItems)
                {
                    item.SubTotal = item.Quantity * item.Rate;
                    if (item.Days > 0)
                    {
                        item.SubTotal = item.Quantity * item.Rate * item.Days;
                    }
                }

                bool result = await objBilling.CreateInvoice(billingDTO);

                return Request.CreateResponse(HttpStatusCode.OK, billingDTO);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetBillList([FromBody] BillingDTO dto)
        {
            try
            {
                Billing objBilling = new Billing();
                var list = objBilling.GetBilList(dto.From.ToShortDateString(), dto.To.ToShortDateString(), new LoggedInUser().DefaultCompanyId, dto.LedgerId, dto.LedgerSiteId, dto.StatusId, dto.InvoiceType);
                if (!string.IsNullOrWhiteSpace(dto.InvoiceNumber))
                {
                    var invNo = dto.InvoiceNumber.Trim();
                    list = list.Where(x => x.InvoiceNumber != null && x.InvoiceNumber.IndexOf(invNo, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                }
                var totalCount = list.Count;
                if (dto.PageSize > 0 && dto.PageIndex >= 1)
                {
                    var pageData = list.Skip((dto.PageIndex - 1) * dto.PageSize).Take(dto.PageSize).ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, new { Data = pageData, TotalCount = totalCount });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { Data = list, TotalCount = totalCount });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage GetBillItems([FromBody] BillingDTO dto)
        {
            try
            {
                Billing objBilling = new Billing();
                return Request.CreateResponse(HttpStatusCode.OK, objBilling.BillItems(dto.InvoiceId));

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage GetBillLossItems([FromBody] BillingDTO dto)
        {
            try
            {
                Billing objBilling = new Billing();

                DataSet ds = objBilling.GetLossItems(dto.InvoiceId);
                return Request.CreateResponse(HttpStatusCode.OK, new Utils<LostItemDTO>().ConstructList(ds));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage GetBreakageItems([FromBody] BillingDTO dto)
        {
            try
            {
                Billing objBilling = new Billing();

                DataSet ds = objBilling.GetBreakageItems(dto.InvoiceId);
                return Request.CreateResponse(HttpStatusCode.OK, new Utils<LostItemDTO>().ConstructList(ds));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage PrintBill([FromBody] BillingDTO dto)
        {
            try
            {

                Billing billing = new Billing();
                DataSet ds = billing.PrintBill(dto.InvoiceId);
                int partyId = 0;
                string subReportName = "invoiceTax-subreport-period";
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        partyId = Convert.ToInt32(ds.Tables[0].Rows[0]["LedgerId"]);

                        if (ds.Tables[0].Rows[0]["SignedQrCode"] != DBNull.Value)
                        {
                            var qrCodeStr = Convert.ToString(ds.Tables[0].Rows[0]["SignedQrCode"]);
                            var codeGen = new QRCodeGenerator();

                            var qrCodeData = codeGen.CreateQrCode(Encoding.UTF8.GetBytes(qrCodeStr), QRCodeGenerator.ECCLevel.Q);
                            using (Base64QRCode qrCode = new Base64QRCode(qrCodeData))
                            {
                                qrCode.GetGraphic(20);

                                ds.Tables[0].Rows[0]["SignedQrCode"] = qrCode.GetGraphic(20);// Convert.ToBase64String(qrCodeImage);
                            }




                        }

                        if (ds.Tables[0].Rows[0]["CompanyLogo"] != DBNull.Value)
                        {
                            var logo = Convert.ToString(ds.Tables[0].Rows[0]["CompanyLogo"]);
                            logo = docsPath + @"\comp\" + logo;
                            if (File.Exists(logo))
                            {
                                var fileBytes = File.ReadAllBytes(logo);
                                ds.Tables[0].Rows[0]["CompanyLogo"] = Convert.ToBase64String(fileBytes);
                            }

                        }


                    }
                }
                String reportFileName = "invoice-rent.rdlc";
                if (Convert.ToInt16(ds.Tables[0].Rows[0]["InvoiceType"]) == 3) //Site invoice
                {
                    reportFileName = "invoice-site.rdlc";
                    subReportName = "invoiceTax-subreport";
                }
                if (Convert.ToInt16(ds.Tables[0].Rows[0]["InvoiceType"]) == 4) //Site invoice
                {
                    reportFileName = "salesReceipt.rdlc";
                    subReportName = "invoiceTax-subreport";
                }
                if (Convert.ToInt16(ds.Tables[0].Rows[0]["InvoiceType"]) == 5) //Site invoice
                {
                    reportFileName = "contractBill.rdlc";
                    subReportName = "invoiceTax-subreport";
                }
                string lossReport = "invoice-lossItems";
                string breakageReport = "invoice-breakageItems";
                BAL.Objects.Report objRPT = new BAL.Objects.Report();


                List<string> files = new List<string>();
                String outPutFIle = "";
                if (dto.HeaderTypes == null)
                {
                    dto.HeaderTypes = new List<int> { 1 };
                }
                if (dto.HeaderTypes.Count == 0)
                {
                    dto.HeaderTypes = new List<int> { 1 };
                }
                if (dto.HeaderTypes != null)
                {
                    foreach (int x in dto.HeaderTypes)
                    {
                        string pdfName = dto.InvoiceId.ToString() + x.ToString();

                        DataSet headerDataSet = objRPT.GetReportHeader_Bill(partyId, new LoggedInUser().DefaultCompanyId, dto.InvoiceId, x);



                        string fileName = CreateReportFile(pdfName, reportFileName, headerDataSet, ds, BAL.Enums.ExportFormat.PDF, breakageReport, lossReport);
                        files.Add(fileName);
                    }
                }
                if (files.Count > 0)
                {
                    outPutFIle = MergeFiles(files, Guid.NewGuid().ToString() + ".pdf");
                }
                return Request.CreateResponse(HttpStatusCode.OK, outPutFIle);

            }
            catch (Exception ex)
            {
                String msg = ex.Message;
                if (ex.InnerException != null)
                {
                    msg += ex.InnerException.Message;
                }
                return Request.CreateResponse(HttpStatusCode.OK, msg);
            }
        }
        [HttpPost]
        public HttpResponseMessage PrintContractBill([FromBody] BillingDTO dto)
        {
            try
            {

                Billing billing = new Billing();
                DataSet ds = billing.PrintContractBill(dto.InvoiceId);
                int partyId = 0;
                string subReportName = "invoiceTax-subreport";

                string reportFileName = "contractBill.rdlc";


                BAL.Objects.Report objRPT = new BAL.Objects.Report();


                List<string> files = new List<string>();
                string outPutFIle = "";
                if (dto.HeaderTypes == null)
                {
                    dto.HeaderTypes = new List<int> { 1 };
                }
                if (dto.HeaderTypes.Count == 0)
                {
                    dto.HeaderTypes = new List<int> { 1 };
                }

                string pdfName = "contract-" + dto.InvoiceId.ToString();
                if (ds.Tables[0].Rows[0]["CompanyLogo"] != DBNull.Value)
                {
                    var logo = Convert.ToString(ds.Tables[0].Rows[0]["CompanyLogo"]);
                    logo = docsPath + @"\comp\" + logo;
                    if (File.Exists(logo))
                    {
                        var fileBytes = File.ReadAllBytes(logo);
                        ds.Tables[0].Rows[0]["CompanyLogo"] = Convert.ToBase64String(fileBytes);
                    }

                }

                string fileName = CreateReportFile(pdfName, reportFileName, null, ds, BAL.Enums.ExportFormat.PDF);
                files.Add(fileName);
                if (files.Count > 0)
                {
                    outPutFIle = MergeFiles(files, Guid.NewGuid().ToString() + ".pdf");
                }
                return Request.CreateResponse(HttpStatusCode.OK, outPutFIle);

            }
            catch (Exception ex)
            {
                String msg = ex.Message;
                if (ex.InnerException != null)
                {
                    msg += ex.InnerException.Message;
                }
                return Request.CreateResponse(HttpStatusCode.OK, msg);
            }
        }

        [HttpPost]
        public HttpResponseMessage BillingItemsTax([FromBody] BillingDTO dto)
        {
            try
            {
                Billing billing = new Billing();
                return Request.CreateResponse(HttpStatusCode.OK, billing.BillingItemsTax(dto.InvoiceId));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ApiMessage> CancelBill([FromBody] BillingDTO dto)
        {
            var res = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                Billing billing = new Billing();
                res.Data = await billing.CancelInvoice(dto.InvoiceId, user.UserId, DateTime.Now, user.DefaultCompanyId);
                res.Code = ApiMessageCodes.SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.Code = ApiMessageCodes.ERROR;
                res.Message = ex.Message;
            }
            return res;
        }

        [HttpPost]
        public HttpResponseMessage MarkSettle([FromBody] BillingDTO dto)
        {
            try
            {
                Billing billing = new Billing();
                var user = new LoggedInUser();
                return Request.CreateResponse(HttpStatusCode.OK, billing.MarkSettle(dto.InvoiceId, user.UserId, DateTime.Now, user.DefaultCompanyId));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage DueBills([FromBody] BillingDTO dto)
        {
            try
            {
                Billing billing = new Billing();
                //  DateTime to = Utils.FormatDate(dto.To);
                var user = new LoggedInUser();
                DataSet ds = billing.DueBillsSummary(dto.LedgerSiteId, user.DefaultCompanyId, dto.LedgerId, dto.From, dto.To, new LoggedInUser().FinYearId);
                List<BillingItemDTO> lst = new Utils<BillingItemDTO>().ConstructList(ds);
                var objLedger = new Ledger();
                //var balance = objLedger.ClientWiseItems(0, 0, user.DefaultCompanyId,
                //dto.From.AddDays(-1).ToShortDateString(), dto.From.AddDays(-1).ToShortTimeString(), "rent");

                //if (balance != null)
                //{
                //    var balanceList = (from d in balance.Tables[0].AsEnumerable()
                //                       select new BillingItemDTO
                //                       {
                //                           LedgerSiteId = Convert.ToInt32(d["LedgerSiteId"]),
                //                           SentQty = Convert.ToInt32(d["IssuedQty"]),
                //                           RecQty = Convert.ToInt32(d["ReceivedQty"]),
                //                           ClosingBalance = Convert.ToInt32(d["ClosingBalance"])

                //                       });

                //    var itemGroups = balanceList.GroupBy(o => o.LedgerSiteId).Select(o => new BillingItemDTO
                //    {
                //        LedgerSiteId = o.Key,
                //        OpBalance = o.Sum(x => x.OpBalance),

                //        //SentQty = o.Sum(x => x.SentQty),
                //        //RecQty = o.Sum(x => x.RecQty),
                //        ClosingBalance = o.Sum((x) => x.ClosingBalance)
                //    });
                //    foreach (var item in itemGroups)
                //    {
                //        var site = lst.Where(o => o.LedgerSiteId == item.LedgerSiteId).FirstOrDefault();
                //        if (site != null)
                //        {
                //            site.OpBalance = site.ClosingBalance - item.ClosingBalance;

                //            //site.SentQty = item.SentQty;
                //            //site.RecQty = item.RecQty;
                //            //site.ClosingBalance = item.ClosingBalance;
                //        }
                //    }
                //}

                var orderedList = lst.OrderBy(o => o.LastBillDate).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, lst);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage PrintDueBills([FromBody] BillingDTO dto)
        {
            try
            {
                Billing billing = new Billing();
                LoggedInUser user = new LoggedInUser();
                // DateTime to = Utils.FormatDate(dto.To);
                String fileName = new ReportUtility().DueBills(dto.LedgerSiteId, user.DefaultCompanyId, dto.LedgerId, dto.To, user.FinYearId);
                return Request.CreateResponse(HttpStatusCode.OK, fileName);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage PrintDueBillsPdf([FromBody] BillingDTO dto)
        {
            try
            {
                LoggedInUser user = new LoggedInUser();
                byte[] fileBytes = new ReportUtility().DueBillsPdf(dto.LedgerSiteId, user.DefaultCompanyId, dto.LedgerId, dto.To, user.FinYearId);
                var result = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(fileBytes)
                };
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment") { FileName = "dueBills.pdf" };
                result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                return result;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public ApiMessage GetBillInfo([FromBody] BillingDTO dto)
        {
            var message = new ApiMessage();
            try
            {
                Billing billing = new Billing();
                LoggedInUser user = new LoggedInUser();
                var invoice = billing.GetBillingInfo(dto.InvoiceId);


                message.Data = invoice;
                message.Code = ApiMessageCodes.SUCCESS;

                return message;
            }
            catch (Exception ex)
            {
                message.Description = ex.Message;
                message.Code = ApiMessageCodes.ERROR;
                return message;

            }
        }
        [HttpPost]
        public async Task<ApiMessage> PushToIRP([FromBody] BillingDTO dto)
        {
            var message = new ApiMessage();
            try
            {
                Billing billing = new Billing();
                LoggedInUser user = new LoggedInUser();
                var invoice = billing.GetBillingInfo(dto.InvoiceId);
                invoice.BillableItems = billing.BillItems(dto.InvoiceId);
                if (invoice != null)
                {
                    var eInvoice = new EInvoiceService();
                    invoice.UserId = user.UserId;
                    message.Data = await eInvoice.CreateIRN(invoice);
                    message.Code = ApiMessageCodes.SUCCESS;
                }
                return message;
            }
            catch (Exception ex)
            {
                message.Message = ex.Message;
                message.Code = ApiMessageCodes.ERROR;
                return message;

            }
        }
        [HttpPost]
        public async Task<ApiMessage> SendForApproval([FromBody] BillingDTO dto)
        {
            var message = new ApiMessage();
            try
            {
                Billing billing = new Billing();
                LoggedInUser user = new LoggedInUser();
                message.Data = await billing.SendForApproval(dto.InvoiceId, user.UserId, DateTime.Now, user.DefaultCompanyId);
                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (Exception ex)
            {
                message.Description = ex.Message;
                message.Code = ApiMessageCodes.ERROR;
                return message;

            }
        }
        [HttpPost]
        public async Task<ApiMessage> ApproveBill([FromBody] BillingDTO dto)
        {
            var message = new ApiMessage();
            try
            {
                Billing billing = new Billing();
                LoggedInUser user = new LoggedInUser();
                message.Data = await billing.Approve(dto.InvoiceId, user.UserId, DateTime.Now, user.DefaultCompanyId);
                message.Code = ApiMessageCodes.SUCCESS;


                return message;
            }
            catch (Exception ex)
            {
                message.Description = ex.Message;
                message.Code = ApiMessageCodes.ERROR;
                return message;

            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> UnpaidInvoices([FromBody] PurchaseDTO dto)
        {
            var msg = new ApiMessage();
            try
            {

                var user = new LoggedInUser();
                dto.CompanyId = user.DefaultCompanyId;
                Billing bill = new Billing();
                msg.Data = await bill.GetUnpaidBills(dto.LedgerId, dto.CompanyId, dto.LedgerSiteId);
                msg.Code = ApiMessageCodes.SUCCESS;
                return Ok(msg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
