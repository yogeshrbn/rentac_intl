using System.Collections.Generic;
using BAL.DTO;

namespace ReportViewer.Models
{
    public class MaterialAdjustPrintData
    {
        public CompanyDTO Company { get; set; }
        public LedgerDTO Ledger { get; set; }
        public string Number { get; set; }
        public string WorkOrderDate { get; set; }
        public string SiteAddress { get; set; }
        public List<MaterialAdjustIssuePrintRow> IssueItems { get; set; }
        public List<MaterialAdjustReceivePrintRow> ReceiveItems { get; set; }
    }

    public class MaterialAdjustIssuePrintRow
    {
        public string Product { get; set; }
        public string ExcessQty { get; set; }
        public string SentQty { get; set; }
    }

    public class MaterialAdjustReceivePrintRow
    {
        public string Product { get; set; }
        public string Quantity { get; set; }
    }
}
