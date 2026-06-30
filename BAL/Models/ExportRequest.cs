using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Models
{
    public class ExportRequest
    {
        public List<string> ViewNames { get; set; }
        public string AccessFilePath { get; set; }
        public bool CreateNewDatabase { get; set; } = false;
        public bool OverwriteExistingTables { get; set; } = true;
        public int? BatchSize { get; set; } = 500;
        public int CompanyId { get; set; }
        public string BackupCreatedBy { get; set; }
        public string GuId { get; set; }
    }
    public class ExportResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<ViewExportResult> Results { get; set; }
        public DateTime ExportTime { get; set; }
        public string AccessFilePath { get; set; } = string.Empty;
        public long FileSizeInBytes { get; set; }
    }

    public class ViewExportResult
    {
        public string ViewName { get; set; } = string.Empty;
        public string AccessTableName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public int RowsExported { get; set; }
        public TimeSpan ExportDuration { get; set; }
    }
    public class ViewInfo
    {
        public string ViewName { get; set; } = string.Empty;
        public string SchemaName { get; set; } = string.Empty;
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int? ColumnCount { get; set; }
    }
    public class BackupLog
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public decimal Progress { get; set; }
        public DateTime StartTime { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; } = string.Empty; // Success, Failed, Partial
        public string SourceDatabase { get; set; } = string.Empty;
        public string AccessFilePath { get; set; } = string.Empty;
        public string BackupFileName { get; set; }
        public int TotalViews { get; set; }
       
        public int SuccessfulExports { get; set; }
        public int FailedExports { get; set; }
        public long TotalRowsExported { get; set; }
        public string ErrorMessage { get; set; }
        public string RequestedBy { get; set; }
        public string MachineName { get; set; }
        public string GuId { get; set; }
    }
}
