using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class PurchaseDTO : LedgerTransactionDTO
    {
     //   public int PurchaseId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public short PurchaseType { get; set; }
        public int PurchaseAccountId { get; set; }
        public string PartyName { get; set; }
        public List<PurchaseItemDTO> Items { get; set; }
        //public int FileFormat { get; set; }
        //public double DiscountPercent { get; set; }
        public double DiscountAmount { get; set; }
        public string VendorCreditNoteNumber { get; set; }
        public DateTime VendorCreditNoteDate { get; set; }
        public string Guid { get; set; }

       // public short StatusId { get; set; }
        //public string StatusName { get; set; }
        //public int ModifiedBy { get; set; }
        //public DateTime ModifiedOn { get; set; }

        public CompanyDTO CompanyDTO { get; set; }
        public LedgerDTO LedgerDTO { get; set; }

        /// <summary>
        /// this is used on vendor payments.
        /// </summary>
        public double AppliedAmount { get; set; }

        /// <summary>Optional TDS against this bill on quick payment.</summary>
        public double TdsAmount { get; set; }

        public string Doc1 { get; set; }
        public string Doc2 { get; set; }
        public string Doc3 { get; set; }

        public int WarehouseId { get; set; }

    }
    public class PurchaseItemDTO : PurchaseDTO
    {
        public int ProductId { get; set; }
        public string Item { get; set; }
        public double Rate { get; set; }
        public double Quantity { get; set; }
        public decimal Unit1Quantity { get; set; }
        public double Amount { get; set; }
        public string TaxName { get; set; }
        public double TaxRate { get; set; }
        public int PurchaseUnitId { get; set; }
        public int UOM { get; set; }
        public string Unit { get; set; }
        public string PurchaseUnitName { get; set; }

        public short TaxCategoryId { get; set; }
        public double IGST { get; set; }
        public double SGST { get; set; }
        public double CGST { get; set; }



    }

    public class PurchaseFilterDTO
    {
        public int LedgerId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int CompanyId { get; set; }
        public int FinYearId { get; set; }
        public Byte PurchaseType { get; set; }
    }
}
