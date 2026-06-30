using BAL.DAL;
using BAL.Data.Contracts;
using BAL.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Data.Repository
{
    public class DbBackupRepository : IDBBackupRespository
    {
        Logger _logger = LogManager.GetCurrentClassLogger();
        public async Task<int> Create(BackupLog log)
        {
            try
            {
                var sql = new SQL();
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, log.CompanyId);
                sql.AddParameter("@createdBy", DbType.String, ParameterDirection.Input, 0, log.RequestedBy);
                sql.AddParameter("@createdOn", DbType.DateTime, ParameterDirection.Input, 0, log.CreatedOn);
                sql.AddParameter("@startedOn", DbType.DateTime, ParameterDirection.Input, 0, log.StartTime);
                sql.AddParameter("@status", DbType.String, ParameterDirection.Input, 0, log.Status);
        
                sql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, log.GuId);
                return await sql.ExecuteNonQueryAsync("p_backuplogs_ins");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error while creating backuplog{ex.Message}", ex);
                throw new Exception($"Error while creating backuplog{ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<BackupLog>> List(BackupLog log)
        {
            try
            {
                var sql = new SQL();
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, log.CompanyId);
                return await sql.QueryAsync<BackupLog>("p_backuplogs_list");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error while fetching backuplog{ex.Message}", ex);
                throw new Exception($"Error while fetching backuplog{ex.Message}", ex);
            }
        }

        public async Task<int> UpdateProgress(BackupLog log)
        {
            try
            {
                var sql = new SQL();
                sql.AddParameter("@status", DbType.String, ParameterDirection.Input, 0, log.Status);
                if (log.EndTime.HasValue)
                {
                    sql.AddParameter("@completedOn", DbType.DateTime, ParameterDirection.Input, 0, log.EndTime);
                }
                sql.AddParameter("@progress", DbType.Int32, ParameterDirection.Input, 0, log.Progress);
                sql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, log.GuId);
                return await sql.ExecuteNonQueryAsync("p_backuplogs_updProgress");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error while updating backuplog{ex.Message}", ex);
                throw new Exception($"Error while updating backuplog{ex.Message}", ex);
            }
        }
    }
}
