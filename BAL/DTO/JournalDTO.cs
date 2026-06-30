using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class JournalDTO : MasterDTO
    {
        public int SiteId { get; set; }
        public DateTime EntryDate { get; set; }
        public Double Amount { get; set; }
        public int ClientId { get; set; }
        public string ChallanNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string ChequeNumber { get; set; }
        public string TransactionId { get; set; }
        public string Bank { get; set; }
        public string Remarks { get; set; }
        public string Site { get; set; }
        public string JobNumber { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

    }

    
}
