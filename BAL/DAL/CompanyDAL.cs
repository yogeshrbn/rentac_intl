using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;

using System.Data;
using System.Data.SqlClient;
using BAL.Exceptions;
namespace BAL.DAL
{
    public class CompanyDAL
    {
        public int Save(CompanyDTO _data)
        {
            SQL objSql = new SQL();
            try
            {
                var cId = 0;
                objSql.BeginTransaction();
                objSql.NewCommand();
                objSql.AddParameter("@Name", DbType.String, ParameterDirection.Input, 0, _data.Name);
                objSql.AddParameter("@Address1", DbType.String, ParameterDirection.Input, 0, _data.Address1);
                objSql.AddParameter("@Address2", DbType.String, ParameterDirection.Input, 0, _data.Address2);
                objSql.AddParameter("@Email", DbType.String, ParameterDirection.Input, 0, _data.Email);

                objSql.AddParameter("@Fax", DbType.String, ParameterDirection.Input, 0, _data.Fax);
                objSql.AddParameter("@Phone1", DbType.String, ParameterDirection.Input, 0, _data.Phone1);
                objSql.AddParameter("@Phone2", DbType.String, ParameterDirection.Input, 0, _data.Phone2);
                objSql.AddParameter("@Contact", DbType.String, ParameterDirection.Input, 0, _data.Contact);
                objSql.AddParameter("@City", DbType.String, ParameterDirection.Input, 0, _data.City);
                objSql.AddParameter("@State", DbType.String, ParameterDirection.Input, 0, _data.State);
                objSql.AddParameter("@StateId", DbType.Int16, ParameterDirection.Input, 0, _data.StateId);

                objSql.AddParameter("@ZipCode", DbType.String, ParameterDirection.Input, 0, _data.ZipCode);
                objSql.AddParameter("@Web", DbType.String, ParameterDirection.Input, 0, _data.Web);
                objSql.AddParameter("@TIN", DbType.String, ParameterDirection.Input, 0, _data.TIN);
                objSql.AddParameter("@TAN", DbType.String, ParameterDirection.Input, 0, _data.TAN);
                objSql.AddParameter("@PAN", DbType.String, ParameterDirection.Input, 0, _data.PAN);

                objSql.AddParameter("@SignAuthority", DbType.String, ParameterDirection.Input, 0, _data.SignAuthority);
                objSql.AddParameter("@GSTNo", DbType.String, ParameterDirection.Input, 0, _data.GSTNo);
                objSql.AddParameter("@ReportHeader", DbType.String, ParameterDirection.Input, 0, _data.ReportHeader);
                objSql.AddParameter("@logo", DbType.String, ParameterDirection.Input, 0, _data.Logo);
                objSql.AddParameter("@msmeNumber", DbType.String, ParameterDirection.Input, 0, _data.MSMENumber);
                objSql.AddParameter("@signature", DbType.String, ParameterDirection.Input, 0, _data.Signature);


                objSql.AddParameter("@RbnClientId", DbType.Int16, ParameterDirection.Input, 0, _data.RbnClientId);
                objSql.AddParameter("@bankName", DbType.String, ParameterDirection.Input, 0, _data.BankName);
                objSql.AddParameter("@bankBranch", DbType.String, ParameterDirection.Input, 0, _data.BankBranch);
                objSql.AddParameter("@bankAccNumber", DbType.String, ParameterDirection.Input, 0, _data.BankAccNumber);
                objSql.AddParameter("@ifscCode", DbType.String, ParameterDirection.Input, 0, _data.IFSCCode);
                objSql.AddParameter("@qrCode", DbType.String, ParameterDirection.Input, 0, _data.QrCode);
                objSql.AddParameter("@warehouses", DbType.String, ParameterDirection.Input, 0, _data.Warehouses);
                objSql.AddParameter("@defaultWarehouseId", DbType.Int32, ParameterDirection.Input, 0, _data.DefaultWarehouseId);
                objSql.AddParameter("@VAT", DbType.String, ParameterDirection.Input, 0, _data.VAT);

                if (_data.CompanyId == 0)
                {
                    objSql.AddParameter("@ParentCompanyId", DbType.Int32, ParameterDirection.Input, 0, _data.ParentCompanyId);

                    cId = Convert.ToInt32(objSql.ExecuteScalar(ADD));

                    //import master data
                    objSql.NewCommand();
                    objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, cId);
                    objSql.ExecuteNonQuery(IMPORT_MASTER_DATA);

                }
                else
                {
                    objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, _data.CompanyId);
                var x=    objSql.ExecuteNonQuery(UPDATE);
                    if(x == 0)
                    {
                        throw new Exception("Company could not be saved");
                    }
                    cId = _data.CompanyId;

                }
                objSql.Commit();
                return cId;
            }
            catch (SqlException ex)
            {
                objSql.Rollback();
                throw new UDFException(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                objSql.Rollback();
                throw ex;
            }
            finally
            {
                objSql.Dispose();
            }
        }

        public static List<CompanyDTO> GetAll(int rbnClientId, string query)
        {
            SQL objSql = new SQL();
            if (!String.IsNullOrEmpty(query))
            {
                objSql.AddParameter("@CompanyName", DbType.String, ParameterDirection.Input, 0, query);
            }
            objSql.AddParameter("@RbnClientId", DbType.Int16, ParameterDirection.Input, 0, rbnClientId);
            return objSql.ContructList<CompanyDTO>(objSql.ExecuteDataSet(GETALL));
        }
        public CompanyDTO GetDetails(int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            var ds = objSql.ExecuteDataSet(SELCECTCOMPANY);
            var comp = objSql.ContructList<CompanyDTO>(ds).FirstOrDefault();
            if (comp != null)
            {
                var dt = ds.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    comp.IRPPassword = Convert.ToString(dt.Rows[0]["irpPassword"]);
                    comp.IRPUserName = Convert.ToString(dt.Rows[0]["irpuserName"]);
                    comp.StateCode = Convert.ToInt16(dt.Rows[0]["StateCode"]);
                    comp.IRPToken = Convert.ToString(dt.Rows[0]["irpToken"]);
                    if (dt.Rows[0]["irpTokenExpiry"] != DBNull.Value)
                    {
                        comp.IRPTokenExpiry = Convert.ToDateTime(dt.Rows[0]["irpTokenExpiry"]);
                    }
                    comp.EwayPassword = Convert.ToString(dt.Rows[0]["EwayPassword"]);

                    comp.EwayUserName = Convert.ToString(dt.Rows[0]["EwayUserName"]);
                    if (dt.Rows[0]["EwayLastAuthenticatedOn"] != DBNull.Value)
                    {
                        comp.EwayLastAuthenticatedOn = Convert.ToDateTime(dt.Rows[0]["EwayLastAuthenticatedOn"]);
                    }
                }
            }
            return comp;
        }
        public CompanyDAL() { }

        public bool ActivateDeActivate(bool activate, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@IsActive", DbType.Boolean, ParameterDirection.Input, 0, activate);
            return objSql.ExecuteNonQuery(ACTIVATE_DEACTIVATE) == 1;

        }
        public bool UpdateGSTDetails(CompanyDTO dto)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
            objSql.AddParameter("@legalName", DbType.String, ParameterDirection.Input, 0, dto.LegalName);
            objSql.AddParameter("@tradeName", DbType.String, ParameterDirection.Input, 0, dto.TradeName);
            objSql.AddParameter("@gstno", DbType.String, ParameterDirection.Input, 0, dto.GSTNo);
            objSql.AddParameter("@gstRegistrationDate", DbType.Date, ParameterDirection.Input, 0, dto.GSTRegistrationDate);
            objSql.AddParameter("@gstStatus", DbType.String, ParameterDirection.Input, 0, dto.GSTStatus);

            return objSql.ExecuteNonQuery(UPDATE_TAXDETAILS) == 1;

        }
        public bool UpdateEInvoiceEnabled(CompanyDTO dto)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
            objSql.AddParameter("@eInvoiceStartDate", DbType.String, ParameterDirection.Input, 0, dto.EInvoiceStartDate);
            objSql.AddParameter("@eInvoiceEnabledBy", DbType.Int32, ParameterDirection.Input, 0, dto.EInvoiceEnabledBy);
            objSql.AddParameter("@eInvoiceEnabledOn", DbType.DateTime, ParameterDirection.Input, 0, dto.EInvoiceEnabledOn);


            return objSql.ExecuteNonQuery(UPDATE_EINVOICE_Enabled) == 1;

        }
        public bool UpdateIPRUserCrentials(CompanyDTO dto)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
            objSql.AddParameter("@irpUserName", DbType.String, ParameterDirection.Input, 0, dto.IRPUserName);
            objSql.AddParameter("@irpPassword", DbType.String, ParameterDirection.Input, 0, dto.IRPPassword);
            objSql.AddParameter("@irpUpdatedBy", DbType.Int32, ParameterDirection.Input, 0, dto.IRPUpdatedBy);

            objSql.AddParameter("@irpUpdatedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.IRPUpdatedOn);

            return objSql.ExecuteNonQuery(UPDATE_IRP_USERCREDS) == 1;

        }
        public bool UpdateIPRToken(CompanyDTO dto)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
            objSql.AddParameter("@irpToken", DbType.String, ParameterDirection.Input, 0, dto.IRPToken);
            objSql.AddParameter("@irpTokenExpiry", DbType.DateTime, ParameterDirection.Input, 0, dto.IRPTokenExpiry);

            return objSql.ExecuteNonQuery(UPDATE_IRP_TOKEN) == 1;

        }
        public bool UpdateEwayBillCreds(CompanyDTO dto)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
            objSql.AddParameter("@ewayUserName", DbType.String, ParameterDirection.Input, 0, dto.EwayUserName);
            objSql.AddParameter("@ewayPassword", DbType.String, ParameterDirection.Input, 0, dto.EwayPassword);

            return objSql.ExecuteNonQuery(p_updateEwayBill_Creds) == 1;

        }
        public CompanyDTO ApiKeyExists(string apiKey)
        {
            if (String.IsNullOrWhiteSpace(apiKey))
            {
                return null;
            }

            using (var con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString))
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT TOP 1 [CompanyId] FROM [Company] WHERE [rentacApiKey] = @apiKey";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@apiKey", apiKey);
                con.Open();
                var companyId = cmd.ExecuteScalar();
                if (companyId == null || companyId == DBNull.Value)
                {
                    return null;
                }

                return GetDetails(Convert.ToInt32(companyId));
            }
        }

        public static string GetRentacApiKeyByCompanyId(int companyId)
        {
            if (companyId <= 0)
            {
                return null;
            }

            using (var con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString))
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT TOP 1 [rentacApiKey] FROM [Company] WHERE [CompanyId] = @companyId";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@companyId", companyId);
                con.Open();
                var value = cmd.ExecuteScalar();
                if (value == null || value == DBNull.Value)
                {
                    return null;
                }

                return Convert.ToString(value);
            }
        }

        #region Procedures
        const string ADD = "p_Company_ins";
        const string UPDATE = "p_company_upd";
        const string GETALL = "p_company_getAll";
        const string SELCECTCOMPANY = "p_company_sel";
        const string ACTIVATE_DEACTIVATE = "p_company_ActivateDeactivate";
        const string CHANGE_ADDRESS = "";
        const string UPDATE_TAXDETAILS = "p_updateCompanyGSTInfo";
        const string UPDATE_EINVOICE_Enabled = "p_updateEInvoicingSetting";
        const string UPDATE_IRP_USERCREDS = "p_updateIrpUserCreds";
        const string UPDATE_IRP_TOKEN = "p_updateIrpToken";
        const string p_updateEwayBill_Creds = "p_updateEwayBill_Creds";
        const string IMPORT_MASTER_DATA = "p_importMasterDataToNewCompany";

        #endregion
    }
}
