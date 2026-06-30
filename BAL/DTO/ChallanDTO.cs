using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.Enums;
namespace BAL.DTO
{
    public class ChallanDTO
    {
        int _challanId;
        public int ChallanId { get { return _challanId; } }
        public string ChallanNumber { get; set; }
        public int SiteId { get; set; }
        public string JobNumber { get; set; }
        public DateTime ChallanDate { get; set; }
        public double SubTotal { get; set; }
        public double TotalTax { get; set; }
        public double DiscountRate { get; set; }
        public double Discount { get; set; }
        public double Total { get; set; }
        // public ChallanType Type { get; set; }
        public List<ChallanItemDTO> Items { get; set; }
        public List<TaxDTO> ApplicableTaxes { get; set; }
        public string Number { get; set; }
        public LedgerDTO Ledger { get; set; }
        public string Client { get; set; }
        public Int16 StoreId { get; set; }
        public string Vehicle { get; set; }
        public string Driver { get; set; }
        public double Freight { get; set; }
        public int CompanyId { get; set; }
        public int LedgerId { get; set; }
        public string Site { get; set; }
        //  public Ledger Ledger { get; set; }
        public ChallanType ChallanType { get; set; }

        public ChallanDTO(int challanId)
        {
            _challanId = challanId;
        }
        public ChallanDTO() { }


    }
}
