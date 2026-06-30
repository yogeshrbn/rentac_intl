using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL.DAL;
using BAL.DTO;
using BAL.Objects;
using Microsoft.Reporting.WebForms;
using System.Data;
using System.IO;
using FarmaAPI.Helper;
using NReco.PdfGenerator;

namespace FarmaAPI
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                //  PrintReport(null, null);
                //CommHelpler com = new CommHelpler();
                //EmailParameters p = new EmailParameters();
                //p.ChallanNumber = "3333";
                //string message = com.GetMessageBody(p.GetXML(), MessageEvent.ISSUE_CHALLAN);
                //com.sendSms("9811553130", message, MessageEvent.ISSUE_CHALLAN);
                //ReportUtility rep = new ReportUtility();
                //HtmlToPdfConverter con = new HtmlToPdfConverter();
                //string retpData = rep.GetReportBody(ReportUtility.GetEmailURL("rent-bill.xsl"), "<data></data>");

                //byte[] file = con.GeneratePdf(retpData);
                //string fileName = "bill" + ".pdf";

                //string fileToSave = System.Web.HttpContext.Current.Server.MapPath("~/temp/");
                //fileToSave += fileName;
                //if (File.Exists(fileToSave))
                //{
                //    File.Delete(fileToSave);
                //}
                //  File.WriteAllBytes(fileToSave, file);
                //lblWords.Text += Request.UserHostAddress;
                //foreach (var x in Request.ServerVariables)
                //{
                //    var y = x.ToString();

                //    lblWords.Text +=  "Name:" + y + "," + Request.ServerVariables[y].ToString() + "<br/>";
                //}
            }
        }


        protected void PrintReport(object sender, EventArgs e)
        {
            Billing billing = new Billing();
            DataSet ds = billing.PrintBill(2156);
            string reportPath = System.Web.HttpContext.Current.Server.MapPath("~/rpts/invoice-rent.rdlc");
            //  ReportViewer rpt = new ReportViewer();
            rpt.LocalReport.ReportPath = reportPath;
            ReportDataSource rDsource = new ReportDataSource("DataSet1", ds.Tables[0]);
            rpt.LocalReport.DataSources.Add(rDsource);
            BAL.Objects.Report objRPT = new BAL.Objects.Report();
            DataSet headerDataSet = objRPT.GetReportHeader_Bill(1038, 1007, 2156, 1);
            string filePath = System.Web.HttpContext.Current.Server.MapPath("~/temp/");
            string fileName = "2180.pdf";
            rpt.LocalReport.SubreportProcessing += delegate (object o, SubreportProcessingEventArgs e1)
            {
                if (e1.ReportPath.Contains("billHeaderv2"))
                {
                    ReportDataSource rsHeader = new ReportDataSource("DataSet1", headerDataSet.Tables[0]);
                    e1.DataSources.Add(rsHeader);
                }
                else if (e1.ReportPath.Contains("invoiceTax-subreport-period"))
                {
                    ReportDataSource rsHeader = new ReportDataSource("DataSet1", ds.Tables[1]);
                    e1.DataSources.Add(rsHeader);
                }

            };
            rpt.LocalReport.Refresh();
            filePath = filePath + fileName;
        }


        protected void convertToWords(object sender, EventArgs e)
        {
            lblWords.Text = Number.ConvertToWords(txtAmount.Text);
        }

        protected void SendEmail(object sender, EventArgs e)
        {
            // ReportUtility rep = new ReportUtility();
            // rep.PrintAndEmailReceiveReceipt(4171, true);
            SendEmails.SendEmail("yvashishth@gmail.com", "yvashishth@gmail.com", "Test", "Test");

        }

    }



}