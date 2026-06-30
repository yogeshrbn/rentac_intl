using BAL.DTO;
using BAL.Objects;
using FarmaAPI.Helper;
using OfficeOpenXml;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace FarmaAPI.Controllers
{

    public class SubscriptionController : BaseApiController
    {

        /// <summary>


        /// </summary>
        [HttpPost]

        public ApiMessage GetBills([FromBody] RentacBillingFilterDTO filter)
        {
            var apiMessage = new ApiMessage();
            //var filter = new RentacBillingFilterDTO();
            //var _currentDate = DateTime.Now;
            //filter.EndDate = _currentDate.AddDays(-(_currentDate.Day - 1));
            //filter.FromDate = filter.EndDate.AddMonths(-6);

            filter.ClientId = new LoggedInUser().RbnClientId;
            filter.CompanyId = new LoggedInUser().DefaultCompanyId;

            var _subService = new SubscriptionService();
            var bills = _subService.GetInvoice(filter);
            //var lastMonthDate = new DateTime(filter.EndDate.Year, filter.EndDate.Month, 1).AddMonths(-1);
            //var currentBill = bills.Where(o => new DateTime(o.Year, o.Month, 1) == lastMonthDate).FirstOrDefault();
            //if (currentBill != null)
            //{
            //    if (currentBill.Balance == 0)
            //    {
            //        currentBill.Paid = 1;
            //    }
            //}
            //var prevBalance = bills.Where(o => o.PaymentRefId == null).Sum(o => o.TotalAmount);
            //apiMessage.Data = new
            //{
            //    currentBill = currentBill,
            //    prevBalance = prevBalance == null ? 0 : prevBalance,
            //    prev = bills
            //    //prev = currentBill != null ?
            //    // bills.Where(o => new DateTime(o.Year, o.Month, 1) < new DateTime(currentBill.Year, currentBill.Month, 1)) : null
            //};
            apiMessage.Data = bills;
            apiMessage.Code = ApiMessageCodes.SUCCESS;
            return apiMessage;
        }
        [HttpPost]
        public async Task<ApiMessage> AllPayments([FromBody] RentacBillingFilterDTO filter)
        {
            var msg = new ApiMessage();
            try
            {
                var _subService = new SubscriptionService();
                var paymentService = new PaymentService();
                var payOrder = new PaymentOrderDTO();
                payOrder.ClientId = new LoggedInUser().RbnClientId;
                payOrder.CompanyId = new LoggedInUser().DefaultCompanyId;
                // var x = paymentService.AllPayments(payOrder.ClientId, filter.FromDate, filter.EndDate);
                var bills = _subService.GetInvoice(filter);
                msg.Code = ApiMessageCodes.SUCCESS;
                msg.Data = bills;
                return msg;
            }
            catch (Exception ex)
            {
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = ex.Message;
                throw ex;
            }
        }


        [HttpGet]
        public HttpResponseMessage PrintPaymentInvoice(int paymentId)
        {
            var msg = new ApiMessage();
            var rep = new ReportUtility();
            //  var fileName = ReportUtility.GetReportFileName("clientinvoice.xslt");
            var repFileName = "clientinvoice.rdlc";
            var service = new SubscriptionService();
            var ds = service.GetByIdToPrint(new RentacBillingDTO { BillingId = paymentId });
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].Rows[0]["AmountInWords"] = Utils.ConvertNumbertoWords(ds.Tables[0].Rows[0]["Amount"].ToString());
                }
            }
            var fileData = GenerateReport(repFileName, "rentacbill", ds, BAL.Enums.ExportFormat.PDF);


            // var strhtml = rep.GetReportBody(fileName, strXml);
            //  var repFile = HttpContext.Current.Server.MapPath("~/temp/1007.pdf");
            //            rep.ConvertToPdf(strhtml, repFile);
            //rep.createPdf(strhtml, repFile);
            //var stream
            //var dataBytes = File.ReadAllBytes(repFile);
            //adding bytes to memory stream
            /* var dataStream = ReportUtility.ConvertToPdf(strhtml);// new MemoryStream(dataBytes);
             //File.WriteAllBytes(repFile, dataStream);
             var result = new HttpResponseMessage(HttpStatusCode.OK)
             {
                 Content = new ByteArrayContent(dataStream)
             };
             result.Content.Headers.ContentDisposition =
                 new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                 {
                     FileName = "CertificationCard.pdf"
                 };
             result.Content.Headers.ContentType =
                 new MediaTypeHeaderValue("application/octet-stream");
             // System.Web.Mvc.FileStreamResult
             return result;
          

            string templateName = HttpContext.Current.Server.MapPath(@"~/rpts/rentacinvoice.xlsx");
            FileInfo tempINfo = new FileInfo(templateName);

            ExcelPackage ep = new ExcelPackage(tempINfo);
            ExcelWorksheet workSheet = ep.Workbook.Worksheets[1];
            // List<ReportDTO> reports = objReports.SiteWiseInventory(dto.Site, dto.JobNumber);
            // workSheet.Cells["A1"].LoadFromCollection(reports, true);
            string filePath = System.Web.HttpContext.Current.Server.MapPath("~/temp/");
            string fileName = Guid.NewGuid().ToString() + ".xlsx";
            filePath = filePath + fileName;
         //   workSheet.Cells["A1:B1"].Value = "Billing";
            //PrepareSiteInventoryReport(workSheet, reports);
            FileInfo fInfo = new FileInfo(filePath);
            
            var ms = new MemoryStream();
            ep.SaveAs(fInfo);
            Workbook wb = new Workbook();
            
            wb.LoadFromFile(filePath);
            var pdfStream = new MemoryStream();
            wb.SaveToStream(pdfStream, FileFormat.PDF);  */
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(fileData)
            };
            //  wb.SaveToHttpResponse("file.pdf", HttpContext.Current.Response);
            result.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = "CertificationCard.pdf"
                };
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            return result;

        }

        /// <summary>
        /// print invoice
        /// </summary>
        [HttpGet]
        public HttpResponseMessage printFile(int paymentId)
        {
            var msg = new ApiMessage();
            var rep = new ReportUtility();
            //  var fileName = ReportUtility.GetReportFileName("clientinvoice.xslt");
            var repFileName = "clientinvoice.rdlc";
            var service = new SubscriptionService();
            var ds = service.GetByIdToPrint(new RentacBillingDTO { BillingId = paymentId });
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].Rows[0]["AmountInWords"] = Utils.ConvertNumbertoWords(ds.Tables[0].Rows[0]["TotalAmount"].ToString());
                }
            }
            var fileData = GenerateReport(repFileName, "rentacbill", ds, BAL.Enums.ExportFormat.PDF);


            // var strhtml = rep.GetReportBody(fileName, strXml);
            //  var repFile = HttpContext.Current.Server.MapPath("~/temp/1007.pdf");
            //            rep.ConvertToPdf(strhtml, repFile);
            //rep.createPdf(strhtml, repFile);
            //var stream
            //var dataBytes = File.ReadAllBytes(repFile);
            //adding bytes to memory stream
            /* var dataStream = ReportUtility.ConvertToPdf(strhtml);// new MemoryStream(dataBytes);
             //File.WriteAllBytes(repFile, dataStream);
             var result = new HttpResponseMessage(HttpStatusCode.OK)
             {
                 Content = new ByteArrayContent(dataStream)
             };
             result.Content.Headers.ContentDisposition =
                 new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                 {
                     FileName = "CertificationCard.pdf"
                 };
             result.Content.Headers.ContentType =
                 new MediaTypeHeaderValue("application/octet-stream");
             // System.Web.Mvc.FileStreamResult
             return result;
          

            string templateName = HttpContext.Current.Server.MapPath(@"~/rpts/rentacinvoice.xlsx");
            FileInfo tempINfo = new FileInfo(templateName);

            ExcelPackage ep = new ExcelPackage(tempINfo);
            ExcelWorksheet workSheet = ep.Workbook.Worksheets[1];
            // List<ReportDTO> reports = objReports.SiteWiseInventory(dto.Site, dto.JobNumber);
            // workSheet.Cells["A1"].LoadFromCollection(reports, true);
            string filePath = System.Web.HttpContext.Current.Server.MapPath("~/temp/");
            string fileName = Guid.NewGuid().ToString() + ".xlsx";
            filePath = filePath + fileName;
         //   workSheet.Cells["A1:B1"].Value = "Billing";
            //PrepareSiteInventoryReport(workSheet, reports);
            FileInfo fInfo = new FileInfo(filePath);
            
            var ms = new MemoryStream();
            ep.SaveAs(fInfo);
            Workbook wb = new Workbook();
            
            wb.LoadFromFile(filePath);
            var pdfStream = new MemoryStream();
            wb.SaveToStream(pdfStream, FileFormat.PDF);  */
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(fileData)
            };
            //  wb.SaveToHttpResponse("file.pdf", HttpContext.Current.Response);
            result.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = "CertificationCard.pdf"
                };
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            return result;

        }

        [HttpPost]
        [AllowAnonymous]
        public ApiMessage RegisterClient([FromBody] UserRegisterDTO dto)
        {
            var res = new ApiMessage();
            try
            {
                var rbnClient = new RBNClient();
                
                int ret = rbnClient.Create(dto);
                res.Data = ret;

                if (ret > 0)
                {
                    var rep = new ReportUtility();
                    EmailParameters param = new EmailParameters();
                    param.Company = dto.Company;
                    rep.sendEmail(dto.Email, "Rentac", "Welcome To Rentac", param, ReortFileName.NEW_CLIENT_WELCOME_EMAIL);
                }
                res.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                res.Code = ApiMessageCodes.ERROR;
                res.Message = ex.Message;
            }
            return res;
        }

    }
}
