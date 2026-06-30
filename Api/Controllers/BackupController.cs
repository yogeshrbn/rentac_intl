using Azure.Messaging.ServiceBus;
using BAL.Models;
using BAL.Services.Contracts;
using FarmaAPI.Helper;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace FarmaAPI.Controllers
{
    [System.Web.Http.Authorize]
    public class BackupController : BaseApiController
    {

        private readonly IDBBackupService _backupService;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public BackupController(
            IDBBackupService backupService
            )
        {
            _backupService = backupService;
        }

        [System.Web.Http.HttpPost]
        public async Task<ApiMessage> backupdb()
        {
            var res = new ApiMessage();
            try
            {
                // _logger.Info("Export request received for {ViewCount} views", request.ViewNames?.Count);
                var user = new LoggedInUser();
                var exportRequest = new ExportRequest();
                //exportRequest.ViewNames = new List<string>();
                //exportRequest.ViewNames.Add("vwbkp_products");
                //exportRequest.ViewNames.Add("vwbkp_party");
                //exportRequest.ViewNames.Add("vwbkp_delChallan");
                //exportRequest.ViewNames.Add("vwbkp_retChallan");
                //exportRequest.ViewNames.Add("vwbkp_bills");
                //exportRequest.ViewNames.Add("vwbkp_quotations");
                //exportRequest.ViewNames.Add("vwbkp_ledgertxns");
                //exportRequest.ViewNames.Add("vwbkp_ewayBills");
                //exportRequest.ViewNames.Add("vwbkp_contracts");

                // exportRequest.ViewNames.Add("vwbkp_partyRates");
                // exportRequest.ViewNames.Add("vwbkp_purchase");

                //var backupFile = HttpContext.Current.Server.MapPath("~/backup/rentac.accdb");

                //if (!File.Exists(backupFile))
                //{
                //    throw new Exception("backup media not found");
                //}
                ////var currentBackupFile = HttpContext.Current.Server.MapPath("~/backup/") +
                ////                        user.DefaultCompanyId.ToString() + DateTime.Now.ToString() + ".mdf";

                //var currentBackupFile = HttpContext.Current.Server.MapPath("~/backup/") + user.DefaultCompanyId.ToString() + $"Backup_{DateTime.Now:yyyyMMdd_HHmmss}.accdb";
                //File.Copy(backupFile, currentBackupFile);


                exportRequest.CompanyId = user.DefaultCompanyId;
                exportRequest.BackupCreatedBy = user.FullName;
                exportRequest.GuId = Guid.NewGuid().ToString();
                // Send CompanyId to Azure Service Bus backup queue instead of running backup inline
                var result = await _backupService.ExportViewsToAccessAsync(exportRequest);
                if (result.Success)
                {
                    var data = exportRequest.GuId + "," + exportRequest.CompanyId.ToString();
                    var (sent, error) = await SendBackupRequestToQueueAsync(data);

                    if (!sent)
                    {
                        res.Message = "Backup request not submitted to queue: " + error;
                        res.Code = ApiMessageCodes.ERROR;
                        return res;
                    }

                    res.Message = "Backup request submitted to queue successfully.";
                    res.Data = exportRequest.CompanyId;
                    res.Code = ApiMessageCodes.SUCCESS;
                }
                return res;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error processing export request");
                res.Message = "Backup request  not submitted.>" + ex.Message;
                res.Code = ApiMessageCodes.ERROR;
                return res;
            }
        }

        private async Task<(bool Sent, string Error)> SendBackupRequestToQueueAsync(object data)
        {
            var connectionString = ConfigurationManager.AppSettings["BackupServiceBusConnectionString"];
            var queueName = ConfigurationManager.AppSettings["BackupServiceBusQueueName"];
            if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(queueName))
            {
                return (false, "Service Bus configuration is missing.");
            }
            var client = new ServiceBusClient(connectionString);
            try
            {
                ServiceBusSender sender = client.CreateSender(queueName);
                var messageBody = data.ToString();
                var message = new ServiceBusMessage(messageBody)
                {
                    ContentType = "text/plain"
                };
                await sender.SendMessageAsync(message);
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error sending backup request to Service Bus.");
                return (false, ex.Message);
            }
            finally
            {
                await client.DisposeAsync();
            }

        }

        [System.Web.Http.HttpPost]
        public async Task<ApiMessage> list()
        {
            var res = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                res.Data = await _backupService.GetBackupHistoryAsync(new BackupLog
                {
                    CompanyId = user.DefaultCompanyId
                });
                res.Code = ApiMessageCodes.SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.ErrorCode = 500;
                res.Code = ApiMessageCodes.ERROR;
                res.Message += ex.Message;
                return res;
            }
        }


        public IHttpActionResult downloadBackup([FromBody] BackupLog backupLog)
        {
            try
            {
                if (backupLog == null)
                {
                    throw new Exception("Incorrect or empty input");
                }
                var root = HttpContext.Current.Server.MapPath("~/backup/");
                if (String.IsNullOrEmpty(backupLog.BackupFileName))
                {
                    throw new Exception("Incorrect or invalid backup file name");
                }
                var user = new LoggedInUser();
                if (!backupLog.BackupFileName.Contains(user.DefaultCompanyId.ToString()))
                {
                    throw new Exception("Incorrect or invalid backup file name for the current selected company");
                }
                var fileName = root + backupLog.BackupFileName;
                if (!File.Exists(fileName))
                {
                    throw new Exception("Backup file not found");
                }

                byte[] fileBytes = File.ReadAllBytes(fileName);
                return new FileHttpActionResult(fileBytes, fileName, "application/zip");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw ex;
            }
        }

    }
}
