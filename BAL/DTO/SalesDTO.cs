using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class SalesDTO : LedgerTransactionDTO
    {
        public int SalesId { get; set; }
        public DateTime SalesDate { get; set; }
        public short SalesType { get; set; }
        public int SalesAccountId { get; set; }
        public string PartyName { get; set; }
        public List<SalesItemDTO> Items { get; set; }
        public int FileFormat { get; set; }
        public double DiscountPercent { get; set; }
        public double DiscountAmount { get; set; }
    }
    public class SalesItemDTO : SalesDTO
    {
        public int ProductId { get; set; }
        public string Item { get; set; }
        public double Rate { get; set; }
        public double Quantity { get; set; }
        public double Amount { get; set; }
        public string TaxName { get; set; }
        public double TaxRate { get; set; }
        public string TaxAmount { get; set; }
        public short TaxCategoryId { get; set; }
        public double IGST { get; set; }
        public double SGST { get; set; }
        public double CGST { get; set; }
        public double Total { get; set; }

    }
}
