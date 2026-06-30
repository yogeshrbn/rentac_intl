using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using System.Data;
namespace BAL.DAL
{
    class SalesDAL
    {
        internal bool Add(SalesDTO dataOBject)
        {
            SQL objSql = new SQL();
            Int32 _salesId = 0;
            try
            {

                if (dataOBject.ApplicableTaxes.Count > 0)
                {
                    dataOBject.TaxAmount = dataOBject.Items.Sum(o => o.IGST + o.CGST + o.SGST);
                }
                dataOBject.Total = dataOBject.SubTotal - dataOBject.DiscountAmount + dataOBject.TaxAmount;
                objSql.BeginTransaction();
                objSql.NewCommand();
                #region Sales
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 50, dataOBject.CompanyId);
                objSql.AddParameter("@VchNumber", DbType.String, ParameterDirection.Input, 50, dataOBject.VoucherNumber);
                objSql.AddParameter("@PartyBillNo", DbType.String, ParameterDirection.Input, 50, dataOBject.BillNumber);
                objSql.AddParameter("@SalesType", DbType.Int32, ParameterDirection.Input, 50, dataOBject.SalesType);
                objSql.AddParameter("@SalesDate", DbType.DateTime, ParameterDirection.Input, 50, dataOBject.SalesDate);
                objSql.AddParameter("@CreatedBy", DbType.Int32, ParameterDirection.Input, 50, dataOBject.CreatedBy);
                objSql.AddParameter("@Details", DbType.String, ParameterDirection.Input, 50, dataOBject.Details);
                objSql.AddParameter("@SubTotal", DbType.Double, ParameterDirection.Input, 50, dataOBject.SubTotal);
                objSql.AddParameter("@TaxAmount", DbType.Double, ParameterDirection.Input, 50, dataOBject.TaxAmount);
                objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 50, dataOBject.LedgerId);
                objSql.AddParameter("@SalesAccount", DbType.Int32, ParameterDirection.Input, 50, dataOBject.SalesAccountId);
                objSql.AddParameter("@Freight", DbType.Double, ParameterDirection.Input, 50, dataOBject.Freight);
                objSql.AddParameter("@FreightTax", DbType.Double, ParameterDirection.Input, 50, dataOBject.FreightTax);
                objSql.AddParameter("@FinYearId", DbType.Int32, ParameterDirection.Input, 50, dataOBject.FinYearId);
                objSql.AddParameter("@DiscountPercent", DbType.Double, ParameterDirection.Input, 50, dataOBject.DiscountPercent);

                objSql.AddParameter("@DiscountAmount", DbType.Double, ParameterDirection.Input, 50, dataOBject.DiscountAmount);
                objSql.AddParameter("@total ", DbType.Double, ParameterDirection.Input, 50, dataOBject.Total);

                dataOBject.SalesId = _salesId = Convert.ToInt32(objSql.ExecuteScalar(ADD));
                #region ItemAndTax
                foreach (SalesItemDTO item in dataOBject.Items)
                {
                    // item.WorkOrder = new WorkOrderDTO { InvoiceId = _invoiceId };
                    item.SalesId = _salesId;
                    objSql.NewCommand();
                    AddItem(item, objSql);
                }
                foreach (TaxDTO item in dataOBject.ApplicableTaxes)
                {
                    objSql.NewCommand();
                    AddTax(dataOBject, item, objSql);
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


            return _salesId > 0;
        }
        bool AddTax(SalesDTO dto, TaxDTO taxDto, SQL objSql)
        {
            taxDto.Amount = dto.SubTotal;
            objSql.AddParameter("@TaxId", DbType.Int32, ParameterDirection.Input, 0, taxDto.TaxId);
            objSql.AddParameter("@ProductId", DbType.Int32, ParameterDirection.Input, 0, taxDto.ItemValue);

            objSql.AddParameter("@Rate", DbType.Double, ParameterDirection.Input, 0, taxDto.Rate);
            objSql.AddParameter("@Amount", DbType.Double, ParameterDirection.Input, 0, taxDto.TaxAmount);
            objSql.AddParameter("@SalesId", DbType.Int32, ParameterDirection.Input, 0, dto.SalesId);
            return objSql.ExecuteNonQuery(ADD_TAX) > 0;
        }
        public void AddItem(SalesItemDTO dto, SQL objSql)
        {
            try
            {


                objSql.AddParameter("@ProductId", DbType.Int64, ParameterDirection.Input, 0, dto.ProductId);
                objSql.AddParameter("@Rate", DbType.Double, ParameterDirection.Input, 0, dto.Rate);
                objSql.AddParameter("@SubTotal", DbType.Double, ParameterDirection.Input, 0, dto.SubTotal);
                objSql.AddParameter("@Quantity", DbType.Double, ParameterDirection.Input, 0, dto.Quantity);
                objSql.AddParameter("@SalesId", DbType.Int32, ParameterDirection.Input, 0, dto.SalesId);

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

        public List<SalesDTO> SalesRegister(int companyId)
        {
            SQL objSql = new SQL();
            if (companyId > 0)
            {
                objSql.AddParameter("@companyId", DbType.String, ParameterDirection.Input, 0, companyId);
            }
            return objSql.ContructList<SalesDTO>(objSql.ExecuteDataSet(SALES_REGISTER));
        }
        public List<SalesItemDTO> SalesItemsList(int salesId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@SalesId", DbType.Int32, ParameterDirection.Input, 0, salesId);
            return objSql.ContructList<SalesItemDTO>(objSql.ExecuteDataSet(SALES_ITEMS_LIST));
        }
        public List<SalesItemDTO> SalesItemsTax(int salesId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@SalesId", DbType.Int32, ParameterDirection.Input, 0, salesId);
            return objSql.ContructList<SalesItemDTO>(objSql.ExecuteDataSet(SALES_ITEMS_TAX));
        }
        public DataSet GetReceiptRegisterPRT(int purchaseId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@SalesId", DbType.Int32, ParameterDirection.Input, 0, purchaseId);
            return objSql.ExecuteDataSet(SALES_ITEMS_LIST);
        }

        #region Procedures
        const string ADD = "p_Sales_ins";
        const string ADD_ITEMS = "p_SalesItems_ins";
        const string ADD_TAX = "p_SalesTax_ins";
        const string SALES_REGISTER = "p_SalesReg_sel";
        const string SALES_ITEMS_LIST = "p_getSalesItems";
        const string SALES_ITEMS_TAX = "p_SalesItemsTax_sel";
        #endregion
    }
}
