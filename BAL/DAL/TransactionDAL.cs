using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using BAL.DTO;
namespace BAL.DAL
{
    internal class TransactionDAL
    {

        internal bool Add(TransactionDTO dto)
        {
            SQL sql = new SQL();
            sql.AddParameter("@Number", DbType.String, ParameterDirection.Input, 0, dto.Number);
            sql.AddParameter("@TransactionType", DbType.Int16, ParameterDirection.Input, 0, dto.TransactionType);
            sql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerId);
            sql.AddParameter("@LedgerSiteId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerSiteId);
            sql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, dto.CompanyId);
            sql.AddParameter("@FinYearId", DbType.Int16, ParameterDirection.Input, 0, dto.FinYearId);
            sql.AddParameter("@TransDate", DbType.DateTime, ParameterDirection.Input, 0, dto.TransactionDate);
            sql.AddParameter("@UserId", DbType.Int16, ParameterDirection.Input, 0, dto.UserId);

            dto.TransactionId = Convert.ToInt32(sql.ExecuteScalar(ADD));

            return dto.TransactionId > 0;
        }

        #region Procedures

        const string ADD = "p_Transactions_ins";
        #endregion
    }
}
