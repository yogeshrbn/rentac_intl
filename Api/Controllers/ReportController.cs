using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BAL.Objects;
using BAL.DTO;
using System.IO;
using System.Data;
using PdfSharp.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using PdfSharp;
using OfficeOpenXml;
using FarmaAPI.Helper;
using Spire.Pdf.Widget;
using Spire.Pdf.Fields;
using Spire.Pdf.HtmlConverter;
using Spire.Pdf;
using System.Drawing;
using System.Threading;
using PdfDocument = Spire.Pdf.PdfDocument;
using Newtonsoft;
using Omu.ValueInjecter;
using System.Web;
using System.Threading.Tasks;
using System.Globalization;

namespace FarmaAPI.Controllers
{
    [Authorize]
    public class ReportController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage PendingPayments([FromBody] ReportDTO dto)
        {
            Report objReports = new Report();
            if (dto.Site == "All")
            {
                dto.Site = null;
            }
            if (dto.JobNumber == "All")
            {
                dto.JobNumber = null;
            }
            return Request.CreateResponse(HttpStatusCode.OK, objReports.PendingPayments(dto.DaysLeft, dto.Site, dto.JobNumber));
        }
        //[HttpPost]
        //public HttpResponseMessage SiteWiseInventory([FromBody] ReportDTO dto)
        //{
        //    Report objReports = new Report();
        //    if (dto.JobNumber == "All")
        //    {
        //        dto.JobNumber = null;
        //    }
        //    if (dto.Site == "All")
        //    {
        //        dto.Site = null;
        //    }
        //    return Request.CreateResponse(HttpStatusCode.OK, objReports.SiteWiseInventory(dto.Site, dto.JobNumber));

        //}
        //[HttpPost]
        //public HttpResponseMessage SiteWiseInventorySummary([FromBody] ReportDTO dto)
        //{
        //    Report objReports = new Report();
        //    if (dto.JobNumber == "All")
        //    {
        //        dto.JobNumber = null;
        //    }
        //    if (dto.Site == "All")
        //    {
        //        dto.Site = null;
        //    }
        //    return Request.CreateResponse(HttpStatusCode.OK, objReports.SiteWiseInventorySummary(dto.Site, dto.JobNumber));
        //}
        //[HttpPost]
        //public HttpResponseMessage PaymentReceived()
        //{
        //    return Request.CreateResponse(HttpStatusCode.OK, Report.PaymentReceived());
        //}

        //[HttpPost]
        //public HttpResponseMessage DownloadInventory([FromBody] ReportDTO dto)
        //{
        //    Report objReports = new Report();
        //    if (dto.JobNumber == "All")
        //    {
        //        dto.JobNumber = null;
        //    }
        //    if (dto.Site == "All")
        //    {
        //        dto.Site = null;
        //    }
        //    ExcelPackage ep = new ExcelPackage();
        //    ExcelWorksheet workSheet = ep.Workbook.Worksheets.Add("detail");
        //    List<ReportDTO> reports = objReports.SiteWiseInventory(dto.Site, dto.JobNumber);
        //    // workSheet.Cells["A1"].LoadFromCollection(reports, true);
        //    string filePath = System.Web.HttpContext.Current.Server.MapPath("~/temp/");
        //    string fileName = Guid.NewGuid().ToString() + ".xlsx";
        //    filePath = filePath + fileName;
        //    try
        //    {
        //        PrepareSiteInventoryReport(workSheet, reports);
        //        FileInfo fInfo = new FileInfo(filePath);
        //        ep.SaveAs(fInfo);
        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK, fileName);
        //}

        //[HttpGet]
        //public HttpResponseMessage DownloadReportFile()
        //{
        //    Report objReports = new Report();
        //    ExcelPackage ep = new ExcelPackage();
        //    ExcelWorksheet workSheet = ep.Workbook.Worksheets.Add("detail");
        //    List<ReportDTO> reports = objReports.SiteWiseInventory(null, null);
        //    //workSheet.Cells["A1"].LoadFromCollection(reports);
        //    string filePath = System.Web.HttpContext.Current.Server.MapPath("~/temp/");
        //    string fileName = Guid.NewGuid().ToString() + ".xlsx";
        //    filePath = filePath + fileName;
        //    try
        //    {
        //        //PrepareSiteInventoryReport(workSheet, reports);
        //        FileInfo fInfo = new FileInfo(filePath);
        //        ep.SaveAs(fInfo);
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return Request.CreateResponse(HttpStatusCode.OK, fileName);

        //}

        void PrepareSiteInventoryReport(ExcelWorksheet sheet, List<ReportDTO> data)
        {
            if (data.Count == 0)
            {
                return;
            }
            sheet.Cells["D2:O2"].Merge = true;
            sheet.Cells["D2"].Value = data[0].Company;
            sheet.Cells["D2"].Style.Font.Size = 30;
            StyleWorkSheet(sheet, "D2:O2");
            sheet.Cells["D2:O4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells["D3:O3"].Merge = true;
            sheet.Cells["D3"].Value = data[0].Client;
            sheet.Cells["D3"].Style.Font.Size = 22;
            StyleWorkSheet(sheet, "D3:O3");
            sheet.Cells["D4:O4"].Merge = true;
            sheet.Cells["D4"].Value = data[0].Site;
            sheet.Cells["D4"].Style.Font.Size = 22;
            StyleWorkSheet(sheet, "D4:O4");

            #region FillData

            var columns = (from x in data
                           select new { column = x.Product }).Distinct().OrderBy(o => o.column);

            sheet.Cells["D5"].Value = "S#";
            sheet.Cells["E5"].Value = "Date";
            int colIndex = 6;
            #region SentItems
            var dates = (from x in data
                         select new { Date = x.SentDate }).Distinct();
            sheet.Cells["E6"].LoadFromCollection(dates);
            int rowIndex = 6;

            foreach (var date in dates)
            {
                colIndex = 6;
                foreach (var column in columns)
                {
                    sheet.Cells[5, colIndex].Value = column.column;
                    var values = data.Where(o => o.Product == column.column && o.SentDate == date.Date).ToList();
                    if (values.Count > 0)
                    {
                        sheet.Cells[rowIndex, colIndex].Value = values.FirstOrDefault().SentQty;
                    }
                    sheet.Cells[rowIndex, 5].Style.Numberformat.Format = "dd/mm/yyyy";
                    sheet.Cells[rowIndex, 4].Value = rowIndex - 5;
                    colIndex++;
                }

                rowIndex++;
            }
            //rowIndex++;
            //colIndex++;
            sheet.SelectedRange[rowIndex, 4, rowIndex, columns.Count() + 5].Merge = true;

            //sheet.Cells[rowIndex, colIndex].Merge = true;
            sheet.SelectedRange[rowIndex, 4, rowIndex, columns.Count() + 5].Value = "RETURN";
            sheet.SelectedRange[rowIndex, 4, rowIndex, columns.Count() + 5].Style.Font.Bold = true;
            #endregion
            rowIndex++;
            var recDates = (from x in data
                            select new { Date = x.ReceivingDate }).Distinct();
            sheet.Cells[rowIndex, 5].LoadFromCollection(recDates);


            foreach (var date in recDates)
            {
                colIndex = 6;
                foreach (var column in columns)
                {

                    var values = data.Where(o => o.Product == column.column && o.ReceivingDate == date.Date).ToList();
                    if (values.Count > 0)
                    {
                        sheet.Cells[rowIndex, colIndex].Value = values.FirstOrDefault().ReceivingQty;
                    }
                    sheet.Cells[rowIndex, 5].Style.Numberformat.Format = "dd/mm/yyyy";
                    //sheet.Cells[rowIndex, 4].Value = rowIndex - 5;
                    colIndex++;
                }
                rowIndex++;

            }
            Setborder(sheet, 5, 4, rowIndex, columns.Count() + 5);

            sheet.Cells.AutoFitColumns();




            #endregion
        }


        void Setborder(ExcelWorksheet sheet, int startRow, int startCol, int endRow, int endCol)
        {
            for (int i = startRow; i <= endRow; i++)
            {
                for (int j = startCol; j <= endCol; j++)
                {
                    sheet.Cells[i, j].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                }
            }
        }
        void StyleWorkSheet(ExcelWorksheet sheet, String address)
        {
            sheet.Cells[address].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        }

        //[HttpPost]
        //public HttpResponseMessage ClosedSites([FromBody] ReportDTO dto)
        //{

        //    Report objReport = new Report();
        //    return Request.CreateResponse(HttpStatusCode.OK, objReport.ClosedSites(dto.FromDate, dto.ToDate));

        //}
        //[HttpGet]
        //public HttpResponseMessage DashboardSummary()
        //{
        //    Report objReport = new Report();
        //    return Request.CreateResponse(HttpStatusCode.OK, objReport.DashboardSummary().FirstOrDefault());

        //}
        [HttpPost]
        public HttpResponseMessage DailyInOutTransactions([FromBody] FilterCriteria dto)
        {

            Report objReport = new Report();
            string from = Utils.FormatDate(dto.From).ToShortDateString();
            string to = Utils.FormatDate(dto.To).ToShortDateString();
            dto.RbnClientId = new LoggedInUser().RbnClientId;

            return Request.CreateResponse(HttpStatusCode.OK, objReport.DailyInOutTransactions(dto.LedgerId, dto.LedgerSiteId, new LoggedInUser().DefaultCompanyId, from, to));

        }

        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage PrintReport([FromBody] ReportModel dto)
        {


            string reportName = Guid.NewGuid().ToString();
            // string xml = GetReportXML(reportName, dto);
            ApiMessage message = new ApiMessage();

            ReportUtility rep = new ReportUtility();
            string html = dto.Html;// rep.getReportHTML(GetReportTemplate(reportName), xml.ToString());
            try
            {
                Byte[] res = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    var config = new PdfGenerateConfig()
                    {
                        MarginBottom = 70,
                        MarginLeft = 20,
                        MarginRight = 20,
                        MarginTop = 70,
                        PageSize = PageSize.A4
                    };

                    string css = System.Web.Hosting.HostingEnvironment.MapPath("~/content/resume/tempalate2.css");
                    string cssContent = "";
                    if (File.Exists(css))
                    {
                        cssContent = File.ReadAllText(css);
                    }
                    var cssData = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.ParseStyleSheet(cssContent, true);
                    var pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(html, config, cssData);
                    var hpdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                    // htoddf.CustomWkHtmlArgs = " --javascript-debug --javascript-delay 3000 ";
                    //res = htoddf.GeneratePdfFromFile(dto.Url, null);

                    // res = htoddf.GeneratePdf(dto.Url);
                    //SaveFile(dto.Url);
                    // pdf.Save(ms);
                    var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/temp");
                    var fullPath = mappedPath + "/resume.pdf";
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                    pdf.Save(fullPath);
                    // res = ms.ToArray();
                }
                message.Code = ApiMessageCodes.SUCCESS;
                message.Data = res;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
                message.Description = ex.StackTrace;
            }

            return Request.CreateResponse(HttpStatusCode.OK, message);
        }
        void SaveFile(string url)
        {
            //Create a pdf document.
            PdfDocument doc = new PdfDocument();

            PdfPageSettings setting = new PdfPageSettings();

            setting.Size = new SizeF(1000, 1000);
            setting.Margins = new Spire.Pdf.Graphics.PdfMargins(20);

            PdfHtmlLayoutFormat htmlLayoutFormat = new PdfHtmlLayoutFormat();
            htmlLayoutFormat.IsWaiting = true;

            //  String url = "https://www.wikipedia.org/";

            Thread thread = new Thread(() =>
            { doc.LoadFromHTML(url, false, false, false, setting, htmlLayoutFormat); });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            //Save pdf file.
            var file = System.Web.Hosting.HostingEnvironment.MapPath("~/temp/") + "output-resume.pdf";
            if (File.Exists(file))
            {
                File.Delete(file);
            }
            doc.SaveToFile(file);
            doc.Close();


        }
        [HttpPost]
        public HttpResponseMessage BillOverDueSummary(BillOverDueSummaryFilter filter)
        {
            Report objReport = new Report();
            filter.CompanyId = new LoggedInUser().DefaultCompanyId;
            var data = objReport.BillOverDueSummary(filter);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpPost]
        public HttpResponseMessage NewCustomers(NewCustomersReportsFilterDTO filter)
        {
            Report objReport = new Report();

            var reportFilter = new ReportFilter();
            reportFilter.InjectFrom(filter);
            //filter.DateFrom = Utils.FormatDate(filter.DateFrom).ToShortDateString();
            //filter.DateTo = Utils.FormatDate(filter.DateTo).ToShortDateString();

            //reportFilter.DateTo = DateTime.Today.ToShortDateString();
            //reportFilter.DateFrom = DateTime.Today.AddMonths(-6).ToShortDateString();
            reportFilter.CompanyId = new LoggedInUser().DefaultCompanyId;
            reportFilter.FinYearId = new LoggedInUser().FinYearId;
            var data = objReport.NewCustomers(reportFilter);
            var x = Utils.FormatDate(filter.DateTo) - Utils.FormatDate(filter.DateFrom);
            var totalMonths = (int)(x.TotalDays / 30);
            for (var i = 0; i < totalMonths; i++)
            {
                var sDate = Utils.FormatDate(filter.DateFrom).AddMonths(i);//.AddMonths(-totalMonths).AddMonths(i);
                var exists = data.FirstOrDefault(o => o.Month == sDate.Month && o.Year == sDate.Year);
                if (exists == null)
                {
                    data.Add(new NewCustomersDTO
                    {
                        Month = sDate.Month,
                        Year = sDate.Year,
                        MonthYear = sDate.ToString("MMM yy"),
                        Customers = 0
                    });
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, data.OrderBy(o => o.Year).ThenBy(o => o.Month));
        }


        [HttpPost]
        public async Task<HttpResponseMessage> TotalStats(NewCustomersReportsFilterDTO filter)
        {
            Report objReport = new Report();

            var reportFilter = new ReportFilter();
            reportFilter.InjectFrom(filter);
            //reportFilter.DateTo = DateTime.Today.ToShortDateString();
            //reportFilter.DateFrom = DateTime.Today.AddMonths(-6).ToShortDateString();
            //filter.DateFrom = Utils.FormatDate(filter.DateFrom).ToShortDateString();
            //filter.DateTo = Utils.FormatDate(filter.DateTo).ToShortDateString();

            reportFilter.CompanyId = new LoggedInUser().DefaultCompanyId;
            reportFilter.FinYearId = new LoggedInUser().FinYearId;
            var data = objReport.ClientDashboardDTO(reportFilter);
            var fundSummary = objReport.FundSummary(reportFilter);
            var stock = objReport.StockSummary(reportFilter);
            var itemsOnrent = objReport.TopItemsOnRent(reportFilter).Take(5);

            var billingSummary = await objReport.MonthlyBillingSummaryForDashBoard(reportFilter);
            // object _billData = null;
            object _newBillData = null;
            if (billingSummary != null)
            {
                //_billData = billingSummary.Select(o => new
                //{
                //    Total = o.Amount/1000,
                //    MonthYear = (new DateTime(o.Year, o.Month, 1)).ToString("MMM yy")

                //});
                var x = Utils.FormatDate(filter.DateTo) - Utils.FormatDate(filter.DateFrom);
                var totalMonths = (int)(x.TotalDays / 30);
                var _billData = billingSummary.ToList();

                for (var i = 0; i < totalMonths; i++)
                {
                    var sDate = Utils.FormatDate(filter.DateFrom).AddMonths(i);//.AddMonths(-totalMonths).AddMonths(i);
                    var exists = _billData.FirstOrDefault(o => o.Month == sDate.Month && o.Year == sDate.Year);
                    if (exists == null)
                    {
                        _billData.Add(new ReportDTO
                        {
                            Month = sDate.Month,
                            Year = sDate.Year,
                            MonthYear = sDate.ToString("MMM yy"),
                            Amount = 0
                        });
                    }
                }
                _newBillData = _billData.Select(o => new
                {
                    Sales = o.TotalSaleBill,
                    Rent = o.TotalRentBill,
                    MonthYear = (new DateTime(o.Year, o.Month, 1)).ToString("MMM yy")

                });

            }

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                stats = data,
                fundSummary = fundSummary,
                stock = stock,
                topItemsOnRent = itemsOnrent,
                billingSummary = _newBillData
            });
        }

        #region gstrReports

      
        [HttpPost]
        public async Task<IHttpActionResult> GstSalesTaxReport([FromBody] GSTRFilterDto filter)
        {
            ApiMessage msg = new ApiMessage();
            try
            {
                Report objReport = new Report();
                filter.CompanyId = new LoggedInUser().DefaultCompanyId;
                msg.Data = await objReport.GstTaxSales(filter);
                msg.Code = ApiMessageCodes.SUCCESS;
                return Ok(msg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //  return Request.CreateResponse(HttpStatusCode.OK, new { stats = data, fundSummary = fundSummary, stock = stock, topItemsOnRent = itemsOnrent });
        }
        [HttpPost]
        public async Task<IHttpActionResult> GstPurchaseTaxReport([FromBody] GSTRFilterDto filter)
        {
            ApiMessage msg = new ApiMessage();
            try
            {
                Report objReport = new Report();
                filter.CompanyId = new LoggedInUser().DefaultCompanyId;
                msg.Data = await objReport.GstTaxPurchase(filter);
                msg.Code = ApiMessageCodes.SUCCESS;
                return Ok(msg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //  return Request.CreateResponse(HttpStatusCode.OK, new { stats = data, fundSummary = fundSummary, stock = stock, topItemsOnRent = itemsOnrent });
        }
        public async Task<IHttpActionResult> TrialBalance([FromBody] FilterCriteria filter)
        {
            ApiMessage msg = new ApiMessage();
            try
            {
                Ledger leder = new Ledger();
                filter.CompanyId = new LoggedInUser().DefaultCompanyId;
                msg.Data = await leder.TrialBalance(filter);
                msg.Code = ApiMessageCodes.SUCCESS;
                return Ok(msg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //  return Request.CreateResponse(HttpStatusCode.OK, new { stats = data, fundSummary = fundSummary, stock = stock, topItemsOnRent = itemsOnrent });
        }
        [HttpPost]
        public async Task<IHttpActionResult> PnlStatement([FromBody] FilterCriteria filter)
        {
            ApiMessage msg = new ApiMessage();
            try
            {
                Report objReport = new Report();
                filter.CompanyId = new LoggedInUser().DefaultCompanyId;
                var _data = await objReport.PnlStatement(filter);



                msg.Data = _data;
                msg.Code = ApiMessageCodes.SUCCESS;
                return Ok(msg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //  return Request.CreateResponse(HttpStatusCode.OK, new { stats = data, fundSummary = fundSummary, stock = stock, topItemsOnRent = itemsOnrent });
        }
        [HttpPost]
        public async Task<IHttpActionResult> BalanceSheet([FromBody] FilterCriteria filter)
        {
            ApiMessage msg = new ApiMessage();
            try
            {
                Report objReport = new Report();
                filter.CompanyId = new LoggedInUser().DefaultCompanyId;
                msg.Data = await objReport.BalanceSheet(filter);
                msg.Code = ApiMessageCodes.SUCCESS;
                return Ok(msg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //  return Request.CreateResponse(HttpStatusCode.OK, new { stats = data, fundSummary = fundSummary, stock = stock, topItemsOnRent = itemsOnrent });
        }
        #endregion
        //[HttpPost]
        //public HttpResponseMessage StockSummary()
        //{
        //    Report objReport = new Report();
        //    var reportFilter = new ReportFilter();
        //    reportFilter.CompanyId = new LoggedInUser().DefaultCompanyId;
        //    reportFilter.FinYearId = new LoggedInUser().FinYearId;
        //    var data = objReport.StockSummary(reportFilter);
        //    return Request.CreateResponse(HttpStatusCode.OK, data);
        //}
    }

    public class ReportModel
    {
        public string Html { get; set; }
        public string Url { get; set; }
    }

}
