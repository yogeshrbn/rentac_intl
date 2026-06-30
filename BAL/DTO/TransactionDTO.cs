using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class TransactionDTO : MasterDTO
    {
        public int CompanyId { get; set; }
        public int LedgerId { get; set; }
        public string Client { get; set; }
        public string Number { get; set; }
        public int LedgerSiteId { get; set; }
        public DateTime TransactionDate { get; set; }
        public short TransactionType {get;set;}
        public int TransactionId { get; set; }
    }
}
