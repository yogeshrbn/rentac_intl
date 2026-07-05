using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class LostItemDTO : InvoiceItemDTO
    {
        public int Id { get; set; }
        public List<InvoiceTaxDTO> LineTaxes { get; set; }
        public int LossItemId { get; set; }


        //   public int InvoiceId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime LostDate { get; set; }
         

        public int MatLossId { get; set; }
        //  public string Product { get; set; }

    }


    public class MatLossDTO : BaseDTO
    {
        public int MatLossId { get; set; }
        public string Number { get; set; }
        public int LedgerId { get; set; }
        public int LedgerSiteId { get; set; }
        public string SiteAddress { get; set; }
        public DateTime EntryDate { get; set; }
        public string Client { get; set; }
        public List<LostItemDTO> Items { get; set; }
        public CompanyDTO Company { get; set; }
        public LedgerDTO Ledger { get; set; }

    }

    public class MatLossFilterDTO
    {
        public int MatLossId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int CompanyId { get; set; }
        public int LedgerId { get; set; }

        public int LedgerSiteId { get; set; }


    }


}
