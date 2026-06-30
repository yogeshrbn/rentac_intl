using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using System.Data;
namespace BAL.DAL
{
    internal class ProductCategoryDAL
    {

        SQL objSql = null;
        internal ProductCategoryDAL()
        {
            objSql = new SQL();
        }

        internal int Save(ProductCategoryDTO objCategory)
        {
            objSql.AddParameter("@Name", DbType.String, ParameterDirection.Input, 0, objCategory.Name);
            objSql.AddParameter("@MinMargin", DbType.Double, ParameterDirection.Input, 0, objCategory.MinMargin);
            //Create new
            if (objCategory.CategoryId == 0)
            {
                objSql.AddParameter("@StoreId", DbType.Int32, ParameterDirection.Input, 0, objCategory.StoreId);
                return Convert.ToInt32(objSql.ExecuteScalar(ADD));
            }
            else//udpate
            {
                objSql.AddParameter("@CategoryId", DbType.Int32, ParameterDirection.Input, 0, objCategory.CategoryId);
                return Convert.ToInt32(objSql.ExecuteScalar(UPDATE));
            }
        }

        internal List<ProductCategoryDTO> GetAll(int storeId)
        {
            objSql = new SQL();
            objSql.AddParameter("@StoreId", DbType.Int32, ParameterDirection.Input, 0, storeId);
            return objSql.ContructList<ProductCategoryDTO>(objSql.ExecuteDataSet(GETALL));
        }

        internal ProductCategoryDTO GetInfo(int categoryId)
        {
            objSql.AddParameter("@CategoryId", DbType.Int32, ParameterDirection.Input, 0, categoryId);
            return objSql.ContructList<ProductCategoryDTO>(objSql.ExecuteDataSet(GETINFO)).FirstOrDefault();

        }

        internal int ChangeStatus(int categoryId, ProductCategoryStatus status)
        {
            objSql.Dispose();
            objSql = new SQL();
            objSql.AddParameter("@CategoryId", DbType.Int32, ParameterDirection.Input, 0, categoryId);
            objSql.AddParameter("@Status", DbType.Int16, ParameterDirection.Input, 0, Convert.ToInt16(status));

            return Convert.ToInt16(objSql.ExecuteScalar(CHANGESTATUS));
        }

        #region Procedures
        const string ADD = "p_ProductCat_ins";
        const string UPDATE = "p_ProductCat_upd";
        const string GETALL = "p_ProductCat_selAll";
        const string GETINFO = "p_ProductCategory_sel";
        const string CHANGESTATUS = "p_ProductCategory_ChangeStatus";
        #endregion
    }
}
