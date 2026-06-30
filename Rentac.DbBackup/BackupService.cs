using Microsoft.Data.SqlClient;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SqlCommand = Microsoft.Data.SqlClient.SqlCommand;
using SqlConnection = Microsoft.Data.SqlClient.SqlConnection;

namespace Rentac.DbBackup
{



    /// <summary>
    /// High-performance SQL Server to MS Access exporter optimized for Azure VMs
    /// </summary>
    public class HighPerformanceExporter
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly ExporterConfiguration _config;
        private readonly ConcurrentBag<long> _batchTimes = new ConcurrentBag<long>();

        public class ExporterConfiguration
        {
            public string SqlConnectionString { get; set; }
            public string AccessConnectionString { get; set; }
            public string SourceTable { get; set; }
            public string DestinationTable { get; set; }
            public string Query { get; set; } // Optional: custom query instead of table
            public int BatchSize { get; set; } = 50000; // Optimized for Azure VM
            public int CommandTimeoutSeconds { get; set; } = 600; // 10 minutes
            public int MaxParallelism { get; set; } = 2; // Conservative for Azure VM
            public bool UseCompression { get; set; } = true; // For network optimization
            public bool EnableLogging { get; set; } = true;
            public string LogPath { get; set; } = "export_log.txt";
        }

        public HighPerformanceExporter(ExporterConfiguration config)
        {
            _config = config;

            if (_config.EnableLogging)
            {
                // Configure Serilog
                var serilogLogger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File(_config.LogPath,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,
                        fileSizeLimitBytes: 100 * 1024 * 1024) // 100MB max
                    .CreateLogger();

                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddSerilog(serilogLogger);
                    builder.AddConsole();
                });

                _logger = loggerFactory.CreateLogger<HighPerformanceExporter>();
            }
        }

    
        private class ColumnInfo
        {
            public string Name { get; set; }
            public Type DataType { get; set; }
            public int MaxLength { get; set; }
        }

        private class BatchResult
        {
            public int BatchNumber { get; set; }
            public int RowsExported { get; set; }
            public long ElapsedMs { get; set; }
            public double RowsPerSecond { get; set; }
        }

        public class ExportResult
        {
            public long TotalRows { get; set; }
            public long TotalRowsExported { get; set; }
            public int SuccessfulBatches { get; set; }
            public double ElapsedSeconds { get; set; }
            public double AverageRowsPerSecond { get; set; }
            public string Error { get; set; }
            public bool Success => string.IsNullOrEmpty(Error);
        }

    }
}
