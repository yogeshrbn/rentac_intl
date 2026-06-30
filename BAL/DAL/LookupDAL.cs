using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using BAL.DTO;
using BAL.Enums;
namespace BAL.DAL
{
    internal class LookupDataDAL
    {

        internal List<RoleDTO> GetRole(RoleType roleType)
        {
            SQL ojbSql = new SQL();
            ojbSql.AddParameter("@RoleTypeId", DbType.Byte, ParameterDirection.Input, 0, Convert.ToByte(roleType));
            return ojbSql.ContructList<RoleDTO>(ojbSql.ExecuteDataSet(GET_ALL_ROLES));
        }
        internal List<OtherChargeDTO> GetOtherCharges()
        {
            SQL ojbSql = new SQL();
            return ojbSql.ContructList<OtherChargeDTO>(ojbSql.ExecuteDataSet(GET_OTHER_CHARGES));
        }
        #region Procs
        const string GET_ALL_ROLES = "p_LookupRoles_sel";
        const string GET_OTHER_CHARGES = "p_OtherCharges_all";

        #endregion

    }
}
