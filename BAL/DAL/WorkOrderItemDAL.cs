using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using BAL.DTO;
namespace BAL.DAL
{
    internal class WorkOrderItemDAL
    {
        public int Save(WorkOrderItemDTO dto, SQL objSql)
        {
            try
            {
                objSql.AddParameter("@ProductId", DbType.Int64, ParameterDirection.Input, 0, dto.ProductId);
                objSql.AddParameter("@Rate", DbType.Double, ParameterDirection.Input, 0, dto.Rate);
                objSql.AddParameter("@SubTotal", DbType.Double, ParameterDirection.Input, 0, dto.SubTotal);
                objSql.AddParameter("@PurchaseQty", DbType.Double, ParameterDirection.Input, 0, dto.PurchaseQty);
                objSql.AddParameter("@SiteId", DbType.Int32, ParameterDirection.Input, 0, dto.SiteId);
                objSql.AddParameter("@Mode", DbType.Int16, ParameterDirection.Input, 0, Convert.ToInt16(dto.SiteItemType));

                objSql.AddParameter("@igst", DbType.Double, ParameterDirection.Input, 0, dto.IGSTAmount);
                objSql.AddParameter("@cgst", DbType.Double, ParameterDirection.Input, 0, dto.CGSTAmount);
                objSql.AddParameter("@sgst", DbType.Double, ParameterDirection.Input, 0, dto.SGSTAmount);

                if (dto.ProductSizeId > 0)
                {
                    objSql.AddParameter("@ProductSizeId", DbType.Int32, ParameterDirection.Input, 0, Convert.ToInt16(dto.ProductSizeId));
                }
                if (dto.GroupItemId > 0)
                {
                    objSql.AddParameter("@GroupItemId", DbType.Int32, ParameterDirection.Input, 0, dto.GroupItemId);
                }
                if (dto.WorkOrderItemId > 0)
                {
                    objSql.AddParameter("@WorkOrderItemId", DbType.Int32, ParameterDirection.Input, 0, dto.WorkOrderItemId);
                    objSql.ExecuteNonQuery(UPDATE);
                    return dto.WorkOrderItemId;
                }

                var result = objSql.ExecuteScalar(SAVE);
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }

                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region StoredProcedures
        const string SAVE = "p_WorkOrderItems_ins";
        const string UPDATE = "p_WorkOrderItems_upd";

        #endregion

    }
}
