using System.Collections.Generic;

namespace ReportViewer.Models
{
    /// <summary>POST body for bill list GST PDF (payload is btoa-wrapped encrypted filter JSON).</summary>
    public class BillListGstPrintRequest
    {
        public string Payload { get; set; }
    }

    public class BillListGstReportFilterDto
    {
        public string From { get; set; }
        public string To { get; set; }
        public int LedgerId { get; set; }
        public int LedgerSiteId { get; set; }
        public int StatusId { get; set; }
        public short InvoiceType { get; set; }
        public string InvoiceNumber { get; set; }
    }

    public class BillListGstReportRow
    {
        public string BillNo { get; set; }
        public string BillDate { get; set; }
        public string Client { get; set; }
        public string InvoiceAmount { get; set; }
        public string TaxableAmount { get; set; }
        public string TaxableCgst { get; set; }
        public string TaxCgst { get; set; }
        public string TaxableSgst { get; set; }
        public string TaxSgst { get; set; }
        public string TaxableIgst { get; set; }
        public string TaxIgst { get; set; }
    }

    public class BillListGstReportViewModel
    {
        public string CompanyName { get; set; }
        public string PeriodLabel { get; set; }
        public IList<BillListGstReportRow> Rows { get; set; }
        public BillListGstReportRow Totals { get; set; }
    }
}
