using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using System.Data;
namespace BAL.DAL
{
    internal class AuthenticationDAL
    {
        internal UserDTO Authenticate(string userName, string password)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@LoginName", DbType.String, ParameterDirection.Input, 0, userName);
            objSql.AddParameter("@Password", DbType.String, ParameterDirection.Input, 0, password);
            return objSql.ContructList<UserDTO>(objSql.ExecuteDataSet(VALIDATE_USER)).FirstOrDefault();
        }
        internal void SaveToken(int userId, string token,int finYear)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@userId", DbType.String, ParameterDirection.Input, 0, userId);
            objSql.AddParameter("@token", DbType.String, ParameterDirection.Input, 0, token);
            objSql.AddParameter("@finYear", DbType.Int16, ParameterDirection.Input, 0, finYear);
            objSql.ExecuteNonQuery(SAVE_TOKEN);
        }
        internal UserDTO GetToken(string token)
        {
            SQL objSql = new SQL();
            
            objSql.AddParameter("@token", DbType.String, ParameterDirection.Input, 0, token);
            return objSql.ContructList<UserDTO>(objSql.ExecuteDataSet(GET_TOKEN)).FirstOrDefault();
        }
        #region StoredProcedures

        const string VALIDATE_USER = "p_validateUserV1";
        const string SAVE_TOKEN = "p_saveToken";
        const string GET_TOKEN = "p_GetToken";
        #endregion
    }
}
