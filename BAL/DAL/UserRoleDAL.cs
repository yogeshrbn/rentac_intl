using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using System.Data;
namespace BAL.DAL
{
    internal class UserRoleDAL
    {

        internal List<FunctionDTO> GetFunctions(int roleId, int companyId)
        {
            SQL sql = new SQL();
            sql.AddParameter("@RoleId", DbType.Int16, ParameterDirection.Input, 0, roleId);
            sql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, companyId);
            var ds = sql.ExecuteDataSet(ROLE_FUNCTION_ALL);
            sql.Dispose();
            return sql.ContructList<FunctionDTO>(ds);
        }
        internal bool AddRoleFunction(int roleId, int companyId, FunctionDTO func)
        {
            SQL sql = new SQL();
            sql.AddParameter("@RoleId", DbType.Int16, ParameterDirection.Input, 0, roleId);
            sql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, companyId);
            sql.AddParameter("@FunctionId", DbType.Int16, ParameterDirection.Input, 0, func.FunctionId);
            sql.AddParameter("@Add", DbType.Int16, ParameterDirection.Input, 0, func.Add);
            sql.AddParameter("@Edit", DbType.Int16, ParameterDirection.Input, 0, func.Edit);
            sql.AddParameter("@View", DbType.Int16, ParameterDirection.Input, 0, func.View);
            sql.AddParameter("@Delete", DbType.Int16, ParameterDirection.Input, 0, func.Delete);
            var x = sql.ExecuteNonQuery(ROLE_FUNCTION_ADD);
            sql.Dispose();
            return x > 0;
        }

        internal List<MenuDTO> AllMenus(int companyId)
        {
            SQL sql = new SQL();
            sql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, companyId);

            return sql.ContructList<MenuDTO>(sql.ExecuteDataSet(GET_ALL_MENUS));
        }

        #region procs

        const string ROLE_FUNCTION_ALL = "p_RoleFunction_selAll";
        const string ROLE_FUNCTION_ADD = "p_rolefunction_ins";
        const string GET_ALL_MENUS = "p_menus_sel";

        #endregion
    }
}
