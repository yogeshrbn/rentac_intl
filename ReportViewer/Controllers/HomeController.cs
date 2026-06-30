
using Antlr.Runtime.Misc;
using BAL.Common;
using BAL.DAL;
using BAL.DTO;
using BAL.Enums;
using BAL.Objects;
using BAL.Services;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using NLog;
using NReco.PdfGenerator;
using OfficeOpenXml;
using OfficeOpenXml.Style;
//using PuppeteerSharp;
//using PuppeteerSharp.Media;
using QRCoder;
using Rentac.Shared.Filters;
using Rentac.Shared.Logger;

using ReportViewer.Helpers;
using ReportViewer.Models;
using Spire.Additions.Chrome;
using Spire.Additions.Html;
using Spire.Additions.Qt;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.Xml;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Windows.Documents;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using WebGrease.Activities;
using HtmlConverter = Spire.Additions.Html.HtmlConverter;
using RouteAttribute = System.Web.Mvc.RouteAttribute;

namespace ReportViewer.Controllers
{

    [CustomActionFilter]
    [LogActionFilter]
    [System.Web.Mvc.Authorize]
    [AllowJsonGet]
    public class HomeController : BaseMVCController
    {
        private string docPath = ConfigurationManager.AppSettings["docsPath"];

        //private static readonly ILogger logger = RentacLogger.logger;

        public NLog.ILogger logger = LogManager.GetCurrentClassLogger();

        AzureStorageService azService = new AzureStorageService();


        public ActionResult Index()
        {
            //ViewBag.FileName = Server.MapPath("Reports") + @"\ewayBill.xslt";

            //string medalsXML = System.IO.File.ReadAllText(Server.MapPath("Data/data.xml"));
            //ViewBag.Medals = medalsXML;


            return View();
        }

        /// <summary>Loads GRN damage component lines when billing config <c>printDamageComponentsDetails</c> is enabled.</summary>
        private async Task SetBreakageDamageDetailsForRentBillAsync(DataSet ds, int companyId, int ledgerId, int invoiceId, int ledgerSiteId, IEnumerable<ConfigDTO> configData)
        {
            ViewBag.BreakageDamageDetails = null;
            var cfg = BillingConfigViewHelper.AsEnumerable(configData);
            if (!BillingConfigViewHelper.GetBool(cfg, "printDamageComponentsDetails", false))
                return;
            try
            {
                var t0 = ds.Tables[0];
                //if (t0 == null || t0.Rows.Count == 0)
                //    return;
                //var billFrom = Convert.ToDateTime(t0.Compute("Min(From)", string.Empty));
                //var billTo = Convert.ToDateTime(t0.Compute("Max(To)", string.Empty));
                var r0 = t0.Rows[0];
                //if (!r0.Table.Columns.Contains("FinYearId") || r0["FinYearId"] == DBNull.Value)
                //    return;
                //int finYearId = Convert.ToInt32(r0["FinYearId"]);
                var dal = new Billing();
                var list = (await dal.GetBreakageDamageDetailsForBill(invoiceId, companyId))?.ToList();
                ViewBag.BreakageDamageDetails = list;
            }
            catch (Exception ex)
            {
                RentacLogger.logger.Error(ex, ex.Message);
                ViewBag.BreakageDamageDetails = null;
            }
        }

        [Route("Home/PreviewRentBill/{encInvoiceId}")]
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ActionResult> PreviewRentBill(string encInvoiceId)
        {
            try
            {


                string file = Encoding.UTF8.GetString(Convert.FromBase64String(encInvoiceId));
                var dec = Security.Decrypt(file);
                var billing = new Billing();
                int invoiceId = Convert.ToInt32(dec);
                (DataSet ds, int companyId) = await SetupRentBillViewBag(invoiceId, billing);
                //var ds = billing.PrintBill(invoiceId);

                //int companyId = 0, ledgerId = 0, ledgerSiteId;
                //var compLogo = Convert.ToString(ds.Tables[0].Rows[0]["CompanyLogo"]);
                //companyId = Convert.ToInt32(ds.Tables[0].Rows[0]["CompanyId"]);
                //ledgerId = Convert.ToInt32(ds.Tables[0].Rows[0]["LedgerId"]);
                //ledgerSiteId = Convert.ToInt32(ds.Tables[0].Rows[0]["LedgerSiteId"]);

                //var config = new Config();
                //var printConfigs = config.GetConfig(companyId, "general", "print");
                //var printSignature = true;
                //var printQrCode = true;
                //var companyDetails = new Company(companyId).GetDetails();


                //ds.Tables[0].Columns.Add("QrCode");
                //if (printConfigs != null && printConfigs.Count() > 0)
                //{
                //    var c = printConfigs.Where(o => o.Key.ToLower() == "signature").FirstOrDefault();
                //    if (c != null && !String.IsNullOrEmpty(c.Value))
                //    {
                //        printSignature = c.Value.Contains("bills");
                //    }
                //    c = printConfigs.Where(o => o.Key.ToLower() == "qrcode").FirstOrDefault();
                //    if (c != null && !String.IsNullOrEmpty(c.Value))
                //    {
                //        printQrCode = c.Value.Contains("bills");
                //    }
                //}

                //if (!String.IsNullOrEmpty(compLogo))
                //{
                //    ds.Tables[0].Rows[0]["CompanyLogo"] = docPath + "/comp/" + compLogo;
                //}
                //var signaure = Convert.ToString(ds.Tables[0].Rows[0]["Signature"]);
                //if (!String.IsNullOrEmpty(signaure) && printSignature)
                //{
                //    ds.Tables[0].Rows[0]["Signature"] = docPath + "/comp/" + signaure;
                //}
                //if (!String.IsNullOrEmpty(companyDetails.QrCode) && printQrCode)
                //{
                //    ds.Tables[0].Rows[0]["QrCode"] = docPath + "/comp/" + companyDetails.QrCode;
                //}
                //if (ds.Tables[0].Rows[0]["IRN"] != DBNull.Value)
                //{
                //    var qrCodeStr = Convert.ToString(ds.Tables[0].Rows[0]["IRN"]);
                //    var codeGen = new QRCodeGenerator();

                //    var qrCodeData = codeGen.CreateQrCode(Encoding.UTF8.GetBytes(qrCodeStr), QRCodeGenerator.ECCLevel.Q);
                //    using (Base64QRCode qrCode = new Base64QRCode(qrCodeData))
                //    {
                //        // qrCode.GetGraphic(50);
                //        Base64QRCode qrCodeBase64 = new Base64QRCode(qrCodeData);
                //        string qrCodeImageAsBase64 = qrCodeBase64.GetGraphic(30);
                //        ds.Tables[0].Rows[0]["SignedQrCode"] = "data:image/png;base64," + qrCodeImageAsBase64;// Convert.ToBase64String(qrCodeImage);
                //    }
                //}

                //ViewBag.BillData = ds;

                //var bal = (from d in ds.Tables[0].AsEnumerable()
                //           group d by new { ProductId = d["ProductId"] } into g
                //           select
                //                Convert.ToString(g.Last()["Item"]) + ":" +
                //                g.Last()["CB"]

                //                    ).ToList();
                //ViewBag.Balance = String.Join(",", bal);

                //// var config = new Config();
                //var configData = config.GetBillingConfig(companyId);
                //ViewBag.ConfigData = configData ?? new List<ConfigDTO>();
                //bool printBankDetails = false;
                //ViewBag.PrintExcessQty = false;
                //ViewBag.ShowChallans = true;
                //bool hideZeroAmountItem = false;
                //bool printBalanceMaterial = false;
                //if (configData != null)
                //{
                //    var printBank = configData.Where(o => o.Key == "printBankDetails").FirstOrDefault();
                //    if (printBank != null && !String.IsNullOrEmpty(printBank.Value))
                //    {
                //        printBankDetails = Convert.ToBoolean(printBank.Value);
                //    }
                //    var hideZero = configData.Where(o => o.Key == "hideZeroAmountItem").FirstOrDefault();
                //    if (hideZero != null && !String.IsNullOrEmpty(hideZero.Value))
                //    {
                //        hideZeroAmountItem = Convert.ToBoolean(hideZero.Value);
                //    }
                //    var printBalanceMaterialConfig = configData.Where(o => o.Key == "printBalanceMaterial").FirstOrDefault();
                //    if (printBalanceMaterialConfig != null && !String.IsNullOrEmpty(printBalanceMaterialConfig.Value))
                //    {
                //        printBalanceMaterial = Convert.ToBoolean(printBalanceMaterialConfig.Value);
                //    }
                //    var showExcessQty = configData.Where(o => o.Key == "showExcessQty").FirstOrDefault();
                //    if (showExcessQty != null && !String.IsNullOrEmpty(showExcessQty.Value))
                //    {

                //        ViewBag.PrintExcessQty = showExcessQty.Value == "1";
                //    }
                //    var showChallans = configData.Where(o => o.Key == "showChallans").FirstOrDefault();
                //    if (showChallans != null && !String.IsNullOrEmpty(showChallans.Value))
                //    {
                //        ViewBag.ShowChallans = showChallans.Value == "1";
                //    }
                //}
                //if (!printBalanceMaterial && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["printBalanceMaterial"] != DBNull.Value)
                //{
                //    printBalanceMaterial = Convert.ToInt16(ds.Tables[0].Rows[0]["printBalanceMaterial"]) == 1;
                //}

                //var uniqueItems = (from d in ds.Tables[0].AsEnumerable()
                //                   group d by d["ProductId"] into g
                //                   select g.Key).ToList();

                //var ledger = new Ledger();
                //ViewBag.LastBill = ledger.GetLastBill(ledgerId, ledgerSiteId, 0, invoiceId);

                //ViewBag.PrintBankDetails = printBankDetails;
                //ViewBag.PrintQrCode = printQrCode;
                //ViewBag.PrintBalanceMaterial = printBalanceMaterial;

                //ViewBag.hideZeroAmountItem = hideZeroAmountItem;
                //ViewBag.CompanyInfo = companyDetails;
                //ViewBag.UniqueItems = uniqueItems;

                //var challansTable = ds.Tables[5];
                //if (challansTable != null)
                //{
                //    var ch = (from r in challansTable.AsEnumerable()
                //              select Convert.ToString(r["ChallanNumber"]) + " : " + Utils.FormatDate(Convert.ToDateTime(r["ChallanDate"]))).ToList();
                //    ViewBag.ChallansWithDate = String.Join(" | ", ch);
                //}

                //await SetBreakageDamageDetailsForRentBillAsync(ds, companyId, ledgerId, invoiceId, ledgerSiteId, configData ?? new List<ConfigDTO>());

                var templateService = new TemplateService();
                var template = await templateService.GetDefaultPdfTemplate(companyId, TemplateGroups.RENTBILLS, ConfigCategoryNames.RENTBILLS, ConfigSubCategoryNames.TEMPLATES);
                string viewName = "PreviewRentBill";
                if (template != null)
                {
                    if (!String.IsNullOrEmpty(template.FileName))
                    {
                        viewName = template.FileName;
                    }
                    // throw new Exception("Report template could not found.");
                }

                return View(viewName);
            }
            catch (Exception ex)
            {

                RentacLogger.logger.Error(ex, ex.Message);
                return null;
            }
        }
        //static BrowserFetcher browserFetcher;

        [Route("Home/PrintRentBill/{encInvoiceId}")]
        public async Task<ActionResult> PrintRentBill(string encInvoiceId)
        {
            try
            {
                string file = Encoding.UTF8.GetString(Convert.FromBase64String(encInvoiceId));
                var dec = Security.Decrypt(file);
                var billing = new Billing();
                int invoiceId = Convert.ToInt32(dec);
                return await getRentBillFileContent(invoiceId);


            }
            catch (Exception ex)
            {
                RentacLogger.logger.Error(ex, ex.Message);
                return null;
            }
        }

        /// <summary>Bill list GST PDF — POST JSON body <c>{"Payload":"&lt;btoa(encrypt(json))&gt;"}</c> to avoid maxUrlLength limits.</summary>
        [System.Web.Mvc.HttpPost]
        [Route("Home/PrintBillListGstReport")]
        public ActionResult PrintBillListGstReport()
        {
            string encPayload = null;
            try
            {
                string body;
                using (var reader = new StreamReader(Request.InputStream, Encoding.UTF8, true, 1024, true))
                    body = reader.ReadToEnd();
                if (!string.IsNullOrWhiteSpace(body))
                {
                    var wrap = JsonConvert.DeserializeObject<BillListGstPrintRequest>(body);
                    encPayload = wrap?.Payload;
                }
            }
            catch (Exception exRead)
            {
                RentacLogger.logger.Error(exRead, "PrintBillListGstReport: read or parse body");
                return new HttpStatusCodeResult(400, "Invalid request body");
            }

            try
            {
                if (string.IsNullOrEmpty(encPayload))
                    return new HttpStatusCodeResult(400, "Missing Payload");
                string inner = Encoding.UTF8.GetString(Convert.FromBase64String(encPayload));
                var dec = Security.Decrypt(inner);
                var filter = JsonConvert.DeserializeObject<BillListGstReportFilterDto>(dec);
                if (filter == null)
                    return null;

                var gb = new CultureInfo("en-US");
                if (!DateTime.TryParse(filter.From, gb, DateTimeStyles.None, out DateTime fromDt)
                    && !DateTime.TryParse(filter.From, CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDt))
                {
                    RentacLogger.logger.Error("PrintBillListGstReport: invalid From date");
                    return null;
                }
                if (!DateTime.TryParse(filter.To, gb, DateTimeStyles.None, out DateTime toDt)
                    && !DateTime.TryParse(filter.To, CultureInfo.InvariantCulture, DateTimeStyles.None, out toDt))
                {
                    RentacLogger.logger.Error("PrintBillListGstReport: invalid To date");
                    return null;
                }

                var user = new LoggedInUser();
                var billing = new Billing();
                var list = billing.GetBilList(fromDt.ToShortDateString(), toDt.ToShortDateString(), user.DefaultCompanyId,
                    filter.LedgerId, filter.LedgerSiteId, filter.StatusId, filter.InvoiceType) ?? new List<BillingDTO>();

                if (!string.IsNullOrWhiteSpace(filter.InvoiceNumber))
                {
                    var invNo = filter.InvoiceNumber.Trim();
                    list = list.Where(x => x.InvoiceNumber != null && x.InvoiceNumber.IndexOf(invNo, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                }

                list = list.OrderBy(x => x.InvoiceDate).ThenBy(x => x.InvoiceNumber).ToList();

                string companyName = "";
                try
                {
                    var cd = new Company(user.DefaultCompanyId).GetDetails();
                    if (cd != null)
                        companyName = cd.Name;
                }
                catch (Exception exComp)
                {
                    RentacLogger.logger.Error(exComp, "PrintBillListGstReport: company name");
                }

                var vm = BuildBillListGstReportViewModel(list, companyName, fromDt, toDt);
                string strHtml = RenderRazorViewToString(this, "PrintBillListGstReport", vm);
                var con = new HtmlToPdfConverter();

                con.CustomWkHtmlArgs = "--print-media-type --enable-smart-shrinking --dpi 300";
                con.Margins.Top = 4;
                con.Margins.Bottom = 4;
                con.Margins.Left = 5;
                con.Margins.Right = 5;
                con.Orientation = PageOrientation.Landscape;
                con.Size = NReco.PdfGenerator.PageSize.A4;

                var fileBytes = con.GeneratePdf(strHtml);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "BillList-GST.pdf");
            }
            catch (Exception ex)
            {
                RentacLogger.logger.Error(ex, ex.Message);
                return null;
            }
        }

        private static BillListGstReportViewModel BuildBillListGstReportViewModel(IList<BillingDTO> bills, string companyName, DateTime fromDt, DateTime toDt)
        {
            var culture = new CultureInfo("en-IN");
            var rows = new List<BillListGstReportRow>();
            double sumInv = 0, sumTaxable = 0, sumTxCgst = 0, sumTxSgst = 0, sumTxIgst = 0;
            double sumTbCgst = 0, sumTbSgst = 0, sumTbIgst = 0;

            foreach (var b in bills)
            {

                sumInv += b.Total;
                double cgst = b.CGST, sgst = b.SGST, igst = b.IGST;
                double taxable = 0;
                double totalGST = b.CGST + b.SGST + b.IGST;
                if (totalGST > 0)
                {
                    taxable = b.SubTotal;
                }
                if (b.FreightTax > 0)
                {
                    taxable += b.Freight;
                }
                if (b.ChargesTax > 0)
                {
                    taxable += b.OtherChargeAmount;
                }

                if (igst > 0 && b.FreightTax > 0)
                {
                    igst += b.FreightTax;
                }
                else if (sgst > 0 && b.FreightTax > 0)
                {
                    sgst += b.FreightTax / 2;
                    cgst += b.FreightTax / 2;
                }

                if (igst > 0 && b.ChargesTax > 0)
                {
                    igst += b.ChargesTax;
                }
                else if (sgst > 0 && b.FreightTax > 0)
                {
                    sgst += b.ChargesTax / 2;
                    cgst += b.ChargesTax / 2;
                }

                sumTaxable += taxable;
                if (igst > 0.001)
                {
                    sumTbIgst += taxable;
                    sumTxIgst += igst;
                }
                else
                {
                    sumTbCgst += taxable;
                    sumTbSgst += taxable;
                    sumTxCgst += cgst;
                    sumTxSgst += sgst;
                }
                rows.Add(BuildBillListGstReportRow(b, culture));
            }

            string F(double v) => v.ToString("N2", culture);
            var totals = new BillListGstReportRow
            {
                Client = "Total",
                InvoiceAmount = F(sumInv),
                TaxableAmount = F(sumTaxable),
                TaxableCgst = sumTbCgst > 0.0001 ? F(sumTbCgst) : "",
                TaxCgst = sumTxCgst > 0.0001 ? F(sumTxCgst) : "",
                TaxableSgst = sumTbSgst > 0.0001 ? F(sumTbSgst) : "",
                TaxSgst = sumTxSgst > 0.0001 ? F(sumTxSgst) : "",
                TaxableIgst = sumTbIgst > 0.0001 ? F(sumTbIgst) : "",
                TaxIgst = sumTxIgst > 0.0001 ? F(sumTxIgst) : ""
            };

            return new BillListGstReportViewModel
            {
                CompanyName = companyName ?? "",
                PeriodLabel = fromDt.ToString("dd-MMM-yy", CultureInfo.InvariantCulture) + " to " + toDt.ToString("dd-MMM-yy", CultureInfo.InvariantCulture),
                Rows = rows,
                Totals = totals
            };
        }

        private static double GetTaxableBase(BillingDTO b)
        {
            double cgst = b.CGST, sgst = b.SGST, igst = b.IGST;
            double taxable = b.SubTotal;
            if (taxable <= 0.0001 && (cgst + sgst + igst) > 0.0001)
                taxable = Math.Max(0, b.Total - cgst - sgst - igst);
            return taxable;
        }

        private static BillListGstReportRow BuildBillListGstReportRow(BillingDTO b, CultureInfo culture)
        {
            double cgst = b.CGST, sgst = b.SGST, igst = b.IGST;
            double taxable = GetTaxableBase(b);
            string fmtv(double v) => v.ToString("N2", culture);
            var clientPart = (b.Client ?? "").Trim();
            var sitePart = !string.IsNullOrWhiteSpace(b.Site) ? b.Site.Trim() : (b.SiteAddress ?? "").Trim();
            var clientDisplay = string.IsNullOrEmpty(sitePart)
                ? clientPart
                : (string.IsNullOrEmpty(clientPart) ? sitePart : clientPart + " — " + sitePart);
            var row = new BillListGstReportRow
            {
                BillNo = b.InvoiceNumber ?? "",
                BillDate = b.InvoiceDate.ToString("dd-MMM-yy", CultureInfo.InvariantCulture),
                Client = clientDisplay.ToUpperInvariant(),
                InvoiceAmount = fmtv(b.Total),
                TaxableAmount = fmtv(taxable)
            };
            if (igst > 0.001)
            {
                row.TaxableIgst = fmtv(taxable);
                row.TaxIgst = fmtv(igst);
            }
            else
            {
                row.TaxableCgst = fmtv(taxable);
                row.TaxCgst = fmtv(cgst);
                row.TaxableSgst = fmtv(taxable);
                row.TaxSgst = fmtv(sgst);
            }
            return row;
        }

        public async Task<FileContentResult> getRentBillFileContent(int invoiceId)
        {
            try
            {

                var billing = new Billing();
                //int invoiceId = Convert.ToInt32(dec);
                (DataSet ds, int companyId) = await SetupRentBillViewBag(invoiceId, billing);

                var templateService = new TemplateService();
                var template = await templateService.GetDefaultPdfTemplate(companyId, TemplateGroups.RENTBILLS, ConfigCategoryNames.RENTBILLS, ConfigSubCategoryNames.TEMPLATES);
                string viewName = "PreviewRentBill";
                if (template != null)
                {
                    if (!String.IsNullOrEmpty(template.FileName))
                    {
                        viewName = template.FileName;
                        ViewBag.Style = Convert.ToString(template.Style);
                    }
                    // throw new Exception("Report template could not found.");
                }

                if (Convert.ToInt16(ds.Tables[0].Rows[0]["IsCashBill"]) == 1)
                {
                    viewName = "PreviewRentBill-cash";
                }
                string strHtml = RenderRazorViewToString(this, viewName);



                //using (HttpClient client = new HttpClient())
                //{
                var _url = Request.Url.ToString();
                _url = _url.Replace("PrintRentBill", "PreviewRentBill");

                // var strHtml = await client.GetStringAsync(_url);
                HtmlToPdfConverter con = new HtmlToPdfConverter();

                //con.CustomWkHtmlArgs = "--print-media-type " +
                //              "--disable-smart-shrinking " +
                //              "--page-size A4 " +
                //              "--margin-top 5mm " +
                //              "--margin-right 10mm " +
                //              "--margin-bottom 5mm " +
                //              "--margin-left 10mm " +
                //              "--encoding UTF-8 " +
                //              "--no-background";

                // Ensure table headers repeat on each page
                //strHtml = System.IO.File.ReadAllText(Server.MapPath("/temp/") + "/bill.txt");
                // con.CustomWkHtmlArgs += " --header-spacing 5"; // Adjust spacing
                // Or try these specific arguments:
                con.CustomWkHtmlArgs = "--print-media-type  --javascript-delay 500 --enable-smart-shrinking --dpi 300";
                con.Margins.Top = 5;
                con.Margins.Bottom = 5;
                if (viewName.ToLower() == "previewrentbill-template4")
                {

                    con.PageHeaderHtml = @"
                <div style='text-align:right; font-size:12pt;'>
                    Page <span class='page'></span> of <span class='topage'></span>
                </div>";

                    con.PageFooterHtml = @"
                <div style='text-align:right; font-size:12pt;'>
                    Page <span class='page'></span> of <span class='topage'></span>
                </div>";
                }
                var fileBytes = con.GeneratePdf(strHtml);
                string dwnFileName = "report.pdf";
                // string file = Encoding.UTF8.GetString(Convert.FromBase64String(encInvoiceId));
                //var dec = Security.Decrypt(file);
                //var billing = new Billing();
                var user = new LoggedInUser();
                var dsbills = await billing.GetBillsByIds(invoiceId.ToString(), user.DefaultCompanyId);


                if (ds != null)
                {
                    var inv = dsbills.First();
                    //var companyId = Convert.ToString(ds.Tables[0].Rows[0]["CompanyId"]);
                    //var invNumber = Convert.ToString(ds.Tables[0].Rows[0]["InvoiceNumber"]);
                    //dwnFileName = ds.Tables[0].Rows[0]["InvoiceNumber"] + "-" +
                    //    ds.Tables[0].Rows[0]["LedgerId"] + ".pdf";
                    var filePath = Server.MapPath("/temp/") + inv.InvoiceId.ToString() + "-bill.pdf";
                    using (var stream = System.IO.File.Create(filePath))
                    {

                        stream.Write(fileBytes, 0, fileBytes.Length);
                        //  ConvertHtmlStringToPdfWithChrome(strHtml, stream);
                        //using (var ms = new MemoryStream())
                        //{
                        //    stream.CopyTo(ms);
                        //    fileBytes = ms.ToArray();
                        //}
                    }

                    await azService.UploadFileAsync(user.FinYearId, user.DefaultCompanyId + "/bills", inv.FileName + ".pdf", filePath);


                }
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);


            }
            catch (Exception ex)
            {
                RentacLogger.logger.Error(ex, ex.Message);
                return null;
            }

        }

        private async Task<(DataSet ds, int companyId)> SetupRentBillViewBag(int invoiceId, Billing billing)
        {
            var ds = billing.PrintBill(invoiceId);

            int companyId = 0, ledgerId = 0, ledgerSiteId;
            var compLogo = Convert.ToString(ds.Tables[0].Rows[0]["CompanyLogo"]);
            companyId = Convert.ToInt32(ds.Tables[0].Rows[0]["CompanyId"]);
            ledgerId = Convert.ToInt32(ds.Tables[0].Rows[0]["LedgerId"]);
            ledgerSiteId = Convert.ToInt32(ds.Tables[0].Rows[0]["LedgerSiteId"]);

            var config = new Config();
            var printConfigs = config.GetConfig(companyId, "general", "print");
            var printSignature = true;
            var printQrCode = true;
            var companyDetails = new Company(companyId).GetDetails();
            DataSet dsExcess = billing.getAdjustedItemsToPrintOnBill(invoiceId, companyId);
            ViewBag.ExcessQty = null;
            if (dsExcess != null)
            {
                ViewBag.ExcessQty = dsExcess.Tables[0];
            }
            ds.Tables[0].Columns.Add("QrCode");
            if (printConfigs != null && printConfigs.Count() > 0)
            {
                var c = printConfigs.Where(o => o.Key.ToLower() == "signature").FirstOrDefault();
                if (c != null && !String.IsNullOrEmpty(c.Value))
                {
                    printSignature = c.Value.Contains("bills");
                }
                c = printConfigs.Where(o => o.Key.ToLower() == "qrcode").FirstOrDefault();
                if (c != null && !String.IsNullOrEmpty(c.Value))
                {
                    printQrCode = c.Value.Contains("bills");
                }
            }

            if (!String.IsNullOrEmpty(compLogo))
            {
                ds.Tables[0].Rows[0]["CompanyLogo"] = docPath + "/comp/" + compLogo;
            }
            var signaure = Convert.ToString(ds.Tables[0].Rows[0]["Signature"]);
            if (!String.IsNullOrEmpty(signaure) && printSignature)
            {
                ds.Tables[0].Rows[0]["Signature"] = docPath + "/comp/" + signaure;
            }
            if (!String.IsNullOrEmpty(companyDetails.QrCode) && printQrCode)
            {
                ds.Tables[0].Rows[0]["QrCode"] = docPath + "/comp/" + companyDetails.QrCode;
            }
            if (ds.Tables[0].Rows[0]["IRN"] != DBNull.Value)
            {
                var qrCodeStr = Convert.ToString(ds.Tables[0].Rows[0]["IRN"]);
                var codeGen = new QRCodeGenerator();

                var qrCodeData = codeGen.CreateQrCode(Encoding.UTF8.GetBytes(qrCodeStr), QRCodeGenerator.ECCLevel.Q);
                using (Base64QRCode qrCode = new Base64QRCode(qrCodeData))
                {
                    // qrCode.GetGraphic(50);
                    Base64QRCode qrCodeBase64 = new Base64QRCode(qrCodeData);
                    string qrCodeImageAsBase64 = qrCodeBase64.GetGraphic(30);
                    ds.Tables[0].Rows[0]["SignedQrCode"] = "data:image/png;base64," + qrCodeImageAsBase64;// Convert.ToBase64String(qrCodeImage);
                }
            }

            ViewBag.BillData = ds;

            var bal = (from d in ds.Tables[0].AsEnumerable()
                       group d by new { ProductId = d["ProductId"] } into g
                       select
                            Convert.ToString(g.Last()["Item"]) + ":" +
                            g.Last()["CB"]

                                ).ToList();
            ViewBag.Balance = String.Join(",", bal);
            //var BalanceTable = (from d in ds.Tables[0].AsEnumerable()
            //                    where Convert.ToInt32(d["CB"]) > 0
            //                    group d by new { ProductId = d["ProductId"] } into g
            //                    select new ReportDTO
            //                    {
            //                        Product = Convert.ToString(g.Last()["Item"]),
            //                        Quantity = Convert.ToInt32(g.Last()["CB"])
            //                    }
            //                    ).ToList();
            var BalanceTable = (from d in ds.Tables[0].AsEnumerable()

                                group d by new { ProductId = d["ProductId"] } into g
                                let maxDateRow = g.OrderByDescending(x => Convert.ToDateTime(x["To"])).FirstOrDefault()
                                select new ReportDTO
                                {
                                    Product = Convert.ToString(maxDateRow["Item"]),
                                    Quantity = Convert.ToInt32(maxDateRow["CB"])
                                }
                ).ToList().Where(o => o.Quantity > 0);

            ViewBag.TotalBalanceQty = 0;
            if (BalanceTable != null)
            {
                ViewBag.TotalBalanceQty = BalanceTable.Sum(o => o.Quantity);
            }
            ViewBag.BalanceTable = BalanceTable;
            // var config = new Config();
            var configData = config.GetBillingConfig(companyId);
            ViewBag.ConfigData = configData ?? new List<ConfigDTO>();
            bool printBankDetails = false;
            ViewBag.PrintExcessQty = false;
            ViewBag.ShowChallans = true;
            bool hideZeroAmountItem = false;
            bool printBalanceMaterial = false;
            if (configData != null)
            {
                var printBank = configData.Where(o => o.Key == "printBankDetails").FirstOrDefault();
                if (printBank != null && !String.IsNullOrEmpty(printBank.Value))
                {
                    printBankDetails = Convert.ToBoolean(printBank.Value);
                }
                var hideZero = configData.Where(o => o.Key == "hideZeroAmountItem").FirstOrDefault();
                if (hideZero != null && !String.IsNullOrEmpty(hideZero.Value))
                {
                    hideZeroAmountItem = Convert.ToBoolean(hideZero.Value);
                }
                var printBalanceMaterialConfig = configData.Where(o => o.Key == "printBalanceMaterial").FirstOrDefault();
                if (printBalanceMaterialConfig != null && !String.IsNullOrEmpty(printBalanceMaterialConfig.Value))
                {
                    printBalanceMaterial = Convert.ToBoolean(printBalanceMaterialConfig.Value);
                }
                var showExcessQty = configData.Where(o => o.Key == "showExcessQty").FirstOrDefault();
                if (showExcessQty != null && !String.IsNullOrEmpty(showExcessQty.Value))
                {

                    ViewBag.PrintExcessQty = showExcessQty.Value == "1";
                }
                var showChallans = configData.Where(o => o.Key == "showChallans").FirstOrDefault();
                if (showChallans != null && !String.IsNullOrEmpty(showChallans.Value))
                {
                    ViewBag.ShowChallans = showChallans.Value == "1";
                }
            }
            if (!printBalanceMaterial && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["printBalanceMaterial"] != DBNull.Value)
            {
                printBalanceMaterial = Convert.ToInt16(ds.Tables[0].Rows[0]["printBalanceMaterial"]) == 1;
            }

            var uniqueItems = (from d in ds.Tables[0].AsEnumerable()
                               group d by d["ProductId"] into g
                               select g.Key).ToList();

            var ledger = new Ledger();
            ViewBag.LastBill = ledger.GetLastBill(ledgerId, ledgerSiteId, 0, invoiceId);

            ViewBag.PrintBankDetails = printBankDetails;
            ViewBag.PrintQrCode = printQrCode;
            ViewBag.PrintBalanceMaterial = printBalanceMaterial;

            ViewBag.hideZeroAmountItem = hideZeroAmountItem;
            ViewBag.CompanyInfo = companyDetails;
            ViewBag.UniqueItems = uniqueItems;

            var challansTable = ds.Tables[5];
            if (challansTable != null)
            {
                var ch = (from r in challansTable.AsEnumerable()
                          select Convert.ToString(r["ChallanNumber"]) + " : " + Utils.FormatDate(Convert.ToDateTime(r["ChallanDate"]))).ToList();
                ViewBag.ChallansWithDate = String.Join(" | ", ch);
            }

            await SetBreakageDamageDetailsForRentBillAsync(ds, companyId, ledgerId, invoiceId, ledgerSiteId, configData ?? new List<ConfigDTO>());
            return (ds, companyId);
        }

        public void ConvertHtmlStringToPdfWithChrome(string htmlContent, Stream outputPath)
        {
            //Create a pdf document.
            PdfDocument doc = new PdfDocument();

            PdfPageSettings setting = new PdfPageSettings();

            setting.Size = new SizeF(1000, 1000);
            setting.Margins = new Spire.Pdf.Graphics.PdfMargins(20);

            PdfHtmlLayoutFormat htmlLayoutFormat = new PdfHtmlLayoutFormat();
            htmlLayoutFormat.IsWaiting = true;

            Thread thread = new Thread(() =>
            {
                // doc.LoadFromHTML(htmlContent, false, false, false, setting, htmlLayoutFormat);
                doc.LoadFromHTML(htmlContent, true, setting, htmlLayoutFormat);
            }
            );
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            //Save pdf file.
            //  doc.SaveToFile("output-wiki.pdf");

            doc.SaveToStream(outputPath);
        }

        private bool PopulateMeasurementBillViewBag(string encInvoiceId, out int invoiceId)
        {
            invoiceId = 0;
            string file = Encoding.UTF8.GetString(Convert.FromBase64String(encInvoiceId));
            var dec = Security.Decrypt(file);
            var billing = new Billing();
            invoiceId = Convert.ToInt32(dec);
            var ds = billing.PrintBill(invoiceId);

            int companyId = 0, ledgerId = 0, ledgerSiteId;
            var compLogo = Convert.ToString(ds.Tables[0].Rows[0]["CompanyLogo"]);
            companyId = Convert.ToInt32(ds.Tables[0].Rows[0]["CompanyId"]);
            ledgerId = Convert.ToInt32(ds.Tables[0].Rows[0]["LedgerId"]);
            ledgerSiteId = Convert.ToInt32(ds.Tables[0].Rows[0]["LedgerSiteId"]);

            var config = new Config();
            var printConfigs = config.GetConfig(companyId, "general", "print");
            var printSignature = true;
            var printQrCode = true;
            var companyDetails = new Company(companyId).GetDetails();

            ds.Tables[0].Columns.Add("QrCode");
            if (printConfigs != null && printConfigs.Count() > 0)
            {
                var c = printConfigs.Where(o => o.Key.ToLower() == "signature").FirstOrDefault();
                if (c != null && !String.IsNullOrEmpty(c.Value))
                {
                    printSignature = c.Value.Contains("bills");
                }
                c = printConfigs.Where(o => o.Key.ToLower() == "qrcode").FirstOrDefault();
                if (c != null && !String.IsNullOrEmpty(c.Value))
                {
                    printQrCode = c.Value.Contains("bills");
                }
            }

            if (!String.IsNullOrEmpty(compLogo))
            {
                ds.Tables[0].Rows[0]["CompanyLogo"] = docPath + "/comp/" + compLogo;
            }
            var signaure = Convert.ToString(ds.Tables[0].Rows[0]["Signature"]);
            if (!String.IsNullOrEmpty(signaure) && printSignature)
            {
                ds.Tables[0].Rows[0]["Signature"] = docPath + "/comp/" + signaure;
            }
            if (!String.IsNullOrEmpty(companyDetails.QrCode) && printQrCode)
            {
                ds.Tables[0].Rows[0]["QrCode"] = docPath + "/comp/" + companyDetails.QrCode;
            }
            if (ds.Tables[0].Rows[0]["IRN"] != DBNull.Value)
            {
                var qrCodeStr = Convert.ToString(ds.Tables[0].Rows[0]["IRN"]);
                var codeGen = new QRCodeGenerator();
                var qrCodeData = codeGen.CreateQrCode(Encoding.UTF8.GetBytes(qrCodeStr), QRCodeGenerator.ECCLevel.Q);
                using (Base64QRCode qrCode = new Base64QRCode(qrCodeData))
                {
                    Base64QRCode qrCodeBase64 = new Base64QRCode(qrCodeData);
                    string qrCodeImageAsBase64 = qrCodeBase64.GetGraphic(30);
                    ds.Tables[0].Rows[0]["SignedQrCode"] = "data:image/png;base64," + qrCodeImageAsBase64;
                }
            }

            ViewBag.BillData = ds;

            var bal = (from d in ds.Tables[0].AsEnumerable()
                       group d by new { ProductId = d["ProductId"] } into g
                       select Convert.ToString(g.Last()["Item"]) + ":" + g.Last()["CB"]).ToList();
            ViewBag.Balance = String.Join(",", bal);

            var configData = config.GetBillingConfig(companyId);
            ViewBag.ConfigData = configData ?? new List<ConfigDTO>();
            bool printBankDetails = false;
            ViewBag.PrintExcessQty = false;
            ViewBag.ShowChallans = true;
            bool hideZeroAmountItem = false;
            if (configData != null)
            {
                var printBank = configData.Where(o => o.Key == "printBankDetails").FirstOrDefault();
                if (printBank != null && !String.IsNullOrEmpty(printBank.Value))
                {
                    printBankDetails = Convert.ToBoolean(printBank.Value);
                }
                var hideZero = configData.Where(o => o.Key == "hideZeroAmountItem").FirstOrDefault();
                if (hideZero != null && !String.IsNullOrEmpty(hideZero.Value))
                {
                    hideZeroAmountItem = Convert.ToBoolean(hideZero.Value);
                }
                var showExcessQty = configData.Where(o => o.Key == "showExcessQty").FirstOrDefault();
                if (showExcessQty != null && !String.IsNullOrEmpty(showExcessQty.Value))
                {
                    ViewBag.PrintExcessQty = showExcessQty.Value == "1";
                }
                var showChallans = configData.Where(o => o.Key == "showChallans").FirstOrDefault();
                if (showChallans != null && !String.IsNullOrEmpty(showChallans.Value))
                {
                    ViewBag.ShowChallans = showChallans.Value == "1";
                }
            }

            var uniqueItems = (from d in ds.Tables[0].AsEnumerable()
                               group d by d["ProductId"] into g
                               select g.Key).ToList();

            var ledger = new Ledger();
            ViewBag.LastBill = ledger.GetLastBill(ledgerId, ledgerSiteId, 0, invoiceId);

            ViewBag.PrintBankDetails = printBankDetails;
            ViewBag.PrintQrCode = printQrCode;
            ViewBag.hideZeroAmountItem = hideZeroAmountItem;
            ViewBag.CompanyInfo = companyDetails;
            ViewBag.UniqueItems = uniqueItems;

            var challansTable = ds.Tables[5];
            if (challansTable != null)
            {
                var ch = (from r in challansTable.AsEnumerable()
                          select Convert.ToString(r["ChallanNumber"]) + " : " + Utils.FormatDate(Convert.ToDateTime(r["ChallanDate"]))).ToList();
                ViewBag.ChallansWithDate = String.Join(" | ", ch);
            }

            return true;
        }

        [Route("Home/PreviewMeasurementBill/{encInvoiceId}")]
        public ActionResult PreviewMeasurementBill(string encInvoiceId)
        {
            try
            {
                if (!PopulateMeasurementBillViewBag(encInvoiceId, out int invoiceId))
                {
                    return null;
                }
                return View("measurement-bill");
            }
            catch (Exception ex)
            {
                RentacLogger.logger.Error(ex, ex.Message);
                return null;
            }
        }

        [Route("Home/PrintMeasurementBill/{encInvoiceId}")]
        public async Task<ActionResult> PrintMeasurementBill(string encInvoiceId)
        {
            try
            {
                if (!PopulateMeasurementBillViewBag(encInvoiceId, out int invoiceId))
                {
                    return null;
                }

                var billing = new Billing();
                var ds = ViewBag.BillData as DataSet;
                string strHtml = RenderRazorViewToString(this, "measurement-bill");


                //using (HttpClient client = new HttpClient())
                //{
                // var _url = Request.Url.ToString();
                //   _url = _url.Replace("PrintRentBill", "PreviewRentBill");

                // var strHtml = await client.GetStringAsync(_url);
                HtmlToPdfConverter con = new HtmlToPdfConverter();
                con.Margins.Top = 5;
                con.Margins.Bottom = 5;

                var fileBytes = con.GeneratePdf(strHtml);
                string dwnFileName = "report.pdf";
                // string file = Encoding.UTF8.GetString(Convert.FromBase64String(encInvoiceId));
                //var dec = Security.Decrypt(file);
                //var billing = new Billing();
                var user = new LoggedInUser();
                var dsbills = await billing.GetBillsByIds(invoiceId.ToString(), user.DefaultCompanyId);


                if (ds != null)
                {
                    var inv = dsbills.First();
                    //var companyId = Convert.ToString(ds.Tables[0].Rows[0]["CompanyId"]);
                    //var invNumber = Convert.ToString(ds.Tables[0].Rows[0]["InvoiceNumber"]);
                    //dwnFileName = ds.Tables[0].Rows[0]["InvoiceNumber"] + "-" +
                    //    ds.Tables[0].Rows[0]["LedgerId"] + ".pdf";
                    var filePath = Server.MapPath("/temp/") + inv.InvoiceId.ToString() + "-bill.pdf";
                    using (var stream = System.IO.File.Create(filePath))
                    {

                        stream.Write(fileBytes, 0, fileBytes.Length);
                    }

                    await azService.UploadFileAsync(user.FinYearId, user.DefaultCompanyId + "/bills", inv.FileName + ".pdf", filePath);


                }
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);


            }
            catch (Exception ex)
            {
                RentacLogger.logger.Error(ex, ex.Message);
                return null;
            }
        }

        [Route("Home/PreviewContractBill/{encInvoiceId}")]
        [System.Web.Mvc.AllowAnonymous]

        public ActionResult PreviewContractBill(string encInvoiceId)
        {
            try
            {

                string file = Encoding.UTF8.GetString(Convert.FromBase64String(encInvoiceId));
                var dec = Security.Decrypt(file);
                var billing = new Billing();
                var ds = billing.PrintContractBill(Convert.ToInt32(dec));

                var compLogo = Convert.ToString(ds.Tables[0].Rows[0]["CompanyLogo"]);
                int companyId = 0;
                if (!String.IsNullOrEmpty(compLogo))
                {
                    ds.Tables[0].Rows[0]["CompanyLogo"] = docPath + "/comp/" + compLogo;
                }
                companyId = Convert.ToInt32(ds.Tables[0].Rows[0]["CompanyId"]);
                var config = new Config();
                var configData = config.GetBillingConfig(companyId);
                ViewBag.ConfigData = configData ?? new List<ConfigDTO>();

                ViewBag.contractbillcodetype = "HSNCode";

                bool printBankDetails = false;
                if (configData != null)
                {
                    var contractbillcodetype = configData.Where(o => o.Key == "contractbillcodetype").FirstOrDefault();
                    if (contractbillcodetype != null && !String.IsNullOrEmpty(contractbillcodetype.Value))
                    {
                        ViewBag.contractbillcodetype = contractbillcodetype.Value == "sac" ? "SACCode" : "HSNCode";
                    }

                    var printBank = configData.Where(o => o.Key == "printBankDetails").FirstOrDefault();
                    if (printBank != null && !String.IsNullOrEmpty(printBank.Value))
                    {
                        printBankDetails = Convert.ToBoolean(printBank.Value);
                    }

                }
                var printConfigs = config.GetConfig(companyId, "general", "print");
                var printSignature = true;

                var company = new Company(companyId).GetDetails();
                ds.Tables[0].Columns.Add("QrCode");
                if (printConfigs != null && printConfigs.Count() > 0)
                {
                    var c = printConfigs.Where(o => o.Key.ToLower() == "signature").FirstOrDefault();
                    if (c != null)
                    {
                        printSignature = c.Value.Contains("quotations");
                    }
                    var signaure = Convert.ToString(ds.Tables[0].Rows[0]["Signature"]);
                    if (!String.IsNullOrEmpty(signaure) && printSignature)
                    {
                        ds.Tables[0].Rows[0]["Signature"] = docPath + "/comp/" + signaure;
                    }
                }

                ViewBag.PrintBankDetails = printBankDetails;
                ViewBag.CompanyInfo = new Company(companyId).GetDetails();

                ViewBag.BillData = ds;
                return View();
            }
            catch (Exception ex)
            {
                RentacLogger.logger.Error(ex, ex.Message);
                return null;
            }
        }


        [Route("Home/PrintContractBill/{encInvoiceId}")]
        public async Task<ActionResult> PrintContractBill(string encInvoiceId)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var _url = Request.Url.ToString();
                    _url = _url.Replace("PrintContractBill", "PreviewContractBill");
                    var strHtml = await client.GetStringAsync(_url);
                    HtmlToPdfConverter con = new HtmlToPdfConverter();
                    con.CustomWkHtmlArgs += " --header-spacing 2"; // Adjust spacing
                                                                   // Or try these specific arguments:
                                                                   //   con.CustomWkHtmlArgs = "--print-media-type --enable-smart-shrinking";
                    con.CustomWkHtmlArgs = "--print-media-type  --javascript-delay 500 --enable-smart-shrinking --dpi 300";
                    con.Margins.Top = 5;
                    con.Margins.Bottom = 5;


                    var fileBytes = con.GeneratePdf(strHtml);
                    string file = Encoding.UTF8.GetString(Convert.FromBase64String(encInvoiceId));
                    var dec = Security.Decrypt(file);
                    var billing = new Billing();
                    var ds = billing.PrintContractBill(Convert.ToInt32(dec));
                    string dwnFileName = "report.pdf";
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            dwnFileName = ds.Tables[0].Rows[0]["InvoiceNumber"] + "-" +
                                ds.Tables[0].Rows[0]["LedgerId"] + ".pdf";

                        }
                    }

                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);
                }
            }
            catch (Exception ex)
            {
                RentacLogger.logger.Error(ex, ex.Message);
                return null;
            }
        }

        private static bool TryParseContractSummaryToken(string decrypted, out int contractId, out int companyId)
        {
            contractId = 0;
            companyId = 0;
            if (string.IsNullOrWhiteSpace(decrypted))
                return false;
            var parts = decrypted.Split(new[] { '|' }, 2, StringSplitOptions.None);
            if (parts.Length >= 2)
            {
                if (int.TryParse(parts[0].Trim(), out contractId) && int.TryParse(parts[1].Trim(), out companyId))
                    return contractId > 0;
            }
            if (parts.Length >= 1 && int.TryParse(parts[0].Trim(), out contractId))
                return contractId > 0;
            return false;
        }

        private static string ContractSummaryActivityLabel(JobCardDto job)
        {
            if (job == null) return string.Empty;
            var t = job.TypeId == 1 ? "Install" : job.TypeId == 2 ? "Dismantle" : job.TypeId == 3 ? "Others" : "Activity";
            return t + (job.JobCardId > 0 ? " #" + job.JobCardId : string.Empty);
        }

        private static List<ContractSummaryChallanDelivery> GroupSummaryDeliveries(IEnumerable<WorkOrderItemDTO> raw)
        {
            var list = (raw ?? Enumerable.Empty<WorkOrderItemDTO>()).Where(x => !string.IsNullOrEmpty(x.ChallanNumber)).ToList();
            return list.GroupBy(x => x.ChallanNumber, StringComparer.OrdinalIgnoreCase)
                .Select(g =>
                {
                    var first = g.OrderBy(x => x.SentDate).First();
                    return new ContractSummaryChallanDelivery
                    {
                        ChallanNumber = first.ChallanNumber,
                        SentDate = first.SentDate,
                        Items = g.OrderBy(x => x.Product ?? string.Empty).ToList()
                    };
                })
                .OrderBy(x => x.SentDate)
                .ToList();
        }

        private static List<ContractSummaryChallanReturn> GroupSummaryReturns(IEnumerable<WorkOrderItemDTO> raw)
        {
            var list = (raw ?? Enumerable.Empty<WorkOrderItemDTO>()).Where(x => !string.IsNullOrEmpty(x.ChallanNumber)).ToList();
            return list.GroupBy(x => x.ChallanNumber, StringComparer.OrdinalIgnoreCase)
                .Select(g =>
                {
                    var first = g.OrderBy(x => x.ChallanDate).First();
                    return new ContractSummaryChallanReturn
                    {
                        ChallanNumber = first.ChallanNumber,
                        ChallanDate = first.ChallanDate,
                        Items = g.OrderBy(x => x.Product ?? string.Empty).ToList()
                    };
                })
                .OrderBy(x => x.ChallanDate)
                .ToList();
        }

        private static List<ContractSummaryInventoryRow> BuildContractSummaryInventory(IEnumerable<WorkOrderItemDTO> raw)
        {
            var rows = new List<ContractSummaryInventoryRow>();
            var inv = raw?.ToList() ?? new List<WorkOrderItemDTO>();
            var products = inv.Select(x => x.Product).Where(p => !string.IsNullOrEmpty(p)).Distinct().OrderBy(p => p).ToList();
            foreach (var p in products)
            {
                var sent = inv.Where(x => x.Product == p && x.ChallanType == 1).Sum(x => x.Quantity);
                var ret = inv.Where(x => x.Product == p && x.ChallanType == 11).Sum(x => x.Quantity);
                rows.Add(new ContractSummaryInventoryRow
                {
                    Product = p,
                    Sent = sent,
                    Returned = ret,
                    Balance = sent - ret
                });
            }
            return rows;
        }

        private async Task<ContractSummaryPrintModel> BuildContractSummaryPrintModelAsync(int contractId, int companyId)
        {
            var model = new ContractSummaryPrintModel();
            try
            {
                var contService = new Contract();
                var filter = new ContractFilterDto { ContractId = contractId, CompanyId = companyId };
                var contract = await contService.GetById(filter).ConfigureAwait(false);
                model.Contract = contract;
                if (contract.Company != null && !string.IsNullOrEmpty(contract.Company.Logo))
                {
                    contract.Company.Logo = docPath + "/comp/" + contract.Company.Logo;
                }
                var bills = (await contService.GetContractBills(companyId, contractId).ConfigureAwait(false))?.ToList()
                    ?? new List<BillingDTO>();
                model.Bills = bills;
                model.BilledTotal = bills.Sum(b => b.Total);
                var invRaw = (await contService.ContractInventory(filter).ConfigureAwait(false))?.ToList()
                    ?? new List<WorkOrderItemDTO>();
                model.Inventory = BuildContractSummaryInventory(invRaw);
                var ledger = new Ledger();
                var payments = ledger.GetContractReceiptPayments(contractId, companyId) ?? new List<LedgerTransactionDTO>();
                model.Payments = payments;
                model.PaidTotal = payments.Sum(x => x.TransactionAmount);
                model.BalanceBilledVsPaid = model.BilledTotal - model.PaidTotal;
                var jobSections = new List<ContractSummaryJobSection>();
                var jcService = new JobCard();
                //foreach (var job in contract.JobCards ?? new List<JobCardDto>())
                //{
                var del = await contService.ContractDelChallanItems(contractId, companyId).ConfigureAwait(false);
                var ret = await contService.ContractReturnChallanItems(contractId, companyId).ConfigureAwait(false);
                jobSections.Add(new ContractSummaryJobSection
                {
                    // JobCardId = job.JobCardId,
                    ActivityLabel = "",//ContractSummaryActivityLabel(job),
                    DeliveryChallans = GroupSummaryDeliveries(del),
                    ReturnChallans = GroupSummaryReturns(ret)
                });
                //}
                model.JobSections = jobSections;
            }
            catch (Exception ex)
            {
                RentacLogger.logger.Error(ex, ex.Message);
                model.ErrorMessage = ex.Message;
            }
            return model;
        }

        [Route("Home/PreviewContractSummary/{encoded}")]
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ActionResult> PreviewContractSummary(string encoded)
        {
            try
            {
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
                var dec = Security.Decrypt(decoded);
                if (!TryParseContractSummaryToken(dec, out var contractId, out var companyId))
                    return Content("Invalid link.");
                var model = await BuildContractSummaryPrintModelAsync(contractId, companyId).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(model.ErrorMessage))
                    return Content(System.Net.WebUtility.HtmlEncode(model.ErrorMessage));
                return View(model);
            }
            catch (Exception ex)
            {
                RentacLogger.logger.Error(ex, ex.Message);
                return Content("Could not load contract summary.");
            }
        }

        [Route("Home/PrintContractSummary/{encoded}")]
        public async Task<ActionResult> PrintContractSummary(string encoded)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var _url = Request.Url.ToString().Replace("PrintContractSummary", "PreviewContractSummary");
                    var strHtml = await client.GetStringAsync(_url).ConfigureAwait(false);
                    var con = new HtmlToPdfConverter();
                    con.CustomWkHtmlArgs = "--print-media-type --enable-smart-shrinking --dpi 300";
                    con.Margins.Top = 5;
                    con.Margins.Bottom = 5;
                    var fileBytes = con.GeneratePdf(strHtml);
                    var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
                    var dec = Security.Decrypt(decoded);
                    TryParseContractSummaryToken(dec, out var contractId, out var companyId);
                    var dwnFileName = "ContractSummary-" + contractId + ".pdf";
                    try
                    {
                        var m = await BuildContractSummaryPrintModelAsync(contractId, companyId).ConfigureAwait(false);
                        if (m.Contract != null && !string.IsNullOrEmpty(m.Contract.Title))
                        {
                            var safe = new string(m.Contract.Title.Where(ch => !Path.GetInvalidFileNameChars().Contains(ch)).ToArray());
                            if (!string.IsNullOrWhiteSpace(safe))
                                dwnFileName = "ContractSummary-" + safe.Trim() + ".pdf";
                        }
                    }
                    catch
                    {
                        // keep default file name
                    }
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);
                }
            }
            catch (Exception ex)
            {
                RentacLogger.logger.Error(ex, ex.Message);
                return null;
            }
        }

        //   [Authorize]
        [Route("Home/Preview/{reportName}")]
        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> Preview(string reportName)
        {
            try
            {
                var user = new LoggedInUser();
                string file = Encoding.UTF8.GetString(Convert.FromBase64String(reportName));

                var dec = Security.Decrypt(file);

                var strParams = dec.Split(',');
                var repName = strParams[0];
                ViewBag.CssPath = Server.MapPath("~/Content") + @"\preview.css";

                if (repName == ReportNames.E_WAYBILL_PREVIEW)
                {
                    var templateService = new TemplateService();
                    var template = await templateService.GetDefaultPdfTemplate(user.DefaultCompanyId, TemplateGroups.EWAYBILL,
                        ConfigCategoryNames.EWAYBILL, ConfigSubCategoryNames.TEMPLATES);
                    //if (template == null)
                    //{
                    //    throw new Exception("Report template could not found.");
                    //}
                    var xslFileName = "ewayBill-single.xslt";
                    if (template != null)
                    {
                        xslFileName = template.FileName;
                    }
                    var ewayBilLId = strParams[1];
                    var ewayBillService = new EwayBillService();
                    var ds = ewayBillService.PrintEwayBill(Convert.ToInt32(ewayBilLId));
                    ViewBag.XML = ds.GetXml();
                    ViewBag.RepFileName = Server.MapPath("~/Reports") + @"\" + xslFileName;

                }
                if (repName == ReportNames.MAT_LOSS_RECEIPT)
                {

                    var matLossId = Convert.ToInt32(strParams[1]);
                    var billing = new Billing();
                    var ds = await billing.MatLossById(new MatLossFilterDTO { CompanyId = user.DefaultCompanyId, MatLossId = matLossId });
                    var comp = new Company(ds.CompanyId);
                    ds.Company = comp.GetDetails();
                    if (!String.IsNullOrEmpty(ds.Company.Logo))
                    {
                        ds.Company.Logo = docPath + "/comp/" + ds.Company.Logo;
                    }

                    var client = new Ledger(ds.LedgerId);

                    ds.Ledger = client.GetDetails();

                    var d = new { data = ds };
                    string jsonText = JsonConvert.SerializeObject(d);
                    // To convert JSON text contained in string json into an XML node
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                    ViewBag.XML = doc.InnerXml;
                    ViewBag.RepFileName = Server.MapPath("~/Reports") + @"\matlossreceipt.xslt";

                }
                if (repName == ReportNames.MATERIAL_ADJUST)
                {
                    var workOrderId = Convert.ToInt32(strParams[1]);
                    PopulateMaterialAdjustReportViewBag(workOrderId, user.DefaultCompanyId);
                    ViewBag.RepFileName = Server.MapPath("~/Reports") + @"\materialadjust.xslt";
                }
                if (repName == ReportNames.CONTRACT_PREVIEW)
                {

                    var contractId = strParams[1];
                    var ewayBillService = new EwayBillService();
                    var contService = new Contract();
                    var ds = await contService.GetById(new BAL.DTO.ContractFilterDto { ContractId = Convert.ToInt32(contractId), CompanyId = user.DefaultCompanyId });
                    if (!String.IsNullOrEmpty(ds.Company.Logo))
                    {
                        ds.Company.Logo = docPath + "/comp/" + ds.Company.Logo;
                    }

                    var d = new { data = ds };
                    string jsonText = JsonConvert.SerializeObject(d);
                    // To convert JSON text contained in string json into an XML node
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                    string xml = doc.InnerXml;
                    ViewBag.XML = xml;
                    ViewBag.RepFileName = Server.MapPath("~/Reports") + @"\contract.xslt";

                }
                if (repName == ReportNames.QUOTATION)
                {

                    var quotationId = strParams[1];
                    var billing = new Billing();

                    var ds = billing.GetQuotationItems(Convert.ToInt32(quotationId));
                    var compLogo = Convert.ToString(ds.Tables[0].Rows[0]["CompanyLogo"]);
                    var signaure = Convert.ToString(ds.Tables[0].Rows[0]["Signature"]);
                    int companyId = Convert.ToInt32(ds.Tables[0].Rows[0]["CompanyId"]);


                    var config = new Config();
                    var printConfigs = config.GetConfig(companyId, "general", "print");
                    var printSignature = true;
                    var printQrCode = true;
                    var company = new Company(companyId).GetDetails();
                    ds.Tables[0].Columns.Add("QrCode");
                    if (printConfigs != null && printConfigs.Count() > 0)
                    {
                        var c = printConfigs.Where(o => o.Key.ToLower() == "signature").FirstOrDefault();
                        if (c != null)
                        {
                            printSignature = c.Value.Contains("quotations");
                        }
                        c = printConfigs.Where(o => o.Key.ToLower() == "qrcode").FirstOrDefault();
                        if (c != null && !String.IsNullOrEmpty(c.Value))
                        {
                            printQrCode = c.Value.Contains("quotations");
                        }
                    }

                    if (!String.IsNullOrEmpty(compLogo))
                    {
                        ds.Tables[0].Rows[0]["CompanyLogo"] = docPath + "/comp/" + compLogo;
                    }

                    if (!String.IsNullOrEmpty(signaure) && printSignature)
                    {
                        ds.Tables[0].Rows[0]["Signature"] = docPath + "/comp/" + signaure;
                    }
                    if (!String.IsNullOrEmpty(company.QrCode) && printQrCode)
                    {
                        ds.Tables[0].Rows[0]["QrCode"] = docPath + "/comp/" + company.QrCode;
                    }
                    if (Convert.ToDouble(ds.Tables[0].Rows[0]["IGST"]) > 0)
                    {
                        ds.Tables[0].Rows[0]["IGST"] = Convert.ToDouble(ds.Tables[0].Rows[0]["IGST"]) +
                                                        Convert.ToDouble(ds.Tables[0].Rows[0]["FreightTax"]) +
                                                        Convert.ToDouble(ds.Tables[0].Rows[0]["chargesTax"]);
                    }
                    else if (Convert.ToDouble(ds.Tables[0].Rows[0]["SGST"]) > 0)
                    {
                        ds.Tables[0].Rows[0]["SGST"] = Convert.ToDouble(ds.Tables[0].Rows[0]["SGST"]) + Convert.ToDouble(ds.Tables[0].Rows[0]["FreightTax"]) / 2 +
                                                        Convert.ToDouble(ds.Tables[0].Rows[0]["chargesTax"]) / 2;
                        ds.Tables[0].Rows[0]["CGST"] = Convert.ToDouble(ds.Tables[0].Rows[0]["CGST"]) + Convert.ToDouble(ds.Tables[0].Rows[0]["FreightTax"]) / 2 +
                                                       Convert.ToDouble(ds.Tables[0].Rows[0]["chargesTax"]) / 2;
                    }
                    if (Convert.ToInt32(ds.Tables[0].Rows[0]["LedgerSiteId"]) > 0)
                    {
                        ds.Tables[0].Rows[0]["ShipAddress1"] = ds.Tables[0].Rows[0]["SiteAddress"];
                        ds.Tables[0].Rows[0]["ShipAddress2"] = "";
                        ds.Tables[0].Rows[0]["ShipCity"] = ds.Tables[0].Rows[0]["SiteCity"];
                        ds.Tables[0].Rows[0]["ShipStateName"] = ds.Tables[0].Rows[0]["SiteState"];
                        ds.Tables[0].Rows[0]["ShipZipCode"] = ds.Tables[0].Rows[0]["SiteZipCode"];

                    }
                    var rowsToSpan = 6;
                    rowsToSpan += Convert.ToDecimal(ds.Tables[0].Rows[0]["charge1"]) > 0 ? 1 : 0;
                    rowsToSpan += Convert.ToDecimal(ds.Tables[0].Rows[0]["charge2"]) > 0 ? 1 : 0;
                    rowsToSpan += Convert.ToDecimal(ds.Tables[0].Rows[0]["charge3"]) > 0 ? 1 : 0;
                    rowsToSpan += Convert.ToDecimal(ds.Tables[0].Rows[0]["charge4"]) > 0 ? 1 : 0;
                    rowsToSpan += Convert.ToDecimal(ds.Tables[0].Rows[0]["charge5"]) > 0 ? 1 : 0;
                    rowsToSpan += Convert.ToDecimal(ds.Tables[0].Rows[0]["DiscountAmount"]) > 0 ? 1 : 0;


                    var d = new { d = new { data = ds, rowsToSpan = rowsToSpan } };


                    // var d = new { d = new { data = ds } };
                    //var d = new { repdata = new { data = ds  } };
                    string jsonText = JsonConvert.SerializeObject(d);
                    // To convert JSON text contained in string json into an XML node
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                    var bgNode = doc.CreateNode(XmlNodeType.Element, "bgImage", "");
                    bgNode.InnerText = docPath + "orange-curve.png";

                    doc.FirstChild.AppendChild(bgNode);
                    var configNode = doc.CreateNode(XmlNodeType.Element, "Config", "");
                    // var config = new Config();
                    var configData = config.GetBillingConfig(companyId);
                    string configJson = JsonConvert.SerializeObject(new { c = new { config = configData } });
                    configNode.InnerXml = JsonConvert.DeserializeXmlNode(configJson).InnerXml;
                    doc.FirstChild.AppendChild(configNode);

                    string xml = doc.InnerXml;
                    ViewBag.XML = xml;

                    //bool printBankDetails = false;
                    //if (configData != null)
                    //{
                    //    var printBank = configData.Where(o => o.Key == "printBankDetails").FirstOrDefault();
                    //    if (printBank != null)
                    //    {
                    //        printBankDetails = Convert.ToBoolean(printBank.Value);
                    //    }
                    //}


                    var templateService = new TemplateService();
                    var template = await templateService.GetDefaultPdfTemplate(user.DefaultCompanyId, TemplateGroups.QUOTATIONS, ConfigCategoryNames.QUOTATIONS, ConfigSubCategoryNames.TEMPLATES);
                    string fileName = "quotation.xslt";
                    if (template != null)
                    {
                        fileName = template.FileName;
                        ViewBag.Style = template.Style;
                        // throw new Exception("Report template could not found.");
                    }


                    ViewBag.RepFileName = Server.MapPath("~/Reports") + @"\" + fileName;

                }
                if (repName == ReportNames.SALE_INVOICE)
                {

                    var invoiceId = strParams[1];
                    var billing = new Billing();

                    var ds = billing.PrintBill(Convert.ToInt32(invoiceId));
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
                    var templateService = new TemplateService();
                    var template = await templateService.GetDefaultPdfTemplate(user.DefaultCompanyId, TemplateGroups.SALES, ConfigCategoryNames.SALES, ConfigSubCategoryNames.TEMPLATES);
                    string fileName = "salebill.xslt";
                    //if (template != null)
                    //{
                    //    if (!String.IsNullOrEmpty(template.FileName))
                    //    {
                    //        fileName = template.FileName;
                    //    }
                    //    // throw new Exception("Report template could not found.");
                    //}

                    var d = new { data = ds };
                    string jsonText = JsonConvert.SerializeObject(d);
                    // To convert JSON text contained in string json into an XML node
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                    string xml = doc.InnerXml;
                    ViewBag.XML = xml;
                    ViewBag.RepFileName = Server.MapPath("~/Reports") + @"\" + fileName;

                }
                if (repName == ReportNames.SALERETURN_INVOICE)
                {

                    var invoiceId = strParams[1];
                    var billing = new Billing();

                    var ds = billing.PrintBill(Convert.ToInt32(invoiceId));
                    var compLogo = Convert.ToString(ds.Tables[0].Rows[0]["CompanyLogo"]);

                    if (!String.IsNullOrEmpty(compLogo))
                    {
                        ds.Tables[0].Rows[0]["CompanyLogo"] = docPath + "/comp/" + compLogo;
                    }


                    var d = new { data = ds };



                    string jsonText = JsonConvert.SerializeObject(d);
                    // To convert JSON text contained in string json into an XML node
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                    string xml = doc.InnerXml;

                    ViewBag.XML = xml;
                    ViewBag.RepFileName = Server.MapPath("~/Reports") + @"\salereturn-invoice.xslt";

                }
                if (repName == ReportNames.PURCHASE_BILL)
                {

                    var purchaseId = Convert.ToInt32(strParams[1]);


                    Purchase objPurchase = new Purchase();

                    var ds = await objPurchase.ById(purchaseId, user.DefaultCompanyId);
                    var comp = new Company(ds.CompanyId);
                    var client = new Ledger(ds.LedgerId);
                    ds.CompanyDTO = comp.GetDetails();
                    ds.LedgerDTO = client.GetDetails();
                    if (!String.IsNullOrEmpty(ds.CompanyDTO.Logo))
                    {
                        ds.CompanyDTO.Logo = docPath + "/comp/" + ds.CompanyDTO.Logo;
                    }

                    var d = new { data = ds };
                    string jsonText = JsonConvert.SerializeObject(d);
                    // To convert JSON text contained in string json into an XML node
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                    string xml = doc.InnerXml;

                    ViewBag.XML = xml;
                    ViewBag.RepFileName = Server.MapPath("~/Reports") + @"\purchasebill.xslt";

                }
                if (repName == ReportNames.PURCHASE_RETURN)
                {

                    var purchaseId = Convert.ToInt32(strParams[1]);


                    Purchase objPurchase = new Purchase();

                    var ds = await objPurchase.ById(purchaseId, user.DefaultCompanyId);
                    var comp = new Company(ds.CompanyId);
                    var client = new Ledger(ds.LedgerId);
                    ds.CompanyDTO = comp.GetDetails();
                    ds.LedgerDTO = client.GetDetails();
                    if (!String.IsNullOrEmpty(ds.CompanyDTO.Logo))
                    {
                        ds.CompanyDTO.Logo = docPath + "/comp/" + ds.CompanyDTO.Logo;
                    }

                    var d = new { data = ds };
                    string jsonText = JsonConvert.SerializeObject(d);
                    // To convert JSON text contained in string json into an XML node
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                    string xml = doc.InnerXml;

                    ViewBag.XML = xml;
                    ViewBag.RepFileName = Server.MapPath("~/Reports") + @"\purchasereturn.xslt";

                }
                if (repName == ReportNames.PURCHASE_ORDER)
                {

                    var purchaseId = Convert.ToInt32(strParams[1]);


                    Purchase objPurchase = new Purchase();

                    var ds = await objPurchase.ById(purchaseId, user.DefaultCompanyId);
                    var comp = new Company(ds.CompanyId);
                    var client = new Ledger(ds.LedgerId);
                    ds.CompanyDTO = comp.GetDetails();
                    ds.LedgerDTO = client.GetDetails();
                    if (!String.IsNullOrEmpty(ds.CompanyDTO.Logo))
                    {
                        ds.CompanyDTO.Logo = docPath + "/comp/" + ds.CompanyDTO.Logo;
                    }

                    var d = new { data = ds };
                    string jsonText = JsonConvert.SerializeObject(d);
                    // To convert JSON text contained in string json into an XML node
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                    string xml = doc.InnerXml;

                    ViewBag.XML = xml;
                    ViewBag.RepFileName = Server.MapPath("~/Reports") + @"\purchaseorder.xslt";

                }
                if (repName == ReportNames.ISSUE_CHALLAN)
                {
                    var workOrderId = Convert.ToInt32(strParams[1]);
                    var previewData = await GetIssueChallanPreviewXmlAndXslt(workOrderId, user.DefaultCompanyId);
                    ViewBag.XML = previewData.Item1;
                    ViewBag.RepFileName = previewData.Item2;
                }
                if (repName == ReportNames.RECEIVED_CHALLAN)
                {
                    var grnId = Convert.ToInt32(strParams[1]);
                    var previewData = await GetReceivedChallanPreviewXmlAndXslt(grnId, user.DefaultCompanyId);
                    ViewBag.XML = previewData.Item1;
                    ViewBag.RepFileName = previewData.Item2;
                }
            }
            catch (Exception ex)
            {
                RentacLogger.logger.Error(ex, ex.Message);
                throw new Exception("Error while processing");
            }



            return View();
        }


        public ActionResult DownloadZip(List<FileInfo> files)
        {
            // Sample files - replace with your actual file sources
            //    var files = new List<FileInfo>
            //{
            //    new FileInfo("path/to/file1.pdf"),
            //    new FileInfo("path/to/file2.jpg"),
            //    new FileInfo("path/to/file3.txt")
            //};

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        var entry = archive.CreateEntry(file.Name, System.IO.Compression.CompressionLevel.Fastest);

                        using (var entryStream = entry.Open())
                        using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                        {
                            fileStream.CopyTo(entryStream);
                        }
                    }
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                return File(memoryStream.ToArray(), "application/zip", "archive.zip");
            }
        }

        [Route("Home/Print/{reportName}")]

        public async Task<ActionResult> Print(string reportName)
        {

            //string fileName = Server.MapPath("~/Reports") + @"\ewayBill.xslt";

            //string medals = System.IO.File.ReadAllText(Server.MapPath("~/Data/data.xml"));
            var user = new LoggedInUser();
            try
            {
                string file = Encoding.UTF8.GetString(Convert.FromBase64String(reportName));

                var dec = Security.Decrypt(file);

                var strParams = dec.Split(',');
                var repName = strParams[0];
                if (repName == ReportNames.RENT_BILL)
                {
                    var billIds = strParams[1].Split('|');
                    var files = new List<FileInfo>();

                    //download single pdf file if only one invoice is selected
                    if (billIds.Length == 1)
                    {
                        return await getRentBillFileContent(Convert.ToInt32(billIds[0]));
                    }
                    //download zip file if only one invoice is selected
                    foreach (var bId in billIds)
                    {
                        var result = await getRentBillFileContent(Convert.ToInt32(bId));
                        if (result != null)
                        {
                            var billObj = new Billing();
                            var bill = (await billObj.GetBillsByIds(bId, user.DefaultCompanyId)).FirstOrDefault();
                            string sanitizedInvoiceNumber = Utils.SanitizeFileName(bill.InvoiceNumber.ToString());

                            var filePath = Server.MapPath("/temp/") + sanitizedInvoiceNumber + "-" + bId + ".pdf";
                            var fileBytes = result.FileContents;
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);
                            }
                            using (var stream = System.IO.File.Create(filePath))
                            {
                                stream.Write(fileBytes, 0, fileBytes.Length);
                                files.Add(new FileInfo(filePath));
                            }
                        }

                    }
                    if (files.Count > 0)
                    {
                        return DownloadZip(files);
                    }
                    return null;
                }
                if (repName == ReportNames.CONTRACT_BILL)
                {
                    return await PrintContractBill(file);
                }
                if (repName == ReportNames.MEASURE_BILL)
                {
                    return await PrintContractBill(file);
                }
                if (repName == ReportNames.E_WAYBILL_PREVIEW)
                {

                    var ewayBilLId = strParams[1];
                    var ewayBillService = new EwayBillService();
                    var ds = ewayBillService.PrintEwayBill(Convert.ToInt32(ewayBilLId));
                    string xml = ds.GetXml();
                    var templateService = new TemplateService();
                    var template = await templateService.GetDefaultPdfTemplate(user.DefaultCompanyId, TemplateGroups.EWAYBILL,
                        ConfigCategoryNames.EWAYBILL, ConfigSubCategoryNames.TEMPLATES);

                    var xslFileName = "ewayBill-single.xslt";
                    if (template != null)
                    {
                        xslFileName = template.FileName;
                    }

                    string fileName = Server.MapPath("~/Reports") + @"\" + xslFileName;
                    var html = RenderXml(xml, fileName);
                    var strHtml = html.ToHtmlString();
                    HtmlToPdfConverter con = new HtmlToPdfConverter();
                    var fileBytes = con.GeneratePdf(strHtml);
                    string dwnFileName = "report.pdf";
                    var docType = Convert.ToString(ds.Tables[0].Rows[0]["docType"]);
                    var docSubType = Convert.ToString(ds.Tables[0].Rows[0]["docSubType"]);
                    var invoiceId = Convert.ToInt32(ds.Tables[0].Rows[0]["InvoiceId"]);
                    var config = new Config();
                    var configs = config.GetConfig(user.DefaultCompanyId, "ewaybill", "print");
                    bool attachChallan = false;
                    if (configs != null)
                    {
                        var attachConfig = configs.Where(o => o.Key.ToLower() == "printchallanwithewabill").FirstOrDefault();
                        if (attachConfig != null)
                        {
                            if (Convert.ToString(attachConfig.Value).ToLower() == "true")
                            {
                                attachChallan = true;
                            }
                        }
                    }
                    if (docType == "chl" && docSubType == "del")
                    {


                        if (attachChallan)
                        {
                            var delChallan = await IssueChallanFIle(user, invoiceId);

                            var streams = new List<Stream>();
                            var ewayMs = new MemoryStream(fileBytes);
                            var chalalnBytes = new MemoryStream(delChallan);
                            streams.Add(ewayMs);
                            streams.Add(chalalnBytes);

                            var output = PdfHelper.MergeFiles(streams);
                            byte[] outputBytes;
                            using (MemoryStream ms = new MemoryStream())
                            {
                                output.CopyTo(ms);
                                outputBytes = ms.ToArray();
                            }
                            return File(outputBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);
                        }


                    }
                    else if (docType == "chl" && docSubType == "ret")
                    {


                        if (attachChallan)
                        {
                            var retChallan = await returnChallanFile(user, invoiceId, "none");

                            var streams = new List<Stream>();
                            var ewayMs = new MemoryStream(fileBytes);
                            var chalalnBytes = new MemoryStream(retChallan);
                            streams.Add(ewayMs);
                            streams.Add(chalalnBytes);

                            var output = PdfHelper.MergeFiles(streams);
                            byte[] outputBytes;
                            using (MemoryStream ms = new MemoryStream())
                            {
                                output.CopyTo(ms);
                                outputBytes = ms.ToArray();
                            }
                            return File(outputBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);
                        }

                    }
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);


                }
                //if (repName == ReportNames.ISSUE_CHALLAN)
                //{
                //    string dwnFileName = "delivery-challan.pdf";
                //    var workOrderId = Convert.ToInt32(strParams[1]);
                //    var fileBytes = await IssueChallanFIle(user, workOrderId);
                //    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);
                //}
                //if (repName == ReportNames.RECEIVED_CHALLAN)
                //{
                //    var grnId = Convert.ToInt32(strParams[1]);
                //    string headerType = "none";
                //    if (strParams.Length > 2)
                //    {
                //        headerType = strParams[2];
                //    }
                //    byte[] fileBytes = await returnChallanFile(user, grnId, headerType);
                //    string dwnFileName = "received-challan.pdf";
                //    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);
                //}
                if (repName == ReportNames.ISSUE_CHALLAN)
                {
                    var workOrderId = Convert.ToInt32(strParams[1]);
                    string headerType = "none";
                    if (strParams.Length > 2)
                    {
                        headerType = strParams[2];
                    }
                    var previewData = await GetIssueChallanPreviewXmlAndXslt(workOrderId, user.DefaultCompanyId, headerType);
                    var html = RenderXml(previewData.Item1, previewData.Item2);
                    var strHtml = html.ToHtmlString();
                    HtmlToPdfConverter con = new HtmlToPdfConverter();

                    var template = previewData.Item3;
                    if (template != null)
                    {
                        if (template.ApplyPrintConfig)
                        {
                            con.CustomWkHtmlArgs += " --header-spacing 5"; // Adjust spacing
                                                                           //Or try these specific arguments:
                            con.CustomWkHtmlArgs = "--print-media-type --enable-smart-shrinking --dpi 300";
                            con.Margins.Top = 5;
                            con.Margins.Left = 6;
                            con.Margins.Right = 6;

                            con.Margins.Bottom = 2;
                        }
                        if (template.Orientation == 2)
                        {
                            con.Orientation = PageOrientation.Landscape;
                        }
                    }
                    var fileBytes = con.GeneratePdf(strHtml);
                    string dwnFileName = "delivery-challan.pdf";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);
                }
                if (repName == ReportNames.RECEIVED_CHALLAN)
                {
                    var grnId = Convert.ToInt32(strParams[1]);
                    string headerType = "none";
                    if (strParams.Length > 2)
                    {
                        headerType = strParams[2];
                    }

                    var previewData = await GetReceivedChallanPreviewXmlAndXslt(grnId, user.DefaultCompanyId, headerType);
                    var html = RenderXml(previewData.Item1, previewData.Item2);
                    var strHtml = html.ToHtmlString();
                    HtmlToPdfConverter con = new HtmlToPdfConverter();
                    var template = previewData.Item3;
                    if (template != null)
                    {
                        if (template.ApplyPrintConfig)
                        {
                            con.CustomWkHtmlArgs += " --header-spacing 2"; // Adjust spacing
                                                                           //Or try these specific arguments:
                            con.CustomWkHtmlArgs = "--print-media-type --enable-smart-shrinking  --dpi 300";
                            con.Margins.Top = 2;
                            con.Margins.Bottom = 2;
                        }
                        if (template.Orientation == 2)
                        {
                            con.Orientation = PageOrientation.Landscape;
                        }
                    }
                    var fileBytes = con.GeneratePdf(strHtml);
                    string dwnFileName = "received-challan.pdf";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);
                }
                if (repName == ReportNames.PURCHASE_BILL)
                {


                    var purchaseId = Convert.ToInt32(strParams[1]);


                    Purchase objPurchase = new Purchase();

                    var ds = await objPurchase.ById(purchaseId, user.DefaultCompanyId);
                    var comp = new Company(ds.CompanyId);
                    var client = new Ledger(ds.LedgerId);
                    ds.CompanyDTO = comp.GetDetails();
                    ds.LedgerDTO = client.GetDetails();


                    if (!String.IsNullOrEmpty(ds.CompanyDTO.Logo))
                    {
                        ds.CompanyDTO.Logo = docPath + "/comp/" + ds.CompanyDTO.Logo;
                    }


                    var d = new { data = ds };
                    string jsonText = JsonConvert.SerializeObject(d);
                    // To convert JSON text contained in string json into an XML node
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                    string xml = doc.InnerXml;
                    string fileName = Server.MapPath("~/Reports") + @"\purchasebill.xslt";


                    var html = RenderXml(xml, fileName);
                    var strHtml = html.ToHtmlString();
                    HtmlToPdfConverter con = new HtmlToPdfConverter();
                    var fileBytes = con.GeneratePdf(strHtml);
                    string dwnFileName = "report.pdf";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);

                }
                if (repName == ReportNames.PURCHASE_RETURN)
                {


                    var purchaseId = Convert.ToInt32(strParams[1]);


                    Purchase objPurchase = new Purchase();

                    var ds = await objPurchase.ById(purchaseId, user.DefaultCompanyId);
                    var comp = new Company(ds.CompanyId);
                    var client = new Ledger(ds.LedgerId);
                    ds.CompanyDTO = comp.GetDetails();
                    ds.LedgerDTO = client.GetDetails();
                    if (!String.IsNullOrEmpty(ds.CompanyDTO.Logo))
                    {
                        ds.CompanyDTO.Logo = docPath + "/comp/" + ds.CompanyDTO.Logo;
                    }

                    var d = new { data = ds };
                    string jsonText = JsonConvert.SerializeObject(d);
                    // To convert JSON text contained in string json into an XML node
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                    string xml = doc.InnerXml;
                    string fileName = Server.MapPath("~/Reports") + @"\purchasereturn.xslt";


                    var html = RenderXml(xml, fileName);
                    var strHtml = html.ToHtmlString();
                    HtmlToPdfConverter con = new HtmlToPdfConverter();
                    var fileBytes = con.GeneratePdf(strHtml);
                    string dwnFileName = "report.pdf";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);

                }
                if (repName == ReportNames.PURCHASE_ORDER)
                {


                    var purchaseId = Convert.ToInt32(strParams[1]);


                    Purchase objPurchase = new Purchase();

                    var ds = await objPurchase.ById(purchaseId, user.DefaultCompanyId);
                    var comp = new Company(ds.CompanyId);
                    var client = new Ledger(ds.LedgerId);
                    ds.CompanyDTO = comp.GetDetails();
                    ds.LedgerDTO = client.GetDetails();
                    if (!String.IsNullOrEmpty(ds.CompanyDTO.Logo))
                    {
                        ds.CompanyDTO.Logo = docPath + "/comp/" + ds.CompanyDTO.Logo;
                    }

                    var d = new { data = ds };
                    string jsonText = JsonConvert.SerializeObject(d);
                    // To convert JSON text contained in string json into an XML node
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                    string xml = doc.InnerXml;
                    string fileName = Server.MapPath("~/Reports") + @"\purchaseorder.xslt";


                    var html = RenderXml(xml, fileName);
                    var strHtml = html.ToHtmlString();
                    HtmlToPdfConverter con = new HtmlToPdfConverter();
                    var fileBytes = con.GeneratePdf(strHtml);
                    string dwnFileName = "report.pdf";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);

                }
                if (repName == ReportNames.MAT_LOSS_RECEIPT)
                {

                    var matLossId = Convert.ToInt32(strParams[1]);
                    var billing = new Billing();
                    var ds = await billing.MatLossById(new MatLossFilterDTO { CompanyId = user.DefaultCompanyId, MatLossId = matLossId });
                    var comp = new Company(ds.CompanyId);

                    ds.Company = comp.GetDetails();
                    if (!String.IsNullOrEmpty(ds.Company.Logo))
                    {
                        ds.Company.Logo = docPath + "/comp/" + ds.Company.Logo;
                    }

                    var client = new Ledger(ds.LedgerId);

                    ds.Ledger = client.GetDetails();
                    var d = new { data = ds };
                    string jsonText = JsonConvert.SerializeObject(d);
                    // To convert JSON text contained in string json into an XML node
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                    string xml = doc.InnerXml;
                    string fileName = Server.MapPath("~/Reports") + @"\matlossreceipt.xslt";


                    var html = RenderXml(xml, fileName);
                    var strHtml = html.ToHtmlString();
                    HtmlToPdfConverter con = new HtmlToPdfConverter();
                    var fileBytes = con.GeneratePdf(strHtml);
                    string dwnFileName = "report.pdf";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);

                }
                if (repName == ReportNames.MATERIAL_ADJUST)
                {
                    var workOrderId = Convert.ToInt32(strParams[1]);
                    PopulateMaterialAdjustReportViewBag(workOrderId, user.DefaultCompanyId);
                    string xml = ViewBag.XML as string;
                    string fileName = Server.MapPath("~/Reports") + @"\materialadjust.xslt";
                    var html = RenderXml(xml, fileName);
                    var strHtml = html.ToHtmlString();
                    HtmlToPdfConverter con = new HtmlToPdfConverter();
                    var fileBytes = con.GeneratePdf(strHtml);
                    string dwnFileName = "report.pdf";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);
                }
                if (repName == ReportNames.CONTRACT_PREVIEW)
                {
                    var contractId = strParams[1];
                    var ewayBillService = new EwayBillService();
                    var contService = new Contract();
                    var ds = await contService.GetById(new BAL.DTO.ContractFilterDto { ContractId = Convert.ToInt32(contractId), CompanyId = user.DefaultCompanyId });
                    if (!String.IsNullOrEmpty(ds.Company.Logo))
                    {
                        ds.Company.Logo = docPath + "/comp/" + ds.Company.Logo;
                    }
                    var d = new { data = ds };
                    string jsonText = JsonConvert.SerializeObject(d);

                    // To convert JSON text contained in string json into an XML node
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);

                    string xml = doc.InnerXml;
                    // string xml = XmlDataSerializer<ContractViewDto>.Serialize(ds);
                    string fileName = Server.MapPath("~/Reports") + @"\contract.xslt";

                    var html = RenderXml(xml, fileName);
                    var strHtml = html.ToHtmlString();
                    HtmlToPdfConverter con = new HtmlToPdfConverter();
                    var fileBytes = con.GeneratePdf(strHtml);
                    string dwnFileName = "report.pdf";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);
                }
                if (repName == ReportNames.QUOTATION)
                {
                    string dwnFileName;
                    byte[] fileBytes;
                    var pdfGen = new PDFGenerator();
                    var quotationId = strParams[1];
                    var billing = new Billing();

                    var ds = billing.GetQuotationItems(Convert.ToInt32(quotationId));
                    string qutoteNumber = "", ledgerid = "";
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {

                            qutoteNumber = Convert.ToString(ds.Tables[0].Rows[0]["QuotationNumber"]);
                            ledgerid = Convert.ToString(ds.Tables[0].Rows[0]["LedgerId"]);

                        }
                    }
                    if (Convert.ToDouble(ds.Tables[0].Rows[0]["IGST"]) > 0)
                    {
                        ds.Tables[0].Rows[0]["IGST"] = Convert.ToDouble(ds.Tables[0].Rows[0]["IGST"]) +
                                                        Convert.ToDouble(ds.Tables[0].Rows[0]["FreightTax"]) +
                                                        Convert.ToDouble(ds.Tables[0].Rows[0]["chargesTax"]);
                    }
                    else if (Convert.ToDouble(ds.Tables[0].Rows[0]["SGST"]) > 0)
                    {
                        ds.Tables[0].Rows[0]["SGST"] = Convert.ToDouble(ds.Tables[0].Rows[0]["SGST"]) + Convert.ToDouble(ds.Tables[0].Rows[0]["FreightTax"]) / 2 +
                                                        Convert.ToDouble(ds.Tables[0].Rows[0]["chargesTax"]) / 2;
                        ds.Tables[0].Rows[0]["CGST"] = Convert.ToDouble(ds.Tables[0].Rows[0]["CGST"]) + Convert.ToDouble(ds.Tables[0].Rows[0]["FreightTax"]) / 2 +
                                                       Convert.ToDouble(ds.Tables[0].Rows[0]["chargesTax"]) / 2;
                    }

                    if (Convert.ToInt32(ds.Tables[0].Rows[0]["LedgerSiteId"]) > 0)
                    {
                        ds.Tables[0].Rows[0]["ShipAddress1"] = ds.Tables[0].Rows[0]["SiteAddress"];
                        ds.Tables[0].Rows[0]["ShipAddress2"] = "";
                        ds.Tables[0].Rows[0]["ShipCity"] = ds.Tables[0].Rows[0]["SiteCity"];
                        ds.Tables[0].Rows[0]["ShipStateName"] = ds.Tables[0].Rows[0]["SiteState"];
                        ds.Tables[0].Rows[0]["ShipZipCode"] = ds.Tables[0].Rows[0]["SiteZipCode"];

                    }
                    Product objProduct = new Product();
                    List<ProductDTO> objProducts = await objProduct.GetAll(user.DefaultCompanyId);
                    objProducts = objProducts.Where(o => o.UOM != 4).ToList();


                    var median = objProducts.Count() / 2;
                    var allItems = new List<object>();
                    for (var i = 0; i < median;)
                    {
                        var it2 = new ProductDTO();
                        var it = objProducts[i];
                        if (objProducts.Count() > (i + median))
                        {
                            it2 = objProducts[i + median];
                        }
                        var d = new
                        {
                            name1 = it.Name,
                            unit1 = it.Unit,
                            cost1 = it.LossRate,
                            name2 = it2.Name,
                            unit2 = it2.Unit,
                            cost2 = it2.LossRate,
                        };
                        allItems.Add(d);
                        i++;
                    }
                    //  var _costData = new { data = allItems };
                    pdfGen.GenerateQuotationPdf(ds, allItems, out dwnFileName, out fileBytes);
                    #region generateItemsCostSheet
                    //  string jsonText = JsonConvert.SerializeObject(_costData);
                    // To convert JSON text contained in string json into an XML node
                    //  XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText, "items");
                    //  string xml = doc.InnerXml;

                    //  string fileName = Server.MapPath("~/Reports") + @"\quotation.items.xslt";
                    // var html = RenderXml(xml, fileName);
                    //  var strHtml = html.ToHtmlString();
                    // strHtml = LoadCss(strHtml, true);
                    //HtmlToPdfConverter con = new HtmlToPdfConverter();

                    // var costSheetBytes = con.GeneratePdf(strHtml);
                    #endregion

                    // var outStream = new MemoryStream();
                    // var streamsToMerge = new List<byte[]>();
                    // streamsToMerge.Add(fileBytes);
                    //var config = new Config();
                    //var configs = config.GetConfig(user.DefaultCompanyId, "quotation", "quotation");
                    //var attachRateSheet = 2; // NO
                    //if (configs != null)
                    //{
                    //    var attRateSheet = configs.Where(o => o.Key.ToLower() == "attachratesheet").FirstOrDefault();
                    //    if (attRateSheet != null && (attRateSheet.Value == "1" || Convert.ToBoolean(attRateSheet.Value) == true))
                    //    {
                    //        streamsToMerge.Add(costSheetBytes);

                    //    }

                    //}


                    //  pdfGen.MergePDF(streamsToMerge, outStream);
                    // var outputBytes = outStream.ToArray();
                    string azureFile = qutoteNumber + ledgerid + ".pdf";
                    await azService.UploadFileAsync(user.FinYearId, user.DefaultCompanyId + "/quots", azureFile, fileBytes);
                    //  outStream.Dispose();
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);

                }
                if (repName == ReportNames.SALE_INVOICE)
                {

                    var invoiceId = strParams[1];
                    var billing = new Billing();

                    var ds = billing.PrintBill(Convert.ToInt32(invoiceId));
                    int companyId = Convert.ToInt32(ds.Tables[0].Rows[0]["CompanyId"]);
                    var comp = new Company(Convert.ToInt32(ds.Tables[0].Rows[0]["CompanyId"]));
                    var compLogo = Convert.ToString(ds.Tables[0].Rows[0]["CompanyLogo"]);

                    if (!String.IsNullOrEmpty(compLogo))
                    {
                        ds.Tables[0].Rows[0]["CompanyLogo"] = docPath + "/comp/" + compLogo;
                    }
                    //  var ds = billing.PrintBill(Convert.ToInt32(invoiceId));
                    if (ds.Tables[0].Rows[0]["IRN"] != DBNull.Value)
                    {
                        var qrCodeStr = Convert.ToString(ds.Tables[0].Rows[0]["IRN"]);
                        var codeGen = new QRCodeGenerator();

                        var qrCodeData = codeGen.CreateQrCode(Encoding.UTF8.GetBytes(qrCodeStr), QRCodeGenerator.ECCLevel.Q);
                        using (Base64QRCode qrCode = new Base64QRCode(qrCodeData))
                        {
                            // qrCode.GetGraphic(50);
                            Base64QRCode qrCodeBase64 = new Base64QRCode(qrCodeData);
                            string qrCodeImageAsBase64 = qrCodeBase64.GetGraphic(30);
                            ds.Tables[0].Rows[0]["SignedQrCode"] = "data:image/png;base64," + qrCodeImageAsBase64;// Convert.ToBase64String(qrCodeImage);
                        }
                    }
                    var config = new Config();
                    var printConfigs = config.GetConfig(companyId, "general", "print");
                    var printSignature = true;
                    var printQrCode = true;
                    var printBankDetails = false;
                    var companyDetails = new Company(companyId).GetDetails();

                    var bllingConfig = config.GetBillingConfig(companyId);
                    ds.Tables[0].Columns.Add("QrCode");
                    ds.Tables[0].Columns.Add("PrintBankDetails");
                    ds.Tables[0].Columns.Add("PrintQrCode");

                    var signaure = Convert.ToString(ds.Tables[0].Rows[0]["Signature"]);

                    if (printConfigs != null && printConfigs.Count() > 0)
                    {
                        var c = printConfigs.Where(o => o.Key.ToLower() == "signature").FirstOrDefault();
                        if (c != null)
                        {
                            printSignature = c.Value.Contains("bills");
                        }
                        c = printConfigs.Where(o => o.Key.ToLower() == "qrcode").FirstOrDefault();
                        if (c != null & !String.IsNullOrEmpty(c.Value))
                        {
                            printQrCode = c.Value.Contains("bills");
                        }
                        var printBank = bllingConfig.Where(o => o.Key == "printBankDetails").FirstOrDefault();
                        if (printBank != null && !String.IsNullOrEmpty(printBank.Value))
                        {
                            printBankDetails = Convert.ToBoolean(printBank.Value);
                        }
                        if (!String.IsNullOrEmpty(signaure) && printSignature)
                        {
                            ds.Tables[0].Rows[0]["Signature"] = docPath + "/comp/" + signaure;
                        }
                    }
                    if (printQrCode && !String.IsNullOrEmpty(companyDetails.QrCode))
                    {
                        ds.Tables[0].Rows[0]["QrCode"] = docPath + "/comp/" + companyDetails.QrCode;
                    }
                    ds.Tables[0].Rows[0]["PrintBankDetails"] = printBankDetails;
                    ds.Tables[0].Rows[0]["PrintQrCode"] = printQrCode;
                    var d = new { data = ds };

                    string jsonText = JsonConvert.SerializeObject(d);
                    // To convert JSON text contained in string json into an XML node
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);


                    string xml = doc.InnerXml;
                    var templateService = new TemplateService();
                    var template = await templateService.GetDefaultPdfTemplate(user.DefaultCompanyId, TemplateGroups.SALES, ConfigCategoryNames.SALES, ConfigSubCategoryNames.TEMPLATES);
                    string templateFile = "salebill.xslt";
                    //if (template != null)
                    //{
                    //    if (!String.IsNullOrEmpty(template.FileName))
                    //    {
                    //        templateFile = template.FileName;
                    //    }
                    //    // throw new Exception("Report template could not found.");
                    //}

                    string fileName = Server.MapPath("~/Reports") + @"\" + templateFile;
                    var html = RenderXml(xml, fileName);
                    var strHtml = html.ToHtmlString();
                    // strHtml = LoadCss(strHtml, true);
                    HtmlToPdfConverter con = new HtmlToPdfConverter();

                    con.CustomWkHtmlArgs += " --header-spacing 2"; // Adjust spacing
                                                                   // Or try these specific arguments:
                    con.CustomWkHtmlArgs = "--print-media-type --enable-smart-shrinking";
                    con.Margins.Top = 5;
                    con.Margins.Bottom = 5;

                    var fileBytes = con.GeneratePdf(strHtml);
                    string dwnFileName = "report.pdf";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);

                }
                if (repName == ReportNames.SALERETURN_INVOICE)
                {

                    var invoiceId = strParams[1];
                    var billing = new Billing();

                    var ds = billing.PrintBill(Convert.ToInt32(invoiceId));
                    var comp = new Company(Convert.ToInt32(ds.Tables[0].Rows[0]["CompanyId"]));
                    var compLogo = Convert.ToString(ds.Tables[0].Rows[0]["CompanyLogo"]);

                    if (!String.IsNullOrEmpty(compLogo))
                    {
                        ds.Tables[0].Rows[0]["CompanyLogo"] = docPath + "/comp/" + compLogo;
                    }

                    var d = new { data = ds };

                    string jsonText = JsonConvert.SerializeObject(d);
                    // To convert JSON text contained in string json into an XML node
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                    string xml = doc.InnerXml;

                    string fileName = Server.MapPath("~/Reports") + @"\salereturn-invoice.xslt";
                    var html = RenderXml(xml, fileName);
                    var strHtml = html.ToHtmlString();
                    // strHtml = LoadCss(strHtml, true);
                    HtmlToPdfConverter con = new HtmlToPdfConverter();

                    var fileBytes = con.GeneratePdf(strHtml);
                    string dwnFileName = "report.pdf";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);

                }
                if (repName == ReportNames.MAT_LOSS_BILL)
                {

                    var invoiceId = strParams[1];
                    var billing = new Billing();
                    var ds = billing.PrintBill(Convert.ToInt32(invoiceId));

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        if (ds.Tables[0].Rows[0]["CompanyLogo"] != DBNull.Value)
                        {
                            var logo = Convert.ToString(ds.Tables[0].Rows[0]["CompanyLogo"]);
                            ds.Tables[0].Rows[0]["CompanyLogo"] = docPath + "/comp/" + logo;
                        }

                    }



                    var d = new { data = ds };
                    string jsonText = JsonConvert.SerializeObject(d);
                    // To convert JSON text contained in string json into an XML node
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                    string xml = doc.InnerXml;



                    string fileName = Server.MapPath("~/Reports") + @"\matlossbill.xslt";
                    var html = RenderXml(xml, fileName);
                    var strHtml = html.ToHtmlString();
                    // strHtml = LoadCss(strHtml, true);
                    HtmlToPdfConverter con = new HtmlToPdfConverter();

                    var fileBytes = con.GeneratePdf(strHtml);
                    string dwnFileName = "report.pdf";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);

                }


                return null;
            }
            catch (Exception ex)
            {
                RentacLogger.logger.Error(ex, ex.Message);
                throw new Exception("Error while processing" + ex.Message);
            }




        }

        private async Task<byte[]> returnChallanFile(LoggedInUser user, int grnId, string headerType = "none")
        {
            var previewData = await GetReceivedChallanPreviewXmlAndXslt(grnId, user.DefaultCompanyId, headerType);
            var html = RenderXml(previewData.Item1, previewData.Item2);
            var strHtml = html.ToHtmlString();
            HtmlToPdfConverter con = new HtmlToPdfConverter();
            var template = previewData.Item3;
            if (template != null)
            {
                if (template.ApplyPrintConfig)
                {
                    con.CustomWkHtmlArgs += " --header-spacing 2"; // Adjust spacing
                                                                   //Or try these specific arguments:
                    con.CustomWkHtmlArgs = "--print-media-type --enable-smart-shrinking";
                    con.Margins.Top = 2;
                    con.Margins.Bottom = 2;
                }
                if (template.Orientation == 2)
                {
                    con.Orientation = PageOrientation.Landscape;
                }
            }
            var fileBytes = con.GeneratePdf(strHtml);
            return fileBytes;
        }

        private async Task<byte[]> IssueChallanFIle(LoggedInUser user, int workOrderId)
        {

            var previewData = await GetIssueChallanPreviewXmlAndXslt(workOrderId, user.DefaultCompanyId);
            var html = RenderXml(previewData.Item1, previewData.Item2);
            var strHtml = html.ToHtmlString();
            HtmlToPdfConverter con = new HtmlToPdfConverter();

            var template = previewData.Item3;
            if (template != null)
            {
                if (template.ApplyPrintConfig)
                {
                    con.CustomWkHtmlArgs += " --header-spacing 5"; // Adjust spacing
                                                                   //Or try these specific arguments:
                    con.CustomWkHtmlArgs = "--print-media-type --enable-smart-shrinking --dpi 300";
                    con.Margins.Top = 5;
                    con.Margins.Left = 6;
                    con.Margins.Right = 6;

                    con.Margins.Bottom = 2;
                }
                if (template.Orientation == 2)
                {
                    con.Orientation = PageOrientation.Landscape;
                }
            }
            var fileBytes = con.GeneratePdf(strHtml);
            return fileBytes;
            //  string dwnFileName = "delivery-challan.pdf";
            //return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);
        }

        private void GenerateQuotationPdf(DataSet ds, out string dwnFileName, out byte[] fileBytes)
        {

            var compLogo = Convert.ToString(ds.Tables[0].Rows[0]["CompanyLogo"]);
            dwnFileName = "";
            string qutoteNumber = "", ledgerid = "";
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dwnFileName = Convert.ToString(ds.Tables[0].Rows[0]["QuotationNumber"]) + ".pdf";


                }
            }
            if (!String.IsNullOrEmpty(compLogo))
            {
                ds.Tables[0].Rows[0]["CompanyLogo"] = docPath + "/comp/" + compLogo;
            }
            var d = new { data = ds };
            string jsonText = JsonConvert.SerializeObject(d);
            // To convert JSON text contained in string json into an XML node
            XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
            string xml = doc.InnerXml;
            string fileName = Server.MapPath("~/Reports") + @"\quotation.xslt";

            var html = RenderXml(xml, fileName);
            var strHtml = html.ToHtmlString();
            HtmlToPdfConverter con = new HtmlToPdfConverter();
            fileBytes = con.GeneratePdf(strHtml);

            //string azureFile = qutoteNumber + ledgerid + ".pdf";
            ////   dto.Body = dto.Body.Replace("{link}", AzureStorageService.ContainerBaseUrl + bill.FinYearId + "/" + bill.CompanyId + "/quots/" + fileName);
            //var user = new LoggedInUser();

            //azService.UploadFileAsync(user.FinYearId, user.DefaultCompanyId + "/quots/", azureFile, fileBytes).
            //    ContinueWith(t =>
            //        TaskContinuationOptions.OnlyOnFaulted
            //);


        }

        string LoadCss(string html)
        {


            var cssPath = Server.MapPath("~/Content") + @"\print.css";
            var printCss = System.IO.File.ReadAllText(cssPath);
            printCss = "<style>" + printCss + "</style>";
            html = html.Replace(" #pdf", printCss);
            html = html.Replace(" #preview", "");

            return html;
        }

        private async Task<Tuple<string, string, Template>> GetIssueChallanPreviewXmlAndXslt(int workOrderId, int companyId, string headerType = "none")
        {
            WorkOrder wOrder = new WorkOrder(0);
            DataSet mainDS = wOrder.ItemIssuedForPrint(workOrderId, companyId);
            if (mainDS.Tables.Count == 0 || mainDS.Tables[0].Rows.Count == 0)
            {
                throw new Exception("Challan not found.");
            }

            companyId = Convert.ToInt32(mainDS.Tables[0].Rows[0]["CompanyId"]);
            var challanType = Convert.ToInt16(mainDS.Tables[0].Rows[0]["ChallanType"]);

            Template template = await GetIssueChallanTemplate(companyId, challanType);
            if (template == null)
            {
                throw new Exception("Report template could not found.");
            }

            DataSet headerDataSet = wOrder.GetChallanReportHeader(workOrderId, 1);
            var config = new Config();
            var printConfigs = config.GetConfig(companyId, "general", "print");
            var challanCfgList = config.GetConfig(companyId, "issuechallan", "issuechallan");
            var challanConfigs = challanCfgList != null ? challanCfgList.ToList() : new List<ConfigDTO>();

            var printSignature = true;
            if (printConfigs != null && printConfigs.Count > 0)
            {
                var c = printConfigs.Where(o => o.Key.ToLower() == "signature").FirstOrDefault();
                if (c != null)
                {
                    printSignature = c.Value.Contains("challans");
                }
            }

            if (headerDataSet.Tables.Count > 0 && headerDataSet.Tables[0].Rows.Count > 0)
            {
                if (!headerDataSet.Tables[0].Columns.Contains("PrintRateType"))
                {
                    headerDataSet.Tables[0].Columns.Add("PrintRateType");
                }
                headerDataSet.Tables[0].Rows[0]["PrintRateType"] = "sale";
                if (headerDataSet.Tables[0].Rows[0]["CompanyLogo"] != DBNull.Value)
                {
                    var logo = Convert.ToString(headerDataSet.Tables[0].Rows[0]["CompanyLogo"]);
                    if (!String.IsNullOrEmpty(logo))
                    {
                        headerDataSet.Tables[0].Rows[0]["CompanyLogo"] = docPath + "/comp/" + logo;
                    }
                }
                var signature = Convert.ToString(headerDataSet.Tables[0].Rows[0]["Signature"]);
                if (!String.IsNullOrEmpty(signature) && printSignature)
                {
                    headerDataSet.Tables[0].Rows[0]["Signature"] = docPath + "/comp/" + signature;
                }
                var printRateType = challanConfigs.Where(o => o.Key.ToLower() == "printratetype").FirstOrDefault();
                if (printRateType != null && !String.IsNullOrEmpty(printRateType.Value))
                {
                    headerDataSet.Tables[0].Rows[0]["PrintRateType"] = printRateType.Value;
                }
            }

            if (printConfigs != null)
            {
                challanConfigs.AddRange(printConfigs);
            }

            string jsonText = JsonConvert.SerializeObject(challanConfigs);
            string wrappedJson = "{\"root\":" + jsonText + "}";
            XmlDocument configDoc = JsonConvert.DeserializeXmlNode(wrappedJson, "config");

            string xml = mainDS.GetXml();
            string headerXML = headerDataSet.GetXml();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlDocument headerDOC = new XmlDocument();
            headerDOC.LoadXml(headerXML);
            XmlNode node = doc.CreateNode(XmlNodeType.Element, "Header", "");
            XmlNode configNode = doc.CreateNode(XmlNodeType.Element, "Config", "");
            if (configDoc != null && configDoc.FirstChild != null)
            {
                configNode.InnerXml = configDoc.FirstChild.InnerXml;
            }

            double total = Convert.ToDouble(mainDS.Tables[0].Rows[0]["ChallanTotal"]);
            XmlElement elem = doc.CreateElement("Rupees");
            elem.InnerText = Utils.ConvertNumbertoWords(total.ToString());

            if (headerDOC.FirstChild != null && headerDOC.FirstChild.FirstChild != null)
            {
                node.InnerXml = headerDOC.FirstChild.FirstChild.InnerXml;
            }
            node.AppendChild(elem);
            var headerTypeNode = doc.CreateElement("headerType");
            headerTypeNode.InnerText = headerType;
            node.AppendChild(headerTypeNode);
            doc.FirstChild.PrependChild(node);
            doc.FirstChild.AppendChild(configNode);
            xml = doc.OuterXml;

            if (challanType == 10)
            {
                string baseName = template.FileName.Split('.')[0];
                var hireFileName = baseName + "_hire.xsl";
                var repFilePath = Server.MapPath("~/Reports") + @"\" + hireFileName;
                if (System.IO.File.Exists(repFilePath))
                {
                    template.FileName = hireFileName;
                }
            }

            string xsltPath = Server.MapPath("~/Reports") + @"\" + template.FileName;
            return Tuple.Create(xml, xsltPath, template);

        }
        async Task<Template> GetIssueChallanTemplate(int companyId, int challanType)
        {
            var templateGroup = TemplateGroups.ISSUECHALLAN;
            var configCategory = ConfigCategoryNames.ISSUECHALLAN;
            if (challanType == (short)ChallanType.LIFT_DELIVERY)
            {
                templateGroup = TemplateGroups.CONTRACTDELIVERYCHALLAN;
                configCategory = ConfigCategoryNames.CONTRACTDELIVERYCHALLAN;
            }
            else if (challanType == (short)ChallanType.SALE)
            {
                templateGroup = TemplateGroups.SALESDELIVERYCHALLAN;
                configCategory = ConfigCategoryNames.SALESDELIVERYCHALLAN;
            }

            var templateService = new TemplateService();
            var template = await templateService.GetDefaultPdfTemplate(companyId, templateGroup,
               configCategory, ConfigSubCategoryNames.TEMPLATES);
            return template;
        }
        private async Task<Tuple<string, string, Template>> GetReceivedChallanPreviewXmlAndXslt(int grnId, int companyId, string headerType = "none")
        {
            WorkOrder wOrder = new WorkOrder(0);
            DataSet mainDS = wOrder.ItemReceived_Report(grnId);
            if (mainDS.Tables.Count == 0 || mainDS.Tables[0].Rows.Count == 0)
            {
                throw new Exception("Receipt not found.");
            }

            GRN grn = new GRN();
            DataSet headerDataSet = grn.GRNHeader(grnId);

            companyId = Convert.ToInt32(mainDS.Tables[0].Rows[0]["CompanyId"]);
            var config = new Config();
            var printConfigs = config.GetConfig(companyId, "general", "print");
            var printSignature = true;
            if (printConfigs != null && printConfigs.Count > 0)
            {
                var sig = printConfigs.Where(o => o.Key.ToLower() == "signature").FirstOrDefault();
                if (sig != null)
                {
                    printSignature = sig.Value.Contains("challans");
                }
            }
            if (headerDataSet.Tables.Count > 0 && headerDataSet.Tables[0].Rows.Count > 0)
            {
                if (headerDataSet.Tables[0].Rows[0]["CompanyLogo"] != DBNull.Value)
                {
                    var logo = Convert.ToString(headerDataSet.Tables[0].Rows[0]["CompanyLogo"]);
                    if (!String.IsNullOrEmpty(logo))
                    {
                        headerDataSet.Tables[0].Rows[0]["CompanyLogo"] = docPath + "/comp/" + logo;
                    }
                }
                var signature = Convert.ToString(headerDataSet.Tables[0].Rows[0]["Signature"]);
                if (!String.IsNullOrEmpty(signature) && printSignature)
                {
                    headerDataSet.Tables[0].Rows[0]["Signature"] = docPath + "/comp/" + signature;
                }
            }

            string xml = mainDS.GetXml();
            string headerXML = headerDataSet.GetXml();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlDocument headerDOC = new XmlDocument();
            headerDOC.LoadXml(headerXML);
            XmlNode node = doc.CreateNode(XmlNodeType.Element, "Header", "");
            if (headerDOC.FirstChild != null && headerDOC.FirstChild.FirstChild != null)
            {
                node.InnerXml = headerDOC.FirstChild.FirstChild.InnerXml;
            }
            var challanConfigs = config.GetConfig(companyId, "RECEIVINGCHALLAN", "RECEIVINGCHALLAN");
            string jsonText = JsonConvert.SerializeObject(challanConfigs);

            string wrappedJson = "{\"root\":" + jsonText + "}";
            XmlDocument configDoc = JsonConvert.DeserializeXmlNode(wrappedJson, "Config");
            XmlNode configNode = doc.CreateNode(XmlNodeType.Element, "Config", "");
            if (configDoc.FirstChild != null)
            {
                configNode.InnerXml = configDoc.FirstChild.InnerXml;
            }

            //if (headerDataSet.Tables.Count > 1)
            //{
            //    var writer = new StringWriter();
            //    headerDataSet.Tables[1].WriteXml(writer);
            //    configNode.InnerXml = writer.ToString();
            //    doc.FirstChild.PrependChild(configNode);
            //}
            var headerTypeNode = doc.CreateElement("headerType");
            headerTypeNode.InnerText = headerType;

            node.PrependChild(headerTypeNode);

            doc.FirstChild.PrependChild(node);

       
            doc.FirstChild.AppendChild(configNode);
            xml = doc.OuterXml;

            var chType = Convert.ToInt16(mainDS.Tables[0].Rows[0]["TypeId"]);
            var templateService = new TemplateService();
            var templateGroup = TemplateGroups.RETURNS;
            var configCategory = ConfigCategoryNames.RETURNS;
            if (chType == (short)ChallanType.CONTRACT_RETURN)
            {
                templateGroup = TemplateGroups.CONTRACTRETURNS;
                configCategory = ConfigCategoryNames.CONTRACTRETURNS;
            }
            else if (chType == (short)ChallanType.SALE)
            {
                templateGroup = TemplateGroups.SALESRETURNS;
                configCategory = ConfigCategoryNames.SALESRETURNS;
            }

            var template = await templateService.GetDefaultPdfTemplate(companyId, templateGroup, configCategory, ConfigSubCategoryNames.TEMPLATES);
            string fileName = "recItem-rent.xsl";
            if (template != null)
            {
                fileName = template.FileName;
            }
            if (chType == 13)
            {
                fileName = "recItem-rent_unhire.xsl";
            }

            string xsltPath = Server.MapPath("~/Reports") + @"\" + fileName;
            return Tuple.Create(xml, xsltPath, template);
        }

        private void PopulateMaterialAdjustReportViewBag(int workOrderId, int defaultCompanyId)
        {
            var wo = new WorkOrder(0);
            DataSet dsAdj = wo.MatAdjustById(workOrderId);
            if (dsAdj == null || dsAdj.Tables.Count == 0 || dsAdj.Tables[0].Rows.Count == 0)
            {
                throw new Exception("Material adjustment not found.");
            }

            DataTable tbl = dsAdj.Tables[0];
            DataRow headerRow = tbl.Rows[0];
            int ledgerId = MatAdjCellInt(headerRow, "LedgerId");
            int companyId = defaultCompanyId;
            if (tbl.Columns.Contains("CompanyId") && headerRow["CompanyId"] != DBNull.Value)
            {
                int cid = MatAdjCellInt(headerRow, "CompanyId");
                if (cid > 0)
                {
                    companyId = cid;
                }
            }

            string number = MatAdjCellString(headerRow, "Number", "WorkOrderNumber", "ChallanNumber");
            string siteAddress = MatAdjCellString(headerRow, "SiteAddress", "LedgerSiteAddress");
            string workOrderDateStr = FormatMatAdjDate(headerRow);

            var issueItems = new List<MaterialAdjustIssuePrintRow>();
            var receiveItems = new List<MaterialAdjustReceivePrintRow>();
            foreach (DataRow r in tbl.Rows)
            {
                if (MatAdjCellInt(r, "WorkOrderItemId") > 0)
                {
                    issueItems.Add(new MaterialAdjustIssuePrintRow
                    {
                        Product = MatAdjCellString(r, "Product", "Item", "ProductName"),
                        ExcessQty = MatAdjCellString(r, "ExcessQty", "ExcessQuantity"),
                        SentQty = MatAdjCellString(r, "SentQty", "Quantity", "Qty")
                    });
                }
                if (MatAdjCellInt(r, "GRNItemId") > 0)
                {
                    receiveItems.Add(new MaterialAdjustReceivePrintRow
                    {
                        Product = MatAdjCellString(r, "Product", "Item", "ProductName"),
                        Quantity = MatAdjCellString(r, "Quantity", "Qty", "RecQty")
                    });
                }
            }

            var comp = new Company(companyId);
            CompanyDTO companyDto = comp.GetDetails();
            if (!String.IsNullOrEmpty(companyDto.Logo))
            {
                companyDto.Logo = docPath + "/comp/" + companyDto.Logo;
            }

            var client = new Ledger(ledgerId);
            LedgerDTO ledgerDto = client.GetDetails();

            var printData = new MaterialAdjustPrintData
            {
                Company = companyDto,
                Ledger = ledgerDto,
                Number = number,
                WorkOrderDate = workOrderDateStr,
                SiteAddress = siteAddress,
                IssueItems = issueItems,
                ReceiveItems = receiveItems
            };

            var d = new { data = printData };
            string jsonText = JsonConvert.SerializeObject(d);
            XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
            ViewBag.XML = doc.InnerXml;
        }

        private static string FormatMatAdjDate(DataRow headerRow)
        {
            if (headerRow.Table.Columns.Contains("WorkOrderDate") && headerRow["WorkOrderDate"] != DBNull.Value)
            {
                object o = headerRow["WorkOrderDate"];
                if (o is DateTime)
                {
                    return ((DateTime)o).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
            }
            string raw = MatAdjCellString(headerRow, "WorkOrderDate", "VoucherDate", "Date");
            if (String.IsNullOrWhiteSpace(raw))
            {
                return "";
            }
            DateTime dt;
            if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)
                || DateTime.TryParse(raw, CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
            {
                return dt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            return raw;
        }

        private static string MatAdjCellString(DataRow r, params string[] columnNames)
        {
            foreach (string name in columnNames)
            {
                if (r.Table.Columns.Contains(name) && r[name] != DBNull.Value && r[name] != null)
                {
                    return Convert.ToString(r[name], CultureInfo.InvariantCulture).Trim();
                }
            }
            return "";
        }

        private static int MatAdjCellInt(DataRow r, params string[] columnNames)
        {
            string s = MatAdjCellString(r, columnNames);
            int v;
            int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out v);
            return v;
        }

        public HtmlString RenderXml(string xml, string xsltPath)
        {
            XsltArgumentList args = new XsltArgumentList();
            var utils = new Utils();
            args.AddExtensionObject("urn:util-format", utils);
            // Create XslCompiledTransform object to loads and compile XSLT file.  
            XslCompiledTransform tranformObj = new XslCompiledTransform();

            tranformObj.Load(xsltPath);

            // Create XMLReaderSetting object to assign DtdProcessing, Validation type  
            XmlReaderSettings xmlSettings = new XmlReaderSettings();
            xmlSettings.DtdProcessing = DtdProcessing.Parse;
            xmlSettings.ValidationType = ValidationType.DTD;

            // Create XMLReader object to Transform xml value with XSLT setting   
            using (XmlReader reader = XmlReader.Create(new StringReader(xml), xmlSettings))
            {
                StringWriter writer = new StringWriter();
                tranformObj.Transform(reader, args, writer);
                string html = writer.ToString();
                html = LoadCss(html);
                // Generate HTML string from StringWriter  
                HtmlString htmlString = new HtmlString(html);

                return htmlString;
            }
        }
    }
    public class XmlDataSerializer<T> where T : class
    {
        public static string Serialize(T obj)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(T));
            using (var sww = new StringWriter())
            {
                using (XmlTextWriter writer = new XmlTextWriter(sww) { Formatting = System.Xml.Formatting.Indented })
                {
                    xsSubmit.Serialize(writer, obj);
                    return sww.ToString();
                }
            }
        }
    }
}