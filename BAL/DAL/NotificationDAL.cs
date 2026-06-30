using BAL.DTO;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using BAL.Services;
using Newtonsoft.Json;
using System.Data;
using Org.BouncyCastle.Crypto.Prng;
//using Razorpay.Api;
using iTextSharp.text;
using Org.BouncyCastle.Utilities.Date;
using BAL.Objects;
namespace BAL.DAL
{
    public class NotificationDAL
    {
        LoggingService logger = new LoggingService();
        public async Task<bool> Add(NotificationDto dto)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@type", DbType.String, ParameterDirection.Input, 0, dto.Type);
                sql.AddParameter("@body", DbType.String, ParameterDirection.Input, 0, dto.Body);
                sql.AddParameter("@receipients", DbType.String, ParameterDirection.Input, 0, dto.Receipients);
                sql.AddParameter("@copyTo", DbType.String, ParameterDirection.Input, 0, dto.CopyTo);
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                sql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, dto.GuId);
                sql.AddParameter("@attachment", DbType.String, ParameterDirection.Input, 0, dto.Attachment);
                sql.AddParameter("@sender", DbType.String, ParameterDirection.Input, 0, dto.Sender);
                sql.AddParameter("@subject", DbType.String, ParameterDirection.Input, 0, dto.Subject);
                sql.AddParameter("@category", DbType.String, ParameterDirection.Input, 0, dto.Category);
                sql.AddParameter("@createdOn", DbType.DateTime, ParameterDirection.Input, 0, dto.CreatedOn);
                sql.AddParameter("@createdBy", DbType.Int32, ParameterDirection.Input, 0, dto.CreatedBy);

                var result = await sql.ExecuteNonQueryAsync(ADD);
                return result > 0;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<bool> StatusUpdate(NotificationDto dto)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, dto.GuId);
                sql.AddParameter("@status", DbType.String, ParameterDirection.Input, 0, dto.Status);
                sql.AddParameter("@DeliveryMessage", DbType.String, ParameterDirection.Input, 0, dto.DeliveryMessage);
                sql.AddParameter("@updatedDate", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedOn);
                var result = await sql.ExecuteNonQueryAsync(UPDATE_STATUS);
                return result > 0;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<bool> StatusUpdateByGsId(NotificationDto dto)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@gsId", DbType.String, ParameterDirection.Input, 0, dto.gsId);
                sql.AddParameter("@status", DbType.String, ParameterDirection.Input, 0, dto.Status);
                sql.AddParameter("@wsId", DbType.String, ParameterDirection.Input, 0, dto.wsId);

                sql.AddParameter("@DeliveryMessage", DbType.String, ParameterDirection.Input, 0, dto.DeliveryMessage);
                sql.AddParameter("@updatedDate", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedOn);
                var result = await sql.ExecuteNonQueryAsync(UPDATE_STATUS_BY_GSID);
                return result > 0;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return false;
            }
        }
        public async Task<NotificationTemplate> GetTemplateByCode(int code, int companyId)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@code", DbType.String, ParameterDirection.Input, 0, code);
                sql.AddParameter("@companyId", DbType.String, ParameterDirection.Input, 0, companyId);

                var list = await sql.QueryAsync<NotificationTemplate>(GET_TEMPLATE_BYCODE);
                if (list != null)
                {
                    return list.FirstOrDefault();
                }
                return null;


            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return null;
            }
        }
        public async Task<bool> UpdateGSId(NotificationDto dto)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, dto.GuId);
                sql.AddParameter("@wsAppId", DbType.String, ParameterDirection.Input, 0, dto.wsAppId);
                sql.AddParameter("@gsId", DbType.String, ParameterDirection.Input, 0, dto.gsId);

                var result = await sql.ExecuteNonQueryAsync(UPDATES_GUPSHUP_ID);
                return result > 0;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<IEnumerable<NotificationDto>> GetMyAlerts(NotificationFilterDTO dto)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@userId", DbType.Int32, ParameterDirection.Input, 0, dto.ReceipientUserId);
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                sql.AddParameter("@from", DbType.DateTime, ParameterDirection.Input, 0, dto.From);
                sql.AddParameter("@to", DbType.DateTime, ParameterDirection.Input, 0, dto.To);
                sql.AddParameter("@status", DbType.String, ParameterDirection.Input, 0, dto.Status);


                return await sql.QueryAsync<NotificationDto>(GET_MY_ALERTS);


            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<bool> CollectContractReminders()
        {
            var sql = new SQL();
            try
            {


                var contracts = await sql.QueryAsync<ContractViewDto>(PENDING_DELAYED_CONTRACTS_REMINDERS);
                foreach (var ct in contracts)
                {
                    var notification = new NotificationDto();



                    if (ct.ActivityStatus.ToLower() == "pending")
                    {
                        notification.Body = notification.Subject = $"{ct.Activity}  of the party {ct.Client}  is due on {Utils.FormatDate(ct.ActivityDate)}";
                    }
                    if (ct.ActivityStatus.ToLower() == "delayed")
                    {
                        notification.Body = notification.Subject = $"{ct.Activity} of the party {ct.Client}  was due on {Utils.FormatDate(ct.ActivityDate)}";
                    }
                    if (ct.ActivityStatus.ToLower() == "expired")
                    {
                        notification.Body = notification.Subject = $"Contract of the party {ct.Client} was due on {Utils.FormatDate(ct.ActivityDate)} has been expired.";
                    }
                    if (ct.ActivityStatus.ToLower() == "completed")
                    {
                        continue;
                    }
                    notification.Type = "al";
                    notification.Receipients = "NA";
                    notification.CompanyId = ct.CompanyId;
                    notification.Sender = "0";
                    notification.Category = "contracts";
                    notification.CreatedOn = DateTime.Now;
                    notification.CreatedBy = 0;
                    notification.GuId = Guid.NewGuid().ToString();

                    await this.Add(notification);

                }
                return true;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<WorkOrderDTO>> CollectPickkupReminders()
        {
            var sql = new SQL();
            try
            {


                 return await sql.QueryAsync<WorkOrderDTO>(PICKUP_REMIDNERS);
              
                

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        #region procs
        const string ADD = "p_notification_ins";
        const string UPDATE_STATUS = "p_notification_updStatus";
        const string UPDATE_STATUS_BY_GSID = "p_notification_updStatusByGsId";

        const string GET_TEMPLATE_BYCODE = "p_notificationTemplate_byCode";
        const string UPDATES_GUPSHUP_ID = "p_notification_updGsId";

        const string GET_MY_ALERTS = "p_getMyAlerts";

        const string PENDING_DELAYED_CONTRACTS_REMINDERS = "p_contracts_notcompleted_forAlerts";
        const string PICKUP_REMIDNERS = "p_pickupreminders";

        #endregion
    }
}
