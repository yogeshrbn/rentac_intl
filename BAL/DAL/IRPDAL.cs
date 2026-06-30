using BAL.DTO;
using BAL.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DAL
{
    public class IRPDAL
    {
        public bool Save(IRPToken dto)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
            objSql.AddParameter("@userName", DbType.String, ParameterDirection.Input, 0, dto.Username);
            objSql.AddParameter("@sek", DbType.String, ParameterDirection.Input, 0, dto.Sek);
            objSql.AddParameter("@clientId", DbType.String, ParameterDirection.Input, 0, dto.ClientId);
            objSql.AddParameter("@authToken", DbType.String, ParameterDirection.Input, 0, dto.AuthToken);
            objSql.AddParameter("@tokenExpriy", DbType.DateTime, ParameterDirection.Input, 0, dto.TokenExpiry);
            objSql.AddParameter("@createdBy", DbType.Int32, ParameterDirection.Input, 0, dto.CreatedBy);
            objSql.AddParameter("@createdOn", DbType.DateTime, ParameterDirection.Input, 0, dto.CreatedOn);

            return objSql.ExecuteNonQuery(ADD) > 0;
        }
        public IRPToken GetValidToken(IRPToken dto)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
            objSql.AddParameter("@userName", DbType.String, ParameterDirection.Input, 0, dto.Username);
            objSql.AddParameter("@today", DbType.String, ParameterDirection.Input, 0, DateTime.Now);


            return objSql.ContructList<IRPToken>(objSql.ExecuteDataSet(SELECT_VALID)).FirstOrDefault();
        }
        public bool UpdateInvoiceIRNDetails(InvoiceIRNDTO dto)
        {
            SQL objSql = new SQL();
            try
            {
                objSql.BeginTransaction();
                objSql.NewCommand();
                
                objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, dto.InvoiceId);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                objSql.AddParameter("@createdBy", DbType.Int32, ParameterDirection.Input, 0, dto.CreatedBy);
                objSql.AddParameter("@createdOn", DbType.DateTime, ParameterDirection.Input, 0, dto.CreatedOn);
                objSql.AddParameter("@ackNo", DbType.String, ParameterDirection.Input, 0, dto.AckNo);
                objSql.AddParameter("@ackDate", DbType.DateTime, ParameterDirection.Input, 0, dto.AckDate);
                objSql.AddParameter("@irn", DbType.String, ParameterDirection.Input, 0, dto.IRN);
                objSql.AddParameter("@SingedInvoice", DbType.String, ParameterDirection.Input, 0, dto.SingedInvoice);
                objSql.AddParameter("@SingedQrCode", DbType.String, ParameterDirection.Input, 0, dto.SingedQrCode);
                objSql.AddParameter("@irnStatus", DbType.String, ParameterDirection.Input, 0, dto.IRNStatus);

                var result = objSql.ExecuteNonQuery(UPDATE_INVOICE_IRNDetails) > 0;
                objSql.Commit();
                return result;
            }
            catch (Exception ex)
            {
                objSql.Rollback();
                throw ex;
            }
        }

        #region procedures
        const string ADD = "p_IRPTokens_ins";
        const string SELECT_VALID = "p_ValidIRPToken_sel";
        const string UPDATE_INVOICE_IRNDetails = "p_Invoice_updIrn";

        #endregion
    }
}
