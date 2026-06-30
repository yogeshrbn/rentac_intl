using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class LedgerTaxDTO : TaxDTO
    {
        public int LedgerId { get; set; }
        public int LedgerSiteId { get; set; }
        public double Rate { get; set; }
        public int FinYearId { get; set; }
        public int LedgerTaxId { get; set; }

        public int StateId { get; set; }
        public int CompanyStateId { get; set; }

    }
}
