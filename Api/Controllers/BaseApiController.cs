using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using Microsoft.Reporting.WebForms;
using System.IO;
using BAL.Enums;
using FarmaAPI.Helper;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Web;
namespace FarmaAPI.Controllers
{
    public class BaseApiController : ApiController
    {
        string reportFilePath = Path.Combine(HttpRuntime.AppDomainAppPath, "rpts") + @"\";
        string tempPath = Path.Combine(HttpRuntime.AppDomainAppPath, "temp") + @"\";

        public string CreateReportFile(string fileName, string reportFileName, DataSet headerDataSet, DataSet reportDataSet, ExportFormat FileFormat)
        {
            //String reportPath = System.Web.HttpContext.Current.Server.MapPath("~/rpts/" + reportFileName);
            String reportPath = reportFilePath + reportFileName;
            ReportViewer rpt = new ReportViewer();
            rpt.LocalReport.ReportPath = reportPath;
            ReportDataSource rDsource = new ReportDataSource("DataSet1", reportDataSet.Tables[0]);
            rpt.LocalReport.DataSources.Add(rDsource);
            string fileType = "." + FileFormat.ToString();
            string filePath = System.Web.HttpContext.Current.Server.MapPath("~/temp/");
            fileName = fileName + fileType;
            filePath = filePath + fileName;
             
            if (headerDataSet != null)
            {
                rpt.LocalReport.SubreportProcessing += delegate (object o, SubreportProcessingEventArgs e)
                {
                    ReportDataSource rsHeader = new ReportDataSource("DataSet1", headerDataSet.Tables[0]);
                    e.DataSources.Add(rsHeader);
                };
            }
            rpt.LocalReport.Refresh();
            byte[] reportData = new byte[1];

            reportData = rpt.LocalReport.Render(Convert.ToString(FileFormat));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            File.WriteAllBytes(filePath, reportData);
            return fileName;

        }
        public byte[] GenerateReport(string reportFileName, string dataSetname, DataSet reportDataSet, ExportFormat FileFormat)
        {


            String reportPath = reportFilePath + reportFileName;
            ReportViewer rpt = new ReportViewer();
            rpt.LocalReport.ReportPath = reportPath;
            ReportDataSource rDsource = new ReportDataSource(dataSetname, reportDataSet.Tables[0]);
            rpt.LocalReport.DataSources.Add(rDsource);
            string fileType = "." + FileFormat.ToString();
            //  string filePath = System.Web.HttpContext.Current.Server.MapPath("~/temp/");
            //  fileName = fileName + fileType;
            //  filePath = filePath + fileName;

            rpt.LocalReport.Refresh();
            byte[] reportData = new byte[1];
            var deviceInfo = @"<DeviceInfo>
                    <EmbedFonts>None</EmbedFonts>
                   </DeviceInfo>";
            reportData = rpt.LocalReport.Render(Convert.ToString(FileFormat), deviceInfo);

            //if (File.Exists(filePath))
            //{
            //    File.Delete(filePath);
            //}
            //File.WriteAllBytes(filePath, reportData);
            return reportData;

        }
        public string CreateReportFile(string fileName, string reportFileName, DataSet headerDataSet, DataSet reportDataSet, ExportFormat FileFormat, params string[] subreport)
        {
           // String reportPath = System.Web.HttpContext.Current.Server.MapPath("~/rpts/" + reportFileName);
            String reportPath = reportFilePath + reportFileName;
            ReportViewer rpt = new ReportViewer();
            rpt.LocalReport.ReportPath = reportPath;
            ReportDataSource rDsource = new ReportDataSource("DataSet1", reportDataSet.Tables[0]);
            rpt.LocalReport.DataSources.Add(rDsource);
            if (reportDataSet.Tables.Count > 1)
            {
                ReportDataSource rDsource2 = new ReportDataSource("DataSet2", reportDataSet.Tables[1]);
                rpt.LocalReport.DataSources.Add(rDsource2);
            }
            string fileType = "." + FileFormat.ToString();
            string filePath = System.Web.HttpContext.Current.Server.MapPath("~/temp/");
            fileName = fileName + fileType;
            filePath = filePath + fileName;
            List<String> subrepList = subreport.ToList();
            rpt.LocalReport.EnableExternalImages = true;

            if (headerDataSet != null)
            {
                rpt.LocalReport.SubreportProcessing += delegate (object o, SubreportProcessingEventArgs e)
                {
                    int index = subrepList.IndexOf(e.ReportPath);
                    ReportDataSource rsHeader;
                    if (index == 0)
                    {
                        rsHeader = new ReportDataSource("DataSet1", reportDataSet.Tables[2]);
                    }
                    else
                    {
                        rsHeader = new ReportDataSource("DataSet1", reportDataSet.Tables[3]);
                    }
                    //else
                    //{
                    //rsHeader = new ReportDataSource("DataSet2", reportDataSet.Tables[index + 1]);
                    // }
                    e.DataSources.Add(rsHeader);
                };
            }
            rpt.LocalReport.Refresh();
            byte[] reportData = new byte[1];

            reportData = rpt.LocalReport.Render(Convert.ToString(FileFormat));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            File.WriteAllBytes(filePath, reportData);
            return fileName;

        }
        public string MergeFiles(List<String> fileNamesInTemp, string outPutFile)
        {
           // String tempPath = System.Web.HttpContext.Current.Server.MapPath("~/temp/");
            // Open the output document
            PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();
            foreach (String fileName in fileNamesInTemp)
            {
                string filePath = tempPath + fileName;


                PdfSharp.Pdf.PdfDocument inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(filePath, PdfDocumentOpenMode.Import);
                // Iterate pages
                int count = inputDocument.PageCount;
                for (int idx = 0; idx < count; idx++)
                {
                    // Get the page from the external document...
                    PdfSharp.Pdf.PdfPage page = inputDocument.Pages[idx];
                    // ...and add it to the output document.
                    outputDocument.AddPage(page);
                }
            }

            // Save the document...
            string filename = tempPath + outPutFile;
            outputDocument.Save(filename);
            return outPutFile;
        }
    }
}
