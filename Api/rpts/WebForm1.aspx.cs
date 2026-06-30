using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL.Objects;
using BAL.DTO;
using Microsoft.Reporting.WebForms;
using System.Data;
using System.IO;
namespace FarmaAPI.rpts
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                showReport();
            }
        }
        ReportDataSource rDsource;
        void showReport()
        {
            try
            {
                Ledger ledger = new Ledger();
                String from = "", to = "";

                int companyId = new LoggedInUser().DefaultCompanyId;
                BAL.Objects.Report objReport = new BAL.Objects.Report();
                DataSet headerDataSet = objReport.GetReportHeader(13, 1014);
                DataSet mainDS = ledger.GetReceiptRegisterPRT(13, from, to, 9, "");

                String reportPath = System.Web.HttpContext.Current.Server.MapPath("~/rpts/receiptRegister.rdlc");
                ReportViewer rpt = new ReportViewer();
                rpt.LocalReport.ReportPath = reportPath;
                ReportDataSource mainDataSource = new ReportDataSource("DataSet1", mainDS.Tables[0]);
                rDsource = new ReportDataSource("DataSet1", headerDataSet.Tables[0]);
                rpt.LocalReport.DataSources.Add(mainDataSource);

                rpt.LocalReport.Refresh();
                string filePath = System.Web.HttpContext.Current.Server.MapPath("~/temp/");
                string fileName = "13" + ".pdf";
                filePath = filePath + fileName;
                rpt.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(subReportProcessing);
                byte[] reportData = rpt.LocalReport.Render("PDF");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.WriteAllBytes(filePath, reportData);
                Response.Write(fileName);
                Response.End();

            }
            catch (Exception ex)
            {

            }
        }

        public void subReportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            e.DataSources.Add(rDsource);
        }
    }

}