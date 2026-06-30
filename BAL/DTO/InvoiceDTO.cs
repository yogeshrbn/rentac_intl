using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class InvoiceDTO : LedgerTransactionDTO
    {
        public int InvoiceId { get; set; }
        public byte ChargeReturnDay { get; set; }
        public int ParentInvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string ShipTo { get; set; }
        public byte BillFromSite { get; set; }
        public string PONumber { get; set; }
        public DateTime PODate { get; set; }
        public short InvoiceType { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        // public int StatusId { get; set; }
        public int SiteId { get; set; }
        public double BreakageAmount { get; set; }
        //  public int WorkOrderId { get; set; }
        public int BillCopyType { get; set; }
        public string BranchCode { get; set; }
        public string Category { get; set; }
        public string ContractorCode { get; set; }
        public bool RoundOff { get; set; }
        //  public string Client { get; set; }
        //   public double Freight { get; set; }
        /// <summary>
        /// balance recorded at the time of invoice creating. 
        /// It is the ledger outstanding of the client which will be adjusted withe the current invoice amount
        /// </summary>
        public double OutStanding { get; set; }
        /// <summary>
        /// DR or CR.
        /// </summary>
        public Int16 OutStandingType { get; set; }
        /// <summary>
        /// This is custom site address added on manual bill screen.
        /// </summary>
        public string SiteAddress { get; set; }
        // public int LedgerSiteId { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonPhone { get; set; }
        public List<LostItemDTO> LostItems { get; set; }
        public double LossCharges { get; set; }
        public double OtherChargeAmount { get; set; }
        public double ChargesTax { get; set; }
        public double Charge1 { get; set; }
        public double Charge2 { get; set; }
        public double Charge3 { get; set; }
        public double Charge4 { get; set; }
        public double Charge5 { get; set; }
        public double IGST { get; set; }
        public double SGST { get; set; }
        public double CGST { get; set; }
        public double IGSTRate { get; set; }
        public double SGSTRate { get; set; }
        public double CGSTRate { get; set; }

        /// <summary>Pre-tax taxable value (items + freight + other charges + damage + loss base; may be rounded per settings).</summary>
        public double Taxable { get; set; }

        public InvoiceIRNDTO IrnDetails { get; set; }
        public string EwayBillNo { get; set; }
        public DateTime EwayBillDate { get; set; }
        public DateTime EwayBillValidUpTo { get; set; }
        public string EwayBillAlert { get; set; }

        public int EwayBillCreatedBy { get; set; }

        public int ContractId { get; set; }


        /// <summary>
        /// this is only used in receipt voucher
        /// </summary>
        public double AppliedAmount { get; set; }

        /// <summary>Optional TDS against this bill on quick receipt.</summary>
        public double TdsAmount { get; set; }

        public string Remarks { get; set; }
        public Int16 ChallanType { get; set; }

        public string FileName { get; set; }

        public int SettledBy { get; set; }
        public string SettlementRemarks { get; set; }
        public DateTime SettlementDate { get; set; }

        public List<BillChallanDto> Challans { get; set; }
        public List<InvoiceItemDTO> StockBalanceAfterBill { get; set; }
        public string Tnc { get; set; }

        public bool IncludeLostItems { get; set; }
        public bool IncludeBreakageItems { get; set; }

        public double BreakageDiscountPercent { get; set; }
        public double LossDiscountPercent { get; set; }
        public double BreakageDiscount { get; set; }
        public double LossDiscount { get; set; }


        // Recurring Invoice fields

        public string Iteration { get; set; }
        public DateTime StartsOn { get; set; }
        public DateTime EndsOn { get; set; }
        public bool Recurring { get; set; }

        public bool IsCashBill { get; set; }

        /// <summary>When true, bill print includes all PO numbers from billed challans.</summary>
        public bool PrintAllPO { get; set; }
        public List<BillPODto> PO { get; set; }
        public string PONumbers { get; set; }

        public int Duration { get; set; }
    }

    public class InvoiceItemDTO : InvoiceDTO
    {
        public int ProductId { get; set; }
        public string Item { get; set; }
        public int ProductType { get; set; }
        public string HSNCode { get; set; }

        public string Product { get; set; }
        // public string Code { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }

        public double Rate { get; set; }
        public double OPB { get; set; }
        public double Quantity { get; set; }
        public double CB { get; set; }
        public double ClosingBalance { get; set; }
        public double SizeBalance { get; set; }
        public double ExcessQty { get; set; }
        public double Amount { get; set; }
        public double Days { get; set; }
        public double SentQty { get; set; }
        public double RecQty { get; set; }
        public double Breakage { get; set; }
        public double BreakageRate { get; set; }
        //   public double BreakageAmount { get { return Breakage * BreakageRate; } }
        public bool ChargeReturnedDate { get; set; }
        public string TaxName { get; set; }
        public double TaxRate { get; set; }
        //    public string TaxAmount { get; set; }
        public string Size { get; set; }
        public double Weight { get; set; }
        public int ItemCategory { get; set; }
        public int ProductSizeId { get; set; }
        public int ChallanId { get; set; }

        public short TaxCategoryId { get; set; }




    }


    /// <summary>POST Sales/LinkQuotationToLedger — link quotation header to a new ledger party.</summary>
    public class LinkQuotationLedgerDto
    {
        public int QuotationId { get; set; }
        public int LedgerId { get; set; }
    }

    public class QuotationDTO : LedgerTransactionDTO
    {
        public int QuotationId { get; set; }
        /// <summary>Contract bill created from this quotation (set after measure bill save from contractInfo).</summary>
        public int InvoiceId { get; set; }
        public int ParentQuotationId { get; set; }
        public DateTime QuotationDate { get; set; }
        public string QuotationNumber { get; set; }
        public short QuotationType { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public float Area { get; set; }
        public short MeasureType { get; set; }
        /// <summary>Contract quotation: default line mode on the header — <c>quantity</c> = Rent (Qty×Rate×Days); <c>area</c> = Qty×Area×Rate. On each <see cref="QuotationItemDTO"/> line, same property is an optional per-line override when persisted (null/empty = use header, then company config).</summary>
        public string LineTotalMode { get; set; }
        //  public int StatusId { get; set; }
        public int SiteId { get; set; }
        public double BreakageAmount { get; set; }
        // public int WorkOrderId { get; set; }
        public int BillCopyType { get; set; }
        public string BranchCode { get; set; }
        public string Category { get; set; }
        public string ContractorCode { get; set; }
        public bool RoundOff { get; set; }
        //  public string Client { get; set; }
        //   public double Freight { get; set; }
        /// <summary>
        /// balance recorded at the time of invoice creating. 
        /// It is the ledger outstanding of the client which will be adjusted withe the current invoice amount
        /// </summary>
        public double OutStanding { get; set; }
        /// <summary>
        /// DR or CR.
        /// </summary>
        public Int16 OutStandingType { get; set; }
        /// <summary>
        /// This is custom site address added on manual bill screen.
        /// </summary>
        public string SiteAddress { get; set; }

        public string Project { get; set; }

        //public int LedgerSiteId { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonPhone { get; set; }
        public List<QuotationItemDTO> Items { get; set; }
        public double LossCharges { get; set; }
        public double OtherChargeAmount { get; set; }

        public string AddInfo { get; set; }
        public string Tnc { get; set; }

        public double Charge1 { get; set; }
        public double Charge2 { get; set; }
        public double Charge3 { get; set; }
        public double Charge4 { get; set; }
        public double Charge5 { get; set; }
        public double ChargesTaxRate { get; set; }
        public double ChargesTax { get; set; }

        public double FreightIn { get; set; }

        public string PoNumber { get; set; }
        public DateTime PoDate { get; set; }
        /// <summary>Quotation validity end date (optional).</summary>
        public DateTime ValidUntil { get; set; }

        /// <summary>1 = Registered (ledger party), 2 = Un-Registered (free-text party).</summary>
        public byte PartyType { get; set; }
        public string UnregisteredPartyName { get; set; }
        public string UnregisteredPartyAddress { get; set; }
        /// <summary>Un-registered party contact phone (optional).</summary>
        public string UnregisteredPartyPhone { get; set; }
        public double GstRate { get; set; }
        /// <summary>Unregistered party: apply IGST component at <see cref="GstRate"/>.</summary>
        public bool IGST { get; set; }
        /// <summary>Unregistered party: apply CGST component at <see cref="GstRate"/>.</summary>
        public bool CGST { get; set; }
        /// <summary>Unregistered party: apply SGST component at <see cref="GstRate"/>.</summary>
        public bool SGST { get; set; }

        /// <summary>1-based page index for server-side pagination</summary>
        public int PageIndex { get; set; }
        /// <summary>Page size for server-side pagination. When > 0, pagination is applied.</summary>
        public int PageSize { get; set; }

        //public string BillingAddress { get; set; }

    }

    /// <summary>Quotation line item. <see cref="QuotationDTO.LineTotalMode"/> on each row is an optional per-line subtotal mode for contract (type 16); leave empty to use the quotation header.</summary>
    public class QuotationItemDTO : QuotationDTO
    {
        public int ProductId { get; set; }
        public string Item { get; set; }
        public string Product { get; set; }


        // public string Code { get; set; }
        public double Rate { get; set; }
        public double OPB { get; set; }
        public double Quantity { get; set; }
        public double CB { get; set; }
        public double ClosingBalance { get; set; }
        public double SizeBalance { get; set; }

        public double Amount { get; set; }
        public double Days { get; set; }
        public double SentQty { get; set; }
        public double RecQty { get; set; }
        public double Breakage { get; set; }
        public double BreakageRate { get; set; }
        //  public double BreakageAmount { get { return Breakage * BreakageRate; } }
        public bool ChargeReturnedDate { get; set; }
        public string TaxName { get; set; }
        public double TaxRate { get; set; }
        //  public string TaxAmount { get; set; }
        public string Size { get; set; }
        public double Weight { get; set; }
        public int ItemCategory { get; set; }
        public int ProductSizeId { get; set; }
        public int TaxCategoryId { get; set; }
        public new double IGST { get; set; }
        public new double CGST { get; set; }
        public new double SGST { get; set; }
        /// <summary>IGST rate % for this line (persisted on QuotationItems).</summary>
        public double IGSTRate { get; set; }
        /// <summary>CGST rate % for this line (persisted on QuotationItems).</summary>
        public double CGSTRate { get; set; }
        /// <summary>SGST rate % for this line (persisted on QuotationItems).</summary>
        public double SGSTRate { get; set; }
        public double Duration { get; set; }
    }

    public class BillChallanDto : MasterDTO
    {
        public int BillChallanId { get; set; }
        public int ChallanId { get; set; }
        public short Type { get; set; }
        public int InvoiceId { get; set; }
        public string ChallanNumber { get; set; }
        public int LedgerId { get; set; }
    }

}
