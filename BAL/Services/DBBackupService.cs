
using BAL.DAL;
using BAL.Data.Contracts;
using BAL.Models;
using BAL.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.util;

namespace BAL.Services
{
    public class SqlServerToAccessBackupService : IDBBackupService
    {
        //private readonly IConfiguration _configuration;
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private string _sqlConnectionString;
        private readonly IDBBackupRespository _respository;
        public SqlServerToAccessBackupService(
            IDBBackupRespository respository
            )
        {
            _respository = respository;

            //  _sqlConnectionString = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;

        }

        public void setConnectionString(string connectionString)
        {

            _sqlConnectionString = connectionString;
        }
        private async Task<int> ExportViewStreamAsync(
    string viewName,
    OleDbConnection accessConnection,
    int companyId)
        {
            int rowsExported = 0;
            var result = new ViewExportResult
            {
                ViewName = viewName,
                AccessTableName = SanitizeTableName(viewName)
            };
            using (var sqlConnection = new SqlConnection(_sqlConnectionString))
            using (var sqlCommand = new SqlCommand(
                $"SELECT * FROM [{viewName}] WHERE CompanyId = @CompanyId",
                sqlConnection))
            {
                sqlCommand.Parameters.Add("@CompanyId", SqlDbType.Int).Value = companyId;

                await sqlConnection.OpenAsync();

                // STREAMING reader (LOW CPU + LOW MEMORY)
                using (var reader = await sqlCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                using (var transaction = accessConnection.BeginTransaction())
                {
                    // Build insert dynamically once
                    var columnNames = Enumerable.Range(0, reader.FieldCount)
                        .Select(i => SanitizeColumnName(reader.GetName(i)))
                        .ToArray();

                    string insertSql = $@"
                INSERT INTO [{SanitizeTableName(viewName)}]
                ([{string.Join("], [", columnNames)}])
                VALUES ({string.Join(", ", columnNames.Select(_ => "?"))})";

                    using (var command = new OleDbCommand(insertSql, accessConnection, transaction))
                    {
                        // Create parameters once
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            command.Parameters.Add(new OleDbParameter());
                        }

                        while (await reader.ReadAsync())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                command.Parameters[i].Value =
                                    reader.IsDBNull(i) ? DBNull.Value : reader.GetValue(i);
                            }

                            await command.ExecuteNonQueryAsync();
                            rowsExported++;
                        }
                    }

                    transaction.Commit();
                }
            }

            return rowsExported;
        }

        public async Task<ExportResponse> ExportViewsToAccessAsync(ExportRequest request)
        {
            var response = new ExportResponse
            {
                ExportTime = DateTime.Now,
                Success = false
            };

            BackupLog backupLog = null;

            try
            {
                // Create backup log
                backupLog = new BackupLog
                {
                    StartTime = DateTime.Now,
                    Status = "InProgress",
                    SourceDatabase = "",
                    TotalViews = 0,
                    RequestedBy = request.BackupCreatedBy,
                    MachineName = Environment.MachineName,
                    CompanyId = request.CompanyId,
                    CreatedOn = DateTime.Now,
                    Progress = 0,
                    GuId = request.GuId
                };

                ////  Validate request
                //if (request.ViewNames == null || !request.ViewNames.Any())
                //{
                //    throw new ArgumentException("At least one view name must be specified");
                //}

                //// Get or create Access file path
                //string accessFilePath = GetAccessFilePath(request.AccessFilePath);
                //response.AccessFilePath = accessFilePath;

                //// Create Access database if needed
                ////if (request.CreateNewDatabase && !File.Exists(accessFilePath))
                ////{
                ////    await CreateAccessDatabaseAsync(accessFilePath);
                ////    _logger.Info("Created new Access database at: {FilePath}", accessFilePath);
                ////}
                //if (request.CompanyId == 0)
                //{
                //    throw new FileNotFoundException($"Invalid companyId: {request.CompanyId.ToString()}");
                //}
                //if (!File.Exists(accessFilePath))
                //{
                //    throw new FileNotFoundException($"Access database not found at: {accessFilePath}");
                //}
                //var finFo = new FileInfo(accessFilePath);

                //string accessConnectionString = GetAccessConnectionString(accessFilePath);
                //backupLog.AccessFilePath = accessFilePath;
                //backupLog.BackupFileName = finFo.Name;

                //// Test connections
                //await TestConnectionsAsync(_sqlConnectionString, accessConnectionString);

                var results = new List<ViewExportResult>();

                var created = await _respository.Create(backupLog);
                //using (var accessConnection = new OleDbConnection(accessConnectionString))
                //{
                //    await accessConnection.OpenAsync();

                //    foreach (var viewName in request.ViewNames)
                //    {
                //        var viewResult = await ExportSingleViewAsync(
                //            viewName,
                //            accessConnection,
                //            request.OverwriteExistingTables,
                //            request.BatchSize, request.CompanyId);
                //        // var viewResult =await ExportViewStreamAsync(viewName,accessConnection,request.CompanyId)
                //        results.Add(viewResult);
                //        if (!viewResult.Success)
                //        {
                //            break;
                //        }
                //        backupLog.Progress = (results.Count / Convert.ToDecimal(request.ViewNames.Count)) * 100;
                //        if (backupLog.Progress == 100)
                //        {
                //            backupLog.EndTime = DateTime.Now;
                //            backupLog.Status = "Completed";

                //        }
                //        var updated = await _respository.UpdateProgress(backupLog);
                //    }
                //}

                // Update response
                response.Results = results;
                response.Success = results.All(r => r.Success);
                response.Message = response.Success
                    ? $"Successfully exported {results.Count} views to Access database"
                    : $"Partially exported {results.Count(r => r.Success)} of {results.Count} views";
                //   response.FileSizeInBytes = new FileInfo(accessFilePath).Length;

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
                throw ex;
                //if (backupLog != null)
                //{
                //    backupLog.EndTime = DateTime.UtcNow;
                //    backupLog.Status = "Failed";
                //    backupLog.ErrorMessage = ex.Message;
                //}
                //var updated = await _respository.UpdateProgress(backupLog);
                //response.Success = false;
                //response.Message = $"Export failed: {ex.Message}";
                //return response;
            }

        }
        private async Task<DataTable> GetSchemaOnlyAsync(string viewName, int companyId)
        {
            using (var sqlConnection = new SqlConnection(_sqlConnectionString))
            {


                using (var command = new SqlCommand(
                    $"SELECT * FROM [{viewName}] WHERE CompanyId = @CompanyId",
                    sqlConnection))
                {

                    command.Parameters.Add("@CompanyId", SqlDbType.Int).Value = companyId;

                    await sqlConnection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                    {
                        var schemaTable = new DataTable();
                        schemaTable.Load(reader); // Loads only schema, almost zero CPU
                        return schemaTable;
                    }
                }
            }
        }
        private async Task<int> ExportViewStreamAsync(
    string viewName,
    OleDbConnection accessConnection,
    string accessTableName,
    int companyId)
        {
            int totalRowsInserted = 0;
            const int batchSize = 500; // 🔥 KEY: batch insert

            using (var sqlConnection = new SqlConnection(_sqlConnectionString))
            using (var sqlCommand = new SqlCommand(
                $"SELECT * FROM [{viewName}] WHERE CompanyId = @CompanyId",
                sqlConnection))
            {
                sqlCommand.Parameters.Add("@CompanyId", SqlDbType.Int).Value = companyId;
                sqlCommand.CommandTimeout = 0; // Prevent timeout for large export

                await sqlConnection.OpenAsync();

                using (var reader = await sqlCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                using (var transaction = accessConnection.BeginTransaction())
                {
                    var columnNames = Enumerable.Range(0, reader.FieldCount)
                        .Select(i => $"[{SanitizeColumnName(reader.GetName(i))}]")
                        .ToArray();

                    string insertSql = $@"
                INSERT INTO [{accessTableName}]
                ({string.Join(",", columnNames)})
                VALUES ({string.Join(",", Enumerable.Repeat("?", reader.FieldCount))})";

                    using (var command = new OleDbCommand(insertSql, accessConnection, transaction))
                    {
                        // Create parameters once
                        for (int i = 0; i < reader.FieldCount; i++)
                            command.Parameters.Add(new OleDbParameter());

                        int batchCounter = 0;

                        while (await reader.ReadAsync())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                object value = reader.IsDBNull(i) ? DBNull.Value : reader.GetValue(i);

                                // 🔥 CRITICAL: Fix DateTime for Access (prevents type mismatch + CPU overhead)
                                if (value is DateTime dt)
                                    value = dt; // Access supports DateTime directly

                                command.Parameters[i].Value = value;
                            }

                            command.ExecuteNonQuery(); // Sync is actually faster for OleDb
                            totalRowsInserted++;
                            batchCounter++;

                            // 🔥 Commit every batch (MASSIVE CPU reduction)
                            //if (batchCounter >= batchSize)
                            //{
                            //    transaction.Commit();
                            //    batchCounter = 0;
                            //}
                        }

                        // Final commit
                        if (batchCounter > 0)
                            transaction.Commit();

                        // Critical for ACE memory leak prevention
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
            }

            _logger.Debug("Stream exported {Rows} rows to {Table}",
                totalRowsInserted, accessTableName);

            return totalRowsInserted;
        }


        private async Task<ViewExportResult> ExportSingleViewAsync(
            string viewName,
            OleDbConnection accessConnection,
            bool overwriteTable,
            int? batchSize, int companyId)
        {
            var result = new ViewExportResult
            {
                ViewName = viewName,
                AccessTableName = SanitizeTableName(viewName)
            };

            var startTime = DateTime.UtcNow;

            try
            {
                _logger.Info("Starting export for view: {ViewName}", viewName);

                // Get data from SQL Server view
                //DataTable dataTable = await GetDataFromSqlViewAsync(viewName, companyId);

                //if (dataTable.Rows.Count == 0)
                //{
                //    result.Success = true;
                //    result.RowsExported = 0;
                //    result.ExportDuration = DateTime.UtcNow - startTime;
                //    _logger.Warn("View {ViewName} contains no data", viewName);
                //    return result;
                //}

                // Create or recreate table in Access
                if (overwriteTable)
                {
                    await DropTableIfExistsAsync(accessConnection, result.AccessTableName);
                }

                //await CreateAccessTableAsync(accessConnection, result.AccessTableName, dataTable);

                //// Insert data into Access
                //int rowsExported = await InsertDataIntoAccessAsync(
                //    accessConnection,
                //    result.AccessTableName,
                //    dataTable,
                //    batchSize);
                // Get schema only (NOT full data)
                using (var schemaTable = await GetSchemaOnlyAsync(viewName, companyId))
                {



                    // Create table using schema
                    await CreateAccessTableAsync(accessConnection, result.AccessTableName, schemaTable);

                    // Stream data directly (NO DataTable in memory)
                    int rowsExported = await ExportViewStreamAsync(
                        viewName,
                        accessConnection,
                        result.AccessTableName,
                        companyId);


                    result.Success = true;
                    result.RowsExported = rowsExported;
                    result.ExportDuration = DateTime.UtcNow - startTime;

                    _logger.Info(
                        "Successfully exported {RowCount} rows from view {ViewName} to table {TableName}",
                        rowsExported, viewName, result.AccessTableName);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error exporting view {ViewName}", viewName);

                result.Success = false;
                result.ErrorMessage = $"Error exporting table {viewName}";
                result.ExportDuration = DateTime.UtcNow - startTime;
                throw ex;
                //return result;
            }
        }

        private async Task<DataTable> GetDataFromSqlViewAsync(string viewName, int companyId)
        {
            using (var sqlConnection = new SqlConnection(_sqlConnectionString))
            {

                // Use parameterized query to prevent SQL injection
                // string query = $"SELECT * FROM [{viewName}] Where companyId = " + companyId.ToString();
                //
                string query = $"SELECT * FROM [{viewName}] WHERE companyId = @companyId";
                using (var command = new SqlCommand(query, sqlConnection))
                {
                    command.Parameters.Add("@companyId", SqlDbType.Int).Value = companyId;
                    await sqlConnection.OpenAsync();

                    var dataTable = new DataTable();
                    using (var adapter = new SqlDataAdapter(command))
                    {

                        // Configure batch size for better performance
                        adapter.SelectCommand.CommandTimeout = 300; // 5 minutes timeout

                        adapter.Fill(dataTable);

                        _logger.Debug("Retrieved {RowCount} rows from view {ViewName}", dataTable.Rows.Count, viewName);

                        return dataTable;
                    }
                }
            }
        }

        private async Task DropTableIfExistsAsync(OleDbConnection connection, string tableName)
        {
            try
            {
                string dropSql = $"DROP TABLE [{tableName}]";
                using (var command = new OleDbCommand(dropSql, connection))
                {
                    await command.ExecuteNonQueryAsync();

                    _logger.Debug("Dropped existing table: {TableName}", tableName);
                }
            }
            catch (OleDbException ex) when (ex.ErrorCode == -2147217865) // Table doesn't exist
            {
                // Table doesn't exist, which is fine
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Error dropping table {TableName}", tableName);
                throw;
            }
        }

        private async Task CreateAccessTableAsync(OleDbConnection connection, string tableName, DataTable schemaTable)
        {
            var columns = new List<string>();

            foreach (DataColumn column in schemaTable.Columns)
            {
                string columnName = SanitizeColumnName(column.ColumnName);
                string dataType = MapToAccessDataType(column.DataType);
                columns.Add($"[{columnName}] {dataType}");
            }

            string createTableSql = $"CREATE TABLE [{tableName}] ({string.Join(", ", columns)})";

            using (var command = new OleDbCommand(createTableSql, connection))
            {
                await command.ExecuteNonQueryAsync();

                _logger.Debug("Created table {TableName} with {ColumnCount} columns",
                    tableName, columns.Count);
            }
        }
        private async Task<int> InsertDataIntoAccessAsync(
    OleDbConnection connection,
    string tableName,
    DataTable dataTable,
    int? batchSize)
        {
            int totalRowsInserted = 0;

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var columns = dataTable.Columns.Cast<DataColumn>().ToList();

                    // Access prefers positional parameters (?)
                    string columnList = string.Join("], [", columns.Select(c => SanitizeColumnName(c.ColumnName)));
                    string paramList = string.Join(", ", columns.Select(c => "?"));

                    string insertSql = $@"
                INSERT INTO [{tableName}] 
                ([{columnList}]) 
                VALUES ({paramList})";

                    using (var command = new OleDbCommand(insertSql, connection, transaction))
                    {
                        // IMPORTANT: Add parameters with correct data types
                        foreach (var column in columns)
                        {
                            //command.Parameters.Add(new OleDbParameter
                            //{
                            //    OleDbType = GetOleDbType(column.DataType)
                            //});

                            command.Parameters.Add($"@{column}", OleDbType.VarWChar);


                        }

                        // Insert rows (single transaction = fastest for 2k rows)
                        foreach (DataRow row in dataTable.Rows)
                        {
                            //for (int i = 0; i < columns.Count; i++)
                            //{
                            //    command.Parameters[i].Value = row[i] ?? DBNull.Value;
                            //}
                            for (int i = 0; i < columns.Count; i++)
                            {
                                var value = row[i];
                                command.Parameters[i].Value = value ?? DBNull.Value;
                            }
                            try
                            {
                                await command.ExecuteNonQueryAsync();
                                totalRowsInserted++;
                            }
                            catch (Exception ex)
                            {
                                _logger.Error($"Type mismatch at row {totalRowsInserted}");
                                for (int i = 0; i < columns.Count; i++)
                                {
                                    _logger.Error($"Column: {columns[i].ColumnName}, Value: {row[i]}, Type: {row[i]?.GetType()}");
                                }
                                throw ex;
                            }

                        }
                    }

                    transaction.Commit(); // Commit once = BIG speed boost
                    return totalRowsInserted;
                }
                catch (Exception ex)
                {

                    transaction.Rollback();

                    throw ex;
                }
            }
        }

        /*
        private async Task<int> InsertDataIntoAccessAsync(
            OleDbConnection connection,
            string tableName,
            DataTable dataTable,
            int? batchSize)
        {
            int totalRowsInserted = 0;
            int currentBatch = 0;

            // Use transactions for better performance
            var transaction = connection.BeginTransaction();



            try
            {
                // Prepare insert command
                var columnNames = dataTable.Columns.Cast<DataColumn>()
                    .Select(c => SanitizeColumnName(c.ColumnName))
                    .ToArray();

                var paramNames = columnNames.Select(c => $"@{c}").ToArray();

                string insertSql = $@"
                    INSERT INTO [{tableName}] 
                    ([{string.Join("], [", columnNames)}]) 
                    VALUES ({string.Join(", ", paramNames)})";

                using (var command = new OleDbCommand(insertSql, connection, transaction))
                {

                    // Add parameters
                    foreach (var columnName in columnNames)
                    {
                        command.Parameters.Add($"@{columnName}", OleDbType.VarWChar);
                    }

                    // Insert rows
                    foreach (DataRow row in dataTable.Rows)
                    {
                        for (int i = 0; i < columnNames.Length; i++)
                        {
                            var value = row[i];
                            command.Parameters[i].Value = value ?? DBNull.Value;
                        }

                        await command.ExecuteNonQueryAsync();
                        totalRowsInserted++;

                        // Commit batch if batch size is specified
                        if (batchSize.HasValue && totalRowsInserted % batchSize.Value == 0)
                        {
                            //    await transaction.CommitAsync();
                            await Task.Run(() => transaction.Commit());
                            await Task.Run(() => transaction.Dispose());


                            transaction = connection.BeginTransaction();
                            command.Transaction = transaction;

                            currentBatch++;
                            _logger.Debug("Committed batch {BatchNumber}, total rows: {TotalRows}",
                                currentBatch, totalRowsInserted);
                        }
                    }

                    // Commit final transaction

                    await Task.Run(() => transaction.Commit());
                    _logger.Debug("Inserted {TotalRows} rows into table {TableName}",
                        totalRowsInserted, tableName);

                    return totalRowsInserted;
                }
            }
            catch (Exception)
            {
                await Task.Run(() => transaction.Rollback());
                throw;
            }

        }
   
        */
        private OleDbType GetOleDbType(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type == typeof(int)) return OleDbType.Integer;
            if (type == typeof(long)) return OleDbType.BigInt;
            if (type == typeof(short)) return OleDbType.SmallInt;
            if (type == typeof(decimal)) return OleDbType.Decimal;
            if (type == typeof(double)) return OleDbType.Double;
            if (type == typeof(float)) return OleDbType.Single;
            if (type == typeof(DateTime)) return OleDbType.DBTimeStamp;
            if (type == typeof(bool)) return OleDbType.Boolean;
            if (type == typeof(Guid)) return OleDbType.Guid;
            if (type == typeof(byte[])) return OleDbType.LongVarBinary;
            if (type == typeof(string)) return OleDbType.LongVarWChar;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {

            }
            return OleDbType.LongVarWChar; // strings
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

        private string SanitizeTableName(string tableName)
        {
            // Remove schema prefix if present
            var name = tableName.Contains('.')
                ? tableName.Split('.')[1]
                : tableName;

            // Replace invalid characters with underscore
            var invalidChars = new char[] { ' ', '-', '.', ',', ';', ':', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '+', '=', '{', '}', '[', ']', '|', '\\', '/', '<', '>', '?', '`', '~' };

            foreach (var c in invalidChars)
            {
                name = name.Replace(c, '_');
            }

            // Ensure it starts with a letter
            if (name.Length > 0 && !char.IsLetter(name[0]))
            {
                name = "T_" + name;
            }

            // Truncate to 64 characters (Access limit)
            return name.Length > 64 ? name.Substring(0, 64) : name;
        }

        private string SanitizeColumnName(string columnName)
        {
            // Remove invalid characters
            var invalidChars = new char[] { ' ', '-', '.', ',', ';', ':', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '+', '=', '{', '}', '[', ']', '|', '\\', '/', '<', '>', '?', '`', '~' };

            var name = columnName;
            foreach (var c in invalidChars)
            {
                name = name.Replace(c, '_');
            }

            // Ensure it starts with a letter
            if (name.Length > 0 && !char.IsLetter(name[0]))
            {
                name = "C_" + name;
            }

            // Truncate to 64 characters (Access limit)
            return name.Length > 64 ? name.Substring(0, 64) : name;
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

        //private async Task CreateAccessDatabaseAsync(string filePath)
        //{
        //    try
        //    {
        //        // Create directory if it doesn't exist
        //        var directory = Path.GetDirectoryName(filePath);
        //        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        //        {
        //            Directory.CreateDirectory(directory);
        //        }

        //        // Create Access database using ADOX
        //        var catalog = new ADOX.Catalog();
        //        catalog.Create(GetAccessConnectionString(filePath));
        //        Marshal.ReleaseComObject(catalog);

        //        await Task.CompletedTask;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error creating Access database");
        //        throw new InvalidOperationException($"Failed to create Access database: {ex.Message}", ex);
        //    }
        //}

        private async Task TestConnectionsAsync(string sqlConnectionString, string accessConnectionString)
        {
            // Test SQL Server connection
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    _logger.Debug("SQL Server connection test successful");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"SQL Server connection failed: {ex.Message}", ex);
                }
            }

            // Test Access connection
            using (var accessConnection = new OleDbConnection(accessConnectionString))
            {
                try
                {
                    await accessConnection.OpenAsync();
                    _logger.Debug("Access database connection test successful");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Access database connection failed: {ex.Message}", ex);
                }
            }
        }

        private string GetDatabaseNameFromConnectionString()
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

        // Implementation of other interface methods
        public async Task<List<ViewInfo>> GetAvailableViewsAsync()
        {
            var views = new List<ViewInfo>();

            using (var connection = new SqlConnection(_sqlConnectionString))
            {


                await connection.OpenAsync();

                string query = @"
                SELECT 
                    TABLE_SCHEMA as SchemaName,
                    TABLE_NAME as ViewName,
                    CREATE_DATE as CreateDate,
                    MODIFY_DATE as ModifyDate
                FROM INFORMATION_SCHEMA.VIEWS
                ORDER BY TABLE_SCHEMA, TABLE_NAME";

                var command = new SqlCommand(query, connection);
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var viewInfo = new ViewInfo
                    {
                        SchemaName = reader["SchemaName"].ToString(),
                        ViewName = reader["ViewName"].ToString(),
                        CreateDate = reader["CreateDate"] as DateTime?,
                        ModifyDate = reader["ModifyDate"] as DateTime?
                    };

                    // Get column count
                    viewInfo.ColumnCount = await GetViewColumnCountAsync(viewInfo.SchemaName, viewInfo.ViewName);

                    views.Add(viewInfo);
                }

                return views;
            }
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

        public async Task<IEnumerable<BackupLog>> GetBackupHistoryAsync(BackupLog dto)
        {
            // In a real implementation, this would query a database
            // For now, return empty list or mock data
            return await _respository.List(dto);
        }
    }
}