
using Dapper;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentac.DbBackup
{
    public class DbBackupRepository
    {
        IConfiguration _configuration;
        private readonly string connectionString;
        public DbBackupRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("sqlCon");
        }
        Logger _logger = LogManager.GetCurrentClassLogger();
        public async Task<int> Create(BackupLog log)
        {
            try
            {
                using (var sql = new SqlConnection(connectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@companyId", log.CompanyId, DbType.Int32);
                    parameters.Add("@createdBy", log.RequestedBy, DbType.String);
                    parameters.Add("@createdOn", log.CreatedOn, DbType.DateTime);
                    parameters.Add("@startedOn", log.StartTime, DbType.DateTime);
                    parameters.Add("@status", log.Status, DbType.String);
                    parameters.Add("@fileName", log.BackupFileName, DbType.String);
                    parameters.Add("@guId", log.GuId, DbType.String);
                    return await sql.ExecuteAsync("p_backuplogs_ins", parameters);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error while creating backuplog{ex.Message}", ex);
                throw new Exception($"Error while creating backuplog{ex.Message}", ex);
            }
        }


        public async Task<int> UpdateProgress(BackupLog log)
        {
            try
            {
                using (var sql = new SqlConnection(connectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@status", log.Status, DbType.String);

                    if (log.EndTime.HasValue)
                    {

                        parameters.Add("@completedOn", log.EndTime, DbType.DateTime);
                    }

                    parameters.Add("@progress", log.Progress, DbType.Int32);
                    parameters.Add("@guId", log.GuId, DbType.String);

                    return await sql.ExecuteAsync("p_backuplogs_updProgress", parameters);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error while updating backuplog{ex.Message}", ex);
                throw new Exception($"Error while updating backuplog{ex.Message}", ex);
            }
        }
    }
}
