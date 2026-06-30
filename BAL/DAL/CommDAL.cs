using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using System.Data;
namespace BAL.DAL
{
    internal class CommDAL
    {

        internal bool Add(CommDTO dto)
        {
            SQL sql = new SQL();
            sql.AddParameter("@Recipient", DbType.String, ParameterDirection.Input, 0, dto.Recipient);
            sql.AddParameter("@RbnClientId", DbType.Int16, ParameterDirection.Input, 0, dto.RbnClientId);
            sql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, dto.CompanyId);
            sql.AddParameter("@Message", DbType.String, ParameterDirection.Input, 0, dto.Message);
            sql.AddParameter("@UserId", DbType.Int16, ParameterDirection.Input, 0, dto.UserId);
            sql.AddParameter("@For", DbType.String, ParameterDirection.Input, 0, dto.For);

            return sql.ExecuteNonQuery(ADD) > 0;

        }
        internal CommDTO GetMessageTemplate(int code,int rbnClientId)
        {
            SQL sql = new SQL();
            sql.AddParameter("@RbnClientId", DbType.Int16, ParameterDirection.Input, 0, rbnClientId);
            sql.AddParameter("@Code", DbType.Int16, ParameterDirection.Input, 0, code);

            return sql.ContructList<CommDTO>(sql.ExecuteDataSet(GET_MESSAGE_TEMPLATE)).FirstOrDefault();
        }
        #region Procs
        const string ADD = "p_MessageTransaction_ins";
        const string GET_MESSAGE_TEMPLATE = "p_GetMessageTemplate";

        #endregion
    }
}
