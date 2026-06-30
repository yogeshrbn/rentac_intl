using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public partial class LedgerTransactionDTO : MasterDTO
    {
        public int RowNum { get; set; }
        public int LedgerTransactionId { get; set; }
        public String Code { get; set; }
        public String Name { get; set; }
        public int DrLedgerId { get; set; }
        public byte Advance { get; set; }
        //Do not use this
        public String DrLedgerName { get; set; }
        public string RefLedgerName { get; set; }
        public int CrLedgerId { get; set; }
        public String CrLedgerName { get; set; }
        public double TransactionAmount { get; set; }
        public double PaidAmount { get; set; }
        public double Balance { get; set; }
        public double OpBalance { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public string BalanceType { get; set; }

        //    public int CreatedBy { get; set; }
        public BAL.Enums.TransactionModes TransactionMode { get; set; }
        //  public DateTime CreationDate { get; set; }
        public string CrLedger { get; set; }
        public string DrLedger { get; set; }
        public string ReceiptNumber { get; set; }
        public Int16 EntryType { get; set; }
        public String EntryTypeName { get; set; }
        public String TranRefNumber { get; set; }
        public String Narration { get; set; }
        public String LedgerName { get; set; }
        public string Group { get; set; }
        public int TransactionStatus { get; set; }
        public int TransactionType { get; set; }
        public string TransactionTypeName { get; set; }
        //  public int CompanyId { get; set; }
        public int WorkOrderId { get; set; }
        /// <summary>When set on ledger transactions, stored after insert. On quotations, maps to <see cref="QuotationDTO"/> / <c>Quotation.ContractId</c> (FK to contract).</summary>
        public int ContractId { get; set; }
        public int LedgerSiteId { get; set; }
        public int TDS { get; set; }
        public double Discount { get; set; }
        public byte RateCalcType { get; set; }
        public double DiscountPercent { get; set; }

        public String ChequeNumber { get; set; }
        public String ChequeDate { get; set; }
        public int ChequeBankId { get; set; }
        public String ExecutiveName { get; set; }

        public string Details { get; set; }
        public double SubTotal { get; set; }
        public double TaxAmount { get; set; }
        /// <summary>Sum of per-bill TDS on quick receipt / payment vouchers (cash allocation).</summary>
        public double TotalTds { get; set; }
        public double Total { get; set; }
        public double Closingbalance { get; set; }
        public int LedgerId { get; set; }
        public string Client { get; set; }
        public List<TaxDTO> ApplicableTaxes { get; set; }
        public double Freight { get; set; }
        public double FreightTax { get; set; }
        public double BreakageTax { get; set; }
        public string WorkOrderNumber { get; set; }
        public string JobNumber { get; set; }
        public string BillNumber { get; set; }
        public string Site { get; set; }

        public string VoucherNumber { get; set; }
        //  public string TransactionMode { get; set; }
        public int BankId { get; set; }
        public int RefLedgerId { get; set; }
        public int PurchaseId { get; set; }
        public int Invoiceid { get; set; }
        public float ClosingBalanceBeforeFromDate { get; set; }

        public List<LedgerTransactionDetailDto> TxnDetails { get; set; }

        public string ClientEmail { get; set; }
        public int RefTransactionId { get; set; }

        /// <summary>Web-relative temp path from StageQuickReceiptImage; consumed on save then cleared.</summary>
        public string ReceiptStagingPath { get; set; }

        /// <summary>Permanent web-relative path (docs/receipt/...) after save; returned on reads.</summary>
        public string ReceiptDocumentPath { get; set; }
    }

    public class LedgerTransactionDetailDto : BaseDTO
    {
        public int LedgerTransactionId { get; set; }
        public string BillType { get; set; }
        public int BillId { get; set; }
        public double AppliedAmount { get; set; }
        /// <summary>TDS amount allocated to this bill on the voucher (quick receipt / payment).</summary>
        public double TdsAmount { get; set; }
    }


    public partial class LedgerbalanceDTO
    {

        public int LedgerId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ClientName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Product { get; set; }
        public int ProductId { get; set; }
        public int OpeningBalance { get; set; }
        public double IssuedQty { get; set; }
        public double ReceivedQty { get; set; }
        public string SiteAddress { get; set; }
        public int LedgerSiteId { get; set; }
        public double ShortQty { get; set; }
        public double ExcessQty { get; set; }
        public double ScrapQty { get; set; }

        public int ClosingBalance { get; set; }
        public double SizeBalance { get; set; }
        public double WeightBalance { get; set; }

        public string Unit { get; set; }
        public double UnitSizeRate { get; set; }
        public double LossRate { get; set; }
        public double BreakageRate { get; set; }
        public double WeightRate { get; set; }

        public double SaleRate { get; set; }

        public double BalanceQuantitySaleAmount { get { return ClosingBalance * SaleRate; } }
        public double SizeAmount { get { return SizeBalance * UnitSizeRate; } }
        public double WeightAmount { get { return WeightBalance * WeightRate; } }

    }



    public class AccountLedgerDTO
    {
        public string Group { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal OpeningBalance { get; set; }

        /// <summary>
        /// Closing balance in decimal
        /// </summary>
        public decimal Balance { get; set; }
        /// <summary>
        /// Closing balance type 1- Debit, 2- Credit
        /// </summary>
        public decimal BalanceType { get; set; }

        /// <summary>
        /// To print on reports
        /// </summary>
        public string ClosingBalance { get; set; }

        public int OBType { get; set; }

    }

    public partial class StockInventoryDto
    {

        public int LedgerId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ClientName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Product { get; set; }
        public int ProductId { get; set; }
        public int OpeningBalance { get; set; }
        public double IssuedQty { get; set; }
        public double ReceivedQty { get; set; }
        public double PurchaseQty { get; set; }
        public double SaleQty { get; set; }
        public double OnFloor { get; set; }
        public double OnSite { get; set; }
        public decimal Size { get; set; }
        public int UOM { get; set; }
        public int PurchaseUnitId { get; set; }
        public int ClosingBalance { get; set; }
        public decimal PurchaseUnitQty { get; set; }
        public String PurchaseUnitName { get; set; }
        public String Unit { get; set; }
        public decimal BalanceInPurchaseUnit { get; set; }
       


    }
}
