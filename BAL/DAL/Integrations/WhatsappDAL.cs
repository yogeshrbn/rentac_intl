using BAL.DTO;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.Services;

namespace BAL.DAL.Integrations
{
    public class WhatsappDAL
    {
        LoggingService logging = new LoggingService();
        public async Task<bool> CreateApp(WhatsappDTO dto)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@name", DbType.String, ParameterDirection.Input, 0, dto.Name);
                sql.AddParameter("@app_Id", DbType.String, ParameterDirection.Input, 0, dto.App_Id);
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                sql.AddParameter("@rbnClientId", DbType.Int32, ParameterDirection.Input, 0, dto.RbnClientId);
                sql.AddParameter("@createdBy", DbType.Int32, ParameterDirection.Input, 0, dto.CreatedBy);
                sql.AddParameter("@createdOn", DbType.DateTime, ParameterDirection.Input, 0, dto.CreatedOn);
                sql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, dto.GuId);
                return (await sql.ExecuteNonQueryAsync(CREATE_APP)) > 0;
            }
            catch (Exception ex)
            {
                logging.LogError(ex, ex.Message);
                return false;
            }

        }
        public async Task<bool> UpdateAppDetails(WhatsappDTO dto)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@langCode", DbType.String, ParameterDirection.Input, 0, dto.LanguageCode);
                sql.AddParameter("@app_Id", DbType.String, ParameterDirection.Input, 0, dto.App_Id);
                sql.AddParameter("@live", DbType.Boolean, ParameterDirection.Input, 0, dto.Live);
                sql.AddParameter("@liveTs", DbType.String, ParameterDirection.Input, 0, dto.LiveTs);
                sql.AddParameter("@stopped", DbType.Boolean, ParameterDirection.Input, 0, dto.Stopped);
                sql.AddParameter("@phone", DbType.String, ParameterDirection.Input, 0, dto.Phone);
                sql.AddParameter("@templateMessaging", DbType.Boolean, ParameterDirection.Input, 0, dto.TemplateMessaging);
                sql.AddParameter("@type", DbType.String, ParameterDirection.Input, 0, dto.Type);
                sql.AddParameter("@version", DbType.Double, ParameterDirection.Input, 0, dto.Version);

                return (await sql.ExecuteNonQueryAsync(UPDATE_APP_DETAILS)) > 0;
            }
            catch (Exception ex)
            {
                logging.LogError(ex, ex.Message);
                return false;
            }

        }

        public async Task<bool> UpdateAppToken(WhatsappDTO dto)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@token", DbType.String, ParameterDirection.Input, 0, dto.AppToken);
                sql.AddParameter("@app_Id", DbType.String, ParameterDirection.Input, 0, dto.App_Id);


                return (await sql.ExecuteNonQueryAsync(UPDATE_APP_TOKEN)) > 0;
            }
            catch (Exception ex)
            {
                logging.LogError(ex, ex.Message);
                return false;
            }

        }

        public async Task<IEnumerable<WhatsappDTO>> ListApps(int companyId, int clientId)
        {
            var sql = new SQL();
            try
            {

                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
                sql.AddParameter("@rbnClientId", DbType.Int32, ParameterDirection.Input, 0, clientId);


                return await sql.QueryAsync<WhatsappDTO>(LIST_APPS);
            }
            catch (Exception ex)
            {
                logging.LogError(ex, ex.Message);
                return null;
            }

        }
        public async Task<bool> UpdateEmbedLink(WhatsappDTO dto)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@embed_link", DbType.String, ParameterDirection.Input, 0, dto.Embed_Link);
                sql.AddParameter("@app_Id", DbType.String, ParameterDirection.Input, 0, dto.App_Id);


                return (await sql.ExecuteNonQueryAsync(UPDATE_EMBED_LINK)) > 0;
            }
            catch (Exception ex)
            {
                logging.LogError(ex, ex.Message);
                return false;
            }

        }

        public async Task<bool> SetCallback(WhatsappDTO dto)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@callback_url", DbType.String, ParameterDirection.Input, 0, "success");
                sql.AddParameter("@app_Id", DbType.String, ParameterDirection.Input, 0, dto.App_Id);


                return (await sql.ExecuteNonQueryAsync(SET_CALLBACK)) > 0;
            }
            catch (Exception ex)
            {
                logging.LogError(ex, ex.Message);
                return false;
            }

        }


        public async Task<bool> SavePartnerToken(GupShupToken dto)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@token", DbType.String, ParameterDirection.Input, 0, dto.Token);
                sql.AddParameter("@createdOn", DbType.DateTime, ParameterDirection.Input, 0, dto.CreatedOn);
                sql.AddParameter("@validTill", DbType.DateTime, ParameterDirection.Input, 0, dto.ValidTill);
                sql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, dto.GuId);


                return (await sql.ExecuteNonQueryAsync(SAVE_PARTNER_TOKEN)) > 0;
            }
            catch (Exception ex)
            {
                logging.LogError(ex, ex.Message);
                return false;
            }

        }
        public async Task<GupShupToken> GetPartnerToken(DateTime _validity)
        {
            var sql = new SQL();
            try
            {

                sql.AddParameter("@validTill", DbType.DateTime, ParameterDirection.Input, 0, _validity);


                return await sql.QueryFirstAsync<GupShupToken>(GET_VALID_PARTNER_TOKEN);
            }
            catch (Exception ex)
            {
                logging.LogError(ex, ex.Message);
                return null;
            }

        }
        public async Task<bool> UpdateAppSubscription(WhatsappDTO dto)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@subscription", DbType.Boolean, ParameterDirection.Input, 0, dto.SubscriptionEnabled);
                sql.AddParameter("@app_Id", DbType.String, ParameterDirection.Input, 0, dto.App_Id);


                return (await sql.ExecuteNonQueryAsync(UPDATE_APP_TOKEN)) > 0;
            }
            catch (Exception ex)
            {
                logging.LogError(ex, ex.Message);
                return false;
            }

        }

        #region procedures
        const string CREATE_APP = "p_whatsapp_ins";
        const string LIST_APPS = "p_whatsapp_list";

        const string UPDATE_APP_DETAILS = "p_whatsapp_updGupshupAppDetails";
        const string UPDATE_EMBED_LINK = "p_whatsapp_updSignedLink";
        const string SET_CALLBACK = "p_whatsapp_setCallback";
        const string SAVE_PARTNER_TOKEN = "p_gupshupTokens_ins";
        const string GET_VALID_PARTNER_TOKEN = "p_gupshupToken_selectValidToken";
        const string UPDATE_APP_TOKEN = "p_whatsapp_updToken";

        #endregion
    }
}
