using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using System.Data;
namespace BAL.DAL
{
    class InvoiceDAL
    {
        internal bool Add(WorkOrderDTO dataOBject)
        {
            SQL objSql = new SQL();
            Int32 _invoiceId = 0;
            try
            {

                if (dataOBject.ApplicableTaxes.Count > 0)
                {
                    dataOBject.TotalTax = dataOBject.ApplicableTaxes.Sum(o => o.TaxAmount);
                }

                objSql.BeginTransaction();
                objSql.NewCommand();
                #region Invoice
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 50, dataOBject.CompanyId);
                objSql.AddParameter("@InvoiceNumber", DbType.String, ParameterDirection.Input, 50, dataOBject.InvoiceNumber);
                objSql.AddParameter("@SiteId", DbType.Int32, ParameterDirection.Input, 50, dataOBject.SiteId);
                objSql.AddParameter("@InvoiceDate", DbType.DateTime, ParameterDirection.Input, 50, dataOBject.InvoiceDate);
                objSql.AddParameter("@CreatedBy", DbType.Int32, ParameterDirection.Input, 50, dataOBject.CreatedBy);
                objSql.AddParameter("@Details", DbType.String, ParameterDirection.Input, 50, dataOBject.Details);
                objSql.AddParameter("@SubTotal", DbType.Double, ParameterDirection.Input, 50, dataOBject.SubTotal);
                objSql.AddParameter("@TaxAmount", DbType.Double, ParameterDirection.Input, 50, dataOBject.TotalTax);


                dataOBject.InvoiceId = _invoiceId = Convert.ToInt32(objSql.ExecuteScalar(ADD));
                #region ItemAndTax
                foreach (WorkOrderItemDTO item in dataOBject.Items)
                {
                    item.WorkOrder = new WorkOrderDTO { InvoiceId = _invoiceId };
                    item.InvoiceId = _invoiceId;
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


            return _invoiceId > 0;
        }
        bool AddTax(WorkOrderDTO dto, TaxDTO taxDto, SQL objSql)
        {
            taxDto.Amount = dto.SubTotal;
            objSql.AddParameter("@TaxId", DbType.Int32, ParameterDirection.Input, 0, taxDto.TaxId);
            objSql.AddParameter("@Rate", DbType.Double, ParameterDirection.Input, 0, taxDto.Rate);
            objSql.AddParameter("@Amount", DbType.Double, ParameterDirection.Input, 0, taxDto.TaxAmount);
            objSql.AddParameter("@InvoiceId", DbType.Int32, ParameterDirection.Input, 0, dto.InvoiceId);
            return objSql.ExecuteNonQuery(ADD_TAX) > 0;
        }
        public void AddItem(WorkOrderItemDTO dto, SQL objSql)
        {
            try
            {


                objSql.AddParameter("@ProductId", DbType.Int64, ParameterDirection.Input, 0, dto.ProductId);
                objSql.AddParameter("@Rate", DbType.Double, ParameterDirection.Input, 0, dto.Rate);
                objSql.AddParameter("@SubTotal", DbType.Double, ParameterDirection.Input, 0, dto.SubTotal);
                objSql.AddParameter("@Quantity", DbType.Double, ParameterDirection.Input, 0, dto.PurchaseQty);
                objSql.AddParameter("@InvoiceId", DbType.Int32, ParameterDirection.Input, 0, dto.InvoiceId);


                objSql.ExecuteNonQuery(ADD_ITEMS);

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public List<WorkOrderDTO> InvoiceList(int companyId)
        {
            SQL objSql = new SQL();
            if (companyId > 0)
            {
                objSql.AddParameter("@companyId", DbType.String, ParameterDirection.Input, 0, companyId);
            }
            return objSql.ContructList<WorkOrderDTO>(objSql.ExecuteDataSet(INVOICE_LIST));
        }
        #region Procedures
        const string ADD = "p_Invoice_ins";
        const string ADD_ITEMS = "p_InvoiceItems_ins";
        const string ADD_TAX = "p_InvoiceTax_ins";
        const string INVOICE_LIST = "p_invoices_sel";

        #endregion
    }
}
