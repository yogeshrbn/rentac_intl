using BAL.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DAL
{
    public class ErrorCodeDAL
    {

        public ErrorCodes GetMessage(int errorCode)
        {
            try
            {
                var sql = new SQL();
                sql.AddParameter("@code", System.Data.DbType.Int16, System.Data.ParameterDirection.Input, 0, errorCode);
                var ds = sql.ExecuteDataSet(GET_message);
                var codeObj = new ErrorCodes();

                codeObj = sql.ContructList<ErrorCodes>(ds).FirstOrDefault();
                return codeObj;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region procedures
        const string GET_message = "p_errorMessage_sel";
        #endregion
    }
}
