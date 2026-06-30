using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using System.Data;
using BAL.Exceptions;
using BAL.Objects;
using System.Diagnostics;
using System.ComponentModel.Design;
namespace BAL.DAL
{
    class PurchaseDAL
    {
        internal async Task<bool> Add(PurchaseDTO dataOBject)
        {
            SQL objSql = new SQL();
            Int32 _purchaseId = 0;
            try
            {
                dataOBject.TaxAmount = 0;
                if (dataOBject.Items.Count > 0)
                {
                    dataOBject.TaxAmount = dataOBject.Items.Sum(o => o.IGST + o.CGST + o.SGST);
                }
                dataOBject.Total = dataOBject.SubTotal - dataOBject.DiscountAmount + dataOBject.TaxAmount;
                objSql.BeginTransaction();
                objSql.NewCommand();
                #region Purchase
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 50, dataOBject.CompanyId);
                //      objSql.AddParameter("@InvoiceNumber", DbType.String, ParameterDirection.Input, 50, dataOBject.InvoiceNumber);
                objSql.AddParameter("@VchNumber", DbType.String, ParameterDirection.Input, 50, dataOBject.VoucherNumber);
                objSql.AddParameter("@PartyBillNo", DbType.String, ParameterDirection.Input, 50, dataOBject.BillNumber);
                objSql.AddParameter("@PurchaseType", DbType.Int32, ParameterDirection.Input, 50, dataOBject.PurchaseType);
                objSql.AddParameter("@PurchaseDate", DbType.DateTime, ParameterDirection.Input, 50, dataOBject.PurchaseDate);
                objSql.AddParameter("@CreatedBy", DbType.Int32, ParameterDirection.Input, 50, dataOBject.CreatedBy);
                objSql.AddParameter("@Details", DbType.String, ParameterDirection.Input, 50, dataOBject.Details);
                objSql.AddParameter("@SubTotal", DbType.Double, ParameterDirection.Input, 50, dataOBject.SubTotal);
                objSql.AddParameter("@TaxAmount", DbType.Double, ParameterDirection.Input, 50, dataOBject.TaxAmount);
                objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 50, dataOBject.LedgerId);
                objSql.AddParameter("@PurchaseAccount", DbType.Int32, ParameterDirection.Input, 50, dataOBject.PurchaseAccountId);
                objSql.AddParameter("@Freight", DbType.Double, ParameterDirection.Input, 50, dataOBject.Freight);
                objSql.AddParameter("@FreightTax", DbType.Double, ParameterDirection.Input, 50, dataOBject.FreightTax);
                objSql.AddParameter("@FinYearId", DbType.Int32, ParameterDirection.Input, 50, dataOBject.FinYearId);

                objSql.AddParameter("@DiscountPercent", DbType.Double, ParameterDirection.Input, 50, dataOBject.DiscountPercent);

                objSql.AddParameter("@DiscountAmount", DbType.Double, ParameterDirection.Input, 50, dataOBject.DiscountAmount);
                objSql.AddParameter("@vendorCreditNoteNumber", DbType.String, ParameterDirection.Input, 0, dataOBject.VendorCreditNoteNumber);

          
                if (dataOBject.VendorCreditNoteDate.Year > 2000)
                    objSql.AddParameter("@vendorCreditNoteDate", DbType.Date, ParameterDirection.Input, 0, dataOBject.VendorCreditNoteDate);

                objSql.AddParameter("@guid", DbType.String, ParameterDirection.Input, 0, dataOBject.Guid);
                if (!String.IsNullOrEmpty(dataOBject.Doc1))
                    objSql.AddParameter("@doc1", DbType.String, ParameterDirection.Input, 0, dataOBject.Doc1);

                objSql.AddParameter("@warehouseId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.WarehouseId);

                objSql.AddParameter("@total ", DbType.Double, ParameterDirection.Input, 0, dataOBject.Total);
                if (dataOBject.PurchaseId > 0)
                {
                    objSql.AddParameter("@PurchaseId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.PurchaseId);
                    dataOBject.PurchaseId = _purchaseId = Convert.ToInt32(await objSql.ExecuteScalarAsync(UPDATE));
                }
                else
                {

                    var ds = objSql.ExecuteDataSet(ADD);
                    var newPurchase = objSql.ContructList<PurchaseDTO>(ds).FirstOrDefault(); ;
                    if (newPurchase != null)
                    {
                        dataOBject.PurchaseId = _purchaseId = newPurchase.PurchaseId;
                        dataOBject.VoucherNumber = newPurchase.VoucherNumber;
                    }

                }
                #region ItemAndTax
                foreach (PurchaseItemDTO item in dataOBject.Items)
                {
                    // item.WorkOrder = new WorkOrderDTO { InvoiceId = _invoiceId };
                    item.PurchaseId = _purchaseId;
                    objSql.NewCommand();
                    AddItem(item, objSql);

                    if (dataOBject.PurchaseType == 1 || dataOBject.PurchaseType == 2)
                    {
                        //update item balance
                        var inv = new InventoryDAL();
                        var i = await inv.UpdateItemBalance(dataOBject.FinYearId, dataOBject.CompanyId, item.ProductId);
                        if (i == 0)
                        {
                            throw new Exception("Could not update item stock. Please try again.");
                        }
                    }
                }
                //foreach (TaxDTO item in dataOBject.ApplicableTaxes)
                //{
                //    objSql.NewCommand();
                //    AddTax(dataOBject, item, objSql);
                //}
                objSql.NewCommand();
                LedgerDAL objLedger = new LedgerDAL();
                var txnType = 2;
                short entryType = 14;
                string description = "Purchase Ref No:" + dataOBject.VoucherNumber;
                if (dataOBject.PurchaseType == 2)
                {
                    entryType = 15;
                    txnType = 1;
                    description = "Purchase return ref No:" + dataOBject.VoucherNumber;
                }
                int txnId = objLedger.CreateTransactions(new LedgerTransactionDTO
                {
                    LedgerId = dataOBject.LedgerId,
                    TransactionAmount = dataOBject.Total,
                    // TransactionDate = dataOBject.PurchaseDate.ToShortDateString(),
                    TransactionDate = dataOBject.PurchaseDate,

                    DrLedgerId = dataOBject.LedgerId,
                    Description = description,
                    CreatedBy = dataOBject.CreatedBy,
                    TransactionType = txnType,
                    TransactionMode = 0,
                    EntryType = entryType,
                    Narration = description,
                    TranRefNumber = dataOBject.PurchaseId.ToString(),
                    FinYearId = dataOBject.FinYearId,
                    CompanyId = dataOBject.CompanyId,
                    LedgerSiteId = dataOBject.LedgerSiteId,
                    Invoiceid = 0

                }, objSql);
                if (txnId == 0)
                {
                    throw new Exception("Could not save transaction");
                }


                objSql.Commit();
                #endregion
            }
            catch (Exception ex)
            {
                objSql.Rollback();
                throw ex;
            }
            #endregion


            return _purchaseId > 0;
        }
        bool AddTax(PurchaseDTO dto, TaxDTO taxDto, SQL objSql)
        {
            taxDto.Amount = dto.SubTotal;
            objSql.AddParameter("@TaxId", DbType.Int32, ParameterDirection.Input, 0, taxDto.TaxId);
            objSql.AddParameter("@ProductId", DbType.Int32, ParameterDirection.Input, 0, taxDto.ItemValue);

            objSql.AddParameter("@Rate", DbType.Double, ParameterDirection.Input, 0, taxDto.Rate);
            objSql.AddParameter("@Amount", DbType.Double, ParameterDirection.Input, 0, taxDto.TaxAmount);
            objSql.AddParameter("@PurchaseId", DbType.Int32, ParameterDirection.Input, 0, dto.PurchaseId);
            return objSql.ExecuteNonQuery(ADD_TAX) > 0;
        }
        public void AddItem(PurchaseItemDTO dto, SQL objSql)
        {
            try
            {


                objSql.AddParameter("@ProductId", DbType.Int64, ParameterDirection.Input, 0, dto.ProductId);
                objSql.AddParameter("@Rate", DbType.Double, ParameterDirection.Input, 0, dto.Rate);
                objSql.AddParameter("@SubTotal", DbType.Double, ParameterDirection.Input, 0, dto.SubTotal);
                objSql.AddParameter("@Quantity", DbType.Double, ParameterDirection.Input, 0, dto.Quantity);
                objSql.AddParameter("@Unit1Quantity", DbType.Decimal, ParameterDirection.Input, 0, dto.Unit1Quantity);
                objSql.AddParameter("@PurchaseId", DbType.Int32, ParameterDirection.Input, 0, dto.PurchaseId);

                objSql.AddParameter("@TaxCategoryId", DbType.Int16, ParameterDirection.Input, 0, dto.TaxCategoryId);
                objSql.AddParameter("@IGST", DbType.Double, ParameterDirection.Input, 0, dto.IGST);
                objSql.AddParameter("@CGST", DbType.Double, ParameterDirection.Input, 0, dto.CGST);
                objSql.AddParameter("@SGST", DbType.Double, ParameterDirection.Input, 0, dto.SGST);
                objSql.AddParameter("@Total", DbType.Double, ParameterDirection.Input, 0, dto.Total);


                objSql.ExecuteNonQuery(ADD_ITEMS);

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public List<PurchaseDTO> PurchaseRegister(PurchaseFilterDTO filter)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);
            objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, filter.LedgerId);
            objSql.AddParameter("@purchaseType", DbType.Byte, ParameterDirection.Input, 0, filter.PurchaseType);
            objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, filter.From);
            objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, filter.To);


            return objSql.ContructList<PurchaseDTO>(objSql.ExecuteDataSet(PURCHASE_REGISTER));
        }
        public List<PurchaseItemDTO> PurchaseItemsList(int purchaseId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@purchaseId", DbType.Int32, ParameterDirection.Input, 0, purchaseId);
            return objSql.ContructList<PurchaseItemDTO>(objSql.ExecuteDataSet(PURCHASe_ITEMS_LIST));
        }
        public List<PurchaseItemDTO> PurchaseItemsTax(int puchaseId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@purchaseId", DbType.Int32, ParameterDirection.Input, 0, puchaseId);
            return objSql.ContructList<PurchaseItemDTO>(objSql.ExecuteDataSet(PURCHASE_ITEMS_TAX));
        }
        public DataSet GetReceiptRegisterPRT(int purchaseId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@purchaseId", DbType.Int32, ParameterDirection.Input, 0, purchaseId);
            return objSql.ExecuteDataSet(PURCHASe_ITEMS_LIST);
        }


        public async Task<PurchaseDTO> ById(int purchaseId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@purchaseId", DbType.Int32, ParameterDirection.Input, 0, purchaseId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            var purchase = await objSql.QueryFirstAsync<PurchaseDTO>(PURCHASE_BY_ID);
            if (purchase == null)
            {
                throw new UDFException("Purhcase entry not found", ErrorCodes.PURCHASE_NOT_FOUND_BYID);
            }
            purchase.Items = PurchaseItems(purchaseId);
            return purchase;
        }
        public List<PurchaseItemDTO> PurchaseItems(int purchaseId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@purchaseId", DbType.Int32, ParameterDirection.Input, 0, purchaseId);
            return objSql.ContructList<PurchaseItemDTO>(objSql.ExecuteDataSet(PURCHASEITEMS_BY_ID));
        }

        public async Task<IEnumerable<PurchaseDTO>> GetUnpaidBills(int ledgerId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            var _list = await objSql.QueryAsync<PurchaseDTO>(PURCHASE_INVOICE_UNPAID);

            return _list;
        }

        public async Task<IEnumerable<PurchaseDTO>> GetBillsByIds(string billIds, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@ids", DbType.String, ParameterDirection.Input, 0, billIds);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            var _list = await objSql.QueryAsync<PurchaseDTO>(PURCHASE_INVOICE_BYIDS);

            return _list;
        }

        public async Task<bool> UpdateStatus(PurchaseDTO dto)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@purchaseId", DbType.Int32, ParameterDirection.Input, 0, dto.PurchaseId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
            objSql.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, dto.ModifiedBy);
            objSql.AddParameter("@statusId", DbType.Byte, ParameterDirection.Input, 0, dto.StatusId);
            objSql.AddParameter("@modifiedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedOn);


            var _list = await objSql.ExecuteNonQueryAsync(PURCHASE_STATUS_UPDATE);

            return _list > 0;
        }

        #region Procedures
        const string ADD = "p_Purchase_ins";
        const string UPDATE = "p_Purchase_upd";

        const string ADD_ITEMS = "p_PurchaseItems_ins";
        const string ADD_TAX = "p_PurchaseTax_ins";
        const string PURCHASE_REGISTER = "p_PurchaseReg_sel";
        const string PURCHASe_ITEMS_LIST = "p_getPurchaseItems";
        const string PURCHASE_ITEMS_TAX = "p_purchaseItemsTax_sel";
        const string PURCHASE_BY_ID = "p_Purchase_byId";
        const string PURCHASEITEMS_BY_ID = "p_getPurchaseItems_byPurchaseId";
        const string PURCHASE_INVOICE_UNPAID = "p_unpaidPurchaseInvoice_sel";
        const string PURCHASE_INVOICE_BYIDS = "p_pourchaseBills_ByIds";

        const string PURCHASE_STATUS_UPDATE = "p_pourchase_updStatus";



        #endregion
    }
}
