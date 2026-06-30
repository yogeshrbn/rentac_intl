using Microsoft.Data.SqlClient;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Text;

namespace SqlToAccessExporter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== SQL Server to MS Access Exporter ===");
            Console.WriteLine();

            try
            {
                // Shared configuration values
                var sqlConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=rentac-live;User ID=sa;Password=password_1234;Encrypt=false";
                var accessDbPath = @"D:\projects\rentac.accdb";
                var batchSize = 50;

                // List of tables/views to backup
                var exportConfigs = new[]
                {
                    // Original backup with specific columns and filter
                    new ExportConfiguration
                    {
                        SqlServerConnectionString = sqlConnectionString,
                        AccessDbPath = accessDbPath,
                        SourceTableName = "vwbkp_delChallan",
                        DestinationTableName = "vwbkp_delChallan",
                        BatchSize = batchSize,
                        WhereClause = "CompanyId = 1111",
                        Columns = new[] { "Party", "PartyCode" }
                    },
                    // Additional backups (all columns, no filter)
                    new ExportConfiguration
                    {
                        SqlServerConnectionString = sqlConnectionString,
                        AccessDbPath = accessDbPath,
                        SourceTableName = "vwbkp_products",
                        DestinationTableName = "vwbkp_products",
                        BatchSize = batchSize
                    },
                    new ExportConfiguration
                    {
                        SqlServerConnectionString = sqlConnectionString,
                        AccessDbPath = accessDbPath,
                        SourceTableName = "vwbkp_party",
                        DestinationTableName = "vwbkp_party",
                        BatchSize = batchSize
                    },
                    new ExportConfiguration
                    {
                        SqlServerConnectionString = sqlConnectionString,
                        AccessDbPath = accessDbPath,
                        SourceTableName = "vwbkp_partyRates",
                        DestinationTableName = "vwbkp_partyRates",
                        BatchSize = batchSize
                    },
                    new ExportConfiguration
                    {
                        SqlServerConnectionString = sqlConnectionString,
                        AccessDbPath = accessDbPath,
                        SourceTableName = "vwbkp_retChallan",
                        DestinationTableName = "vwbkp_retChallan",
                        BatchSize = batchSize
                    },
                    new ExportConfiguration
                    {
                        SqlServerConnectionString = sqlConnectionString,
                        AccessDbPath = accessDbPath,
                        SourceTableName = "vwbkp_quotations",
                        DestinationTableName = "vwbkp_quotations",
                        BatchSize = batchSize
                    },
                    new ExportConfiguration
                    {
                        SqlServerConnectionString = sqlConnectionString,
                        AccessDbPath = accessDbPath,
                        SourceTableName = "vwbkp_ledgertxns",
                        DestinationTableName = "vwbkp_ledgertxns",
                        BatchSize = batchSize
                    },
                    new ExportConfiguration
                    {
                        SqlServerConnectionString = sqlConnectionString,
                        AccessDbPath = accessDbPath,
                        SourceTableName = "vwbkp_bills",
                        DestinationTableName = "vwbkp_bills",
                        BatchSize = batchSize
                    },
                    // Duplicate name in your list; kept as-is in case you intended a second run
                    new ExportConfiguration
                    {
                        SqlServerConnectionString = sqlConnectionString,
                        AccessDbPath = accessDbPath,
                        SourceTableName = "vwbkp_ledgertxns",
                        DestinationTableName = "vwbkp_ledgertxns",
                        BatchSize = batchSize
                    }
                };

                foreach (var config in exportConfigs)
                {
                    Console.WriteLine();
                    Console.WriteLine($"=== Starting export for: {config.SourceTableName} ===");
                    Console.WriteLine($"Target Access DB: {config.AccessDbPath}");
                    Console.WriteLine($"Batch size: {config.BatchSize}");
                    Console.WriteLine();

                    var exporter = new SqlToAccessExporter(config);

                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    int totalRows = await exporter.ExportAsync();
                    stopwatch.Stop();

                    Console.WriteLine();
                    Console.WriteLine($"Export for '{config.SourceTableName}' completed successfully!");
                    Console.WriteLine($"Total rows exported: {totalRows:N0}");
                    Console.WriteLine($"Time taken: {stopwatch.Elapsed:mm\\:ss\\.ff} minutes");
                    Console.WriteLine($"Average speed: {totalRows / stopwatch.Elapsed.TotalSeconds:N0} rows/second");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Export failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Console.ResetColor();

                // Force cleanup of COM-based OleDb objects before process exit
                GC.Collect();
                GC.WaitForPendingFinalizers();

                Environment.Exit(1);
            }

            // Ensure all COM interop objects are finalized before process shutdown
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    public class ExportConfiguration
    {
        public required string SqlServerConnectionString { get; set; }
        public required string AccessDbPath { get; set; }
        public required string SourceTableName { get; set; }
        public required string DestinationTableName { get; set; }
        public int BatchSize { get; set; } = 1000;
        public string[]? Columns { get; set; }
        public string? WhereClause { get; set; }
    }

    public class SqlToAccessExporter
    {
        private readonly ExportConfiguration _config;
        private readonly IProgress<ExportProgress> _progress;
        private string? _resolvedColumns;

        public SqlToAccessExporter(ExportConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _progress = new Progress<ExportProgress>(progress =>
            {
                Console.WriteLine($"Progress: {progress.Percentage:F1}% - {progress.RowsProcessed:N0} of {progress.TotalRows:N0} rows exported");
            });
        }

        public async Task<int> ExportAsync()
        {
            ValidateAccessDatabase();

            // Use ACE 2016+ provider (Microsoft.ACE.OLEDB.16.0) to match the installed Access Database Engine.
            // Make sure your project Platform target (x86/x64) matches the installed provider bitness.
            string accessConnectionString = $@"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={_config.AccessDbPath};Persist Security Info=False;";

            using (var sqlConnection = new SqlConnection(_config.SqlServerConnectionString))
            {
                await sqlConnection.OpenAsync();

                // Get total row count for progress tracking
                int totalRows = await GetTotalRowCountAsync(sqlConnection);
                Console.WriteLine($"Total rows to export: {totalRows:N0}");

                string selectSql = BuildSelectSql();

                using (var sqlCommand = new SqlCommand(selectSql, sqlConnection))
                {
                    sqlCommand.CommandTimeout = 300; // 5 minutes timeout

                    using (var reader = await sqlCommand.ExecuteReaderAsync())
                    {
                        using (var accessConnection = new OleDbConnection(accessConnectionString))
                        {
                            await accessConnection.OpenAsync();

                            // Verify destination table exists and matches schema
                            await EnsureDestinationTableExistsAsync(accessConnection, reader);

                            int rowsExported = 0;
                            var batch = new StringBuilder();
                            var batchValues = new List<string>();

                            // Start transaction for better performance
                            using (var transaction = accessConnection.BeginTransaction())
                            {
                                var accessCommand = new OleDbCommand();
                                accessCommand.Connection = accessConnection;
                                accessCommand.Transaction = transaction;
                                accessCommand.CommandTimeout = 120;

                                try
                                {
                                    while (await reader.ReadAsync())
                                    {
                                        // Build the VALUES part for this row
                                        string rowValues = BuildRowValues(reader);
                                        batchValues.Add(rowValues);

                                        rowsExported++;

                                        // Execute batch when it reaches the batch size
                                        if (rowsExported % _config.BatchSize == 0)
                                        {
                                            await ExecuteBatchAsync(accessCommand, batchValues);
                                            batchValues.Clear();

                                            // Report progress
                                            var progress = new ExportProgress
                                            {
                                                TotalRows = totalRows,
                                                RowsProcessed = rowsExported,
                                                Percentage = (double)rowsExported / totalRows * 100
                                            };
                                            (_progress as IProgress<ExportProgress>)?.Report(progress);
                                        }
                                    }

                                    // Execute final batch
                                    if (batchValues.Any())
                                    {
                                        await ExecuteBatchAsync(accessCommand, batchValues);
                                    }

                                    transaction.Commit();

                                    // Final progress report
                                    var finalProgress = new ExportProgress
                                    {
                                        TotalRows = totalRows,
                                        RowsProcessed = rowsExported,
                                        Percentage = 100
                                    };
                                    (_progress as IProgress<ExportProgress>)?.Report(finalProgress);

                                    return rowsExported;
                                }
                                catch (Exception)
                                {
                                    transaction.Rollback();
                                    throw;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ValidateAccessDatabase()
        {
            if (!File.Exists(_config.AccessDbPath))
            {
                throw new FileNotFoundException($"Access database not found at: {_config.AccessDbPath}");
            }
        }

        private string BuildSelectSql()
        {
            string columns = _config.Columns != null && _config.Columns.Any()
                ? string.Join(", ", _config.Columns)
                : "*";

            string sql = $"SELECT {columns} FROM {_config.SourceTableName}";

            if (!string.IsNullOrWhiteSpace(_config.WhereClause))
            {
                sql += $" WHERE {_config.WhereClause}";
            }

            return sql;
        }

        private async Task<int> GetTotalRowCountAsync(SqlConnection connection)
        {
            string countSql = $"SELECT COUNT(*) FROM {_config.SourceTableName}";
            if (!string.IsNullOrWhiteSpace(_config.WhereClause))
            {
                countSql += $" WHERE {_config.WhereClause}";
            }

            using (var command = new SqlCommand(countSql, connection))
            {
                return (int)await command.ExecuteScalarAsync();
            }
        }

        private string BuildRowValues(SqlDataReader reader)
        {
            var values = new List<string>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.IsDBNull(i))
                {
                    values.Add("NULL");
                }
                else
                {
                    Type fieldType = reader.GetFieldType(i);
                    string value = reader[i].ToString() ?? "";

                    if (fieldType == typeof(string) || fieldType == typeof(DateTime) || fieldType == typeof(Guid))
                    {
                        // Escape single quotes by replacing ' with ''
                        string escapedValue = value.Replace("'", "''");
                        values.Add($"'{escapedValue}'");
                    }
                    else if (fieldType == typeof(bool))
                    {
                        values.Add((bool)reader[i] ? "1" : "0");
                    }
                    else if (fieldType == typeof(byte[]))
                    {
                        // Handle binary data - convert to hex string
                        byte[] bytes = (byte[])reader[i];
                        values.Add($"0x{BitConverter.ToString(bytes).Replace("-", "")}");
                    }
                    else
                    {
                        values.Add(value);
                    }
                }
            }

            return $"({string.Join(", ", values)})";
        }

        private async Task ExecuteBatchAsync(OleDbCommand command, List<string> batchValues)
        {
            if (!batchValues.Any()) return;

            string columns = _config.Columns != null && _config.Columns.Any()
                ? string.Join(", ", _config.Columns)
                : _resolvedColumns ?? throw new InvalidOperationException("Destination columns could not be resolved.");

            // ACE/OleDb does not support multi-row VALUES inserts in a single statement.
            // Execute one INSERT per row within the existing transaction for reliability.
            int insertedCount = 0;
            foreach (var rowValues in batchValues)
            {
                string sql = $"INSERT INTO {_config.DestinationTableName} ({columns}) VALUES {rowValues}";
                command.CommandText = sql;
                await command.ExecuteNonQueryAsync();
                insertedCount++;
            }

            Console.WriteLine($"Batch of {insertedCount} records inserted successfully.");
        }

        private async Task EnsureDestinationTableExistsAsync(OleDbConnection connection, SqlDataReader reader)
        {
            // Always recreate the destination table based on the SQL Server schema.
            // If it exists, drop it first, then create a new one.
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = $"DROP TABLE [{_config.DestinationTableName}]";
                try
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine($"Existing table '{_config.DestinationTableName}' dropped.");
                }
                catch (OleDbException)
                {
                    // Ignore if table does not exist
                }
            }

            string createSql = BuildCreateTableSql(reader);
            using (var cmdCreate = connection.CreateCommand())
            {
                cmdCreate.CommandText = createSql;
                cmdCreate.ExecuteNonQuery();
            }

            Console.WriteLine($"Table '{_config.DestinationTableName}' created/refreshed in Access.");
        }

        private string BuildCreateTableSql(SqlDataReader reader)
        {
            var schemaTable = reader.GetSchemaTable()
                ?? throw new InvalidOperationException("Unable to read schema from SQL Server reader.");

            var columnDefs = new List<string>();
            var columnNames = new List<string>();

            foreach (DataRow row in schemaTable.Rows)
            {
                string columnName = (string)row["ColumnName"];
                Type dataType = (Type)row["DataType"];
                int columnSize = row["ColumnSize"] is int size ? size : 0;
                short numericPrecision = row["NumericPrecision"] is short prec ? prec : (short)0;
                short numericScale = row["NumericScale"] is short scale ? scale : (short)0;
                bool allowDBNull = row["AllowDBNull"] is bool isNullable && isNullable;

                string accessType = MapToAccessType(dataType, columnSize, numericPrecision, numericScale);
                string nullClause = allowDBNull ? "" : " NOT NULL";

                string escapedName = $"[{columnName}]";
                columnNames.Add(escapedName);
                columnDefs.Add($"{escapedName} {accessType}{nullClause}");
            }

            // Cache resolved column list for inserts when Columns is not specified
            if (_config.Columns == null || !_config.Columns.Any())
            {
                _resolvedColumns = string.Join(", ", columnNames);
            }

            string columnsSql = string.Join(", ", columnDefs);
            return $"CREATE TABLE [{_config.DestinationTableName}] ({columnsSql})";
        }

        private string MapToAccessType(Type dataType, int columnSize, short numericPrecision, short numericScale)
        {
            if (dataType == typeof(string))
            {
                // Access TEXT is limited to 255 chars; beyond that use MEMO
                if (columnSize <= 0 || columnSize > 255)
                {
                    return "MEMO";
                }
                return $"TEXT({Math.Max(columnSize, 1)})";
            }

            if (dataType == typeof(int) || dataType == typeof(short))
            {
                return "INTEGER";
            }

            if (dataType == typeof(long))
            {
                return "BIGINT";
            }

            if (dataType == typeof(decimal))
            {
                var precision = numericPrecision > 0 ? numericPrecision : (short)18;
                var scale = numericScale >= 0 ? numericScale : (short)2;
                return $"DECIMAL({precision},{scale})";
            }

            if (dataType == typeof(double) || dataType == typeof(float))
            {
                return "DOUBLE";
            }

            if (dataType == typeof(DateTime))
            {
                return "DATETIME";
            }

            if (dataType == typeof(bool))
            {
                return "YESNO";
            }

            if (dataType == typeof(byte[]))
            {
                return "OLEOBJECT";
            }

            if (dataType == typeof(Guid))
            {
                return "GUID";
            }

            // Fallback to MEMO for unknown types
            return "MEMO";
        }
    }

    public class ExportProgress
    {
        public int TotalRows { get; set; }
        public int RowsProcessed { get; set; }
        public double Percentage { get; set; }
    }
}