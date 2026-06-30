using BAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services.Contracts
{
    public interface IDBBackupService
    {
        Task<ExportResponse> ExportViewsToAccessAsync(ExportRequest request);
        Task<List<ViewInfo>> GetAvailableViewsAsync();
        Task<bool> TestSqlServerConnectionAsync();
        Task<bool> TestAccessConnectionAsync(string accessFilePath);
        Task<byte[]> DownloadBackupFileAsync(string accessFilePath);
        Task<IEnumerable<BackupLog>> GetBackupHistoryAsync(BackupLog dto);

          void setConnectionString(string connectionString);
    }
}
