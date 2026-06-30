using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.Enums;
using BAL.DTO;
using System.Data;
namespace BAL.DAL
{
    public class InventoryDAL
    {
        public bool PostStock(List<StockTransactionDTO> stockList, SQL sql = null)
        {
            SQL objSql = sql;
            if (sql == null)
            {
                objSql = new SQL();
                objSql.BeginTransaction();
            }


            try
            {

                foreach (StockTransactionDTO dto in stockList)
                {

                    objSql.NewCommand();
                    objSql.AddParameter("@ProductId", DbType.Int32, ParameterDirection.Input, 0, dto.ProductId);
                    objSql.AddParameter("@ProductSizeId", DbType.Int32, ParameterDirection.Input, 0, dto.ProductSizeId);
                    objSql.AddParameter("@Quantity", DbType.Double, ParameterDirection.Input, 0, dto.Quantity);
                    objSql.AddParameter("@FINYEAR", DbType.Int16, ParameterDirection.Input, 0, dto.FinYear);
                    objSql.AddParameter("@PostingType", DbType.Int16, ParameterDirection.Input, 0, dto.PostingType);
                    if (dto.PostingDate != DateTime.MinValue)
                    {
                        objSql.AddParameter("@PostingDate", DbType.Date, ParameterDirection.Input, 0, dto.PostingDate);
                    }
                    objSql.AddParameter("@Remarks", DbType.String, ParameterDirection.Input, 0, dto.Remarks);
                    objSql.AddParameter("@VoucherId", DbType.Int32, ParameterDirection.Input, 0, dto.VoucherId);
                    objSql.AddParameter("@PostedBy", DbType.Int32, ParameterDirection.Input, 0, dto.PostedBy);
                    objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);

                    objSql.ExecuteNonQuery(POST_STOCK);
                }
                if (sql == null) // commit if transaction is local
                {
                    objSql.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                if (sql == null) //rollback if transaction is local
                {
                    objSql.Rollback();
                }
                throw ex;
            }
        }

        public bool PostStock(StockTransactionHeaderDTO stock, SQL sql = null)
        {
            SQL objSql = sql;
            if (sql == null)
            {
                objSql = new SQL();
                objSql.BeginTransaction();
            }


            try
            {
                objSql.NewCommand();
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, stock.CompanyId);
                objSql.AddParameter("@VoucherId", DbType.String, ParameterDirection.Input, 0, stock.VoucherId);
                objSql.AddParameter("@postingDate", DbType.Date, ParameterDirection.Input, 0, stock.PostingDate);
                objSql.AddParameter("@postingType", DbType.Int16, ParameterDirection.Input, 0, stock.PostingType);
                objSql.AddParameter("@finYearId", DbType.Int32, ParameterDirection.Input, 0, stock.FinYear);

                objSql.AddParameter("@rbnClientId", DbType.Int32, ParameterDirection.Input, 0, stock.RbnClientId);


                if (stock.StockTransactionHeaderId > 0)
                {
                    objSql.AddParameter("@stockTransactionHeaderId", DbType.Int32, ParameterDirection.Input, 0, stock.StockTransactionHeaderId);
                    objSql.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, stock.ModifiedBy);
                    objSql.AddParameter("@modifiedOn", DbType.DateTime, ParameterDirection.Input, 0, stock.ModifiedOn);
                }
                else
                {
                    objSql.AddParameter("@createdBy", DbType.Int32, ParameterDirection.Input, 0, stock.CreatedBy);
                    objSql.AddParameter("@createdOn", DbType.DateTime, ParameterDirection.Input, 0, stock.CreatedOn);
                    objSql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, stock.GuId);
                }


                int headerId = Convert.ToInt32(objSql.ExecuteScalar(STOCK_HEADER_INS));

                foreach (StockTransactionDTO dto in stock.Items)
                {

                    objSql.NewCommand();

                    objSql.AddParameter("@ProductId", DbType.Int32, ParameterDirection.Input, 0, dto.ProductId);
                    objSql.AddParameter("@ProductSizeId", DbType.Int32, ParameterDirection.Input, 0, dto.ProductSizeId);
                    objSql.AddParameter("@Quantity", DbType.Double, ParameterDirection.Input, 0, dto.Quantity);
                    objSql.AddParameter("@FINYEAR", DbType.Int16, ParameterDirection.Input, 0, dto.FinYear);
                    objSql.AddParameter("@PostingType", DbType.Int16, ParameterDirection.Input, 0, dto.PostingType);
                    objSql.AddParameter("@headerId", DbType.Int32, ParameterDirection.Input, 0, headerId);
                    if (dto.PostingDate != DateTime.MinValue)
                    {
                        objSql.AddParameter("@PostingDate", DbType.Date, ParameterDirection.Input, 0, dto.PostingDate);
                    }
                    objSql.AddParameter("@Remarks", DbType.String, ParameterDirection.Input, 0, dto.Remarks);
                    objSql.AddParameter("@VoucherId", DbType.Int32, ParameterDirection.Input, 0, dto.VoucherId);
                    objSql.AddParameter("@PostedBy", DbType.Int32, ParameterDirection.Input, 0, dto.PostedBy);
                    objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                    objSql.AddParameter("@deleted", DbType.Int32, ParameterDirection.Input, 0, dto.Deleted);
                    objSql.ExecuteNonQuery(POST_STOCK);
                }
                if (sql == null) // commit if transaction is local
                {
                    objSql.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                if (sql == null) //rollback if transaction is local
                {
                    objSql.Rollback();
                }
                throw ex;
            }
        }

        internal List<StockTransactionDTO> ItemStock(int finYearId, int companyId)
        {
            List<StockTransactionDTO> lst = new List<StockTransactionDTO>();
            try
            {
                SQL objSql = new SQL();
                objSql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, companyId);
                objSql.AddParameter("@FinYearId", DbType.Int16, ParameterDirection.Input, 0, finYearId);
                return objSql.ContructList<StockTransactionDTO>(objSql.ExecuteDataSet(ITEM_STOCK));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<LedgerbalanceDTO> StockInhand(int ledgerId, string onDate, int warehouseId = 0)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@warehouseId", DbType.Int32, ParameterDirection.Input, 0, warehouseId);

            objSql.AddParameter("@asOnDate", DbType.Date, ParameterDirection.Input, 0, onDate);
            var ds = objSql.ExecuteDataSet(STOCK_IN_HAND);
            return objSql.ContructList<LedgerbalanceDTO>(ds);

        }
        public List<StockInventoryDto> StockSummary(int ledgerId, string onDate, int warehouseId = 0)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@asOnDate", DbType.Date, ParameterDirection.Input, 0, onDate);
            objSql.AddParameter("@warehouseId", DbType.Int32, ParameterDirection.Input, 0, warehouseId);

            return objSql.ContructList<StockInventoryDto>(objSql.ExecuteDataSet(STOCK_SUMMARY));
        }

        public int StockInsUpd(int rbnClientId, int finyearId, int companyId, DateTime onDate)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@effectiveDate", DbType.Date, ParameterDirection.Input, 0, onDate);
            objSql.AddParameter("@finYearId", DbType.Int32, ParameterDirection.Input, 0, finyearId);
            objSql.AddParameter("@rbnClientId", DbType.Int32, ParameterDirection.Input, 0, rbnClientId);
            return objSql.ExecuteNonQuery(STOCK_INSUPD);
        }
        public List<StockTransactionHeaderDTO> StockAdjustmentList(int companyId, DateTime from, DateTime to)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, from);
            objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, to);

            var ds = objSql.ExecuteDataSet(STOCK_HEADER_SEL);

            var lst = (from d in ds.Tables[0].AsEnumerable()
                       select new StockTransactionHeaderDTO
                       {
                           StockTransactionHeaderId = d.Field<Int32>("StockTransactionHeaderId"),
                           CompanyId = d.Field<Int32>("CompanyId"),
                           GuId = d.Field<String>("GuId"),
                           PostingDate = d.Field<DateTime>("PostingDate"),
                           VoucherId = d.Field<String>("VoucherId"),
                           PostingType = d.Field<byte>("PostingType"),
                           CreatedOn = d.Field<DateTime>("CreatedOn"),
                           Remarks = d.Field<String>("Remarks"),
                           PostingTypeName = d.Field<String>("PostingTypeName"),
                           CreatedByName = d.Field<String>("CreatedByName"),

                       }
                       ).ToList();

            return lst;
        }
        public StockTransactionHeaderDTO StockAdjustmentDetails(int companyId, int transactionHeaderId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@TransactionHeaderId", DbType.Int32, ParameterDirection.Input, 0, transactionHeaderId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            var ds = objSql.ExecuteDataSet(STOCK_TXN_SELBY_HEADER);
            var lst = (from d in ds.Tables[0].AsEnumerable()
                       group d by new
                       {
                           StockTransactionHeaderId = d.Field<Int32>("StockTransactionHeaderId"),
                           CompanyId = d.Field<Int32>("CompanyId"),
                           GuId = d.Field<String>("GuId"),
                           PostingDate = d.Field<DateTime>("PostingDate"),
                           VoucherId = d.Field<String>("VoucherId"),
                           PostingType = d.Field<byte>("PostingType"),
                           CreatedOn = d.Field<DateTime>("CreatedOn"),
                           Remarks = d.Field<String>("Remarks"),

                       } into g
                       select new StockTransactionHeaderDTO
                       {
                           StockTransactionHeaderId = g.Key.StockTransactionHeaderId,
                           CompanyId = g.Key.CompanyId,
                           GuId = g.Key.GuId,
                           PostingDate = g.Key.PostingDate,
                           VoucherId = g.Key.VoucherId,
                           PostingType = g.Key.PostingType,
                           CreatedOn = g.Key.CreatedOn,
                           Remarks = g.Key.Remarks,
                           Items = g.Select(o => new StockTransactionDTO
                           {
                               Quantity = Convert.ToDouble(o.Field<decimal>("Quantity")),
                               ProductId = o.Field<Int32>("ProductId"),
                               Product = o.Field<string>("Product")

                           }).ToList()

                       }
                       ).FirstOrDefault();

            return lst;
        }
        public int StockDelete(int companyId, int transactionHeaderId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@stockTransactionHeaderId", DbType.Int32, ParameterDirection.Input, 0, transactionHeaderId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            return objSql.ExecuteNonQuery(STOCK_HEADER_DEL);
        }

        public async Task<int> UpdateItemBalance(int finyearId, int companyId, int productId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@finYearId", DbType.Int32, ParameterDirection.Input, 0, finyearId);
            objSql.AddParameter("@productId", DbType.Int32, ParameterDirection.Input, 0, productId);
            return await objSql.ExecuteNonQueryAsync(UPDATE_ITEM_BALANCE);

        }

        #region Procedures
        const string POST_STOCK = "p_PostStock";
        const string ITEM_STOCK = "p_itemStock";
        const string STOCK_IN_HAND = "p_StockInhand";
        const string STOCK_SUMMARY = "p_StockSummary";
        const string STOCK_INSUPD = "p_ItemStock_insupd";
        const string STOCK_HEADER_INS = "p_StockTransactionHeader_ins";
        const string STOCK_HEADER_SEL = "p_stockpostingHeader_sel";
        const string STOCK_TXN_SELBY_HEADER = "p_stockpostingByHeader_sel";
        const string STOCK_HEADER_DEL = "p_stockTransaction_del";
        const string UPDATE_ITEM_BALANCE = "p_updateItemBalance";
        #endregion
    }
}
