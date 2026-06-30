using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using BAL.DTO;
namespace BAL.DAL
{
    internal class ChallanItemDAL
    {
        public void Save(ChallanItemDTO dto, SQL objSql)
        {
            try
            {

                objSql.AddParameter("@ChallanId", DbType.Int64, ParameterDirection.Input, 0, dto.Challan.ChallanId);
                objSql.AddParameter("@ProductId", DbType.Int64, ParameterDirection.Input, 0, dto.ProductId);
                objSql.AddParameter("@PurchaseRate", DbType.Double, ParameterDirection.Input, 0, dto.PurchaseRate);
                objSql.AddParameter("@SubTotal", DbType.Double, ParameterDirection.Input, 0, dto.SubTotal);
                objSql.AddParameter("@BatchNumber", DbType.String, ParameterDirection.Input, 0, dto.BatchNumber);
                objSql.AddParameter("@PurchaseQty", DbType.Double, ParameterDirection.Input, 0, dto.Quantity);
                objSql.AddParameter("@ExpiryDate", DbType.Date, ParameterDirection.Input, 0, dto.Expiry);

                objSql.ExecuteNonQuery(SAVE);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

          
        }

        #region StoredProcedures
        const string SAVE = "p_ChallanItems_ins";
        #endregion

    }
}
