using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using System.Data;
namespace BAL.DAL
{
    public class JournalDAL
    {

        public bool CreateEntry(JournalDTO dto)
        {

            SQL objSQL = new SQL();
            objSQL.AddParameter("@SiteId", DbType.Int32, ParameterDirection.Input, 0, dto.SiteId);
            objSQL.AddParameter("@Amount", DbType.Double, ParameterDirection.Input, 0, dto.Amount);
            objSQL.AddParameter("@ClientId", DbType.Int32, ParameterDirection.Input, 0, dto.ClientId);
            objSQL.AddParameter("@ChallanNumber", DbType.String, ParameterDirection.Input, 0, dto.ChallanNumber);
            objSQL.AddParameter("@InvoiceNumber", DbType.String, ParameterDirection.Input, 0, dto.InvoiceNumber);
            objSQL.AddParameter("@ChequeNumber", DbType.String, ParameterDirection.Input, 0, dto.ChequeNumber);
            objSQL.AddParameter("@TransactionId", DbType.String, ParameterDirection.Input, 0, dto.TransactionId);
            objSQL.AddParameter("@Bank", DbType.String, ParameterDirection.Input, 0, dto.Bank);
            objSQL.AddParameter("@Remarks", DbType.String, ParameterDirection.Input, 0, dto.Remarks);
            objSQL.AddParameter("@EntryDate", DbType.Date, ParameterDirection.Input, 0, dto.EntryDate);
            objSQL.AddParameter("@FinYearId", DbType.Date, ParameterDirection.Input, 0, dto.FinYearId);
            return objSQL.ExecuteNonQuery(CREATE_ENTRY) > 0;

        }

        public List<JournalDTO> GetSiteJournal(int siteId)
        {

            SQL objSQL = new SQL();
            objSQL.AddParameter("@SiteId", DbType.Int32, ParameterDirection.Input, 0, siteId);
            return objSQL.ContructList<JournalDTO>(objSQL.ExecuteDataSet(SITE_JOURNAL));
        }
        public List<JournalDTO> GetJournal(string fromDate, string toDate)
        {
            SQL objSQL = new SQL();
            objSQL.AddParameter("@fromDate", DbType.Date, ParameterDirection.Input, 0, fromDate);
            objSQL.AddParameter("@toDate", DbType.Date, ParameterDirection.Input, 0, toDate);

            return objSQL.ContructList<JournalDTO>(objSQL.ExecuteDataSet(GET_JOURNAL));
        }

        #region PROCEDURES
        const string CREATE_ENTRY = "p_Journal_entry";
        const string SITE_JOURNAL = "p_SiteJournal_sel";
        const string GET_JOURNAL = "p_GetJournal";

        #endregion
    }
}
