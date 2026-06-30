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
    internal class NextIdDAL
    {
        internal string GetNextId(NextIdDTO dto, SQL sql = null)
        {
            SQL objSql = new SQL();
            if (sql != null)
            {
                objSql = sql;
            }
            objSql.AddParameter("@Table", DbType.String, ParameterDirection.Input, 0, Enum.GetName(typeof(NextIDTables), dto.Table));
            objSql.AddParameter("@FinYear", DbType.String, ParameterDirection.Input, 0, dto.FinYear);
            objSql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, dto.CompanyId);
            objSql.AddParameter("@PreFix", DbType.String, ParameterDirection.Input, 0, dto.Prefix);
            objSql.AddParameter("@InitialValue", DbType.String, ParameterDirection.Input, 0, dto.InitalValue);
            objSql.AddParameter("@NextId", DbType.String, ParameterDirection.Output, 50, dto.NextId);

            string nId = Convert.ToString(objSql.ExecuteScalar(NEW_ID));
            dto.NextId = objSql.OutParams[5].Value.ToString();
            return dto.NextId;
        }
        internal string GetLastId(NextIdDTO dto)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@Table", DbType.String, ParameterDirection.Input, 0, Enum.GetName(typeof(NextIDTables), dto.Table));
            objSql.AddParameter("@FinYear", DbType.String, ParameterDirection.Input, 0, dto.FinYear);
            objSql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, dto.CompanyId);
            objSql.AddParameter("@PreFix", DbType.String, ParameterDirection.Input, 0, dto.Prefix);
            string lastId = "";
            DataSet ds = objSql.ExecuteDataSet(LAST_ID);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        lastId = Convert.ToString(ds.Tables[0].Rows[0]["NextId"]);
                    }
                }
            }
            return lastId;
        }
        internal int UpdateId(NextIdDTO dto)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@Table", DbType.String, ParameterDirection.Input, 0, Enum.GetName(typeof(NextIDTables), dto.Table));
            objSql.AddParameter("@FinYear", DbType.String, ParameterDirection.Input, 0, dto.FinYear);
            objSql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, dto.CompanyId);
            objSql.AddParameter("@LastId", DbType.String, ParameterDirection.Input, 0, dto.NextId);
            objSql.AddParameter("@PreFix", DbType.String, ParameterDirection.Input, 0, dto.Prefix);
            return objSql.ExecuteNonQuery(UPDATE_LAST_ID);
        }

        #region StoredProcedures
        const string NEW_ID = "p_getNextId";
        const string LAST_ID = "p_getLast_ID";
        const string UPDATE_LAST_ID = "p_lastId_upd";
        #endregion
    }
}
