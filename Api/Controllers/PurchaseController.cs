using BAL.DTO;
using BAL.Objects;
using BAL.Services;
using FarmaAPI.Helper;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Windows.Interop;
namespace FarmaAPI.Controllers
{
    [Authorize]
    public class PurchaseController : ApiController
    {
        string tempRoot = HttpContext.Current.Server.MapPath("~/temp/");
        AzureStorageService storageService = new AzureStorageService();
        [HttpPost]
        public async Task<HttpResponseMessage> Save([FromBody] PurchaseDTO purchaseDTO)
        {
            try
            {
                //  DataCopier<WorkOrderDTO, WorkOrder> dcopier = new DataCopier<WorkOrderDTO, WorkOrder>();
                // dcopier.CopyData(dto, leder);
                //string dto = System.Web.HttpContext.Current.Request["dto"];
                //System.Web.HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                //JObject jsonObject = new JObject();

                //PurchaseDTO purchaseDTO = new PurchaseDTO();

                //   jsonObject = JObject.Parse(dto);
                // NextId n = new NextId();
                //  String nextId = n.GetNextId(BAL.Enums.NextIDTables.Invoice, new LoggedInUser().FinYearId.ToString(), new LoggedInUser().DefaultCompanyId);

                //purchaseDTO.BillNumber = Convert.ToString(jsonObject.GetValue("BillNumber"));
                //purchaseDTO.PurchaseDate = Convert.ToDateTime(jsonObject["PurchaseDate"]);
                //purchaseDTO.LedgerId = Convert.ToInt32(jsonObject.GetValue("LedgerId"));
                //purchaseDTO.PurchaseAccountId = Convert.ToInt16(jsonObject.GetValue("PurchaseAccountId"));
                //purchaseDTO.SubTotal = Convert.ToDouble(jsonObject.GetValue("SubTotal"));
                //purchaseDTO.TaxAmount = Convert.ToDouble(jsonObject.GetValue("TaxAmount"));
                //purchaseDTO.FreightTax = Convert.ToDouble(jsonObject.GetValue("FreightTax"));
                //double BreakageTax = Convert.ToDouble(jsonObject.GetValue("BreakageTax"));
                //purchaseDTO.PurchaseId = Convert.ToInt32(jsonObject.GetValue("PurchaseId"));

                //purchaseDTO.FinYearId = new LoggedInUser().FinYearId;
                ////  purchaseDTO.Freight = Convert.ToDouble(jsonObject.GetValue("Freight"));
                //purchaseDTO.DiscountAmount = Convert.ToDouble(jsonObject.GetValue("DiscountAmount"));
                //purchaseDTO.DiscountPercent = Convert.ToDouble(jsonObject.GetValue("DiscountPercent"));

                //purchaseDTO.VendorCreditNoteNumber = Convert.ToString(jsonObject.GetValue("VendorCreditNoteNumber"));
                //purchaseDTO.VendorCreditNoteDate = Convert.ToDateTime(jsonObject.GetValue("VendorCreditNoteDate"));
                //purchaseDTO.Guid = Guid.NewGuid().ToString();


                //purchaseDTO.CompanyId = new LoggedInUser().DefaultCompanyId;
                ////   purchaseDTO.VoucherNumber = nextId;
                //purchaseDTO.CreatedBy = new LoggedInUser().UserId;
                //purchaseDTO.PurchaseType = Convert.ToInt16(jsonObject.GetValue("PurchaseType"));
                //// purchaseDTO.PurchaseDate = DateTime.Today;
                Purchase objPurchase = new Purchase();
                //var items = jsonObject["Items"];
                ////  var taxInfo = jsonObject["AppliedTaxes"];
                //var taxInfo = jsonObject["Taxes"];

                //// purchaseDTO.ApplicableTaxes = new List<TaxDTO>();
                //purchaseDTO.Items = AddItems(items);
                //Billing objBill = new Billing();

                //foreach (var tax in taxInfo)
                //{
                //    TaxDTO taxDto = new TaxDTO();
                //    taxDto.TaxId = Convert.ToInt16(tax["TaxId"]);
                //    taxDto.ItemValue = 0;// Convert.ToInt16(tax["ProductId"]);
                //    // taxDto.Rate = Convert.ToDouble(tax["TaxRate"]);
                //    taxDto.Rate = Convert.ToDouble(tax["DefaultRate"]);
                //    bool applicable = Convert.ToBoolean(tax["Applicable"]);
                //    taxDto.TaxAmount = Convert.ToDouble(tax["TaxAmount"]);// (billingDTO.SubTotal + billingDTO.Freight + breakageAmount);

                //    purchaseDTO.ApplicableTaxes.Add(taxDto);
                //}

                if (purchaseDTO == null)
                {
                    throw new Exception("Input can not be empty");
                }
                if (purchaseDTO.Items == null)
                {
                    throw new Exception("Line items can not be empty");
                }
                foreach (var item in purchaseDTO.Items)
                {
                    //item.SubTotal = (item.Quantity * item.Rate);
                    item.Total = item.SubTotal + item.IGST + item.CGST + item.SGST;
                }
                purchaseDTO.Guid = Guid.NewGuid().ToString();

                var user = new LoggedInUser();
                purchaseDTO.CompanyId = user.DefaultCompanyId;
                purchaseDTO.FinYearId = user.FinYearId;
                purchaseDTO.CreatedBy = user.UserId;
         
                bool result = await objPurchase.CreateInvoice(purchaseDTO);
                if (result && !String.IsNullOrEmpty(purchaseDTO.Doc1) )
                {
                    var fileToSave = tempRoot + purchaseDTO.Doc1;
                    if (File.Exists(fileToSave))
                    {
                        await storageService.UploadFileAsync(purchaseDTO.FinYearId, user.DefaultCompanyId + "/purchase", purchaseDTO.Doc1, fileToSave);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        List<PurchaseItemDTO> AddItems(JToken items)
        {
            List<PurchaseItemDTO> billItems = new List<PurchaseItemDTO>();
            foreach (var item in items)
            {
                PurchaseItemDTO b = new PurchaseItemDTO();
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
        public HttpResponseMessage GetPurhaseRegister([FromBody] PurchaseFilterDTO dto)
        {
            try
            {
                Purchase objPurchase = new Purchase();
                var user = new LoggedInUser();
                dto.FinYearId = user.FinYearId;
                dto.CompanyId = user.DefaultCompanyId;
                return Request.CreateResponse(HttpStatusCode.OK, objPurchase.PurchaseRegister(dto));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet()]
        public async Task<ApiMessage> GetById(int purchaseId)
        {
            var res = new ApiMessage();
            try
            {
                Purchase objPurchase = new Purchase();
                var user = new LoggedInUser();
                res.Data = await objPurchase.ById(purchaseId, user.DefaultCompanyId);
                res.Code = ApiMessageCodes.SUCCESS;

                return res;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public HttpResponseMessage GetPurchaseItems([FromBody] PurchaseDTO dto)
        {
            try
            {
                Purchase objPurchase = new Purchase();
                return Request.CreateResponse(HttpStatusCode.OK, objPurchase.PurchaseItemsList(dto.PurchaseId));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage PurchaseItemsTax([FromBody] PurchaseDTO dto)
        {
            try
            {
                Purchase purchase = new Purchase();
                return Request.CreateResponse(HttpStatusCode.OK, purchase.PurchaseItemsTax(dto.PurchaseId));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> UnpaidBills([FromBody] PurchaseDTO dto)
        {
            var msg = new ApiMessage();
            try
            {

                var user = new LoggedInUser();
                dto.CompanyId = user.DefaultCompanyId;
                Purchase purchase = new Purchase();
                msg.Data = await purchase.GetUnpaidBills(dto.LedgerId, dto.CompanyId);
                msg.Code = ApiMessageCodes.SUCCESS;
                return Ok(msg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public HttpResponseMessage PrintReceipt([FromBody] PurchaseDTO dto)
        {
            try
            {
                Purchase purchase = new Purchase();

                int partyId = 0;
                int companyId = new LoggedInUser().DefaultCompanyId;
                BAL.Objects.Report objReport = new BAL.Objects.Report();

                DataSet mainDS = purchase.GetReceiptRegisterPRT(dto.PurchaseId);
                if (mainDS.Tables.Count > 0)
                {
                    if (mainDS.Tables[0].Rows.Count > 0)
                    {
                        partyId = Convert.ToInt32(mainDS.Tables[0].Rows[0]["LedgerId"]);
                    }
                }
                DataSet headerDataSet = objReport.GetReportHeader(partyId, companyId);
                String reportPath = System.Web.HttpContext.Current.Server.MapPath("~/rpts/purchaseReceipt.rdlc");
                ReportViewer rpt = new ReportViewer();
                rpt.LocalReport.ReportPath = reportPath;
                ReportDataSource rDsource = new ReportDataSource("DataSet1", mainDS.Tables[0]);
                rpt.LocalReport.DataSources.Add(rDsource);
                string fileType = dto.FileFormat == 1 ? ".xls" : ".pdf";
                string filePath = System.Web.HttpContext.Current.Server.MapPath("~/temp/");
                string fileName = dto.PurchaseId + fileType;
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

        [HttpPost]
        public async Task<IHttpActionResult> Delete([FromBody] PurchaseDTO dto)
        {
            var msg = new ApiMessage();
            try
            {

                var user = new LoggedInUser();
                dto.CompanyId = user.DefaultCompanyId;
                dto.ModifiedOn = DateTime.Now;
                dto.ModifiedBy = user.UserId;
                dto.StatusId = 2;
                Purchase purchase = new Purchase();
                msg.Data = await purchase.UpdateStatus(dto);
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
