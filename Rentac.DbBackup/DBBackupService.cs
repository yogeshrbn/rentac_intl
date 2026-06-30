
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentac.DbBackup
{
    public class SqlServerToAccessBackupService
    {
        //private readonly IConfiguration _configuration;
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private string _sqlConnectionString;
        private readonly DbBackupRepository _respository;
        public SqlServerToAccessBackupService(
            DbBackupRepository dbBackupRepository
            )
        {

            _respository = dbBackupRepository;
            //  _sqlConnectionString = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;

        }

        public void setConnectionString(string connectionString)
        {

            _sqlConnectionString = connectionString;
        }

        public async Task<ExportResponse> ExportViewsToAccessAsync(ExportRequest request, BackupLog backupLog)
        {
            var response = new ExportResponse
            {
                ExportTime = DateTime.UtcNow,
                Success = false
            };

            //BackupLog backupLog = null;

            try
            {
                // Create backup log
                //backupLog = new BackupLog
                //{
                //    StartTime = DateTime.UtcNow,
                //    Status = "InProgress",
                //    SourceDatabase = GetDatabaseNameFromConnectionString(),
                //    TotalViews = request.ViewNames.Count,
                //    RequestedBy = request.BackupCreatedBy,
                //    MachineName = Environment.MachineName,
                //    CompanyId = request.CompanyId,
                //    CreatedOn = DateTime.Now,
                //    Progress = 0,
                //    GuId = Guid.NewGuid().ToString()
                //};

                //  Validate request
                if (request.ViewNames == null || !request.ViewNames.Any())
                {
                    throw new ArgumentException("At least one view name must be specified");
                }

                // Get or create Access file path
                string accessFilePath = GetAccessFilePath(request.AccessFilePath);
                response.AccessFilePath = accessFilePath;

                // Create Access database if needed
                //if (request.CreateNewDatabase && !File.Exists(accessFilePath))
                //{
                //    await CreateAccessDatabaseAsync(accessFilePath);
                //    _logger.Info("Created new Access database at: {FilePath}", accessFilePath);
                //}
                if (request.CompanyId == 0)
                {
                    throw new FileNotFoundException($"Invalid companyId: {request.CompanyId.ToString()}");
                }
                if (!File.Exists(accessFilePath))
                {
                    throw new FileNotFoundException($"Access database not found at: {accessFilePath}");
                }
                var finFo = new FileInfo(accessFilePath);

                string accessConnectionString = GetAccessConnectionString(accessFilePath);
                backupLog.AccessFilePath = accessFilePath;
                backupLog.BackupFileName = finFo.Name;

                // Test connections
                // await TestConnectionsAsync(_sqlConnectionString, accessConnectionString);

                var results = new List<ViewExportResult>();

                string viewName = request.ViewNames[0];

                //   var created = await _respository.Create(backupLog);
                //using (var accessConnection = new OleDbConnection(accessConnectionString))
                //{
                //    await accessConnection.OpenAsync();
                //    string viewName = request.ViewNames[0];
                //    //foreach (var viewName in request.ViewNames)
                //    //{
                //    var viewResult = await ExportSingleViewAsync(
                //        viewName,
                //        accessConnection,
                //        request.OverwriteExistingTables,
                //        request.BatchSize, request.CompanyId);

                //    accessConnection.Close();



                //    // var viewResult =await ExportViewStreamAsync(viewName,accessConnection,request.CompanyId)
                //    results.Add(viewResult);
                //    if (!viewResult.Success)
                //    {
                //        return response;
                //    }
                //    backupLog.Progress = (results.Count / Convert.ToDecimal(request.ViewNames.Count)) * 100;
                //    if (backupLog.Progress == 100)
                //    {
                //        backupLog.EndTime = DateTime.Now;
                //        backupLog.Status = "Completed";

                //    }
                //    var updated = await _respository.UpdateProgress(backupLog);
                //    //}
                //}
                string query = $"SELECT * FROM  [{viewName}] WHERE companyId = {request.CompanyId}";
                var exp = new export1();
                await exp.ExportToAccessAsync(_sqlConnectionString, accessConnectionString, viewName, query);

                // Update response
                response.Results = results;
                response.Success = results.All(r => r.Success);
                response.Message = response.Success
                    ? $"Successfully exported {results.Count} views to Access database"
                    : $"Partially exported {results.Count(r => r.Success)} of {results.Count} views";
                response.FileSizeInBytes = new FileInfo(accessFilePath).Length;

                // Update backup log
                //if (backupLog != null)
                //{
                //    backupLog.EndTime = DateTime.UtcNow;
                //    backupLog.Status = response.Success ? "Success" : "Partial";
                //    backupLog.SuccessfulExports = results.Count(r => r.Success);
                //    backupLog.FailedExports = results.Count(r => !r.Success);
                //    backupLog.TotalRowsExported = results.Sum(r => r.RowsExported);

                //    if (results.Any(r => !r.Success))
                //    {
                //        backupLog.ErrorMessage = string.Join("; ",
                //            results.Where(r => !r.Success)
                //                   .Select(r => $"{r.ViewName}: {r.ErrorMessage}"));
                //    }
                //}

                _logger.Info("Export completed: {Message}", response.Message);
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error exporting views to Access");

                if (backupLog != null)
                {
                    backupLog.EndTime = DateTime.UtcNow;
                    backupLog.Status = "Failed";
                    backupLog.ErrorMessage = ex.Message;
                }
                var updated = await _respository.UpdateProgress(backupLog);
                response.Success = false;
                response.Message = $"Export failed: {ex.Message}";
                return response;
            }
            finally
            {
                // Log backup information (in production, save to database)
                if (backupLog != null)
                {
                    LogBackupInfo(backupLog);
                }
            }
        }


        private string MapToAccessDataType(Type dataType)
        {
            if (dataType == typeof(int))
                return "INTEGER";
            if (dataType == typeof(long))
                return "BIGINT";
            if (dataType == typeof(short))
                return "SMALLINT";
            if (dataType == typeof(decimal))
                return "DOUBLE";
            if (dataType == typeof(double))
                return "DOUBLE";
            if (dataType == typeof(float))
                return "SINGLE";
            if (dataType == typeof(DateTime))
                return "DATETIME";
            if (dataType == typeof(bool))
                return "BIT";
            if (dataType == typeof(Guid))
                return "GUID";
            if (dataType == typeof(byte[]))
                return "LONGBINARY";

            // For string types, determine appropriate length
            if (dataType == typeof(string))
                return "MEMO"; // Default to 255, could be customized

            // For nullable types, get underlying type
            if (dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return MapToAccessDataType(Nullable.GetUnderlyingType(dataType));
            }

            return "MEMO"; // Default fallback
        }

        private string GetAccessFilePath(string requestedPath)
        {
            if (!string.IsNullOrEmpty(requestedPath))
                return requestedPath;

            // Use default path from configuration


            // Generate default path in temp directory
            var fileName = $"Backup_{DateTime.Now:yyyyMMdd_HHmmss}.accdb";
            return Path.Combine(Path.GetTempPath(), "SqlServerBackups", fileName);
        }

        private string GetAccessConnectionString(string filePath)
        {
            return $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Persist Security Info=False;";
        }


        public string GetDatabaseNameFromConnectionString()
        {
            var builder = new SqlConnectionStringBuilder(_sqlConnectionString);
            return builder.InitialCatalog;
        }

        private void LogBackupInfo(BackupLog backupLog)
        {
            // In production, save to database or log file
            _logger.Info(
                "Backup Log: Status={Status}, Views={Total}/{Successful}, Rows={Rows}, Duration={Duration}s",
                backupLog.Status,
                backupLog.TotalViews,
                backupLog.SuccessfulExports,
                backupLog.TotalRowsExported,
                backupLog.EndTime.HasValue
                    ? (backupLog.EndTime.Value - backupLog.StartTime).TotalSeconds
                    : 0);
        }


        private async Task<int?> GetViewColumnCountAsync(string schema, string viewName)
        {
            try
            {
                using (var connection = new SqlConnection(_sqlConnectionString))
                {
                    await connection.OpenAsync();

                    string query = $@"
                    SELECT COUNT(*) as ColumnCount
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = @Schema AND TABLE_NAME = @ViewName";

                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Schema", schema);
                    command.Parameters.AddWithValue("@ViewName", viewName);

                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> TestSqlServerConnectionAsync()
        {
            try
            {
                var connection = new SqlConnection(_sqlConnectionString);
                await connection.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> TestAccessConnectionAsync(string accessFilePath)
        {
            try
            {
                var connectionString = GetAccessConnectionString(accessFilePath);
                var connection = new OleDbConnection(connectionString);
                await connection.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<byte[]> DownloadBackupFileAsync(string accessFilePath)
        {
            if (!File.Exists(accessFilePath))
            {
                throw new FileNotFoundException($"Access file not found: {accessFilePath}");
            }

            return await Task.Run(() => File.ReadAllBytes(accessFilePath));
        }


    }

    public class export1
    {
        public async Task<int> ExportToAccessAsync(
    string sqlConnectionString,
    string accessConnectionString,
    string tableName,
    string query,
    CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query cannot be empty", nameof(query));

            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name cannot be empty", nameof(tableName));

            DataTable dataTable = null;
            int rowsInserted = 0;

            try
            {
                // 1. Get data from SQL Server with better memory management
                dataTable = new DataTable();

                using (var sqlConn = new SqlConnection(sqlConnectionString))
                {
                    await sqlConn.OpenAsync(cancellationToken);

                    // Set command timeout for large queries (5 minutes)
                    using (var cmd = new SqlCommand(query, sqlConn))
                    {
                        cmd.CommandTimeout = 300;

                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            // Optional: Set batch size for large datasets
                            adapter.RowUpdated += OnRowUpdated; // For progress tracking

                            // Fill the DataTable
                            adapter.Fill(dataTable);
                        }
                    }
                }

                if (dataTable.Rows.Count == 0)
                {
                    Console.WriteLine("No data to export.");
                    return 0;
                }

                // 2. Write to Access with proper transaction handling
                rowsInserted = await WriteToAccessAsync(
                    accessConnectionString,
                    tableName,
                    dataTable,
                    cancellationToken);

                Console.WriteLine($"Successfully exported {rowsInserted} rows to {tableName}");
                return rowsInserted;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Export failed: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw; // Re-throw if caller needs to handle it
            }
            finally
            {
                dataTable?.Dispose(); // Clean up
            }
        }

        private async Task<int> WriteToAccessAsync(
            string accessConnectionString,
            string tableName,
            DataTable dataTable,
            CancellationToken cancellationToken)
        {


            // Use smaller batch size for Access (it's not as robust as SQL Server)
            const int batchSize = 10;
            int totalInserted = 0;

            try
            {
                using (var accessConn = new OleDbConnection(accessConnectionString))
                {
                    await accessConn.OpenAsync(cancellationToken);
                    await DropTableIfExistsAsync(accessConn, tableName, cancellationToken);
                    await CreateAccessTableAsync(accessConn, tableName, dataTable, cancellationToken);
                }

                // Process in batches to avoid memory pressure and transaction log issues
                for (int i = 0; i < dataTable.Rows.Count; i += batchSize)
                {
                    using (var accessConn = new OleDbConnection(accessConnectionString))
                    {
                        await accessConn.OpenAsync(cancellationToken);

                        // Drop and recreate table

                        // Create batch DataTable
                        var batchTable = dataTable.Clone(); // Copy schema only

                        int endIndex = Math.Min(i + batchSize, dataTable.Rows.Count);
                        for (int j = i; j < endIndex; j++)
                        {
                            batchTable.ImportRow(dataTable.Rows[j]);
                        }
                        using (var transaction = accessConn.BeginTransaction())
                        {
                            // Insert batch
                            int batchRows = await InsertBatchToAccessAsync(
                                accessConn,
                                tableName,
                                batchTable, transaction,
                                cancellationToken);

                            transaction.Commit();
                        }
                        totalInserted = i + batchSize;
                        // Force garbage collection after each batch
                    }

                    // Progress reporting
                    Console.WriteLine($"Progress: {totalInserted}/{dataTable.Rows.Count} rows inserted");

                    // Optional: Small delay to let Access breathe
                    if (i + batchSize < dataTable.Rows.Count)
                        await Task.Delay(500, cancellationToken);

                }

            }
            catch (Exception ex)
            {

            }
            return totalInserted;



        }

        private async Task<int> InsertBatchToAccessAsync(
            OleDbConnection connection,
            string tableName,
            DataTable batchTable,
            OleDbTransaction transaction,
            CancellationToken cancellationToken)
        {

            try
            {
                using (var adapter = new OleDbDataAdapter())
                {
                    // Set up schema query
                    string selectSql = $"SELECT * FROM [{tableName}] WHERE 1=0";
                    adapter.SelectCommand = new OleDbCommand(selectSql, connection, transaction);

                    using (var builder = new OleDbCommandBuilder(adapter))
                    {
                        // Configure builder for Access
                        builder.QuotePrefix = "[";
                        builder.QuoteSuffix = "]";

                        // Get insert command
                        adapter.InsertCommand = builder.GetInsertCommand();
                        adapter.InsertCommand.Transaction = transaction;
                        adapter.InsertCommand.Connection = connection;
                        adapter.InsertCommand.CommandTimeout = 600;

                        // ** FIXED: Use the actual table name **
                        adapter.TableMappings.Clear();
                        adapter.TableMappings.Add("Table", tableName);

                        // Insert the batch
                        int rowsInserted = adapter.Update(batchTable);


                        return rowsInserted;
                    }
                }
            }
            catch
            {

                throw;
            }

        }

        private async Task DropTableIfExistsAsync(
            OleDbConnection connection,
            string tableName,
            CancellationToken cancellationToken)
        {
            try
            {
                // Check if table exists in Access
                var restrictions = new string[] { null, null, tableName, "TABLE" };
                using (var schemaTable = connection.GetSchema("Tables", restrictions))
                {
                    if (schemaTable.Rows.Count > 0)
                    {
                        using (var cmd = new OleDbCommand($"DROP TABLE [{tableName}]", connection))
                        {
                            cmd.ExecuteNonQuery();
                            Console.WriteLine($"Dropped existing table: {tableName}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not drop table {tableName}: {ex.Message}");
                // Continue anyway - table might not exist
            }
        }

        private async Task CreateAccessTableAsync(
            OleDbConnection connection,
            string tableName,
            DataTable schemaTable,
            CancellationToken cancellationToken)
        {
            var createTableSql = GenerateCreateTableSql(tableName, schemaTable);

            using (var cmd = new OleDbCommand(createTableSql, connection))
            {
                await cmd.ExecuteNonQueryAsync(cancellationToken);
                Console.WriteLine($"Created table: {tableName}");
            }
        }

        private string GenerateCreateTableSql(string tableName, DataTable schemaTable)
        {
            var columns = new List<string>();

            foreach (DataColumn column in schemaTable.Columns)
            {
                string accessType = GetAccessDataType(column.DataType);
                string nullable = column.AllowDBNull ? "NULL" : "NOT NULL";
                columns.Add($"[{column.ColumnName}] {accessType} {nullable}");
            }

            return $"CREATE TABLE [{tableName}] (\n  {string.Join(",\n  ", columns)}\n)";
        }

        private string GetAccessDataType(Type type)
        {
            // Map .NET types to Access data types
            if (type == typeof(string)) return "TEXT(255)";
            if (type == typeof(int)) return "LONG";
            if (type == typeof(long)) return "LONG";
            if (type == typeof(short)) return "INTEGER";
            if (type == typeof(byte)) return "BYTE";
            if (type == typeof(bool)) return "YESNO";
            if (type == typeof(DateTime)) return "DATETIME";
            if (type == typeof(decimal)) return "CURRENCY";
            if (type == typeof(double)) return "DOUBLE";
            if (type == typeof(float)) return "SINGLE";
            if (type == typeof(Guid)) return "GUID";
            if (type == typeof(byte[])) return "LONGBINARY";

            return "TEXT(255)"; // Default
        }

        private void OnRowUpdated(object sender, SqlRowUpdatedEventArgs e)
        {
            // Optional: Track progress for very large datasets
            if (e.Status == UpdateStatus.Continue)
            {
                Console.Write($".");
            }
        }

        //--- apprpach - 3


        //end approach
    }
}
