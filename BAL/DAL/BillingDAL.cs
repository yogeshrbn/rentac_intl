using BAL.DTO;
using BAL.Enums;
using BAL.Exceptions;
using BAL.Objects;
using BAL.Services;
using Microsoft.IdentityModel.Tokens;
using NLog;
using Omu.ValueInjecter;
using Razorpay.Api;
using Razorpay.Api.Errors;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace BAL.DAL
{
    internal class BillingDAL
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        public List<BillingItemDTO> GeneratBill(int ledgerId, string from, string to, int ledgerSiteId, int finYearId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, from);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, to);
            objSql.AddParameter("@LedgerId", DbType.Int16, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@FinYearId", DbType.Int16, ParameterDirection.Input, 0, finYearId);

            objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);

            return objSql.ContructList<BillingItemDTO>(objSql.ExecuteDataSet(GEN_BiLL));
        }
        public List<BillingItemDTO> GeneratBill(BillingDTO dto, int finYearId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, dto.From);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, dto.To);
            objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerId);
            objSql.AddParameter("@FinYearId", DbType.Int16, ParameterDirection.Input, 0, finYearId);
            objSql.AddParameter("@InvoiceId", DbType.Int32, ParameterDirection.Input, 0, dto.InvoiceId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);

            objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerSiteId);
            objSql.AddParameter("@invoiceType", DbType.Int32, ParameterDirection.Input, 0, dto.InvoiceType);
            //  objSql.AddParameter("@filterChallansByPO", DbType.Boolean, ParameterDirection.Input, 0, dto.FilterChallansByPO);

            if (!String.IsNullOrEmpty(dto.PONumber) && dto.FilterChallansByPO)
            {
                objSql.AddParameter("@poNumber", DbType.String, ParameterDirection.Input, 0, dto.PONumber);

            }


            return objSql.ContructList<BillingItemDTO>(objSql.ExecuteDataSet(GEN_BiLL));
        }
        public async Task<IEnumerable<BillingItemDTO>> GetBreakageForBill(int companyId, int ledgerId, int invoiceId, int ledgerSiteId, DateTime from, DateTime to, int finYearId)
        {
            SQL objSql = new SQL();
            //objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, from);
            //objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, to);
            objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, invoiceId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            objSql.AddParameter("@FinYearId", DbType.Int16, ParameterDirection.Input, 0, finYearId);
            objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);

            return await objSql.QueryAsync<BillingItemDTO>(GET_BREAKAGE_FOR_BILL);
            // return objSql.ContructList<BillingItemDTO>(objSql.ExecuteDataSet(GET_BREAKAGE_FOR_BILL));
        }

        /// <summary>Damage component lines for an existing invoice (saved snapshot or live GRN query via SP).</summary>
        public async Task<IEnumerable<BreakageDamageDetailDTO>> GetBreakageDamageDetailsForBill(int companyId, int ledgerId, int invoiceId)
        {
            if (invoiceId <= 0)
                return Enumerable.Empty<BreakageDamageDetailDTO>();
            var info = GetBillingInfo(invoiceId);
            if (info == null || info.InvoiceId <= 0)
                return Enumerable.Empty<BreakageDamageDetailDTO>();
            var cid = companyId > 0 ? companyId : info.CompanyId;
            return await GetBreakageDamageDetailsForBill(cid, ledgerId, invoiceId, info.LedgerSiteId, info.From, info.To, info.FinYearId);
        }

        /// <summary>GRN / saved damage lines: pass bill period and site for live GRN rows; <paramref name="invoiceId"/> selects saved snapshot when present.</summary>
        public async Task<IEnumerable<BreakageDamageDetailDTO>> GetBreakageDamageDetailsForBill(
            int companyId, int ledgerId, int invoiceId, int ledgerSiteId, DateTime from, DateTime to, int finYearId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, invoiceId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@FinYearId", DbType.Int32, ParameterDirection.Input, 0, finYearId);
            objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);
            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, from.Date);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, to.Date);
            return await objSql.QueryAsync<BreakageDamageDetailDTO>(GET_BREAKAGE_DAMAGE_DETAILS_FOR_BILL);
        }

        public async Task<IEnumerable<BreakageDamageDetailDTO>> GetBreakageDamageDetailsForSeparateBill(
        int companyId, int ledgerId, int invoiceId, int ledgerSiteId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, invoiceId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);

            return await objSql.QueryAsync<BreakageDamageDetailDTO>("p_getBreakageDamageComponents_forSeparateBill");
        }


        /// <summary>Bill edit: rows from <c>InvoiceBreakageDamageComponent</c> only (no live GRN query).</summary>
        public async Task<List<BreakageDamageDetailDTO>> GetInvoiceBreakageDamageComponentsForEdit(int invoiceId, int companyId)
        {
            if (invoiceId <= 0 || companyId <= 0)
                return new List<BreakageDamageDetailDTO>();
            SQL objSql = new SQL();
            objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, invoiceId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            var rows = await objSql.QueryAsync<BreakageDamageDetailDTO>(SEL_INVOICE_BREAKAGE_DAMAGE_BY_INVOICE);
            return rows != null ? rows.ToList() : new List<BreakageDamageDetailDTO>();
        }

        //public async Task<BillingDTO> SaveBill(int ledgerId, DateTime from, DateTime to, string workOrderNumber, BillingDTO dataObject)
        //{
        //    SQL objSql = new SQL();
        //    try
        //    {

        //        objSql.BeginTransaction();
        //        objSql.NewCommand();

        //        objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, from);
        //        objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, to);
        //        objSql.AddParameter("@LedgerId", DbType.Int16, ParameterDirection.Input, 0, ledgerId);
        //        objSql.AddParameter("@TaxAmount", DbType.Double, ParameterDirection.Input, 0, dataObject.TaxAmount);
        //        objSql.AddParameter("@FinYearId", DbType.Double, ParameterDirection.Input, 0, dataObject.FinYearId);
        //        // objSql.AddParameter("@WorkOrderNumber", DbType.String, ParameterDirection.Input, 0, workOrderNumber);
        //        BillingDTO billDto = objSql.ContructList<BillingDTO>(objSql.ExecuteDataSet(SAVE_BiLL)).FirstOrDefault();


        //        foreach (TaxDTO item in dataObject.ApplicableTaxes)
        //        {
        //            objSql.NewCommand();
        //            AddTax(dataObject, item, objSql);
        //        }
        //        if (billDto.InvoiceId > 0)
        //        {
        //            List<BillingItemDTO> breakageItems = GetBreakageForBill(ledgerId, dataObject.InvoiceId, dataObject.LedgerSiteId, from, to, dataObject.FinYearId);
        //            if (breakageItems.Count > 0)
        //            {
        //                //dataObject.InvoiceDate = DateTime.Today;
        //                billDto.BillableItems = breakageItems;
        //                //dataObject.InvoiceNumber = billDto.BillNumber;
        //                billDto.From = dataObject.From;
        //                billDto.To = dataObject.To;
        //                billDto.LedgerId = ledgerId;
        //                billDto.ParentInvoiceId = billDto.InvoiceId;
        //                billDto.SubTotal = breakageItems.Sum(o => o.Quantity * o.Rate);
        //                billDto.TaxAmount = 0;
        //                billDto.Total = billDto.SubTotal;
        //                billDto.ApplicableTaxes = new List<TaxDTO>(); //no taxes as of now
        //                var x = await SaveBreakageBill(billDto);
        //            }
        //        }
        //        objSql.Commit();
        //    }
        //    catch (Exception ex)
        //    {
        //        objSql.Rollback();
        //    }

        //    return dataObject;

        //}
        bool AddTax(BillingDTO dto, TaxDTO taxDto, SQL objSql)
        {
            taxDto.Amount = dto.SubTotal;
            objSql.AddParameter("@TaxId", DbType.Int32, ParameterDirection.Input, 0, taxDto.TaxId);
            objSql.AddParameter("@Rate", DbType.Double, ParameterDirection.Input, 0, taxDto.Rate);
            objSql.AddParameter("@Amount", DbType.Double, ParameterDirection.Input, 0, taxDto.TaxAmount);
            objSql.AddParameter("@InvoiceId", DbType.Int32, ParameterDirection.Input, 0, dto.InvoiceId);
            objSql.AddParameter("@ProductId", DbType.Int32, ParameterDirection.Input, 0, taxDto.ItemValue);

            return objSql.ExecuteNonQuery(ADD_TAX) > 0;
        }
        public List<BillingDTO> GetBilList(string from, string to, int companyId, int ledgerId, int ledgerSiteId, int statusId, short InvoiceType)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, from);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, to);
            objSql.AddParameter("@statusId", DbType.Byte, ParameterDirection.Input, 0, statusId);
            objSql.AddParameter("@InvoiceType", DbType.Byte, ParameterDirection.Input, 0, InvoiceType);

            if (ledgerId > 0)
                objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            if (ledgerSiteId > 0)
            {
                objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);
            }
            var ds = objSql.ExecuteDataSet(GET_BILLS);
            var billingList = objSql.ContructList<BillingDTO>(ds);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                var invoiceId = Convert.ToInt32(row["InvoiceId"]);
                var bill = billingList.Where(o => o.InvoiceId == invoiceId).First();
                if (bill != null)
                {
                    if (row["irn"] != DBNull.Value)
                    {
                        bill.IrnDetails = new InvoiceIRNDTO();
                        bill.IrnDetails.IRN = Convert.ToString(row["irn"]);
                    }
                }
            }
            return billingList;
        }
        public async Task<IEnumerable<BillingDTO>> GetContractBills(int companyId, int contractId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@contractId", DbType.Int32, ParameterDirection.Input, 0, contractId);
            return await objSql.QueryAsync<BillingDTO>(GET_CONTRACT_BILLS);
        }

        public List<BillingItemDTO> BillItems(int billId)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@InvoiceId", DbType.Int32, ParameterDirection.Input, 0, billId);
            var ds = objSql.ExecuteDataSet(GET_BILLITEMS);
            var list = objSql.ContructList<BillingItemDTO>(ds);
            foreach (var item in list)
            {
                //var hsnCode = dr["HSNCode"] != DBNull.Value ? Convert.ToString(dr["HSNCode"]) : "";
                //short productType = dr["ProductType"] != DBNull.Value ? Convert.ToInt16(dr["ProductType"]) : (short)0;
                //var unit = dr["Unit"] != DBNull.Value ? Convert.ToString(dr["Unit"]) : "";
                //var productId = Convert.ToInt32(dr["ProductId"]);
                //var description = dr["Description"] != DBNull.Value ? Convert.ToString(dr["Description"]) : "";
                //var itemName = Convert.ToString(dr["Item"]);

                //var item = list.Where(o => o.ProductId == productId).First();
                //if (item != null)
                //{
                item.ItemMaster = new ProductDTO();
                item.ItemMaster.ProductId = item.ProductId;
                item.ItemMaster.ProductType = item.ProductType;
                item.ItemMaster.Description = String.IsNullOrEmpty(item.Description) ? "" : item.Description;
                item.ItemMaster.Unit = String.IsNullOrEmpty(item.Unit) ? "" : item.Unit;
                item.ItemMaster.Name = item.Item;
                item.ItemMaster.HSNCode = String.IsNullOrEmpty(item.HSNCode) ? "" : item.HSNCode;

                // }
            }
            //foreach (DataRow dr in ds.Tables[0].Rows)
            //{
            //    var hsnCode = dr["HSNCode"] != DBNull.Value ? Convert.ToString(dr["HSNCode"]) : "";
            //    short productType = dr["ProductType"] != DBNull.Value ? Convert.ToInt16(dr["ProductType"]) : (short)0;
            //    var unit = dr["Unit"] != DBNull.Value ? Convert.ToString(dr["Unit"]) : "";
            //    var productId = Convert.ToInt32(dr["ProductId"]);
            //    var description = dr["Description"] != DBNull.Value ? Convert.ToString(dr["Description"]) : "";
            //    var itemName = Convert.ToString(dr["Item"]);

            //    var item = list.Where(o => o.ProductId == productId).First();
            //    if (item != null)
            //    {
            //        item.ItemMaster = new ProductDTO();
            //        item.ItemMaster.ProductId = productId;
            //        item.ItemMaster.ProductType = productType;
            //        item.ItemMaster.Description = description;
            //        item.ItemMaster.Unit = unit;
            //        item.ItemMaster.Name = itemName;
            //        item.ItemMaster.HSNCode = hsnCode;

            //    }
            //}

            return list;
        }

        public async Task<BillingDTO> GetByIdForEdit(BillingDTO billdto)
        {
            try
            {
                SQL objSql = new SQL();

                objSql.AddParameter("@InvoiceId", DbType.Int32, ParameterDirection.Input, 0, billdto.InvoiceId);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, billdto.CompanyId);

                var billDto = await objSql.QueryFirstAsync<BillingDTO>(BY_ID);
                if (billDto == null)
                {
                    throw new UDFException("Bill not found", Exceptions.ErrorCodes.BILL_NOT_FOUND_FOR_BLLID);
                }
                objSql.NewCommand();
                objSql.AddParameter("@InvoiceId", DbType.Int32, ParameterDirection.Input, 0, billDto.InvoiceId);

                var items = await objSql.QueryAsync<BillingItemDTO>(GET_BILLITEMS);
                if (items != null)
                {
                    billDto.BillableItems = items.Where(o => !o.IsBreakage).ToList();
                }
                if (!String.IsNullOrEmpty(billDto.PONumbers))
                {
                    billDto.PO = billDto.PONumbers.Split(',').Select(o => new BillPODto
                    {
                        PONumber = o
                    }).ToList();
                }
                objSql.NewCommand();
                objSql.AddParameter("@InvoiceId", DbType.Int32, ParameterDirection.Input, 0, billDto.InvoiceId);
                var lossItems = await objSql.QueryAsync<LostItemDTO>(INVOICE_LOSS_ITEM);
                if (lossItems != null)
                {
                    billDto.LostItems = lossItems.ToList();
                }
                objSql.NewCommand();
                //objSql.AddParameter("@parentInvoiceId", DbType.Int32, ParameterDirection.Input, 0, billDto.InvoiceId);
                objSql.AddParameter("@InvoiceId", DbType.Int32, ParameterDirection.Input, 0, billDto.InvoiceId);
                var breakageItems = await objSql.QueryAsync<BillingItemDTO>(INVOICE_BREAKAGE_ITEM);
                if (breakageItems != null)
                {
                    billDto.BreakageItems = breakageItems.ToList();
                    if (billDto.BreakageItems.Count > 0)
                        billDto.IncludeBreakageItems = true;

                }
                objSql.NewCommand();
                objSql.AddParameter("@InvoiceId", DbType.Int32, ParameterDirection.Input, 0, billDto.InvoiceId);
                var otherCharges = await objSql.QueryAsync<InvoiceChargeDTO>(INVOICE_OTHER_CHARGES_SEL);
                if (otherCharges != null)
                {
                    billDto.OtherCharges = otherCharges.ToList();
                }
                objSql.NewCommand();
                objSql.AddParameter("@InvoiceId", DbType.Int32, ParameterDirection.Input, 0, billDto.InvoiceId);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, billDto.CompanyId);

                var challans = await objSql.QueryAsync<BillChallanDto>(BILL_CHALLANS_SEL);
                if (challans != null)
                {
                    billDto.Challans = challans.ToList();
                }

                objSql.NewCommand();
                objSql.AddParameter("@InvoiceId", DbType.Int32, ParameterDirection.Input, 0, billDto.InvoiceId);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, billDto.CompanyId);

                var payments = await objSql.QueryAsync<LedgerTransactionDTO>("p_getBillPayments");
                if (payments != null)
                {
                    billDto.Payments = payments.ToList();
                }

                if (billDto.BillableItems != null)
                {
                    billDto.StockBalanceAfterBill = (from d in billDto.BillableItems
                                                     group d by new { ProductId = d.ProductId } into g

                                                     select new InvoiceItemDTO
                                                     {
                                                         ProductId = g.Key.ProductId,
                                                         Item = g.Last().Item,
                                                         ClosingBalance = g.Last().ClosingBalance
                                                     }
                                                        ).Where(o => o.ClosingBalance > 0).ToList();
                    // billDto.StockBalanceAfterBill = GetBalanceAffterLastBill(billDto.LedgerId, billDto.LedgerSiteId, billDto.InvoiceId);
                }

                // Edit: damage lines only from InvoiceBreakageDamageComponent (saved at bill save; no GRN recompute).
                billDto.BreakageDamageDetails = new List<BreakageDamageDetailDTO>();
                var companyIdForDamage = billDto.CompanyId > 0 ? billDto.CompanyId : billdto.CompanyId;
                if (billDto.InvoiceId > 0 && companyIdForDamage > 0)
                {
                    try
                    {
                        billDto.BreakageDamageDetails = await GetInvoiceBreakageDamageComponentsForEdit(billDto.InvoiceId, companyIdForDamage);
                        if (billDto.BreakageDamageDetails.Count > 0)
                            billDto.IncludeBreakageItems = true;
                    }
                    catch
                    {
                        billDto.BreakageDamageDetails = new List<BreakageDamageDetailDTO>();
                    }
                }

                return billDto;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet PrintBill(int billId)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@InvoiceId", DbType.Int32, ParameterDirection.Input, 0, billId);

            return objSql.ExecuteDataSet(BILL_REPORT);

        }
        public DataSet PrintContractBill(int billId)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@InvoiceId", DbType.Int32, ParameterDirection.Input, 0, billId);



            return objSql.ExecuteDataSet(BILL_CONTRACT_REPORT);

        }
        public BillingDTO GetBillingInfo(int billId)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@InvoiceId", DbType.Int32, ParameterDirection.Input, 0, billId);
            return objSql.ContructList<BillingDTO>(objSql.ExecuteDataSet(BILL_REPORT)).FirstOrDefault();
        }
        public List<BillingDTO> CheckForBilling(int ledgerId, string from, string to, string workOrderNumber)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, from);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, to);
            objSql.AddParameter("@LedgerId", DbType.Int16, ParameterDirection.Input, 0, ledgerId);
            //objSql.AddParameter("@WorkOrderNumber", DbType.String, ParameterDirection.Input, 0, workOrderNumber);
            return objSql.ContructList<BillingDTO>(objSql.ExecuteDataSet(CHECK_BILL));
        }


        public async Task<bool> SaveBreakageBill(BillingDTO dto)
        {
            dto.InvoiceType = 2; // Breakage bill
            return await Add(dto);
        }

        /// <summary>Replaces saved GRN damage component lines for the invoice (rent bill gen).</summary>
        private async Task SyncInvoiceBreakageDamageDetailsAsync(SQL objSql, BillingDTO dataOBject)
        {
            if (dataOBject.InvoiceId <= 0 || dataOBject.CompanyId <= 0)
                return;
            objSql.NewCommand();
            objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.InvoiceId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.CompanyId);
            await objSql.ExecuteNonQueryAsync(DEL_INVOICE_BREAKAGE_DAMAGE);
            if (dataOBject.BreakageDamageDetails == null || dataOBject.BreakageDamageDetails.Count == 0)
                return;
            foreach (var d in dataOBject.BreakageDamageDetails)
            {
                objSql.NewCommand();
                objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.InvoiceId);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.CompanyId);
                objSql.AddParameter("@grnItemId", DbType.Int32, ParameterDirection.Input, 0, d.GRNItemId == 0 ? (object)DBNull.Value : d.GRNItemId);
                objSql.AddParameter("@productId", DbType.Int32, ParameterDirection.Input, 0, d.ProductId);
                objSql.AddParameter("@parentItem", DbType.String, ParameterDirection.Input, 300, string.IsNullOrEmpty(d.ParentItem) ? (object)DBNull.Value : d.ParentItem);
                objSql.AddParameter("@grn", DbType.String, ParameterDirection.Input, 100, string.IsNullOrEmpty(d.GRN) ? (object)DBNull.Value : d.GRN);
                objSql.AddParameter("@componentName", DbType.String, ParameterDirection.Input, 200, string.IsNullOrEmpty(d.ComponentName) ? (object)DBNull.Value : d.ComponentName);
                objSql.AddParameter("@quantity", DbType.Decimal, ParameterDirection.Input, 0, d.Quantity);
                objSql.AddParameter("@rate", DbType.Decimal, ParameterDirection.Input, 0, d.Rate);
                objSql.AddParameter("@cost", DbType.Decimal, ParameterDirection.Input, 0, d.Cost);
                objSql.AddParameter("@receivingDate", DbType.Date, ParameterDirection.Input, 0, d.ReceivingDate.HasValue ? (object)d.ReceivingDate.Value.Date : DBNull.Value);
                objSql.AddParameter("@igst", DbType.Double, ParameterDirection.Input, 0, d.IGST);
                objSql.AddParameter("@cgst", DbType.Double, ParameterDirection.Input, 0, d.CGST);
                objSql.AddParameter("@sgst", DbType.Double, ParameterDirection.Input, 0, d.SGST);
                objSql.AddParameter("@igstRate", DbType.Double, ParameterDirection.Input, 0, d.IGSTRate);
                objSql.AddParameter("@cgstRate", DbType.Double, ParameterDirection.Input, 0, d.CGSTRate);
                objSql.AddParameter("@sgstRate", DbType.Double, ParameterDirection.Input, 0, d.SGSTRate);
                await objSql.ExecuteNonQueryAsync(INS_INVOICE_BREAKAGE_DAMAGE);
            }
        }

        internal async Task<bool> Add(BillingDTO dataOBject)
        {

            SQL objSql = new SQL();

            Int32 _invoiceId = 0;
            try
            {


                objSql.BeginTransaction();
                NextIdDAL n = new NextIdDAL();
                NextIdDTO nDTO = new NextIdDTO();


                objSql.NewCommand();
                double otherCharges = dataOBject.OtherChargeAmount;
                double lossCharges = 0, lossTax = 0;
                //var config = new ConfigDAL();
                //var configDetails = config.GetBillingConfig(dataOBject.CompanyId);
                //dataOBject.SubTotal = 0;
                if (dataOBject.BillableItems != null)
                {
                    dataOBject.TaxAmount = dataOBject.BillableItems.Sum(o => o.IGST + o.CGST + o.SGST);
                    dataOBject.Total = dataOBject.SubTotal + dataOBject.TaxAmount;
                }
                if (dataOBject.LostItems != null)
                {
                    lossCharges = dataOBject.LostItems.Sum(o => (o.Rate * o.Quantity));
                    lossTax = dataOBject.LostItems.Sum(o => (o.IGST + o.SGST + o.CGST));
                    dataOBject.Total += lossCharges + lossTax;
                }
                if (dataOBject.BreakageDamageDetails != null && dataOBject.BreakageDamageDetails.Count > 0
                    && dataOBject.InvoiceType == 1
                    )
                {
                    dataOBject.BreakageAmount = dataOBject.BreakageDamageDetails.Sum(o => Convert.ToDouble(o.Cost));
                    dataOBject.BreakageTax = dataOBject.BreakageDamageDetails.Sum(o => o.IGST + o.SGST + o.CGST);
                    dataOBject.Total += dataOBject.BreakageAmount + dataOBject.BreakageTax;
                }
                else if (dataOBject.BreakageItems != null && dataOBject.BreakageItems.Count > 0
                        && dataOBject.InvoiceType == 1
                    )
                {
                    dataOBject.BreakageAmount = dataOBject.BreakageItems.Sum(o => (o.Rate * o.Quantity));
                    dataOBject.BreakageTax = dataOBject.BreakageItems.Sum(o => (o.IGST + o.SGST + o.CGST));
                    dataOBject.Total += dataOBject.BreakageAmount + dataOBject.BreakageTax;

                }
                if (dataOBject.OtherCharges != null)
                {
                    otherCharges = dataOBject.OtherCharges.Sum(o => o.Amount);
                    dataOBject.Total += otherCharges;// + dataOBject.ChargesTax;
                }
                if (dataOBject.Taxable > 0)
                {
                    dataOBject.Total = dataOBject.Taxable + dataOBject.TaxAmount + dataOBject.FreightTax + dataOBject.ChargesTax;
                }

                //string discouintApplyConfigValue="";
                //if (configDetails != null)
                //{
                //    var discountApplyConfig = configDetails.Where(o => o.Key == "").FirstOrDefault();
                //    if(discountApplyConfig != null)
                //    {
                //          discouintApplyConfigValue = discountApplyConfig.Value;

                //    }
                //}

                //if (String.IsNullOrEmpty(discouintApplyConfigValue))
                //{
                //    discouintApplyConfigValue = "billamount";
                //}
                //if(discouintApplyConfigValue.ToLower() == "taxable")
                //{

                //}
                //else
                //{

                //}
            //    dataOBject.Total += dataOBject.Freight + dataOBject.FreightTax;


                //dataOBject.Total -= (dataOBject.Discount + dataOBject.LossDiscount + dataOBject.BreakageDiscount);
                //dataOBject.Total -= dataOBject.OutStandingType == 1 ? -dataOBject.OutStanding : dataOBject.OutStanding;

              //  dataOBject.Total = dataOBject.Taxable + dataOBject.TaxAmount;


                //return false;
                //dataOBject.InvoiceNumber = "0";
                #region Invoice
                objSql.AddParameter("@InvoiceId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.InvoiceId);

                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.CompanyId);
                objSql.AddParameter("@InvoiceNumber", DbType.String, ParameterDirection.Input, 0, dataOBject.InvoiceNumber);
                // objSql.AddParameter("@SiteId", DbType.Int32, ParameterDirection.Input, 50, dataOBject.SiteId);
                objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.LedgerId);
                objSql.AddParameter("@InvoiceDate", DbType.DateTime, ParameterDirection.Input, 0, dataOBject.InvoiceDate);
                if (dataOBject.From.Year > 2000)
                    objSql.AddParameter("@From", DbType.DateTime, ParameterDirection.Input, 0, dataOBject.From);
                if (dataOBject.To.Year > 2000)
                    objSql.AddParameter("@To", DbType.DateTime, ParameterDirection.Input, 0, dataOBject.To);

                objSql.AddParameter("@CreatedBy", DbType.Int32, ParameterDirection.Input, 0, dataOBject.CreatedBy);
                objSql.AddParameter("@Details", DbType.String, ParameterDirection.Input, 0, dataOBject.Details);
                objSql.AddParameter("@SubTotal", DbType.Double, ParameterDirection.Input, 0, dataOBject.SubTotal);
                objSql.AddParameter("@Taxable", DbType.Double, ParameterDirection.Input, 0, dataOBject.Taxable);
                objSql.AddParameter("@total", DbType.Double, ParameterDirection.Input, 0, dataOBject.Total);
                objSql.AddParameter("@TaxAmount", DbType.Double, ParameterDirection.Input, 0, dataOBject.TaxAmount);
                objSql.AddParameter("@InvoiceType", DbType.Double, ParameterDirection.Input, 0, dataOBject.InvoiceType);
                objSql.AddParameter("@Freight", DbType.Double, ParameterDirection.Input, 0, dataOBject.Freight);
                objSql.AddParameter("@FreightTax", DbType.Double, ParameterDirection.Input, 0, dataOBject.FreightTax);
                objSql.AddParameter("@FinYearId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.FinYearId);
                objSql.AddParameter("@BreakageAmount", DbType.Double, ParameterDirection.Input, 0, dataOBject.BreakageAmount);
                objSql.AddParameter("@BreakageTax", DbType.Double, ParameterDirection.Input, 0, dataOBject.BreakageTax);

                objSql.AddParameter("@OutStanding", DbType.Double, ParameterDirection.Input, 0, dataOBject.OutStanding);
                objSql.AddParameter("@OutStandingType", DbType.Int16, ParameterDirection.Input, 0, dataOBject.OutStandingType);
                objSql.AddParameter("@RoundOff", DbType.Boolean, ParameterDirection.Input, 0, dataOBject.RoundOff);
                objSql.AddParameter("@isCashBill", DbType.Boolean, ParameterDirection.Input, 0, dataOBject.IsCashBill);
                //if true store all POs of the billed challan in invoice table.PO's should be fechted as the user
                //selects the checkbox on settings dialog box on gen bill screen.
                objSql.AddParameter("@printAllPO", DbType.Boolean, ParameterDirection.Input, 0, dataOBject.PrintAllPO);
                if (dataOBject.PO != null)
                {
                    var poNumbers = String.Join(",", dataOBject.PO.Select(o=> o.PONumber));
                    objSql.AddParameter("@poNumbers", DbType.String, ParameterDirection.Input, 0, poNumbers);
                }
                objSql.AddParameter("@LedgerSiteId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.LedgerSiteId);
                objSql.AddParameter("@discount", DbType.Double, ParameterDirection.Input, 0, dataOBject.Discount);
                objSql.AddParameter("@contractId", DbType.Int64, ParameterDirection.Input, 0, dataOBject.ContractId);
                objSql.AddParameter("@chargeReturnDay", DbType.Byte, ParameterDirection.Input, 0, dataOBject.ChargeReturnDay);
                objSql.AddParameter("@RateCalcType", DbType.Byte, ParameterDirection.Input, 0, dataOBject.RateCalcType);
                objSql.AddParameter("@loss", DbType.Double, ParameterDirection.Input, 0, lossCharges);


                objSql.AddParameter("@otherCharges", DbType.Double, ParameterDirection.Input, 0, otherCharges);
                objSql.AddParameter("@chargesTax", DbType.Double, ParameterDirection.Input, 0, dataOBject.ChargesTax);
                objSql.AddParameter("@charge1", DbType.Double, ParameterDirection.Input, 0, dataOBject.Charge1);
                objSql.AddParameter("@charge2", DbType.Double, ParameterDirection.Input, 0, dataOBject.Charge2);
                objSql.AddParameter("@charge3", DbType.Double, ParameterDirection.Input, 0, dataOBject.Charge3);
                objSql.AddParameter("@charge4", DbType.Double, ParameterDirection.Input, 0, dataOBject.Charge4);
                objSql.AddParameter("@charge5", DbType.Double, ParameterDirection.Input, 0, dataOBject.Charge5);
                objSql.AddParameter("@tnc", DbType.String, ParameterDirection.Input, 0, dataOBject.Tnc);

                objSql.AddParameter("@discountPercent", DbType.Double, ParameterDirection.Input, 0, dataOBject.DiscountPercent);
                objSql.AddParameter("@breakageDiscountPercent", DbType.Double, ParameterDirection.Input, 0, dataOBject.BreakageDiscountPercent);
                objSql.AddParameter("@lossDiscountPercent", DbType.Double, ParameterDirection.Input, 0, dataOBject.LossDiscountPercent);
                objSql.AddParameter("@brekageDiscount", DbType.Double, ParameterDirection.Input, 0, dataOBject.BreakageDiscount);
                objSql.AddParameter("@lossDiscount", DbType.Double, ParameterDirection.Input, 0, dataOBject.LossDiscount);
                objSql.AddParameter("@shipTo", DbType.String, ParameterDirection.Input, 0, dataOBject.ShipTo);

                //    objSql.AddParameter("@billFromSite", DbType.Byte, ParameterDirection.Input, 0, dataOBject.BillFromSite);
                if (dataOBject.Recurring)
                {
                    if (dataOBject.StartsOn != null && dataOBject.StartsOn < dataOBject.InvoiceDate)
                    {
                        throw new Exception("Recurring invoices can not start before invoice date");
                    }
                    if (dataOBject.EndsOn != null && dataOBject.EndsOn < dataOBject.InvoiceDate)
                    {
                        throw new Exception("Recurring invoices can not start before invoice date");
                    }
                    if (dataOBject.EndsOn != null && dataOBject.EndsOn < dataOBject.StartsOn)
                    {
                        throw new Exception("Recurring invoices end date must be ahead of start date");
                    }
                    if (String.IsNullOrEmpty(dataOBject.Iteration))
                    {
                        throw new Exception("Recurring invoices repeat must be selected ('weekly','monthly','yearly')");
                    }
                    objSql.AddParameter("@Recurring", DbType.Boolean, ParameterDirection.Input, 0, dataOBject.Recurring);
                    objSql.AddParameter("@startsOn", DbType.Date, ParameterDirection.Input, 0, dataOBject.StartsOn);
                    objSql.AddParameter("@endsOn", DbType.Date, ParameterDirection.Input, 0, dataOBject.EndsOn);
                    objSql.AddParameter("@iteration", DbType.String, ParameterDirection.Input, 0, dataOBject.Iteration);
                }

                if (!String.IsNullOrEmpty(dataOBject.Remarks))
                    objSql.AddParameter("@remarks", DbType.String, ParameterDirection.Input, 0, dataOBject.Remarks);


                if (dataOBject.WorkOrderId > 0)
                {
                    objSql.AddParameter("@WorkOrderId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.WorkOrderId);
                }

                if (!String.IsNullOrEmpty(dataOBject.ContractorCode))
                {
                    objSql.AddParameter("@ContractorCode", DbType.String, ParameterDirection.Input, 0, dataOBject.ContractorCode);
                }
                if (!String.IsNullOrEmpty(dataOBject.BranchCode))
                {
                    objSql.AddParameter("@BranchCode", DbType.String, ParameterDirection.Input, 0, dataOBject.BranchCode);
                }
                if (!String.IsNullOrEmpty(dataOBject.Category))
                {
                    objSql.AddParameter("@Category", DbType.String, ParameterDirection.Input, 0, dataOBject.Category);
                }
                if (dataOBject.ParentInvoiceId > 0)
                    objSql.AddParameter("@ParentInvoiceId", DbType.Double, ParameterDirection.Input, 0, dataOBject.ParentInvoiceId);

                if (!String.IsNullOrEmpty(dataOBject.SiteAddress))
                {
                    objSql.AddParameter("@SiteAddress", DbType.String, ParameterDirection.Input, 0, dataOBject.SiteAddress);
                }
                if (!String.IsNullOrEmpty(dataOBject.PONumber))
                {
                    objSql.AddParameter("@poNumber", DbType.String, ParameterDirection.Input, 0, dataOBject.PONumber);
                }
                if (dataOBject.PODate.Year > 2000)
                {
                    objSql.AddParameter("@poDate", DbType.Date, ParameterDirection.Input, 0, dataOBject.PODate);
                }


                var inv = new InvoiceDTO();
                if (dataOBject.InvoiceId == 0)
                    inv = objSql.ContructList<InvoiceDTO>(objSql.ExecuteDataSet(ADD)).FirstOrDefault();
                else
                {
                    inv = objSql.ContructList<InvoiceDTO>(objSql.ExecuteDataSet(UPDATE_INVOICE)).FirstOrDefault();
                }
                if (inv.Invoiceid == 0)
                {
                    throw new Exception("Could not save invoice");
                }

                dataOBject.InvoiceId = _invoiceId = inv.InvoiceId;
                dataOBject.InvoiceNumber = inv.InvoiceNumber;
                string strProductIds = "";
                int del = 0;
                #region ItemAndTax
                if (dataOBject.BillableItems != null)
                {
                    var invoiceItems = dataOBject.BillableItems.Select(o => o.ProductId).ToList();
                    strProductIds = String.Join(",", invoiceItems);
                    objSql.NewCommand();
                    objSql.AddParameter("@productIds", DbType.String, ParameterDirection.Input, 0, strProductIds);
                    objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.InvoiceId);
                    objSql.AddParameter("@isLost", DbType.Int32, ParameterDirection.Input, 0, 0);
                    objSql.AddParameter("@isBreak", DbType.Int32, ParameterDirection.Input, 0, 0);

                    del = await objSql.ExecuteNonQueryAsync(DEL_INVOICE_ITEMS);

                    foreach (BillingItemDTO item in dataOBject.BillableItems)
                    {
                        // item.WorkOrder = new BillingItemDTO { InvoiceId = _invoiceId };
                        item.InvoiceId = _invoiceId;
                        objSql.NewCommand();
                        item.IsLost = false;


                        item.Total = item.SubTotal + item.IGST + item.SGST + item.CGST;

                        AddItem(item, objSql);
                        if (dataOBject.InvoiceType == 4 || dataOBject.InvoiceType == 7)
                        {
                            //update item balance
                            var inventory = new InventoryDAL();
                            var i = await inventory.UpdateItemBalance(dataOBject.FinYearId, dataOBject.CompanyId, item.ProductId);
                            if (i == 0)
                            {
                                throw new Exception("Could not update item stock. Please try again.");
                            }
                        }

                    }
                }
                foreach (TaxDTO item in dataOBject.ApplicableTaxes)
                {
                    objSql.NewCommand();


                    objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.InvoiceId);
                    objSql.AddParameter("@taxId", DbType.Int32, ParameterDirection.Input, 0, item.TaxId);

                    var x = await objSql.ExecuteNonQueryAsync(DEL_INVOICE_TAX);

                    objSql.NewCommand();

                    AddTax(dataOBject, item, objSql);
                }
                //if (dataOBject.LostItems != null)
                //{
                //    AddLossItems(dataOBject.LostItems, dataOBject.InvoiceId, objSql);
                //}

                if (dataOBject.LostItems != null)
                {
                    strProductIds = String.Join(",", dataOBject.LostItems.Select(o => o.ProductId));
                    objSql.NewCommand();
                    objSql.AddParameter("@productIds", DbType.String, ParameterDirection.Input, 0, strProductIds);
                    objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.InvoiceId);
                    objSql.AddParameter("@isLost", DbType.Int32, ParameterDirection.Input, 0, 1);
                    objSql.AddParameter("@isBreak", DbType.Int32, ParameterDirection.Input, 0, 0);
                    del = await objSql.ExecuteNonQueryAsync(DEL_INVOICE_ITEMS);

                    foreach (var lostItem in dataOBject.LostItems)
                    {
                        var item = new BillingItemDTO();
                        item.ProductId = lostItem.ProductId;
                        item.Rate = lostItem.Rate;
                        item.Quantity = lostItem.Quantity;
                        item.SubTotal = lostItem.Amount;
                        item.TaxCategoryId = lostItem.TaxCategoryId;
                        item.IGST = lostItem.IGST;
                        item.CGST = lostItem.CGST;
                        item.SGST = lostItem.SGST;
                        item.IGSTRate = lostItem.IGSTRate;
                        item.CGSTRate = lostItem.CGSTRate;
                        item.SGSTRate = lostItem.SGSTRate;
                        item.Total = item.SubTotal + item.IGST + item.SGST + item.CGST;
                        item.IsLost = true;
                        item.ChallanId = lostItem.ChallanId;

                        item.InvoiceId = _invoiceId;
                        objSql.NewCommand();
                        AddItem(item, objSql);

                    }
                }
                if (dataOBject.BreakageItems != null)
                {
                    strProductIds = String.Join(",", dataOBject.BreakageItems.Select(o => o.ProductId));
                    objSql.NewCommand();
                    objSql.AddParameter("@productIds", DbType.String, ParameterDirection.Input, 0, strProductIds);
                    objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.InvoiceId);
                    objSql.AddParameter("@isBreak", DbType.Int32, ParameterDirection.Input, 0, 1);
                    objSql.AddParameter("@isLost", DbType.Int32, ParameterDirection.Input, 0, 0);

                    del = await objSql.ExecuteNonQueryAsync(DEL_INVOICE_ITEMS);

                    foreach (var lostItem in dataOBject.BreakageItems)
                    {
                        var item = new BillingItemDTO();
                        item.ProductId = lostItem.ProductId;
                        item.Rate = lostItem.Rate;
                        item.Quantity = lostItem.Quantity;
                        // item.SubTotal = lostItem.Rate * lostItem.Quantity;
                        item.TaxCategoryId = lostItem.TaxCategoryId;

                        var damageRows = dataOBject.BreakageDamageDetails;
                        if (damageRows != null && damageRows.Count > 0)
                        {
                            item.SubTotal = damageRows.Where(o => o.ProductId == item.ProductId).Sum(o => Convert.ToDouble(o.Cost));
                            item.IGST = damageRows.Where(o => o.ProductId == item.ProductId).Sum(o => Convert.ToDouble(o.IGST));
                            item.CGST = damageRows.Where(o => o.ProductId == item.ProductId).Sum(o => Convert.ToDouble(o.CGST));
                            item.SGST = damageRows.Where(o => o.ProductId == item.ProductId).Sum(o => Convert.ToDouble(o.SGST));
                        }
                        else
                        {
                            item.SubTotal = lostItem.Rate * lostItem.Quantity;
                            item.IGST = lostItem.IGST;
                            item.CGST = lostItem.CGST;
                            item.SGST = lostItem.SGST;
                        }

                        item.IGSTRate = lostItem.IGSTRate;
                        item.CGSTRate = lostItem.CGSTRate;
                        item.SGSTRate = lostItem.SGSTRate;

                        item.Total = item.SubTotal + item.IGST + item.SGST + item.CGST;
                        item.IsBreakage = true;
                        item.ChallanId = lostItem.ChallanId;
                        item.InvoiceId = _invoiceId;
                        objSql.NewCommand();
                        AddItem(item, objSql);

                        //if(lostItem.ChallanId == 0)
                        //{
                        //    throw new UDFException("Challan Id not found for breakage", 500);
                        //}
                        //objSql.NewCommand();
                        //objSql.AddParameter("@productId", DbType.Int32, ParameterDirection.Input, 0, item.ProductId);
                        //objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, item.InvoiceId);
                        //objSql.AddParameter("@challanId", DbType.Int32, ParameterDirection.Input, 0, lostItem.ChallanId);
                        //del = await objSql.ExecuteNonQueryAsync(UPDATE_CHALLAN_ITEM_FOR_BREAKAGE);

                    }
                }
                await SyncInvoiceBreakageDamageDetailsAsync(objSql, dataOBject);
                if (dataOBject.OtherCharges != null)
                {
                    AddOtherCharges(dataOBject.OtherCharges, dataOBject.InvoiceId, objSql);
                }
                if (dataOBject.Challans != null)
                {
                    try
                    {
                        objSql.NewCommand();

                        objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.CompanyId);
                        objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.InvoiceId);
                        var x = await objSql.ExecuteNonQueryAsync(BILL_CHALLANS_DEL);

                        foreach (BillChallanDto item in dataOBject.Challans)
                        {
                            if (item.ChallanId == 0) continue;
                            objSql.NewCommand();

                            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, item.CompanyId);

                            objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.InvoiceId);
                            objSql.AddParameter("@challanId", DbType.Int32, ParameterDirection.Input, 0, item.ChallanId);
                            objSql.AddParameter("@type", DbType.Int16, ParameterDirection.Input, 0, item.Type);
                            objSql.AddParameter("@challanNumber", DbType.String, ParameterDirection.Input, 0, item.ChallanNumber);
                            objSql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, item.GuId);
                            objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, item.LedgerId);
                            objSql.AddParameter("@creationDate", DbType.DateTime, ParameterDirection.Input, 0, item.CreationDate);
                            x = await objSql.ExecuteNonQueryAsync(BILL_CHALLANS_INS);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, ex.Message, "GenBILL");
                        throw new Exception("Could not add challans:" + ex.Message, ex);
                    }
                }

                //Create transaction and update ledger balance


                LedgerDAL objLedger = new LedgerDAL();
                var txnType = 1;
                short entryType = 11;
                string description = "Bill Ref No:" + dataOBject.InvoiceNumber;
                if (dataOBject.InvoiceType == 7)
                {
                    txnType = 2;
                    entryType = 16;
                    description = "Sale return ref No:" + dataOBject.InvoiceNumber;
                }
                else if (dataOBject.InvoiceType == 4)
                {
                    entryType = 18;
                    description = "Sale bill ref No:" + dataOBject.InvoiceNumber;
                }
                else if (dataOBject.InvoiceType == 5)
                {
                    entryType = 19;
                    description = "Contract bill ref No:" + dataOBject.InvoiceNumber;
                }
                else if (dataOBject.InvoiceType == 6)
                {
                    entryType = 20;
                    description = "Material Loss bill ref No:" + dataOBject.InvoiceNumber;
                }
                else if (dataOBject.InvoiceType == 2)
                {
                    entryType = 21;
                    description = "Breakage bill ref No:" + dataOBject.InvoiceNumber;
                }
                else if (dataOBject.InvoiceType == 8)
                {
                    txnType = 2;
                    entryType = 26;
                    description = "Hire bill ref No:" + dataOBject.InvoiceNumber;
                }
                if (dataOBject.InvoiceType == 9)
                {
                    entryType = 28;
                    description = "Measurement bill ref No:" + dataOBject.InvoiceNumber;
                }

                if (dataOBject.Payments != null)
                {
                    foreach (var txnDetail in dataOBject.Payments)
                    {
                        objSql.NewCommand();

                        objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.CompanyId);
                        objSql.AddParameter("@ledgerTransactionId", DbType.Int32, ParameterDirection.Input, 0, txnDetail.LedgerTransactionId);
                        objSql.AddParameter("@amount", DbType.Decimal, ParameterDirection.Input, 0, txnDetail.TransactionAmount);
                        objSql.AddParameter("@billType", DbType.String, ParameterDirection.Input, 0, "invoice");
                        objSql.AddParameter("@billId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.InvoiceId);
                        objSql.AddParameter("@creationDate", DbType.DateTime, ParameterDirection.Input, 0, DateTime.Now);
                        objSql.ExecuteNonQuery("p_ledgerTransactionDetail_ins");
                    }

                    objSql.NewCommand();
                    objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.CompanyId);
                    objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.InvoiceId);
                    objSql.ExecuteNonQuery("p_InvoicePayment_upd");


                }
                objSql.NewCommand();
                int txnId = objLedger.CreateTransactions(new LedgerTransactionDTO
                {
                    LedgerId = dataOBject.LedgerId,
                    TransactionAmount = dataOBject.Total,
                    TransactionDate = dataOBject.InvoiceDate,//.ToShortDateString(),
                    DrLedgerId = dataOBject.LedgerId,
                    Description = description,
                    CreatedBy = dataOBject.CreatedBy,
                    TransactionType = txnType,
                    TransactionMode = TransactionModes.Cash,
                    EntryType = entryType,
                    Narration = description,
                    TranRefNumber = dataOBject.InvoiceId.ToString(),
                    FinYearId = dataOBject.FinYearId,
                    CompanyId = dataOBject.CompanyId,
                    LedgerSiteId = dataOBject.LedgerSiteId,
                    Invoiceid = dataOBject.InvoiceId

                }, objSql);
                if (txnId == 0)
                {
                    throw new Exception("Could not save transaction");
                }




                objSql.Commit();
                #endregion

                //   invoiceId = billingDTO.InvoiceId;
                #region breakage bill
                //if (dataOBject.BreakageItems != null)
                //{
                //    if (dataOBject.BreakageItems.Count > 0)
                //    {

                //        //dataObject.InvoiceDate = DateTime.Today;
                //        dataOBject.BillableItems = dataOBject.BreakageItems;
                //        //dataObject.InvoiceNumber = billDto.BillNumber;
                //        //billDto.From = from;
                //        //billDto.To = to;
                //        // billDto.LedgerId = ledgerId;
                //        dataOBject.ParentInvoiceId = dataOBject.InvoiceId;
                //        dataOBject.SubTotal = dataOBject.BreakageItems.Sum(o => o.Quantity * o.Rate);
                //        // billDto.TaxAmount = 0;
                //        dataOBject.Total = dataOBject.SubTotal;
                //        dataOBject.ApplicableTaxes = new List<TaxDTO>(); //no taxes as of now
                //        dataOBject.InvoiceType = 2;// - breakage bill
                //        dataOBject.Freight = 0;// no freight for breakage items
                //        dataOBject.FreightTax = 0;
                //        dataOBject.TaxAmount = 0;
                //        dataOBject.OtherCharges = null;
                //        dataOBject.OtherChargeAmount = 0;
                //        dataOBject.InvoiceId = 0;
                //        dataOBject.Challans = new List<BillChallanDto>();
                //        dataOBject.BreakageItems = new List<BillingItemDTO>();
                //        return await Add(dataOBject);
                //    }

                //}
                #endregion

            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                logger.Debug(ex, ex.StackTrace, ex.InnerException);
                logger.Error(ex);
                logger.Info(ex);
                objSql.Rollback();
                throw ex;
            }
            #endregion


            return _invoiceId > 0;
        }
        bool AddTax(BillingItemDTO dto, TaxDTO taxDto, SQL objSql)
        {
            taxDto.Amount = dto.SubTotal;
            objSql.AddParameter("@TaxId", DbType.Int32, ParameterDirection.Input, 0, taxDto.TaxId);
            objSql.AddParameter("@Rate", DbType.Double, ParameterDirection.Input, 0, taxDto.Rate);
            objSql.AddParameter("@Amount", DbType.Double, ParameterDirection.Input, 0, taxDto.TaxAmount);
            objSql.AddParameter("@InvoiceId", DbType.Int32, ParameterDirection.Input, 0, dto.InvoiceId);
            return objSql.ExecuteNonQuery(ADD_TAX) > 0;
        }
        public void AddItem(BillingItemDTO dto, SQL objSql)
        {
            try
            {



                objSql.AddParameter("@ProductId", DbType.Int64, ParameterDirection.Input, 0, dto.ProductId);
                objSql.AddParameter("@Rate", DbType.Double, ParameterDirection.Input, 0, dto.Rate);
                objSql.AddParameter("@SubTotal", DbType.Double, ParameterDirection.Input, 0, dto.SubTotal);
                objSql.AddParameter("@Quantity", DbType.Double, ParameterDirection.Input, 0, dto.Quantity);
                objSql.AddParameter("@InvoiceId", DbType.Int32, ParameterDirection.Input, 0, dto.InvoiceId);
                objSql.AddParameter("@ProductSizeId", DbType.Int32, ParameterDirection.Input, 0, dto.ProductSizeId);
                objSql.AddParameter("@groupItemId", DbType.Int32, ParameterDirection.Input, 0, dto.GroupItemId);

                if (dto.From.Year > 2000)
                {
                    objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, dto.From);
                }
                if (dto.To.Year > 2000)
                {
                    objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, dto.To);
                }
                if (!String.IsNullOrEmpty(dto.LinItem))
                {
                    objSql.AddParameter("@lineItem", DbType.String, ParameterDirection.Input, 0, dto.LinItem);
                }
                objSql.AddParameter("@Days", DbType.Double, ParameterDirection.Input, 0, dto.Days);
                objSql.AddParameter("@CB", DbType.Double, ParameterDirection.Input, 0, dto.ClosingBalance);
                objSql.AddParameter("@OPB", DbType.Double, ParameterDirection.Input, 0, dto.OPB);

                objSql.AddParameter("@ChargeReturnedDate", DbType.Boolean, ParameterDirection.Input, 0, dto.ChargeReturnedDate);
                objSql.AddParameter("@TaxCategoryId", DbType.Int16, ParameterDirection.Input, 0, dto.TaxCategoryId);
                objSql.AddParameter("@IGST", DbType.Double, ParameterDirection.Input, 0, dto.IGST);
                objSql.AddParameter("@CGST", DbType.Double, ParameterDirection.Input, 0, dto.CGST);
                objSql.AddParameter("@SGST", DbType.Double, ParameterDirection.Input, 0, dto.SGST);
                objSql.AddParameter("@isLost", DbType.Boolean, ParameterDirection.Input, 0, dto.IsLost);
                objSql.AddParameter("@isBreak", DbType.Boolean, ParameterDirection.Input, 0, dto.IsBreakage);

                objSql.AddParameter("@IGSTRate", DbType.Double, ParameterDirection.Input, 0, dto.IGSTRate);
                objSql.AddParameter("@CGSTRate", DbType.Double, ParameterDirection.Input, 0, dto.CGSTRate);
                objSql.AddParameter("@SGSTRate", DbType.Double, ParameterDirection.Input, 0, dto.SGSTRate);
                objSql.AddParameter("@challanId", DbType.Double, ParameterDirection.Input, 0, dto.ChallanId);

                objSql.AddParameter("@height", DbType.Double, ParameterDirection.Input, 0, dto.Height);
                objSql.AddParameter("@width", DbType.Double, ParameterDirection.Input, 0, dto.Width);
                objSql.AddParameter("@unit", DbType.String, ParameterDirection.Input, 0, dto.Unit ?? "");



                objSql.AddParameter("@Total", DbType.Double, ParameterDirection.Input, 0, dto.Total);
                objSql.AddParameter("@excessQty", DbType.Double, ParameterDirection.Input, 0, dto.ExcessQty);
                objSql.AddParameter("@discountPercent", DbType.Double, ParameterDirection.Input, 0, dto.DiscountPercent);
                objSql.AddParameter("@discountAmount", DbType.Double, ParameterDirection.Input, 0, dto.Discount);
                objSql.AddParameter("@qtyCalculation", DbType.String, ParameterDirection.Input, 0, dto.QtyCalculation);


                objSql.ExecuteNonQuery(ADD_ITEMS);

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public void AddQuotationItem(QuotationItemDTO dto, SQL objSql)
        {
            try
            {
                //objSql.AddParameter("@ProductId", DbType.Int64, ParameterDirection.Input, 0, dto.ProductId);
                //objSql.AddParameter("@QuotationId", DbType.Int32, ParameterDirection.Input, 0, dto.QuotationId);
                //objSql.ExecuteNonQuery(DEL_QUOTATION_ITEMS);


                objSql.NewCommand();

                objSql.AddParameter("@ProductId", DbType.Int64, ParameterDirection.Input, 0, dto.ProductId);
                objSql.AddParameter("@description", DbType.String, ParameterDirection.Input, 0, dto.Description);

                objSql.AddParameter("@Rate", DbType.Double, ParameterDirection.Input, 0, dto.Rate);
                objSql.AddParameter("@SubTotal", DbType.Double, ParameterDirection.Input, 0, dto.SubTotal);
                objSql.AddParameter("@Quantity", DbType.Double, ParameterDirection.Input, 0, dto.Quantity);
                objSql.AddParameter("@QuotationId", DbType.Int32, ParameterDirection.Input, 0, dto.QuotationId);
                objSql.AddParameter("@ProductSizeId", DbType.Int32, ParameterDirection.Input, 0, dto.ProductSizeId);
                if (dto.From.Year > 2000)
                {
                    objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, dto.From);
                }
                if (dto.To.Year > 2000)
                {
                    objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, dto.To);
                }
                objSql.AddParameter("@Days", DbType.Double, ParameterDirection.Input, 0, dto.Days);
                objSql.AddParameter("@CB", DbType.Double, ParameterDirection.Input, 0, dto.ClosingBalance);
                objSql.AddParameter("@OPB", DbType.Double, ParameterDirection.Input, 0, dto.OPB);

                objSql.AddParameter("@ChargeReturnedDate", DbType.Boolean, ParameterDirection.Input, 0, dto.ChargeReturnedDate);

                objSql.AddParameter("@TaxCategoryId", DbType.Int16, ParameterDirection.Input, 0, dto.TaxCategoryId);
                objSql.AddParameter("@IGST", DbType.Double, ParameterDirection.Input, 0, dto.IGST);
                objSql.AddParameter("@CGST", DbType.Double, ParameterDirection.Input, 0, dto.CGST);
                objSql.AddParameter("@SGST", DbType.Double, ParameterDirection.Input, 0, dto.SGST);
                objSql.AddParameter("@IGSTRate", DbType.Double, ParameterDirection.Input, 0, dto.IGSTRate);
                objSql.AddParameter("@CGSTRate", DbType.Double, ParameterDirection.Input, 0, dto.CGSTRate);
                objSql.AddParameter("@SGSTRate", DbType.Double, ParameterDirection.Input, 0, dto.SGSTRate);
                objSql.AddParameter("@Total", DbType.Double, ParameterDirection.Input, 0, dto.Total);
                objSql.AddParameter("@duration", DbType.Int16, ParameterDirection.Input, 0, dto.Duration);
                objSql.AddParameter("@area", DbType.Single, ParameterDirection.Input, 0, dto.Area);
                objSql.AddParameter("@lineTotalMode", DbType.String, ParameterDirection.Input, 0, string.IsNullOrWhiteSpace(dto.LineTotalMode) ? (object)DBNull.Value : dto.LineTotalMode.Trim());


                objSql.ExecuteNonQuery(ADD_QUOTATION_ITEMS);

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public List<BillingItemDTO> BillingItemsTax(int invoiceId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, invoiceId);
            return objSql.ContructList<BillingItemDTO>(objSql.ExecuteDataSet(BILLING_ITEMS_TAX));
        }
        public async Task<bool> ChangeInvoieStatus(int invoiceId, InvoiceStatus status, int modifiedBy, DateTime modifiedDate, int companyId)
        {
            SQL objSql = new SQL();
            try
            {
                objSql.BeginTransaction();
                objSql.NewCommand();

                objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, invoiceId);
                objSql.AddParameter("@statusId", DbType.Int32, ParameterDirection.Input, 0, Convert.ToInt16(status));
                objSql.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, modifiedBy);
                objSql.AddParameter("@modifiedDate", DbType.DateTime, ParameterDirection.Input, 0, modifiedDate);

                var x = await objSql.ExecuteNonQueryAsync(UPDATE_INVOICE_STATUS) > 0;
                if (status == InvoiceStatus.Cancelled)
                {
                    objSql.NewCommand();
                    objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, invoiceId);
                    objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
                    var y = await objSql.ExecuteNonQueryAsync(p_invoiceTransactions_del) > 0;
                }
                objSql.Commit();
                return x;
            }
            catch (Exception ex)
            {
                objSql.Rollback();
                throw ex;
            }

        }
        internal bool AddQuotation(QuotationDataDTO dataOBject)
        {
            SQL objSql = new SQL();
            Int32 _invoiceId = 0;
            try
            {

                //if (dataOBject.ApplicableTaxes.Count > 0)
                //{
                //    dataOBject.TaxAmount = dataOBject.ApplicableTaxes.Sum(o => o.TaxAmount);
                //}

                objSql.BeginTransaction();
                //NextIdDAL n = new NextIdDAL();
                //NextIdDTO nDTO = new NextIdDTO();



                string billPrefix, initalValue;


                objSql.NewCommand();
                double otherCharges = 0;

                dataOBject.OtherChargeAmount = dataOBject.Charge1 + dataOBject.Charge2 + dataOBject.Charge3 + dataOBject.Charge4
                + dataOBject.Charge5;


                dataOBject.TaxAmount = 0;
                if (dataOBject.BillableItems.Count > 0)
                {
                    dataOBject.TaxAmount = dataOBject.BillableItems.Sum(o => o.IGST + o.CGST + o.SGST);
                }
                //   dataOBject.Total = dataOBject.SubTotal - dataOBject.Discount + dataOBject.TaxAmount;
                dataOBject.SubTotal = dataOBject.BillableItems.Sum(o => o.SubTotal);
                dataOBject.Total = dataOBject.SubTotal + dataOBject.Freight + dataOBject.FreightIn + dataOBject.FreightTax
                    + dataOBject.ChargesTax + dataOBject.OtherChargeAmount + dataOBject.TaxAmount;

                if (dataOBject.DiscountPercent > 0)
                {
                    dataOBject.Discount = dataOBject.SubTotal * dataOBject.DiscountPercent / 100;
                }
                dataOBject.Total -= dataOBject.Discount;


                // dataOBject.QuotationNumber = "0";
                #region Invoice
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.CompanyId);
                objSql.AddParameter("@QuotationNumber", DbType.String, ParameterDirection.Input, 0, dataOBject.QuotationNumber);
                // objSql.AddParameter("@SiteId", DbType.Int32, ParameterDirection.Input, 50, dataOBject.SiteId);
                objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.LedgerId);
                objSql.AddParameter("@QuotationDate", DbType.DateTime, ParameterDirection.Input, 0, dataOBject.QuotationDate);
                objSql.AddParameter("@From", DbType.DateTime, ParameterDirection.Input, 0, dataOBject.From);
                objSql.AddParameter("@To", DbType.DateTime, ParameterDirection.Input, 0, dataOBject.To);
                objSql.AddParameter("@area", DbType.Single, ParameterDirection.Input, 0, dataOBject.Area);
                objSql.AddParameter("@measureType", DbType.Byte, ParameterDirection.Input, 0, dataOBject.MeasureType);
                var lineTotalMode = string.IsNullOrWhiteSpace(dataOBject.LineTotalMode) ? "quantity" : dataOBject.LineTotalMode.Trim();
                if (!string.Equals(lineTotalMode, "area", StringComparison.OrdinalIgnoreCase))
                    lineTotalMode = "quantity";
                objSql.AddParameter("@lineTotalMode", DbType.String, ParameterDirection.Input, 0, lineTotalMode);
                objSql.AddParameter("@CreatedBy", DbType.Int32, ParameterDirection.Input, 0, dataOBject.CreatedBy);
                objSql.AddParameter("@Details", DbType.String, ParameterDirection.Input, 0, dataOBject.Details);
                objSql.AddParameter("@SubTotal", DbType.Double, ParameterDirection.Input, 0, dataOBject.SubTotal);
                objSql.AddParameter("@total", DbType.Double, ParameterDirection.Input, 0, dataOBject.Total);
                objSql.AddParameter("@TaxAmount", DbType.Double, ParameterDirection.Input, 0, dataOBject.TaxAmount);
                objSql.AddParameter("@QuotationType", DbType.Double, ParameterDirection.Input, 0, dataOBject.QuotationType);
                objSql.AddParameter("@Freight", DbType.Double, ParameterDirection.Input, 0, dataOBject.Freight);
                objSql.AddParameter("@FreightTax", DbType.Double, ParameterDirection.Input, 0, dataOBject.FreightTax);
                objSql.AddParameter("@FinYearId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.FinYearId);
                objSql.AddParameter("@BreakageAmount", DbType.Double, ParameterDirection.Input, 0, dataOBject.BreakageAmount);
                objSql.AddParameter("@BreakageTax", DbType.Double, ParameterDirection.Input, 0, dataOBject.BreakageTax);

                objSql.AddParameter("@OutStanding", DbType.Double, ParameterDirection.Input, 0, dataOBject.OutStanding);
                objSql.AddParameter("@OutStandingType", DbType.Int16, ParameterDirection.Input, 0, dataOBject.OutStandingType);
                objSql.AddParameter("@RoundOff", DbType.Boolean, ParameterDirection.Input, 0, dataOBject.RoundOff);
                objSql.AddParameter("@LedgerSiteId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.LedgerSiteId);
                objSql.AddParameter("@discount", DbType.Double, ParameterDirection.Input, 0, dataOBject.Discount);
                objSql.AddParameter("@discountPercent", DbType.Double, ParameterDirection.Input, 0, dataOBject.DiscountPercent);

                //  objSql.AddParameter("@partyType", DbType.Byte, ParameterDirection.Input, 0, dataOBject.PartyType > 0 ? dataOBject.PartyType : (byte)1);
                objSql.AddParameter("@unregisteredPartyName", DbType.String, ParameterDirection.Input, 0, dataOBject.UnregisteredPartyName ?? string.Empty);
                objSql.AddParameter("@unregisteredPartyAddress", DbType.String, ParameterDirection.Input, 0, dataOBject.UnregisteredPartyAddress ?? string.Empty);
                objSql.AddParameter("@unregisteredPartyPhone", DbType.String, ParameterDirection.Input, 0, dataOBject.UnregisteredPartyPhone ?? string.Empty);
                objSql.AddParameter("@gstRate", DbType.Double, ParameterDirection.Input, 0, dataOBject.GstRate);
                objSql.AddParameter("@IGST", DbType.Boolean, ParameterDirection.Input, 0, dataOBject.IGST);
                objSql.AddParameter("@CGST", DbType.Boolean, ParameterDirection.Input, 0, dataOBject.CGST);
                objSql.AddParameter("@SGST", DbType.Boolean, ParameterDirection.Input, 0, dataOBject.SGST);

                objSql.AddParameter("@charge1", DbType.Double, ParameterDirection.Input, 0, dataOBject.Charge1);
                objSql.AddParameter("@charge2", DbType.Double, ParameterDirection.Input, 0, dataOBject.Charge2);
                objSql.AddParameter("@charge3", DbType.Double, ParameterDirection.Input, 0, dataOBject.Charge3);
                objSql.AddParameter("@charge4", DbType.Double, ParameterDirection.Input, 0, dataOBject.Charge4);
                objSql.AddParameter("@charge5", DbType.Double, ParameterDirection.Input, 0, dataOBject.Charge5);
                objSql.AddParameter("@ChargesTax", DbType.Double, ParameterDirection.Input, 0, dataOBject.ChargesTax);
                objSql.AddParameter("@ChargesTaxRate", DbType.Double, ParameterDirection.Input, 0, dataOBject.ChargesTaxRate);

                objSql.AddParameter("@otherCharges", DbType.Double, ParameterDirection.Input, 0, dataOBject.OtherChargeAmount);
                objSql.AddParameter("@addInfo", DbType.String, ParameterDirection.Input, 0, dataOBject.AddInfo);
                objSql.AddParameter("@tnc", DbType.String, ParameterDirection.Input, 0, dataOBject.Tnc);
                objSql.AddParameter("@freightIn", DbType.Double, ParameterDirection.Input, 0, dataOBject.FreightIn);
                //objSql.AddParameter("@partyName", DbType.String, ParameterDirection.Input, 0, dataOBject.Party);
                //objSql.AddParameter("@billingAddress", DbType.String, ParameterDirection.Input, 0, dataOBject.BillingAddress);
                //objSql.AddParameter("@deliveryAddress", DbType.String, ParameterDirection.Input, 0, dataOBject.SiteAddress);


                if (dataOBject.WorkOrderId > 0)
                {
                    objSql.AddParameter("@WorkOrderId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.WorkOrderId);
                }

                if (!String.IsNullOrEmpty(dataOBject.ContractorCode))
                {
                    objSql.AddParameter("@ContractorCode", DbType.String, ParameterDirection.Input, 0, dataOBject.ContractorCode);
                }
                if (!String.IsNullOrEmpty(dataOBject.BranchCode))
                {
                    objSql.AddParameter("@BranchCode", DbType.String, ParameterDirection.Input, 0, dataOBject.BranchCode);
                }
                if (!String.IsNullOrEmpty(dataOBject.Category))
                {
                    objSql.AddParameter("@Category", DbType.String, ParameterDirection.Input, 0, dataOBject.Category);
                }
                if (dataOBject.ParentQuotationId > 0)
                    objSql.AddParameter("@ParentQuotationId", DbType.Double, ParameterDirection.Input, 0, dataOBject.ParentQuotationId);

                if (!String.IsNullOrEmpty(dataOBject.SiteAddress))
                {
                    objSql.AddParameter("@SiteAddress", DbType.String, ParameterDirection.Input, 0, dataOBject.SiteAddress);
                }
                if (!String.IsNullOrEmpty(dataOBject.PoNumber))
                {
                    objSql.AddParameter("@poNumber", DbType.String, ParameterDirection.Input, 0, dataOBject.PoNumber);
                }
                if (dataOBject.PoDate.Year > 2000)
                {
                    objSql.AddParameter("@poDate", DbType.Date, ParameterDirection.Input, 0, dataOBject.PoDate);
                }
                if (dataOBject.ValidUntil.Year > 2000)
                {
                    objSql.AddParameter("@validUntil", DbType.Date, ParameterDirection.Input, 0, dataOBject.ValidUntil);
                }
                if (dataOBject.ContractId > 0)
                {
                    objSql.AddParameter("@contractId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.ContractId);
                }
                var newQutotation = new QuotationDTO();
                if (dataOBject.QuotationId > 0)
                {
                    objSql.AddParameter("@quotationId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.QuotationId);
                    newQutotation = objSql.ContructList<QuotationDTO>(objSql.ExecuteDataSet(UPD_QUOTATION)).FirstOrDefault();

                }
                else
                    newQutotation = objSql.ContructList<QuotationDTO>(objSql.ExecuteDataSet(ADD_QUOTATION)).FirstOrDefault();

                if (newQutotation.QuotationId == 0)
                {
                    throw new Exception("Could not save Quotation");
                }

                dataOBject.QuotationId = newQutotation.QuotationId;
                //  dataOBject.InvoiceNumber = inv.InvoiceNumber;
                #region ItemAndTax
                objSql.NewCommand();
                //objSql.AddParameter("@ProductId", DbType.Int64, ParameterDirection.Input, 0, dto.ProductId);
                objSql.AddParameter("@QuotationId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.QuotationId);
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.CompanyId);

                objSql.ExecuteNonQuery(DEL_QUOTATION_ITEMS);

                foreach (QuotationItemDTO item in dataOBject.BillableItems)
                {
                    // item.WorkOrder = new BillingItemDTO { InvoiceId = _invoiceId };
                    item.QuotationId = dataOBject.QuotationId;
                    objSql.NewCommand();
                    AddQuotationItem(item, objSql);
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


            return dataOBject.QuotationId > 0;
        }
        public async Task<bool> UpdateQuotationStatus(QuotationDTO dto)
        {
            SQL objSql = new SQL();
            try
            {
                objSql.BeginTransaction();
                objSql.NewCommand();

                objSql.AddParameter("@quotationId", DbType.Int32, ParameterDirection.Input, 0, dto.QuotationId);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);

                objSql.AddParameter("@statusId", DbType.Int32, ParameterDirection.Input, 0, dto.StatusId);
                objSql.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, dto.ModifiedBy);
                objSql.AddParameter("@modifiedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedOn);

                var x = await objSql.ExecuteNonQueryAsync(UPDATE_QUOTATION_STATUS) > 0;
                objSql.Commit();
                return x;
            }
            catch (Exception ex)
            {
                objSql.Rollback();
                throw ex;
            }

        }

        public async Task<bool> LinkQuotationToLedger(int quotationId, int ledgerId, int companyId)
        {
            SQL objSql = new SQL();
            try
            {
                objSql.BeginTransaction();
                objSql.NewCommand();
                objSql.AddParameter("@quotationId", DbType.Int32, ParameterDirection.Input, 0, quotationId);
                objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
                var n = await objSql.ExecuteNonQueryAsync(LINK_QUOTATION_PARTY);
                objSql.Commit();
                return n > 0;
            }
            catch (Exception ex)
            {
                objSql.Rollback();
                throw ex;
            }
        }

        /// <summary>After contract bill save, stamp InvoiceId on each selected quotation.</summary>
        public async Task LinkQuotationsToInvoice(int companyId, int invoiceId, IEnumerable<int> quotationIds)
        {
            if (quotationIds == null)
                return;
            var ids = quotationIds.Where(id => id > 0).Distinct().ToList();
            if (ids.Count == 0)
                return;

            SQL objSql = new SQL();
            try
            {
                objSql.BeginTransaction();
                objSql.NewCommand();
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
                objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, invoiceId);
                objSql.AddParameter("@quotationIds", DbType.String, ParameterDirection.Input, 0, String.Join(",", ids));
                await objSql.ExecuteNonQueryAsync(LINK_QUOTATION_INVOICE);
                objSql.Commit();
            }
            catch (Exception ex)
            {
                objSql.Rollback();
                throw ex;
            }
        }

        private string GetBillPrefix(ConfigCategory category, ConfigCategory subCategory, ConfigKey key, int companyId, SQL sql)
        {
            ConfigDAL configDal = new ConfigDAL();
            ConfigDTO cdto = new ConfigDTO();
            cdto.Category = Convert.ToString(category);
            cdto.SubCategory = Convert.ToString(subCategory);
            cdto.Key = Convert.ToString(key);
            cdto.CompanyId = companyId;
            cdto = configDal.GetValue(cdto, sql);
            if (cdto != null)
                return cdto.Value;
            else
                return null;
        }

        public DataSet DueBills(int finYearId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@FinYearId", DbType.Int16, ParameterDirection.Input, 0, finYearId);
            return objSql.ExecuteDataSet(DUE_BILLS);
        }
        public DataSet DueBillsSummary(int ledgerSiteId, int companyId, int ledgerId, DateTime from, DateTime to, int finYearId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@FinYearId", DbType.Int16, ParameterDirection.Input, 0, finYearId);
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            if (ledgerId > 0)
                objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            if (ledgerSiteId > 0)
                objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);

            objSql.AddParameter("@from", DbType.DateTime, ParameterDirection.Input, 0, from);
            objSql.AddParameter("@To", DbType.DateTime, ParameterDirection.Input, 0, to);

            return objSql.ExecuteDataSet(DUE_BILLS_SUMMARY);
        }
        public void AddLossItems(List<LostItemDTO> lst, int invoiceId, SQL objSql)
        {
            foreach (LostItemDTO item in lst)
            {
                objSql.NewCommand();
                item.InvoiceId = invoiceId;
                AddLossItem(item, objSql);
            }
        }
        public void AddLossItem(LostItemDTO dto, SQL objSql)
        {
            try
            {
                objSql.AddParameter("@ProductId", DbType.Int32, ParameterDirection.Input, 0, dto.ProductId);
                objSql.AddParameter("@Rate", DbType.Double, ParameterDirection.Input, 0, dto.Rate);
                objSql.AddParameter("@Quantity", DbType.Double, ParameterDirection.Input, 0, dto.Quantity);
                objSql.AddParameter("@InvoiceId", DbType.Int32, ParameterDirection.Input, 0, dto.InvoiceId);
                objSql.AddParameter("@Amount", DbType.Double, ParameterDirection.Input, 0, dto.Amount);

                objSql.AddParameter("@MatLossId", DbType.Int32, ParameterDirection.Input, 0, dto.MatLossId);
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);

                objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerId);
                objSql.AddParameter("@FinYearId", DbType.Int32, ParameterDirection.Input, 0, dto.FinYearId);
                objSql.AddParameter("@LostDate", DbType.Date, ParameterDirection.Input, 0, dto.LostDate);
                objSql.ExecuteNonQuery(ADD_LOST_ITEM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void AddOtherCharges(List<InvoiceChargeDTO> lst, int invoiceId, SQL objSql)
        {
            try
            {
                foreach (InvoiceChargeDTO ch in lst)
                {
                    ch.InvoiceId = invoiceId;
                    objSql.NewCommand();
                    objSql.AddParameter("@ChargeId", DbType.Int32, ParameterDirection.Input, 0, ch.ChargeId);


                    objSql.AddParameter("@InvoiceId", DbType.Int32, ParameterDirection.Input, 0, ch.InvoiceId);

                    objSql.ExecuteNonQuery(DEL_OTHER_CHARGE);

                    objSql.NewCommand();
                    AddOtherCharge(ch, objSql);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void AddOtherCharge(InvoiceChargeDTO dto, SQL objSql)
        {
            try
            {
                objSql.AddParameter("@ChargeId", DbType.Int32, ParameterDirection.Input, 0, dto.ChargeId);
                objSql.AddParameter("@Amount", DbType.Double, ParameterDirection.Input, 0, dto.Amount);

                objSql.AddParameter("@InvoiceId", DbType.Int32, ParameterDirection.Input, 0, dto.InvoiceId);

                objSql.ExecuteNonQuery(ADD_OTHER_CHARGE);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet GetLossItems(int invoiceId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, invoiceId);
            return objSql.ExecuteDataSet(INVOICE_LOSS_ITEM);
        }
        /// <summary>
        /// Get breakage items of the main invoice
        /// </summary>
        /// <param name="invoiceId">Main invoice</param>
        /// <returns></returns>
        public DataSet GetBreakageItems(int invoiceId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@ParentinvoiceId", DbType.Int32, ParameterDirection.Input, 0, invoiceId);
            return objSql.ExecuteDataSet(INVOICE_BREAKAGE_ITEM);
        }
        public List<InvoiceItemDTO> GetBalanceAffterLastBill(int ledgerId, int ledgerSiteId, int invoiceId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, invoiceId);

            objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);
            return objSql.ContructList<InvoiceItemDTO>(objSql.ExecuteDataSet(p_getBalanceAfterLastBill));


        }
        public DataSet SelInvoiceHeader(int invoiceId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, invoiceId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            return objSql.ExecuteDataSet(INVOICE_HEADER_FOR_NOTIFICATION);
        }

        #region Quotations
        public List<QuotationDTO> QuotationsList(QuotationDTO dto)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
            objSql.AddParameter("@from", DbType.DateTime, ParameterDirection.Input, 0, dto.From);
            objSql.AddParameter("@to", DbType.DateTime, ParameterDirection.Input, 0, dto.To);
            objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerId);
            objSql.AddParameter("@statusId", DbType.Int16, ParameterDirection.Input, 0, dto.StatusId);
            objSql.AddParameter("@quotationTypeId", DbType.Int16, ParameterDirection.Input, 0, dto.QuotationType);
            if (!String.IsNullOrEmpty(dto.Category))
            {
                objSql.AddParameter("@category", DbType.String, ParameterDirection.Input, 0, dto.Category);
            }
            return objSql.ContructList<QuotationDTO>(objSql.ExecuteDataSet(QUOTATIONS_LIST));
        }

        public List<QuotationDTO> QuotationsListByContractId(int contractId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@contractId", DbType.Int32, ParameterDirection.Input, 0, contractId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            return objSql.ContructList<QuotationDTO>(objSql.ExecuteDataSet(QUOTATIONS_BY_CONTRACT));
        }

        public async Task<QuotationDTO> QuotationById(int quotationId, int companyId)
        {
            SQL objSql = new SQL();
            if (companyId > 0)
            {
                objSql.AddParameter("@quotationId", DbType.Int32, ParameterDirection.Input, 0, quotationId);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            }
            var d = await objSql.QueryFirstAsync<QuotationDTO>(QUOTATION_BYID);
            if (d != null)
            {
                objSql.NewCommand();
                objSql.AddParameter("@quotationId", DbType.Int32, ParameterDirection.Input, 0, quotationId);
                var items = await objSql.QueryAsync<QuotationItemDTO>(GET_QUOTATION_ITEMS);
                d.Items = items.ToList();
            }
            return d;
        }
        public DataSet GetQuotationItems(int quotationId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@quotationId", DbType.Int32, ParameterDirection.Input, 0, quotationId);
            return objSql.ExecuteDataSet(GET_QUOTATION_ITEMS);
        }

        #endregion

        #region EwayBills
        public async Task<bool> AddEwayBill(EwayBillDTO dto)
        {
            SQL objSql = new SQL();

            try
            {
                objSql.BeginTransaction();
                objSql.NewCommand();
                objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, dto.InvoiceId);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                objSql.AddParameter("@transactionType", DbType.Int32, ParameterDirection.Input, 0, dto.TransactionType);

                objSql.AddParameter("@transporterId", DbType.Int32, ParameterDirection.Input, 0, dto.TransporterId);
                objSql.AddParameter("@SubTypeId", DbType.Int16, ParameterDirection.Input, 0, dto.SubTypeId);
                objSql.AddParameter("@distance", DbType.Int32, ParameterDirection.Input, 0, dto.Distance);
                objSql.AddParameter("@transMode", DbType.Int32, ParameterDirection.Input, 0, dto.TransportationMode);
                objSql.AddParameter("@vehicleType", DbType.String, ParameterDirection.Input, 0, dto.VehicleType);
                objSql.AddParameter("@VehicleNo", DbType.String, ParameterDirection.Input, 0, dto.VehicleNo);
                objSql.AddParameter("@TransDocNo", DbType.String, ParameterDirection.Input, 0, dto.TransporterDocNo);
                objSql.AddParameter("@otherTypeDesc", DbType.String, ParameterDirection.Input, 0, dto.OtherTypeDesc);
                objSql.AddParameter("@approximateValue", DbType.Decimal, ParameterDirection.Input, 0,
                    dto.ApproximateValue.HasValue && dto.ApproximateValue.Value > 0 ? (object)dto.ApproximateValue.Value : DBNull.Value);
                if (!String.IsNullOrEmpty(dto.ShipFromAddress))
                {
                    objSql.AddParameter("@fromAddr", DbType.String, ParameterDirection.Input, 0, dto.ShipFromAddress);
                }
                if (!String.IsNullOrEmpty(dto.ShipToAddress))
                {
                    objSql.AddParameter("@toAddrr", DbType.String, ParameterDirection.Input, 0, dto.ShipToAddress);
                }




                if (dto.TransporterDocDate.Year > 1)
                    objSql.AddParameter("@transDocDate", DbType.Date, ParameterDirection.Input, 0, dto.TransporterDocDate);

                objSql.AddParameter("@createdBy", DbType.Int32, ParameterDirection.Input, 0, dto.CreatedBy);
                objSql.AddParameter("@createdOn", DbType.DateTime, ParameterDirection.Input, 0, dto.CreatedOn);
                objSql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, dto.GuId);
                var result = 0;
                if (dto.EwayBillId > 0)
                {
                    objSql.AddParameter("@ewayBillId", DbType.Int32, ParameterDirection.Input, 0, dto.EwayBillId);
                    result = await objSql.ExecuteNonQueryAsync(UPDATE_EWAY_BILL);
                }
                else
                {
                    objSql.AddParameter("@docType", DbType.String, ParameterDirection.Input, 0, dto.DocType);
                    objSql.AddParameter("@docSubType", DbType.String, ParameterDirection.Input, 0, dto.DocSubType);

                    dto.EwayBillId = Convert.ToInt32(await objSql.ExecuteScalarAsync("p_EwayBill_insv2"));
                    result = dto.EwayBillId > 0 ? 1 : 0;
                }


                if (dto.Items != null && dto.EwayBillId > 0)
                {
                    result = 1;
                    objSql.NewCommand();
                    objSql.AddParameter("@ewayBillId", DbType.Int32, ParameterDirection.Input, 0, dto.EwayBillId);
                    objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                    var ret = await objSql.ExecuteNonQueryAsync(EWAYBILL_DETAILS_DEL);
                    foreach (var item in dto.Items)
                    {
                        objSql.NewCommand();
                        objSql.AddParameter("@productId", DbType.Int32, ParameterDirection.Input, 0, item.ProductId);

                        objSql.AddParameter("@ewayBillId", DbType.Int32, ParameterDirection.Input, 0, dto.EwayBillId);
                        objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                        objSql.AddParameter("@product", DbType.String, ParameterDirection.Input, 0, item.Product);
                        objSql.AddParameter("@hsnCode", DbType.String, ParameterDirection.Input, 0, item.HSNCode);
                        objSql.AddParameter("@unit", DbType.String, ParameterDirection.Input, 0, item.Unit);
                        objSql.AddParameter("@rate", DbType.Double, ParameterDirection.Input, 0, item.Rate);
                        objSql.AddParameter("@quantity", DbType.Double, ParameterDirection.Input, 0, item.Quantity);
                        objSql.AddParameter("@subTotal", DbType.Double, ParameterDirection.Input, 0, item.SubTotal);
                        objSql.AddParameter("@igstRate", DbType.Double, ParameterDirection.Input, 0, item.IGSTRate);
                        objSql.AddParameter("@cgstRate", DbType.Double, ParameterDirection.Input, 0, item.CGSTRate);
                        objSql.AddParameter("@sgstRate", DbType.Double, ParameterDirection.Input, 0, item.SGSTRate);
                        objSql.AddParameter("@igst", DbType.Double, ParameterDirection.Input, 0, item.IGST);
                        objSql.AddParameter("@cgst", DbType.Double, ParameterDirection.Input, 0, item.CGST);
                        objSql.AddParameter("@sgst", DbType.Double, ParameterDirection.Input, 0, item.SGST);
                        objSql.AddParameter("@amount", DbType.Double, ParameterDirection.Input, 0, item.Amount);
                        ret = await objSql.ExecuteNonQueryAsync(EWAYBILL_DETAILS_INS);

                    }
                }



                if (dto.EwayBillId <= 0)
                {
                    throw new Exception("Could not create e-way bill on Rentac");
                }
                objSql.Commit();
                return result >= 1;
            }
            catch (Exception ex)
            {
                objSql.Rollback();
                throw ex;
            }
        }
        public List<EwayBillDTO> GetAllEwayBills(EwayBillFilterDto filter)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);
            objSql.AddParameter("@type", DbType.String, ParameterDirection.Input, 0, filter.DocType);
            objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, filter.LedgerId);
            objSql.AddParameter("@status", DbType.String, ParameterDirection.Input, 0, filter.Status);
            objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, filter.From);
            objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, filter.To);

            var ds = objSql.ExecuteDataSet(GETALL_EWAY_BILL);
            return objSql.ContructList<EwayBillDTO>(ds);
        }
        public async Task<EwayBillDTO> GetEwayBill(int ewayBillId, int companyId)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@ewayBillId", DbType.Int32, ParameterDirection.Input, 0, ewayBillId);
            var ds = objSql.ContructList<EwayBillDTO>(objSql.ExecuteDataSet(GET_EWAY_BILL_BYID)).FirstOrDefault();

            if (ds != null)
            {
                objSql = new SQL();
                objSql.AddParameter("@ewayBillId", DbType.Int32, ParameterDirection.Input, 0, ewayBillId);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

                ds.Items = (await objSql.QueryAsync<BillingItemDTO>(EWAYBILL_DETAILS_SEL)).ToList();
            }
            return ds;
        }
        public bool UpdateEwayBillInfoInBill(EwayBillDTO dto)
        {
            try
            {
                SQL objSql = new SQL();
                objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, dto.InvoiceId);

                objSql.AddParameter("@ewayBillNo", DbType.String, ParameterDirection.Input, 0, dto.EwayBillNo);
                objSql.AddParameter("@ewayBillDate", DbType.DateTime, ParameterDirection.Input, 0, dto.EwayBillDate);
                objSql.AddParameter("@ewayBillValidUpTo", DbType.DateTime, ParameterDirection.Input, 0, dto.EwayBillValidUpTo);
                objSql.AddParameter("@ewayBillAlert", DbType.String, ParameterDirection.Input, 0, dto.EwayBillAlert);


                objSql.AddParameter("@createdBy", DbType.Int32, ParameterDirection.Input, 0, dto.EwayBillCreatedBy);


                return objSql.ExecuteNonQuery(UPDATE_EWAYBILL_INFO) > 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool UpdateEwayBillPortalInfo(EwayBillDTO dto)
        {
            try
            {
                SQL objSql = new SQL();
                objSql.AddParameter("@ewayBillId", DbType.Int32, ParameterDirection.Input, 0, dto.EwayBillId);

                objSql.AddParameter("@ewayBillNo", DbType.String, ParameterDirection.Input, 0, dto.EwayBillNo);
                objSql.AddParameter("@ewayBillDate", DbType.DateTime, ParameterDirection.Input, 0, dto.EwayBillDate);
                objSql.AddParameter("@ewayBillValidUpTo", DbType.DateTime, ParameterDirection.Input, 0, dto.EwayBillValidUpTo);
                objSql.AddParameter("@ewayBillAlert", DbType.String, ParameterDirection.Input, 0, dto.EwayBillAlert);


                objSql.AddParameter("@createdBy", DbType.Int32, ParameterDirection.Input, 0, dto.EwayBillCreatedBy);


                return objSql.ExecuteNonQuery(p_EwayBill_updatePortalInfo) > 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet PrintEwayBill(EwayBillDTO dto)
        {
            try
            {
                SQL objSql = new SQL();
                objSql.AddParameter("@ewayBillId", DbType.Int32, ParameterDirection.Input, 0, dto.EwayBillId);



                return objSql.ExecuteDataSet(p_printEwayBill);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool CancelEwayBill(CancelEwayBillDto dto)
        {
            try
            {
                SQL objSql = new SQL();
                objSql.AddParameter("@ewayBillNo", DbType.String, ParameterDirection.Input, 0, dto.EwayBillNo);
                objSql.AddParameter("@cancelledBy", DbType.Int32, ParameterDirection.Input, 0, dto.CancelledBy);
                objSql.AddParameter("@canclledon", DbType.DateTime, ParameterDirection.Input, 0, dto.CancelledOn);
                objSql.AddParameter("@cancellationRemarks", DbType.String, ParameterDirection.Input, 0, dto.CancelRemarks);
                objSql.AddParameter("@cancelReasonCode", DbType.Int16, ParameterDirection.Input, 0, dto.CancelReasonCode);


                return objSql.ExecuteNonQuery(CANCEL_EWAY_BILL) > 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<EwayBillDTO>> GetEwayBillByDocNumber(EwayBillDTO dto)
        {
            try
            {
                SQL objSql = new SQL();
                objSql.AddParameter("@docNumber", DbType.String, ParameterDirection.Input, 0, dto.DocNumber);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);


                return await objSql.QueryAsync<EwayBillDTO>(EWAYBILL_BY_DOC_NUMBER);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> SaveMatLoss(MatLossDTO dto)
        {
            SQL objSql = new SQL();
            try
            {

                objSql.BeginTransaction();
                objSql.NewCommand();
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerId);
                objSql.AddParameter("@finYearId", DbType.Int32, ParameterDirection.Input, 0, dto.FinYearId);
                objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerSiteId);
                objSql.AddParameter("@entryDate", DbType.DateTime, ParameterDirection.Input, 0, dto.EntryDate);
                objSql.AddParameter("@createdOn", DbType.DateTime, ParameterDirection.Input, 0, dto.CreatedOn);
                objSql.AddParameter("@createdBy", DbType.Int32, ParameterDirection.Input, 0, dto.CreatedBy);
                objSql.AddParameter("@modifiedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedOn);
                objSql.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, dto.ModifiedBy);
                objSql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, dto.GuId);
                objSql.AddParameter("@matLossId", DbType.Int32, ParameterDirection.Input, 0, dto.MatLossId);

                dto.MatLossId = Convert.ToInt32(await objSql.ExecuteScalarAsync(ADD_MAT_LOSS));


                foreach (var item in dto.Items)
                {
                    item.MatLossId = dto.MatLossId;
                    objSql.NewCommand();
                    item.InjectFrom(dto);
                    item.LostDate = dto.EntryDate;
                    item.LossItemId = item.ProductId;

                    AddLossItem(item, objSql);
                }
                objSql.Commit();
                return true;
            }
            catch (Exception ex)
            {
                objSql.Rollback();
                throw ex;
            }
        }
        public async Task<MatLossDTO> MatLossById(MatLossFilterDTO dto)
        {
            SQL objSql = new SQL();
            try
            {


                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                objSql.AddParameter("@matLossId", DbType.Int32, ParameterDirection.Input, 0, dto.MatLossId);



                var list = await objSql.QueryFirstAsync<MatLossDTO>(MAT_LOSS_BYID);
                var lossList = new List<MatLossDTO>();

                var items = await objSql.QueryAsync<LostItemDTO>(MAT_LOSS_ITEMS_BYID);

                if (list != null)
                {
                    list.Items = items.ToList();
                }


                return list;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<IEnumerable<MatLossDTO>> MatLossList(MatLossFilterDTO dto)
        {
            SQL objSql = new SQL();
            try
            {


                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, dto.From);
                objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, dto.To);
                objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerId);
                objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerSiteId);



                var list = await objSql.QueryAsync<MatLossDTO>(MAT_LOSS_ALL);


                return list;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> DeleteMatLoss(MatLossDTO dto)
        {
            SQL objSql = new SQL();
            try
            {

                //  objSql.BeginTransaction();
                //  objSql.NewCommand();
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);

                objSql.AddParameter("@matLossId", DbType.Int32, ParameterDirection.Input, 0, dto.MatLossId);

                return Convert.ToInt32(await objSql.ExecuteNonQueryAsync(MAT_LOSS_DEL)) > 0;


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        #endregion
        public async Task<IEnumerable<InvoiceDTO>> GetUnpaidBills(int ledgerId, int companyId, int ledgerSiteId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);


            var _list = await objSql.QueryAsync<InvoiceDTO>(INVOICE_UNPAID_SEL);
            objSql.Dispose();
            return _list;
        }
        public async Task<IEnumerable<InvoiceDTO>> GetBillsByIds(string billIds, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@ids", DbType.String, ParameterDirection.Input, 0, billIds);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            var _list = await objSql.QueryAsync<InvoiceDTO>(INVOICE_BYIDS);
            objSql.Dispose();
            return _list;
        }
        public async Task<int> SettleBill(InvoiceDTO dto)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, dto.InvoiceId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
            objSql.AddParameter("@settlementDate", DbType.DateTime, ParameterDirection.Input, 0, dto.SettlementDate);
            objSql.AddParameter("@settleBy", DbType.Int32, ParameterDirection.Input, 0, dto.SettledBy);
            objSql.AddParameter("@remarks", DbType.String, ParameterDirection.Input, 0, dto.SettlementRemarks);
            return await objSql.ExecuteNonQueryAsync(SETTLE_BILL);

        }
        public Task<IEnumerable<BillingItemDTO>> GetLossItemsToBill(BillingDTO dto, int finYearId)
        {
            SQL objSql = new SQL();
            //objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, dto.From);
            //objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, dto.To);
            objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerId);
            //objSql.AddParameter("@FinYearId", DbType.Int16, ParameterDirection.Input, 0, finYearId);
            objSql.AddParameter("@InvoiceId", DbType.Int32, ParameterDirection.Input, 0, dto.InvoiceId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);

            objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerSiteId);
            // objSql.AddParameter("@invoiceType", DbType.Int32, ParameterDirection.Input, 0, dto.InvoiceType);

            return objSql.QueryAsync<BillingItemDTO>(GET_LOSS_ITEMS_TOBILL);
        }


        public Task<IEnumerable<TaxCategoryDTO>> GetAllTaxCategories()
        {
            SQL objSql = new SQL();

            return objSql.QueryAsync<TaxCategoryDTO>(GET_ALL_TAXCATOGRIES);
        }

        public async Task<QuotationDTO> QuotationByNumber(QuotationDTO dto)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@number", DbType.String, ParameterDirection.Input, 0, dto.QuotationNumber);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
            return await objSql.QueryFirstAsync<QuotationDTO>(GET_QUOTATION_BY_NUMBER);
        }
        public DataSet getAdjustedItemsToPrintOnBill(int invoiceId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@invoiceId", DbType.String, ParameterDirection.Input, 0, invoiceId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            return objSql.ExecuteDataSet(p_getAdjustedItems);
        }
        public DataSet getOhterChargesToBill(int ledgersiteId, int companyId, DateTime from, DateTime to)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgersiteId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, from);
            objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, to);

            return objSql.ExecuteDataSet("p_LedgerSiteChargesV1");
        }
        #region
        const string GEN_BiLL = "p_GeneratBillV10";
        const string SAVE_BiLL = "p_SaveBill";
        const string GET_BILLS = "p_selectBills";
        const string GET_CONTRACT_BILLS = "p_selectContractBills";

        const string GET_BILLITEMS = "p_selectBillItems";
        const string BILL_REPORT = "p_BillingReport";
        const string BY_ID = "p_invoiceById";

        const string CHECK_BILL = "p_CheckForBill";
        const string ADD_TAX = "p_InvoiceTax_ins";
        const string DEL_INVOICE_TAX = "p_InvoiceTax_del";
        const string GET_BREAKAGE_FOR_BILL = "p_getBreakage_ItemsForBill";
        const string GET_BREAKAGE_DAMAGE_DETAILS_FOR_BILL = "p_getBreakageDamageDetailsForBill";
        const string DEL_INVOICE_BREAKAGE_DAMAGE = "p_InvoiceBreakageDamageComponent_del";
        const string INS_INVOICE_BREAKAGE_DAMAGE = "p_InvoiceBreakageDamageComponent_ins";
        const string SEL_INVOICE_BREAKAGE_DAMAGE_BY_INVOICE = "p_InvoiceBreakageDamageComponent_selByInvoice";
        const string ADD = "p_Invoice_ins_v3";
        const string UPDATE_INVOICE = "p_Invoice_upd_v3";
        const string ADD_ITEMS = "p_InvoiceItems_ins";
        const string DEL_INVOICE_ITEMS = "p_InvoiceItems_del";

        const string BILLING_ITEMS_TAX = "p_BillingItemsTax_sel";
        const string UPDATE_INVOICE_STATUS = "p_InvoiceStatus_upd";
        const string DUE_BILLS = "p_DueBills";
        const string DUE_BILLS_SUMMARY = "p_DueBills_summaryV2";

        const string ADD_LOST_ITEM = "p_LossItems_ins";
        const string INVOICE_LOSS_ITEM = "p_LossItemsByInvoice_sel";
        const string INVOICE_BREAKAGE_ITEM = "p_BreakageItemsByInvoice_selV1";
        const string ADD_OTHER_CHARGE = "p_InvoiceCharge_ins";
        const string DEL_OTHER_CHARGE = "p_InvoiceCharge_del";


        const string p_getBalanceAfterLastBill = "p_getBalanceAfterLastBill";
        const string INVOICE_HEADER_FOR_NOTIFICATION = "p_invoiceHeader_sel";

        const string ADD_QUOTATION = "p_Quotation_ins";
        const string UPD_QUOTATION = "p_Quotation_upd";
        const string DEL_QUOTATION_ITEMS = "p_QuotationItems_del";
        const string UPDATE_QUOTATION_STATUS = "p_Quotation_updStatus";
        const string LINK_QUOTATION_PARTY = "p_Quotation_linkParty";
        const string LINK_QUOTATION_INVOICE = "p_Quotation_linkInvoice";

        const string ADD_QUOTATION_ITEMS = "p_QuotationItems_ins";
        const string QUOTATIONS_LIST = "p_quotations_sel";
        const string QUOTATIONS_BY_CONTRACT = "p_quotations_selByContractId";
        const string QUOTATION_BYID = "p_getQuotation_byId";

        const string GET_QUOTATION_ITEMS = "p_getQuotationItems";
        const string ADD_EWAY_BILL = "p_EwayBill_ins";
        const string UPDATE_EWAY_BILL = "p_EwayBill_upd";
        const string EWAYBILL_DETAILS_INS = "p_EwayBillDetails_ins";
        const string EWAYBILL_DETAILS_DEL = "p_EwayBillDetails_del";
        const string EWAYBILL_DETAILS_SEL = "p_EwayBillDetails_byId";


        const string GETALL_EWAY_BILL = "p_EwayBill_all";
        const string GET_EWAY_BILL_BYID = "p_EwayBill_ById";

        const string UPDATE_EWAYBILL_INFO = "p_updateEwayBill_info";
        const string p_EwayBill_updatePortalInfo = "p_EwayBill_updatePortalInfo";
        const string p_printEwayBill = "p_printEwayBill";
        const string UPDATE_VEHICLE_INFO = "p_printEwayBill";
        const string CANCEL_EWAY_BILL = "p_cancelEwayBill";

        const string EWAYBILL_BY_DOC_NUMBER = "p_ewayBillselByDocNo";

        const string p_invoiceTransactions_del = "p_invoiceTransactions_del";
        const string BILL_CONTRACT_REPORT = "p_ContractBillingReport";

        const string INVOICE_OTHER_CHARGES_SEL = "p_invoiceOtherCharges_sel";

        const string ADD_MAT_LOSS = "p_MatLoss_ins";
        const string MAT_LOSS_ALL = "p_matLoss_sel";
        const string MAT_LOSS_BYID = "p_matLoss_byId";
        const string MAT_LOSS_ITEMS_BYID = "p_matLossItems_ById";
        const string MAT_LOSS_DEL = "p_matloss_del";
        const string INVOICE_UNPAID_SEL = "p_unpaidInvoices_sel";
        const string INVOICE_BYIDS = "p_invoices_ByIds";
        const string SETTLE_BILL = "p_settleBill";
        const string BILL_CHALLANS_INS = "p_billChallans_ins";
        const string BILL_CHALLANS_SEL = "p_billChallans_sel";
        const string BILL_CHALLANS_DEL = "p_billChallans_del";
        const string GET_LOSS_ITEMS_TOBILL = "p_getLossItemsToBillV1";
        const string GET_ALL_TAXCATOGRIES = "p_taxCategory_all";
        const string UPDATE_CHALLAN_ITEM_FOR_BREAKAGE = "p_updateGRnItems_InvoiceId_ForBreakage";
        const string GET_QUOTATION_BY_NUMBER = "p_quotation_byNumber";
        const string p_getAdjustedItems = "p_getAdjustedItems";

        #endregion
    }
}
