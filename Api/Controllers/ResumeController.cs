using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using NReco.PdfGenerator;

namespace FarmaAPI.Controllers
{
    public class ResumeController : ApiController
    {
        //[HttpPost]
        //public void GenPdf([FromBody] ReportModel dto)
        //{
        //    //Save pdf file.
        //    var file = System.Web.Hosting.HostingEnvironment.MapPath("~/temp/") + "output-resume.pdf";
        //    if (File.Exists(file))
        //    {
        //        File.Delete(file);
        //    }

        //    StringReader sr = new StringReader(dto.Html);
        //    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
        //    using (var ms = new MemoryStream())
        //    {
        //        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);
        //        pdfDoc.Open();
        //        XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
        //        pdfDoc.Close();
        //    }

        //}
        [HttpPost]
        public ApiResponse pdfv1([FromBody] RepData d)
        {
            var r = new ApiResponse();
            HtmlToPdfConverter con = new HtmlToPdfConverter();
            string htmlFileName = "";
            if (String.IsNullOrEmpty(d.FileName))
            {
                var guid = "test";
                htmlFileName = guid + ".html";
                d.FileName = guid + ".pdf";
            }

            var fileName = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/temp/"), d.FileName);
            htmlFileName = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/temp/"), htmlFileName);
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }
            byte[] file;
            if (d.UseUrl)
            {
                System.IO.File.WriteAllText(htmlFileName, d.Html);
                file = con.GeneratePdfFromFile(htmlFileName, null);
            }
            else
                file = con.GeneratePdf(d.Html);

            con.Size = NReco.PdfGenerator.PageSize.A4;
           System.IO.File.WriteAllBytes(fileName, file);
            r.FileName = d.FileName;
            return r;
        }

    }

    public class RepData
    {
        public string Html { get; set; }
        public string FileName { get; set; }
        public bool UseUrl { get; set; }
    }
    public class ApiResponse
    {
        public string Data { get; set; }
        public string FileName { get; set; }
    }
}




