using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class BillingDTO : InvoiceDTO
    {
        /// <summary>When 16, contract bill line totals use <see cref="QuotationDTO"/> rules (Qty×Rate or Qty×Area×Rate), not measure H×W.</summary>
        public short QuotationType { get; set; }
        /// <summary>Contract bill header area (same role as contract quotation).</summary>
        public float Area { get; set; }
        public short MeasureType { get; set; }
        /// <summary>Contract line mode: <c>quantity</c> = Rent (Qty×Rate×Days); <c>area</c> = Qty×Area×Rate (from config when empty for type 16).</summary>
        public string LineTotalMode { get; set; }

        public List<BillingItemDTO> BillableItems { get; set; }
        public List<BillingItemDTO> BreakageItems { get; set; }
        /// <summary>Optional: GRN damage lines when loading/editing a bill with breakage (same shape as <see cref="RentBillDto.BreakageDamageDetails"/>).</summary>
        public List<BreakageDamageDetailDTO> BreakageDamageDetails { get; set; }
        public List<InvoiceChargeDTO> OtherCharges { get; set; }
        public List<int> HeaderTypes { get; set; }

        public bool ApplyTax { get; set; }

        public List<LedgerTransactionDTO> Payments { get; set; }
        public bool FilterChallansByPO { get; set; }
        public string ChallanNo { get; set; }
        /// <summary>Issued list: All | Active | Deleted — passed to p_ItemsIssuedRegister as @listStatusFilter (byte).</summary>
        public string IssuedListStatusFilter { get; set; }
        /// <summary>Received list: All | Active | Deleted — passed to p_ItemsReceived_register as @listStatusFilter (byte).</summary>
        public string ReceivedListStatusFilter { get; set; }
        /// <summary>1-based page index for server-side pagination</summary>
        public int PageIndex { get; set; }
        /// <summary>Page size for server-side pagination. When > 0, pagination is applied.</summary>
        public int PageSize { get; set; }

        /// <summary>Contract bill from contractInfo: quotation ids chosen in the picker (linked to invoice after save).</summary>
        public List<int> BillQuotationIds { get; set; }

        /// <summary>Aggregated unique taxes for sale bill display (InvoiceType 4 / 7).</summary>
        public List<InvoiceTaxDTO> AppliedTaxes { get; set; }
    }
    public class QuotationDataDTO : QuotationDTO
    {
        public List<QuotationItemDTO> BillableItems { get; set; }

        public List<InvoiceChargeDTO> OtherCharges { get; set; }
        public List<int> HeaderTypes { get; set; }

    }

    public class RentBillDto
    {
        public List<BillingItemDTO> BillingItems { get; set; }
        public List<BillChallanDto> Challans { get; set; }
        public List<InvoiceItemDTO> StockBalanceAfterBill { get; set; }
        public List<BillingItemDTO> LostItems { get; set; }
        public List<BillingItemDTO> Breakage { get; set; }
        /// <summary>GRN damage component lines (qty × rate cost) for the bill period when billing breakage.</summary>
        public List<BreakageDamageDetailDTO> BreakageDamageDetails { get; set; }
        public bool ApplyTax { get; set; }
        public List<InvoiceChargeDTO> OtherCharges { get; set; }

        public AccountLedgerDTO AccountLedger { get; set; }

        public List<BillPODto> PO { get; set; }

    }
    public class BillPODto
    {
        public string PONumber { get; set; }
    }
    /// <summary>One row from GRNItemDamageComponents for display on rent bill (breakage tab).</summary>
    public class BreakageDamageDetailDTO
    {
        public int GRNItemId { get; set; }
        public int ProductId { get; set; }
        public string ParentItem { get; set; }
        public string GRN { get; set; }
        public string ComponentName { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Cost { get; set; }
        public DateTime? ReceivingDate { get; set; }
        /// <summary>GST % on damage cost (same rules as rent line items).</summary>
        public double IGSTRate { get; set; }
        public double CGSTRate { get; set; }
        public double SGSTRate { get; set; }
        public double IGST { get; set; }
        public double CGST { get; set; }
        public double SGST { get; set; }
        public int TaxCategoryId { get; set; }
    }
    public class BillingItemDTO : InvoiceItemDTO
    {
        public int Id { get; set; }
        //   public double SentQty { get; set; }
        public DateTime SentDate { get; set; }
        //    public string Product { get; set; }
        public DateTime ReceivingDate { get; set; }

        public string Project { get; set; }
        public string GRN { get; set; }
        public Int16 TranType { get; set; }
        public DateTime TransDate { get; set; }
        //public string TaxName { get; set; }
        //public double TaxRate { get; set; }
        //public string TaxAmount { get; set; }
        public DateTime LastBillDate { get; set; }
        public string LastInvoiceNumber { get; set; }
        /// <summary>InvoiceId of the last rent bill for this site (Due bills / summary screens).</summary>
        public int LastInvoiceId { get; set; }
        public string QtyCalculation { get; set; }
        //  public string GuId { get; set; }

        // public new int TaxCategoryId { get; set; }
        //public double IGST { get; set; }
        //public double CGST { get; set; }
        //public double SGST { get; set; }
        //public double IGSTRate { get; set; }
        //public double CGSTRate { get; set; }
        //public double SGSTRate { get; set; }
        public string Unit { get; set; }
        /// <summary>
        /// Used only for contract
        /// </summary>
        public string LinItem { get; set; }
        public ProductDTO ItemMaster { get; set; }

        public int GroupItemId { get; set; }
        public bool IsLost { get; set; }

        public bool IsBreakage { get; set; }

        //  public double SizeBalance { get; set; }
        public double WeightBalance { get; set; }

        /// <summary>Per-line area for contract (QuotationType 16) when <see cref="BillingDTO.LineTotalMode"/> is area.</summary>
        public float Area { get; set; }

        /// <summary>Contract measure (InvoiceType 5, QuotationType 16): optional per-line subtotal mode; empty = use <see cref="BillingDTO.LineTotalMode"/>.</summary>
        public string LineTotalMode { get; set; }

        /// <summary>Dynamic taxes applied on this invoice line (InvoiceType 4 / 7).</summary>
        public List<InvoiceTaxDTO> LineTaxes { get; set; }

    }



}
