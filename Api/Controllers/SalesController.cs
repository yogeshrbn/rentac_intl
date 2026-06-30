using BAL.DAL;
using BAL.DTO;
using BAL.Objects;
using FarmaAPI.Helper;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
namespace FarmaAPI.Controllers
{
    [Authorize]
    public class SalesController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public async Task<IHttpActionResult> SaveInvoiceBill()
        {
            var apiMessage = new ApiMessage();
            try
            {
                //  DataCopier<WorkOrderDTO, WorkOrder> dcopier = new DataCopier<WorkOrderDTO, WorkOrder>();
                // dcopier.CopyData(dto, leder);
                string dto = System.Web.HttpContext.Current.Request["dto"];
                System.Web.HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                JObject jsonObject = new JObject();

                BillingDTO billingDTO = new BillingDTO();
                Billing objBilling = new Billing();
                jsonObject = JObject.Parse(dto);
                NextId n = new NextId();



                billingDTO.WorkOrderNumber = Convert.ToString(jsonObject.GetValue("WorkOrderNumber"));
                //   billingDTO.From = Utils.FormatDate(Convert.ToString(jsonObject.GetValue("From"))).ToShortDateString();
                billingDTO.To = Convert.ToDateTime(jsonObject.GetValue("To"));
                billingDTO.LedgerId = Convert.ToInt16(jsonObject.GetValue("LedgerId"));
                billingDTO.SubTotal = Convert.ToDouble(jsonObject.GetValue("SubTotal"));
                billingDTO.TaxAmount = Convert.ToDouble(jsonObject.GetValue("TaxAmount"));
                billingDTO.FreightTax = Convert.ToDouble(jsonObject.GetValue("FreightTax"));
                billingDTO.Discount = Convert.ToDouble(jsonObject.GetValue("Discount"));
                double BreakageTax = Convert.ToDouble(jsonObject.GetValue("BreakageTax"));

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

                billingDTO.Freight = Convert.ToDouble(jsonObject.GetValue("Freight"));
                billingDTO.CompanyId = new LoggedInUser().DefaultCompanyId;
                //   billingDTO.InvoiceNumber = nextId;
                billingDTO.CreatedBy = new LoggedInUser().UserId;
                billingDTO.InvoiceType = 1; // bill
                billingDTO.FinYearId = new LoggedInUser().FinYearId;
                billingDTO.InvoiceDate = DateTime.Today;

                var items = jsonObject["Items"];
                //var taxInfo = jsonObject["AppliedTaxes"]; modified on 10 march 19. Now we will apply tax on whole bill not on individual items
                var taxInfo = jsonObject["Taxes"];
                billingDTO.ApplicableTaxes = new List<TaxDTO>();
                // billingDTO.BillableItems = AddItems(items);
                billingDTO.From = billingDTO.BillableItems.Min(o => o.From);

                Billing objBill = new Billing();
                List<BillingItemDTO> breakageItems = new List<BillingItemDTO>();// objBill.GetBreakageForBill(billingDTO.LedgerId, billingDTO.From, billingDTO.To, new LoggedInUser().FinYearId);
                var breakages = jsonObject["Breakages"];
                double breakageAmount = 0;
                foreach (var br in breakages)
                {
                    BillingItemDTO b = new BillingItemDTO();
                    b.ProductId = Convert.ToInt32(br["ProductId"]);
                    b.Quantity = Convert.ToDouble(br["Quantity"]);
                    b.Rate = Convert.ToDouble(br["Rate"]);
                    b.SubTotal = b.Quantity * b.Rate;
                    // b.BreakageAmount = b.SubTotal * b.Rate;
                    breakageItems.Add(b);
                }
                billingDTO.BreakageItems = breakageItems;
                if (breakageItems != null)
                {
                    breakageAmount = breakageItems.Sum(o => o.SubTotal);
                    billingDTO.BreakageAmount = breakageAmount;
                    billingDTO.BreakageTax = BreakageTax;
                }
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

                var lossItems = jsonObject["LossItems"];
                billingDTO.LostItems = new List<LostItemDTO>();
                foreach (var item in lossItems)
                {
                    LostItemDTO lstDTO = new LostItemDTO();
                    lstDTO.ProductId = Convert.ToInt32(item["ProductId"]);
                    lstDTO.CompanyId = billingDTO.CompanyId;
                    lstDTO.LedgerId = billingDTO.LedgerId;
                    lstDTO.FinYearId = new LoggedInUser().FinYearId;

                    lstDTO.Quantity = Convert.ToInt32(item["Quantity"]);
                    lstDTO.Rate = Convert.ToInt32(item["Rate"]);
                    lstDTO.Amount = lstDTO.Quantity * lstDTO.Rate;
                    lstDTO.LostDate = Utils.FormatDate(Convert.ToString(jsonObject.GetValue("To")));

                    billingDTO.LostItems.Add(lstDTO);

                }
                var otherCharges = jsonObject["OtherCharges"];
                billingDTO.OtherCharges = new List<InvoiceChargeDTO>();
                foreach (var item in otherCharges)
                {
                    InvoiceChargeDTO lstDTO = new InvoiceChargeDTO();
                    lstDTO.ChargeId = Convert.ToInt32(item["ChargeId"]);
                    lstDTO.Amount = Convert.ToDouble(item["Amount"]);
                    billingDTO.OtherCharges.Add(lstDTO);

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

                    //billingDTO.Freight = 0;
                }
                billingDTO.InvoiceId = invoiceId;// parent invoice

                apiMessage.Data = billingDTO;
                apiMessage.Code = ApiMessageCodes.SUCCESS;
                return Ok(apiMessage);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                //   return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);
                //  apiMessage.Data = billingDTO;
                apiMessage.Message = ex.Message;
                apiMessage.Code = ApiMessageCodes.ERROR;
                return Ok(apiMessage);
            }
        }

        [HttpPost]
        [System.Web.Mvc.ValidateInput(false)]
        public async Task<HttpResponseMessage> Save()
        {
            try
            {
                BillingDTO salesDTO = new BillingDTO();
                Billing objBilling = new Billing();
                //  DataCopier<WorkOrderDTO, WorkOrder> dcopier = new DataCopier<WorkOrderDTO, WorkOrder>();
                // dcopier.CopyData(dto, leder);
                string dto = System.Web.HttpContext.Current.Request["dto"];
                System.Web.HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                JObject jsonObject = new JObject();

                //SalesDTO salesDTO = new SalesDTO();

                jsonObject = JObject.Parse(dto);
                //NextId n = new NextId();
                //String nextId = n.GetNextId(BAL.Enums.NextIDTables.Invoice, new LoggedInUser().FinYearId.ToString(), new LoggedInUser().DefaultCompanyId);
                salesDTO.InvoiceNumber = Convert.ToString(jsonObject.GetValue("InvoiceNumber"));
                salesDTO.InvoiceDate = Utils.FormatDate(Convert.ToString(jsonObject["InvoiceDate"]));
                salesDTO.InvoiceType = Convert.ToInt16(jsonObject.GetValue("InvoiceType"));
                if (jsonObject.GetValue("QuotationType") != null)
                    salesDTO.QuotationType = Convert.ToInt16(jsonObject.GetValue("QuotationType"));
                if (jsonObject.GetValue("Area") != null)
                    salesDTO.Area = Convert.ToSingle(jsonObject.GetValue("Area"));
                if (jsonObject.GetValue("MeasureType") != null)
                    salesDTO.MeasureType = Convert.ToInt16(jsonObject.GetValue("MeasureType"));
                if (jsonObject.GetValue("LineTotalMode") != null)
                    salesDTO.LineTotalMode = Convert.ToString(jsonObject.GetValue("LineTotalMode"));

                if (salesDTO.InvoiceType == 5 && jsonObject.GetValue("From") != null && jsonObject.GetValue("To") != null)
                {
                    var fromStr = Convert.ToString(jsonObject.GetValue("From"));
                    var toStr = Convert.ToString(jsonObject.GetValue("To"));
                    if (!String.IsNullOrWhiteSpace(fromStr) && !String.IsNullOrWhiteSpace(toStr))
                    {
                        salesDTO.From = Utils.FormatDate(fromStr);
                        salesDTO.To = Utils.FormatDate(toStr);
                    }
                    else
                        salesDTO.From = salesDTO.To = salesDTO.InvoiceDate;
                }
                else
                    salesDTO.From = salesDTO.To = salesDTO.InvoiceDate;

                salesDTO.LedgerId = Convert.ToInt32(jsonObject.GetValue("LedgerId"));
                salesDTO.LedgerSiteId = Convert.ToInt32(jsonObject.GetValue("LedgerSiteId"));
                salesDTO.InvoiceId = Convert.ToInt32(jsonObject.GetValue("InvoiceId"));

                //   salesDTO.SalesAccountId = Convert.ToInt16(jsonObject.GetValue("SalesAccountId"));
                salesDTO.SubTotal = Convert.ToDouble(jsonObject.GetValue("SubTotal"));
                if (jsonObject.GetValue("Taxable") != null)
                    salesDTO.Taxable = Convert.ToDouble(jsonObject.GetValue("Taxable"));
                salesDTO.TaxAmount = Convert.ToDouble(jsonObject.GetValue("TaxAmount"));
                salesDTO.FreightTax = Convert.ToDouble(jsonObject.GetValue("FreightTax"));
                double BreakageTax = Convert.ToDouble(jsonObject.GetValue("BreakageTax"));
                salesDTO.FinYearId = new LoggedInUser().FinYearId;
                salesDTO.Freight = Convert.ToDouble(jsonObject.GetValue("Freight"));

                salesDTO.OtherChargeAmount = Convert.ToDouble(jsonObject.GetValue("OtherChargeAmount"));
                salesDTO.ChargesTax = Convert.ToDouble(jsonObject.GetValue("ChargesTax"));
                salesDTO.Charge1 = Convert.ToDouble(jsonObject.GetValue("Charge1"));
                salesDTO.Charge2 = Convert.ToDouble(jsonObject.GetValue("Charge2"));
                salesDTO.Charge3 = Convert.ToDouble(jsonObject.GetValue("Charge3"));
                salesDTO.Charge4 = Convert.ToDouble(jsonObject.GetValue("Charge4"));
                salesDTO.Charge5 = Convert.ToDouble(jsonObject.GetValue("Charge5"));

                if (jsonObject.GetValue("Tnc") != null)
                {
                    salesDTO.Tnc = HttpUtility.HtmlDecode(Convert.ToString(jsonObject.GetValue("Tnc")));
                }

                if (jsonObject.GetValue("ShipTo") != null)
                {
                    salesDTO.ShipTo = Convert.ToString(jsonObject.GetValue("ShipTo"));
                }

                if (jsonObject.GetValue("BillFromSite") != null)
                {

                    salesDTO.BillFromSite = Convert.ToByte(jsonObject.GetValue("BillFromSite"));
                }

                if (jsonObject.GetValue("PODate") != null
                    && !String.IsNullOrEmpty(Convert.ToString(jsonObject.GetValue("PODate"))))
                {
                    var _poDt = Convert.ToDateTime(jsonObject.GetValue("PODate"));
                    if (_poDt.Year > 1900)
                        salesDTO.PODate = _poDt;// Utils.FormatDate(Convert.ToString(jsonObject.GetValue("PODate")));
                }
                if (jsonObject.GetValue("PONumber") != null
                   && !String.IsNullOrEmpty(Convert.ToString(jsonObject.GetValue("PONumber"))))
                {
                    salesDTO.PONumber = Convert.ToString(jsonObject.GetValue("PONumber"));


                }
                if (!String.IsNullOrEmpty(salesDTO.PONumber) && salesDTO.PODate == DateTime.MinValue)
                {
                    throw new Exception("PO date is required");
                }
                if (String.IsNullOrEmpty(salesDTO.PONumber) && salesDTO.PODate != DateTime.MinValue)
                {
                    throw new Exception("PO number is required");
                }
                if (jsonObject.GetValue("Iteration") != null)
                {
                    salesDTO.Iteration = Convert.ToString(jsonObject.GetValue("Iteration"));
                }
                if (jsonObject.GetValue("Recurring") != null)
                {
                    salesDTO.Recurring = Convert.ToBoolean(jsonObject.GetValue("Recurring"));
                    if (salesDTO.Recurring)
                    {
                        if (jsonObject.GetValue("StartsOn") != null
                    && !String.IsNullOrEmpty(Convert.ToString(jsonObject.GetValue("StartsOn"))))
                        {
                            salesDTO.StartsOn = Convert.ToDateTime(jsonObject.GetValue("StartsOn"));
                        }
                        if (jsonObject.GetValue("EndsOn") != null && !String.IsNullOrEmpty(Convert.ToString(jsonObject.GetValue("EndsOn"))))
                        {
                            salesDTO.EndsOn = Convert.ToDateTime(jsonObject.GetValue("EndsOn"));
                        }
                    }
                }
                salesDTO.Discount = Convert.ToDouble(jsonObject.GetValue("Discount"));
                salesDTO.DiscountPercent = Convert.ToDouble(jsonObject.GetValue("DiscountPercent"));

                salesDTO.CompanyId = new LoggedInUser().DefaultCompanyId;
                // salesDTO.VoucherNumber = nextId;
                salesDTO.CreatedBy = new LoggedInUser().UserId;
                if (jsonObject.GetValue("ContractId") != null)
                {
                    salesDTO.ContractId = Convert.ToInt32(jsonObject.GetValue("ContractId"));
                }
                if (jsonObject.GetValue("BillQuotationIds") != null && jsonObject["BillQuotationIds"].Type != JTokenType.Null)
                {
                    salesDTO.BillQuotationIds = jsonObject["BillQuotationIds"].ToObject<List<int>>();
                }
                // salesDTO.InvoiceType = 4; // bill
                // salesDTO.SalesDate = DateTime.Today;
                Sales objSales = new Sales();
                var items = jsonObject["Items"];
                //var taxInfo = jsonObject["AppliedTaxes"];
                salesDTO.ApplicableTaxes = new List<TaxDTO>();

                if (salesDTO.InvoiceType == 6)
                {
                    salesDTO.LostItems = AddLostItem(items);

                }
                else if (salesDTO.InvoiceType == 2)
                {
                    salesDTO.BreakageItems = AddBillItems(items, salesDTO);

                }
                else
                {
                    salesDTO.BillableItems = AddBillItems(items, salesDTO);
                }

                if (salesDTO.InvoiceType == 5 && salesDTO.QuotationType == 16 && salesDTO.BillableItems != null && salesDTO.BillableItems.Count > 0)
                {
                    string headerContractLineMode = ResolveContractLineTotalMode(salesDTO);
                    foreach (var item in salesDTO.BillableItems)
                    {
                        item.Duration = 1;
                        if (item.From.Year < 2000)
                            item.From = salesDTO.From;
                        if (item.To.Year < 2000)
                            item.To = salesDTO.To;
                        string lineMode = ResolveContractLineTotalModeForBillingLine(item, headerContractLineMode);
                        item.SubTotal = ContractQuotationLineSubTotal(lineMode, item, salesDTO.Area, salesDTO.From, salesDTO.To);
                        item.Total = item.SubTotal + item.IGST + item.CGST + item.SGST;
                    }
                    salesDTO.SubTotal = salesDTO.BillableItems.Sum(o => o.SubTotal);
                }

                if (salesDTO.Recurring)
                {
                    if (salesDTO.StartsOn != null && salesDTO.StartsOn < salesDTO.InvoiceDate)
                    {
                        throw new Exception("Recurring invoices can not start before invoice date");
                    }
                    if (salesDTO.EndsOn != null && salesDTO.EndsOn < salesDTO.InvoiceDate)
                    {
                        throw new Exception("Recurring invoices can not start before invoice date");
                    }
                    if (salesDTO.EndsOn != null && salesDTO.EndsOn < salesDTO.StartsOn)
                    {
                        throw new Exception("Recurring invoices end date must be ahead of start date");
                    }
                    if (String.IsNullOrEmpty(salesDTO.Iteration))
                    {
                        throw new Exception("Recurring invoices repeat must be selected ('weekly','monthly','yearly')");
                    }
                }
                bool result = await objBilling.CreateInvoice(salesDTO);

                if (result && salesDTO.InvoiceId > 0 && salesDTO.InvoiceType == 5
                    && salesDTO.BillQuotationIds != null && salesDTO.BillQuotationIds.Count > 0)
                {
                    await objBilling.LinkQuotationsToInvoice(
                        salesDTO.BillQuotationIds, salesDTO.InvoiceId, salesDTO.CompanyId);
                }

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        List<BillingItemDTO> AddBillItems(JToken items, BillingDTO dto)
        {
            List<BillingItemDTO> billItems = new List<BillingItemDTO>();
            bool contractMeasureStyle = dto.InvoiceType == 5 && dto.QuotationType == 16;
            foreach (var item in items)
            {
                BillingItemDTO b = new BillingItemDTO();
                b.ProductId = Convert.ToInt32(item["ProductId"]);
                b.GroupItemId = Convert.ToInt32(item["GroupItemId"]);

                b.Rate = Convert.ToDouble(item["Rate"]);
                b.Quantity = Convert.ToInt32(item["Quantity"]);
                b.TaxCategoryId = Convert.ToInt16(item["TaxCategoryId"]);

                b.IGST = Convert.ToDouble(item["IGST"]);
                b.CGST = Convert.ToDouble(item["CGST"]);
                b.SGST = Convert.ToDouble(item["SGST"]);
                b.IGSTRate = Convert.ToDouble(item["IGSTRate"]);
                b.CGSTRate = Convert.ToDouble(item["CGSTRate"]);
                b.SGSTRate = Convert.ToDouble(item["SGSTRate"]);

                if (item["Area"] != null && !String.IsNullOrEmpty(Convert.ToString(item["Area"])))
                    b.Area = Convert.ToSingle(item["Area"]);

                if (contractMeasureStyle && item["LineTotalMode"] != null && !String.IsNullOrWhiteSpace(Convert.ToString(item["LineTotalMode"])))
                    b.LineTotalMode = Convert.ToString(item["LineTotalMode"]).Trim();
                if (contractMeasureStyle)
                {
                    if (item["Days"] != null && !String.IsNullOrWhiteSpace(Convert.ToString(item["Days"])))
                        b.Days = Convert.ToDouble(item["Days"]);
                    var fromStr = item["From"] != null ? Convert.ToString(item["From"]) : null;
                    var toStr = item["To"] != null ? Convert.ToString(item["To"]) : null;
                    if (!String.IsNullOrWhiteSpace(fromStr) && DateTime.TryParse(fromStr, out var fromDt))
                        b.From = fromDt;
                    if (!String.IsNullOrWhiteSpace(toStr) && DateTime.TryParse(toStr, out var toDt))
                        b.To = toDt;
                }

                if (!contractMeasureStyle && item["Height"] != null && item["Width"] != null && (dto.InvoiceType == 9 || dto.InvoiceType == 5))
                {
                    b.Height = Convert.ToDouble(item["Height"]);
                    b.Width = Convert.ToDouble(item["Width"]);
                    b.SubTotal = (b.Quantity * b.Rate * b.Height * b.Width);
                }
                else
                {
                    b.Height = item["Height"] != null ? Convert.ToDouble(item["Height"]) : 0;
                    b.Width = item["Width"] != null ? Convert.ToDouble(item["Width"]) : 0;
                    b.SubTotal = (b.Quantity * b.Rate);
                }

                if (item["Description"] != null && !String.IsNullOrEmpty(Convert.ToString(item["Description"])))
                {
                    b.Description = b.LinItem = Convert.ToString(item["Description"]);
                }
                else if (item["Item"] != null && !String.IsNullOrEmpty(Convert.ToString(item["Item"])))
                {
                    b.Description = Convert.ToString(item["Item"]);
                }
                else if (item["Product"] != null && !String.IsNullOrEmpty(Convert.ToString(item["Product"])))
                {
                    b.Description =  Convert.ToString(item["Product"]);
                }
                b.Unit = item["Unit"] != null ? Convert.ToString(item["Unit"]).Trim() : "";
                b.DiscountPercent = Convert.ToDouble(item["DiscountPercent"]);
                b.Discount = Convert.ToDouble(item["Discount"]);

                b.Total = b.SubTotal + b.IGST + b.CGST + b.SGST;

                b.ChallanId = Convert.ToInt32(item["ChallanId"]);
                billItems.Add(b);
            }
            return billItems;
        }
        List<LostItemDTO> AddLostItem(JToken items)
        {
            List<LostItemDTO> billItems = new List<LostItemDTO>();
            foreach (var item in items)
            {
                LostItemDTO b = new LostItemDTO();
                b.ProductId = Convert.ToInt32(item["ProductId"]);


                b.Rate = Convert.ToDouble(item["Rate"]);
                b.Quantity = Convert.ToInt32(item["Quantity"]);
                b.TaxCategoryId = Convert.ToInt16(item["TaxCategoryId"]);

                b.IGST = Convert.ToDouble(item["IGST"]);
                b.CGST = Convert.ToDouble(item["CGST"]);
                b.SGST = Convert.ToDouble(item["SGST"]);
                b.IGSTRate = Convert.ToDouble(item["IGSTRate"]);
                b.CGSTRate = Convert.ToDouble(item["CGSTRate"]);
                b.SGSTRate = Convert.ToDouble(item["SGSTRate"]);
                b.SubTotal = (b.Quantity * b.Rate);
                b.Amount = (b.Quantity * b.Rate);
                b.ChallanId = Convert.ToInt32(item["ChallanId"]);



                billItems.Add(b);
            }
            return billItems;
        }

        List<SalesItemDTO> AddItems(JToken items)
        {
            List<SalesItemDTO> billItems = new List<SalesItemDTO>();
            foreach (var item in items)
            {
                SalesItemDTO b = new SalesItemDTO();
                b.ProductId = Convert.ToInt32(item["ProductId"]);
                b.Rate = Convert.ToDouble(item["PurchaseRate"]);
                b.Quantity = Convert.ToInt32(item["PurchaseQty"]);
                b.TaxCategoryId = Convert.ToInt16(item["TaxCategoryId"]);

                b.IGST = Convert.ToDouble(item["IGST"]);
                b.CGST = Convert.ToDouble(item["CGST"]);
                b.SGST = Convert.ToDouble(item["SGST"]);

                b.SubTotal = (b.Quantity * b.Rate);
                b.Total = b.SubTotal + b.IGST + b.CGST + b.SGST;


                billItems.Add(b);
            }
            return billItems;
        }

        #region Quotations
        List<QuotationItemDTO> AddQuotationItems(JToken items)
        {
            List<QuotationItemDTO> billItems = new List<QuotationItemDTO>();
            foreach (var item in items)
            {
                QuotationItemDTO b = new QuotationItemDTO();
                b.ProductId = Convert.ToInt32(item["ProductId"]);
                b.Rate = Convert.ToDouble(item["Rate"]);
                b.Quantity = Convert.ToInt32(item["Quantity"]);
                b.TaxCategoryId = Convert.ToInt16(item["TaxCategoryId"]);

                b.IGST = Convert.ToDouble(item["IGST"]);
                b.CGST = Convert.ToDouble(item["CGST"]);
                b.SGST = Convert.ToDouble(item["SGST"]);

                b.SubTotal = (b.Quantity * b.Rate);
                b.Total = b.SubTotal + b.IGST + b.CGST + b.SGST;


                billItems.Add(b);
            }
            return billItems;
        }

        [HttpPost]
        public HttpResponseMessage SaveQuotation([FromBody] QuotationDataDTO salesDTO)
        {
            try
            {

                Billing objBilling = new Billing();

                string dto = System.Web.HttpContext.Current.Request["dto"];
                System.Web.HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                JObject jsonObject = new JObject();

                if (salesDTO.QuotationType == 16)
                {
                    if (salesDTO.From.Year < 2000)
                        throw new Exception("Period From date is required");
                    if (salesDTO.To.Year < 2000)
                        throw new Exception("Period To date is required");
                    if (salesDTO.To < salesDTO.From)
                        throw new Exception("Period To must be on or after From");
                    if (salesDTO.Area <= 0)
                        throw new Exception("Area is required for contract quotation");
                    if (salesDTO.MeasureType <= 0)
                        salesDTO.MeasureType = 1;
                }
                else
                {
                    salesDTO.From = salesDTO.To = salesDTO.QuotationDate;
                }
                salesDTO.FinYearId = new LoggedInUser().FinYearId;

                salesDTO.CompanyId = new LoggedInUser().DefaultCompanyId;

                salesDTO.CreatedBy = new LoggedInUser().UserId;
                //salesDTO.QuotationType = 1; // bill

                byte partyType = salesDTO.PartyType > 0 ? salesDTO.PartyType : (byte)1;
                salesDTO.PartyType = partyType;
                if (partyType == 2)
                {
                    salesDTO.LedgerId = 0;
                    salesDTO.LedgerSiteId = 0;
                    salesDTO.UnregisteredPartyName = (salesDTO.UnregisteredPartyName ?? string.Empty).Trim();
                    salesDTO.UnregisteredPartyAddress = (salesDTO.UnregisteredPartyAddress ?? string.Empty).Trim();
                    salesDTO.UnregisteredPartyPhone = string.IsNullOrWhiteSpace(salesDTO.UnregisteredPartyPhone)
                        ? null
                        : salesDTO.UnregisteredPartyPhone.Trim();
                    if (string.IsNullOrWhiteSpace(salesDTO.UnregisteredPartyName))
                    {
                        throw new Exception("Party name is required for un-registered party");
                    }
                    if (string.IsNullOrWhiteSpace(salesDTO.UnregisteredPartyAddress))
                    {
                        throw new Exception("Party address is required for un-registered party");
                    }
                }
                else
                {
                    salesDTO.UnregisteredPartyName = null;
                    salesDTO.UnregisteredPartyAddress = null;
                    salesDTO.UnregisteredPartyPhone = null;
                    //salesDTO.GstRate = 0;
                    //salesDTO.IGST = false;
                    //salesDTO.CGST = false;
                    //salesDTO.SGST = false;
                    if (salesDTO.LedgerId <= 0)
                    {
                        throw new Exception("Please select the client");
                    }
                }
                //if (salesDTO.LedgerSiteId <= 0)
                //{
                //    throw new Exception("Please select the site");
                //}


                string headerContractLineMode = ResolveContractLineTotalMode(salesDTO);

                foreach (var item in salesDTO.BillableItems)
                {
                    if (salesDTO.QuotationType == 15)
                    {
                        item.SubTotal = (item.Quantity * item.Rate);
                        if (item.Duration <= 0)
                            item.Duration = 1;
                        item.SubTotal = item.SubTotal * item.Duration;
                    }
                    else if (salesDTO.QuotationType == 16)
                    {
                        item.Duration = 1;
                        if (item.From.Year < 2000)
                            item.From = salesDTO.From;
                        if (item.To.Year < 2000)
                            item.To = salesDTO.To;

                        string lineMode = ResolveContractLineTotalModeForQuotationLine(item, headerContractLineMode);
                        item.SubTotal = ContractQuotationLineSubTotal(lineMode, item, salesDTO.Area, salesDTO.From, salesDTO.To);
                    }
                    else
                    {
                        item.Duration = 1;
                        item.SubTotal = (item.Quantity * item.Rate);
                    }
                    item.Total = item.SubTotal + item.IGST + item.CGST + item.SGST;
                }
                salesDTO.SubTotal = salesDTO.BillableItems.Sum(o => o.SubTotal);
                if (String.IsNullOrEmpty(salesDTO.Category))
                {
                    salesDTO.Category = "quotation";
                }
                if (!String.IsNullOrEmpty(salesDTO.PoNumber))
                {
                    salesDTO.PoNumber = salesDTO.PoNumber.Trim();
                }

                if (!String.IsNullOrEmpty(salesDTO.PoNumber) && salesDTO.PoDate.Year < 2000)
                {
                    throw new Exception("PO date is required");
                }
                if (String.IsNullOrEmpty(salesDTO.PoNumber) && salesDTO.PoDate.Year > 2000)
                {
                    throw new Exception("PO number is required");
                }
                bool result = objBilling.CreateQuotation(salesDTO);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage QuotationsList([FromBody] QuotationDTO dto)
        {
            try
            {
                Billing objBilling = new Billing();
                var user = new LoggedInUser();
                dto.CompanyId = user.DefaultCompanyId;
                var list = objBilling.QuotationsList(dto);
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
        public async Task<HttpResponseMessage> QuotationByNumber([FromBody] QuotationDTO dto)
        {
            var msg = new ApiMessage();
            try
            {

                Billing objBilling = new Billing();
                var user = new LoggedInUser();
                // DateTime fromDate, toDate;
                dto.CompanyId = user.DefaultCompanyId;
                //fromDate = Utils.FormatDate(dto.From);
                //toDate = Utils.FormatDate(dto.To);
                msg.Code = ApiMessageCodes.SUCCESS;
                msg.Data = await objBilling.QuotationByNumber(dto);
                return Request.CreateResponse(HttpStatusCode.OK, msg);
            }
            catch (Exception ex)
            {
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = ex.Message;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, msg);
            }
        }
        [HttpGet]
        public async Task<ApiMessage> QuotationById(int quoteId)
        {
            var msg = new ApiMessage();
            try
            {
                Billing objBilling = new Billing();
                var user = new LoggedInUser();

                msg.Data = await objBilling.QuotationById(quoteId, user.DefaultCompanyId);
                msg.Code = ApiMessageCodes.SUCCESS;

            }
            catch (Exception ex)
            {
                msg.Message = ex.Message;
                msg.Code = ApiMessageCodes.ERROR;
            }
            return msg;
        }

        [HttpPost]
        public async Task<ApiMessage> LinkQuotationToLedger([FromBody] LinkQuotationLedgerDto dto)
        {
            var msg = new ApiMessage();
            try
            {
                if (dto == null || dto.QuotationId <= 0 || dto.LedgerId <= 0)
                {
                    msg.Code = ApiMessageCodes.ERROR;
                    msg.Message = "Quotation and ledger are required.";
                    return msg;
                }
                var user = new LoggedInUser();
                var billing = new Billing();
                var ok = await billing.LinkQuotationToLedger(dto.QuotationId, dto.LedgerId, user.DefaultCompanyId);
                if (ok)
                {
                    msg.Code = ApiMessageCodes.SUCCESS;
                }
                else
                {
                    msg.Code = ApiMessageCodes.ERROR;
                    msg.Message = "Quotation was not updated (not found or no access).";
                }
            }
            catch (Exception ex)
            {
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = ex.Message;
            }
            return msg;
        }

        [HttpPost]
        public async Task<ApiMessage> UpdatedQuotationStatus([FromBody] QuotationDTO model)
        {
            var msg = new ApiMessage();
            try
            {
                Billing objBilling = new Billing();
                var user = new LoggedInUser();
                var dto = new QuotationDTO();
                dto.QuotationId = model.QuotationId;
                dto.CompanyId = user.DefaultCompanyId;
                dto.StatusId = model.StatusId;
                dto.ModifiedBy = user.UserId;
                dto.ModifiedOn = DateTime.Now;
                msg.Data = await objBilling.UpdateQuotationStatus(dto);
                msg.Code = ApiMessageCodes.SUCCESS;

            }
            catch (Exception ex)
            {
                msg.Message = ex.Message;
                msg.Code = ApiMessageCodes.ERROR;
            }
            return msg;
        }

        [HttpPost]
        public HttpResponseMessage PrintQuotation([FromBody] QuotationDTO dto)
        {
            try
            {
                Billing billing = new Billing();

                int partyId = 0;
                int companyId = new LoggedInUser().DefaultCompanyId;
                BAL.Objects.Report objReport = new BAL.Objects.Report();

                DataSet mainDS = billing.GetQuotationItems(dto.QuotationId);
                string number = "";
                if (mainDS.Tables.Count > 0)
                {
                    if (mainDS.Tables[0].Rows.Count > 0)
                    {
                        partyId = Convert.ToInt32(mainDS.Tables[0].Rows[0]["LedgerId"]);
                        number = Convert.ToString(mainDS.Tables[0].Rows[0]["QuotationNumber"]);
                    }
                }
                DataSet headerDataSet = objReport.GetReportHeader(partyId, companyId);
                String reportPath = System.Web.HttpContext.Current.Server.MapPath("~/rpts/quotation.rdlc");
                ReportViewer rpt = new ReportViewer();
                rpt.LocalReport.ReportPath = reportPath;
                ReportDataSource rDsource = new ReportDataSource("DataSet1", mainDS.Tables[0]);
                rpt.LocalReport.DataSources.Add(rDsource);
                string fileType = dto.FileFormat == 1 ? ".xls" : ".pdf";
                string filePath = System.Web.HttpContext.Current.Server.MapPath("~/temp/");
                string fileName = number + fileType;
                filePath = filePath + number + fileType;
                //rpt.LocalReport.SubreportProcessing += delegate (object o, SubreportProcessingEventArgs e)
                //{
                //    ReportDataSource rsHeader = new ReportDataSource("DataSet1", headerDataSet.Tables[0]);
                //    e.DataSources.Add(rsHeader);
                //};
                rpt.LocalReport.Refresh();
                byte[] reportData = new byte[1];
                //if (dto.FileFormat == 1)
                //{
                //    reportData = rpt.LocalReport.Render("EXCEL");
                //}
                //else if (dto.FileFormat == 2)
                //{
                reportData = rpt.LocalReport.Render("PDF");
                // }
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.WriteAllBytes(filePath, reportData);
                return Request.CreateResponse(HttpStatusCode.OK, fileName);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        #endregion

        [HttpPost]
        public HttpResponseMessage GetSalesRegister([FromBody] PurchaseDTO dto)
        {
            try
            {
                Sales objSales = new Sales();

                return Request.CreateResponse(HttpStatusCode.OK, objSales.SalesRegister(new LoggedInUser().DefaultCompanyId));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage GetSalesItems([FromBody] SalesDTO dto)
        {
            try
            {
                Sales objSales = new Sales();
                return Request.CreateResponse(HttpStatusCode.OK, objSales.SalesItemsList(dto.SalesId));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage SalesItemsTax([FromBody] SalesDTO dto)
        {
            try
            {
                Sales objSales = new Sales();
                return Request.CreateResponse(HttpStatusCode.OK, objSales.SalesItemsTax(dto.SalesId));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpPost]
        public HttpResponseMessage PrintReceipt([FromBody] SalesDTO dto)
        {
            try
            {
                Sales sales = new Sales();

                int partyId = 0;
                int companyId = new LoggedInUser().DefaultCompanyId;
                BAL.Objects.Report objReport = new BAL.Objects.Report();

                DataSet mainDS = sales.GetReceiptRegisterPRT(dto.SalesId);
                if (mainDS.Tables.Count > 0)
                {
                    if (mainDS.Tables[0].Rows.Count > 0)
                    {
                        partyId = Convert.ToInt32(mainDS.Tables[0].Rows[0]["LedgerId"]);
                    }
                }
                DataSet headerDataSet = objReport.GetReportHeader(partyId, companyId);
                String reportPath = System.Web.HttpContext.Current.Server.MapPath("~/rpts/salesReceipt_v0.rdlc");
                ReportViewer rpt = new ReportViewer();
                rpt.LocalReport.ReportPath = reportPath;
                ReportDataSource rDsource = new ReportDataSource("DataSet1", mainDS.Tables[0]);
                rpt.LocalReport.DataSources.Add(rDsource);
                string fileType = dto.FileFormat == 1 ? ".xls" : ".pdf";
                string filePath = System.Web.HttpContext.Current.Server.MapPath("~/temp/");
                string fileName = dto.SalesId + fileType;
                filePath = filePath + fileName;
                rpt.LocalReport.SubreportProcessing += delegate (object o, SubreportProcessingEventArgs e)
                {
                    ReportDataSource rsHeader = new ReportDataSource("DataSet1", headerDataSet.Tables[0]);
                    e.DataSources.Add(rsHeader);
                };
                rpt.LocalReport.Refresh();
                byte[] reportData = new byte[1];
                if (dto.FileFormat == 1)
                {
                    reportData = rpt.LocalReport.Render("EXCEL");
                }
                else if (dto.FileFormat == 2)
                {
                    reportData = rpt.LocalReport.Render("PDF");
                }
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.WriteAllBytes(filePath, reportData);
                return Request.CreateResponse(HttpStatusCode.OK, fileName);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        static string ResolveContractLineTotalMode(QuotationDataDTO salesDTO)
        {
            if (!string.IsNullOrWhiteSpace(salesDTO.LineTotalMode))
            {
                var m = salesDTO.LineTotalMode.Trim();
                if (string.Equals(m, "area", StringComparison.OrdinalIgnoreCase))
                {
                    salesDTO.LineTotalMode = "area";
                    return "area";
                }
                salesDTO.LineTotalMode = "quantity";
                return "quantity";
            }
            if (salesDTO.QuotationType != 16)
                return "quantity";
            var configCategory = string.Equals(salesDTO.Category, "pi", StringComparison.OrdinalIgnoreCase) ? "pi" : "quotation";
            var configDal = new ConfigDAL();
            var modeCfg = configDal.GetValue(new ConfigDTO
            {
                Category = configCategory,
                SubCategory = "contract",
                Key = "contractLineTotalMode",
                CompanyId = salesDTO.CompanyId
            });
            var fromConfig = string.IsNullOrWhiteSpace(modeCfg?.Value) ? "quantity" : modeCfg.Value.Trim();
            if (!string.Equals(fromConfig, "area", StringComparison.OrdinalIgnoreCase))
                fromConfig = "quantity";
            salesDTO.LineTotalMode = fromConfig;
            return fromConfig;
        }

        /// <summary>Per-line override for contract quotation items; falls back to <paramref name="headerResolvedMode"/>.</summary>
        static string ResolveContractLineTotalModeForQuotationLine(QuotationItemDTO item, string headerResolvedMode)
        {
            if (!string.IsNullOrWhiteSpace(item.LineTotalMode))
            {
                var m = item.LineTotalMode.Trim();
                if (string.Equals(m, "area", StringComparison.OrdinalIgnoreCase))
                    return "area";
                return "quantity";
            }
            return headerResolvedMode;
        }

        /// <summary>Per-line override for contract-style billing lines; falls back to <paramref name="headerResolvedMode"/>.</summary>
        static string ResolveContractLineTotalModeForBillingLine(BillingItemDTO item, string headerResolvedMode)
        {
            if (!string.IsNullOrWhiteSpace(item.LineTotalMode))
            {
                var m = item.LineTotalMode.Trim();
                if (string.Equals(m, "area", StringComparison.OrdinalIgnoreCase))
                    return "area";
                return "quantity";
            }
            return headerResolvedMode;
        }

        /// <summary>Same contract line-total mode rules as <see cref="ResolveContractLineTotalMode(QuotationDataDTO)"/> for invoice save.</summary>
        static string ResolveContractLineTotalMode(BillingDTO dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.LineTotalMode))
            {
                var m = dto.LineTotalMode.Trim();
                if (string.Equals(m, "area", StringComparison.OrdinalIgnoreCase))
                {
                    dto.LineTotalMode = "area";
                    return "area";
                }
                dto.LineTotalMode = "quantity";
                return "quantity";
            }
            if (dto.QuotationType != 16)
                return "quantity";
            var configDal = new ConfigDAL();
            var modeCfg = configDal.GetValue(new ConfigDTO
            {
                Category = "quotation",
                SubCategory = "contract",
                Key = "contractLineTotalMode",
                CompanyId = dto.CompanyId
            });
            var fromConfig = string.IsNullOrWhiteSpace(modeCfg?.Value) ? "quantity" : modeCfg.Value.Trim();
            if (!string.Equals(fromConfig, "area", StringComparison.OrdinalIgnoreCase))
                fromConfig = "quantity";
            dto.LineTotalMode = fromConfig;
            return fromConfig;
        }

        static int InclusiveCalendarDays(DateTime from, DateTime to)
        {
            var a = from.Date;
            var b = to.Date;
            if (b < a)
                return 0;
            return (int)(b - a).TotalDays + 1;
        }

        static double ResolveContractRentDaysForQuotationLine(QuotationItemDTO item, DateTime headerFrom, DateTime headerTo)
        {
            if (item.Days > 0.001)
                return item.Days;
            if (item.From.Year > 2000 && item.To.Year > 2000)
            {
                var n = InclusiveCalendarDays(item.From, item.To);
                if (n > 0)
                    return n;
            }
            if (headerFrom.Year > 2000 && headerTo.Year > 2000)
            {
                var n = InclusiveCalendarDays(headerFrom, headerTo);
                if (n > 0)
                    return n;
            }
            return 1;
        }

        static double ResolveContractRentDaysForBillingLine(BillingItemDTO item, DateTime headerFrom, DateTime headerTo)
        {
            if (item.Days > 0.001)
                return item.Days;
            if (item.From.Year > 2000 && item.To.Year > 2000)
            {
                var n = InclusiveCalendarDays(item.From, item.To);
                if (n > 0)
                    return n;
            }
            if (headerFrom.Year > 2000 && headerTo.Year > 2000)
            {
                var n = InclusiveCalendarDays(headerFrom, headerTo);
                if (n > 0)
                    return n;
            }
            return 1;
        }

        static double ContractQuotationLineSubTotal(string mode, QuotationItemDTO item, float headerArea, DateTime headerFrom, DateTime headerTo)
        {
            if (string.Equals(mode, "area", StringComparison.OrdinalIgnoreCase))
            {
                float area = item.Area > 0 ? item.Area : headerArea;
                if (area <= 0)
                    throw new Exception("Area is required on each line (or header) for area-wise calculation");
                return item.Quantity * area * item.Rate;
            }
            var rentDays = ResolveContractRentDaysForQuotationLine(item, headerFrom, headerTo);
            return item.Quantity * item.Rate * rentDays;
        }

        static double ContractQuotationLineSubTotal(string mode, BillingItemDTO item, float headerArea, DateTime headerFrom, DateTime headerTo)
        {
            if (string.Equals(mode, "area", StringComparison.OrdinalIgnoreCase))
            {
                float area = item.Area > 0 ? item.Area : headerArea;
                if (area <= 0)
                    throw new Exception("Area is required on each line (or header) for area-wise calculation");
                return item.Quantity * area * item.Rate;
            }
            var rentDays = ResolveContractRentDaysForBillingLine(item, headerFrom, headerTo);
            return item.Quantity * item.Rate * rentDays;
        }
    }
}
