using Azure.Messaging.ServiceBus;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data;
using System.Data.OleDb;
using System.Text;

namespace Rentac.BackupJob;

internal partial class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(AppContext.BaseDirectory);
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddHostedService<BackupWorker>();
            })
            .Build();

        await host.RunAsync();
    }
}

internal sealed record BackupJob(
    string SourceTable,
    string DestinationTable)
{
    public string? WhereClause { get; init; }
}

internal sealed record CompanyInfo(int CompanyId, string CompanyName);

internal partial class Program
{
    public static async Task<CompanyInfo?> LoadCompanyByIdAsync(string sqlConnectionString, int companyId)
    {
        await using var connection = new SqlConnection(sqlConnectionString);
        await connection.OpenAsync();

        const string sql = "SELECT CompanyId, Name FROM Company WHERE CompanyId = @CompanyId";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@CompanyId", companyId);

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            int id = reader.GetInt32(0);
            string name = reader.GetString(1);
            return new CompanyInfo(id, name);
        }

        return null;
    }

    

    public static string SanitizePathSegment(string segment)
    {
        // Remove invalid path characters and trim
        var invalid = Path.GetInvalidFileNameChars();
        var cleaned = new string(segment.Where(c => !invalid.Contains(c)).ToArray()).Trim();

        if (string.IsNullOrWhiteSpace(cleaned))
            cleaned = "Company";

        return cleaned;
    }

    public static async Task UpdateBackupStatusAsync(string sqlConnectionString,string  zipFileName, string guId, string status)
    {
        await using var connection = new SqlConnection(sqlConnectionString);
        await connection.OpenAsync();

        // Adjust table/column names to match your schema if different.
        const string sql =
            @"UPDATE BackupLogs
              SET Status = @Status, CompletedOn = SYSDATETIME(),
                FileName = @zipFileName,
                progress= 100
              WHERE guId = @guId";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Status", status);
        command.Parameters.AddWithValue("@guId", guId);
        command.Parameters.AddWithValue("@zipFileName", zipFileName);

        await command.ExecuteNonQueryAsync();
    }
}

internal sealed class ExportConfiguration
{
    public required string SqlServerConnectionString { get; init; }
    public required string ExcelFilePath { get; init; }
    public required string SourceTableName { get; init; }
    public required string DestinationTableName { get; init; }
    public int BatchSize { get; init; } = 100;
    public string? WhereClause { get; init; }
}

internal sealed class SqlToAccessExporter
{
    private readonly ExportConfiguration _config;
    private string? _resolvedColumns;
    private readonly string _sheetName;

    public SqlToAccessExporter(ExportConfiguration config)
    {
        _config = config;
        _sheetName = SanitizeSheetName(_config.DestinationTableName);
    }

    public async Task<int> ExportAsync()
    {
        // If the Excel file already exists, delete it so we always create a fresh workbook.
        if (File.Exists(_config.ExcelFilePath))
        {
            File.Delete(_config.ExcelFilePath);
        }

        // Connection string for Excel via ACE OLEDB.
        // Match this with installed ACE provider bitness (x86/x64).
        string accessConnectionString =
            $@"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={_config.ExcelFilePath};Extended Properties=""Excel 12.0 Xml;HDR=YES;"";";

        using var sqlConnection = new SqlConnection(_config.SqlServerConnectionString);
        await sqlConnection.OpenAsync();

        int totalRows = await GetTotalRowCountAsync(sqlConnection);
        Console.WriteLine($"Total rows to export: {totalRows:N0}");

        string selectSql = BuildSelectSql();

        using var sqlCommand = new SqlCommand(selectSql, sqlConnection)
        {
            CommandTimeout = 300
        };

        using var reader = await sqlCommand.ExecuteReaderAsync();
        using var accessConnection = new OleDbConnection(accessConnectionString);
        await accessConnection.OpenAsync();

        // Always recreate the destination sheet (table) from SQL Server schema
        RecreateDestinationTable(accessConnection, reader);

        int rowsExported = 0;
        var batchValues = new List<string>(_config.BatchSize);

        using var transaction = accessConnection.BeginTransaction();
        using var accessCommand = new OleDbCommand
        {
            Connection = accessConnection,
            Transaction = transaction,
            CommandTimeout = 120
        };

        try
        {
            while (await reader.ReadAsync())
            {
                string rowValues = BuildRowValues(reader);
                batchValues.Add(rowValues);
                rowsExported++;

                if (batchValues.Count >= _config.BatchSize)
                {
                    await ExecuteBatchAsync(accessCommand, batchValues);
                    batchValues.Clear();
                    Console.WriteLine($"Progress: {rowsExported:N0}/{totalRows:N0} rows");
                }
            }

            if (batchValues.Count > 0)
            {
                await ExecuteBatchAsync(accessCommand, batchValues);
            }

            transaction.Commit();
            return rowsExported;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private string BuildSelectSql()
    {
        string columns = "*";

        var sb = new StringBuilder();
        sb.Append("SELECT ").Append(columns)
          .Append(" FROM ").Append(_config.SourceTableName);

        if (!string.IsNullOrWhiteSpace(_config.WhereClause))
        {
            sb.Append(" WHERE ").Append(_config.WhereClause);
        }

        return sb.ToString();
    }

    private async Task<int> GetTotalRowCountAsync(SqlConnection connection)
    {
        var sb = new StringBuilder();
        sb.Append("SELECT COUNT(*) FROM ").Append(_config.SourceTableName);
        if (!string.IsNullOrWhiteSpace(_config.WhereClause))
        {
            sb.Append(" WHERE ").Append(_config.WhereClause);
        }

        using var cmd = new SqlCommand(sb.ToString(), connection);
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    private static string BuildRowValues(SqlDataReader reader)
    {
        var values = new List<string>(reader.FieldCount);

        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (reader.IsDBNull(i))
            {
                values.Add("NULL");
                continue;
            }

            var fieldType = reader.GetFieldType(i);
            var value = reader.GetValue(i);

            if (fieldType == typeof(string) || fieldType == typeof(char))
            {
                // Excel cells are limited in size; very long strings can cause
                // "field is too small" errors via the ACE provider.
                // Truncate to 255 characters to be safe for backup purposes.
                string s = ((string)value);
                if (s.Length > 255)
                {
                    s = s.Substring(0, 255);
                }
                s = s.Replace("'", "''");
                values.Add($"'{s}'");
            }
            else if (fieldType == typeof(DateTime))
            {
                var dt = (DateTime)value;
                // Access recognizes #...# date literals
                values.Add($"#{dt:yyyy-MM-dd HH:mm:ss}#");
            }
            else if (fieldType == typeof(Guid))
            {
                values.Add($"'{{{value}}}'");
            }
            else if (fieldType == typeof(bool))
            {
                values.Add((bool)value ? "1" : "0");
            }
            else if (fieldType == typeof(byte[]))
            {
                var bytes = (byte[])value;
                values.Add($"0x{BitConverter.ToString(bytes).Replace("-", "")}");
            }
            else
            {
                // numeric and other primitive types
                values.Add(Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture) ?? "NULL");
            }
        }

        return $"({string.Join(", ", values)})";
    }

    private async Task ExecuteBatchAsync(OleDbCommand command, List<string> batchValues)
    {
        if (batchValues.Count == 0) return;

        string columns = _resolvedColumns ?? throw new InvalidOperationException("Destination columns not resolved.");

        // ACE/OleDb does not support multi-row VALUES in one INSERT reliably,
        // so execute one INSERT per row inside the transaction.
        int inserted = 0;
        foreach (var rowValues in batchValues)
        {
            command.CommandText = $"INSERT INTO [{_sheetName}$] ({columns}) VALUES {rowValues}";
            await command.ExecuteNonQueryAsync();
            inserted++;
        }

        Console.WriteLine($"Inserted {inserted} records into '{_sheetName}$'.");
    }

    private void RecreateDestinationTable(OleDbConnection accessConnection, SqlDataReader reader)
    {
        // Build CREATE TABLE from SQL Server schema into an Excel sheet.
        string createSql = BuildCreateTableSql(reader);
        using (var createCmd = accessConnection.CreateCommand())
        {
            createCmd.CommandText = createSql;
            createCmd.ExecuteNonQuery();
        }

        Console.WriteLine($"Created sheet '{_sheetName}' in Excel file '{_config.ExcelFilePath}'.");
    }

    private string BuildCreateTableSql(SqlDataReader reader)
    {
        var schema = reader.GetSchemaTable() ??
                     throw new InvalidOperationException("Could not read schema from SQL Server reader.");

        var columnDefs = new List<string>();
        var columnNames = new List<string>();

        foreach (DataRow row in schema.Rows)
        {
            string columnName = Convert.ToString(row["ColumnName"])
                                ?? throw new InvalidOperationException("ColumnName missing.");

            var dataType = (Type)row["DataType"];

            int columnSize = row["ColumnSize"] == DBNull.Value
                ? 0
                : Convert.ToInt32(row["ColumnSize"]);

            short numericPrecision = row["NumericPrecision"] == DBNull.Value
                ? (short)0
                : Convert.ToInt16(row["NumericPrecision"]);

            short numericScale = row["NumericScale"] == DBNull.Value
                ? (short)0
                : Convert.ToInt16(row["NumericScale"]);

            // For Excel via ACE OLEDB, many constraint properties (NOT NULL, etc.)
            // are not supported. We therefore avoid specifying nullability and
            // just choose a compatible column type.
            string accessType = MapToAccessType(dataType, columnSize, numericPrecision, numericScale);
            string nullClause = string.Empty;

            string escapedName = $"[{columnName}]";
            columnNames.Add(escapedName);
            columnDefs.Add($"{escapedName} {accessType}{nullClause}");
        }

        _resolvedColumns = string.Join(", ", columnNames);

        string columnsSql = string.Join(", ", columnDefs);
        // When creating a sheet in Excel via OLEDB, use [SheetName] (without $).
        // The $ suffix is only used when querying/inserting.
        return $"CREATE TABLE [{_sheetName}] ({columnsSql})";
    }

    private static string MapToAccessType(Type dataType, int columnSize, short numericPrecision, short numericScale)
    {
        if (dataType == typeof(string) || dataType == typeof(char))
        {
            // In Excel, TEXT without explicit length is generally safest; very long
            // values are already truncated in BuildRowValues.
            return "TEXT";
        }

        if (dataType == typeof(int) || dataType == typeof(short) || dataType == typeof(byte) || dataType == typeof(long))
            return "DOUBLE"; // Excel stores numbers as floating point

        if (dataType == typeof(decimal) || dataType == typeof(double) || dataType == typeof(float))
            return "DOUBLE";

        if (dataType == typeof(DateTime))
            return "DATETIME";

        if (dataType == typeof(bool))
            return "YESNO";

        if (dataType == typeof(byte[]) || dataType == typeof(Guid))
            return "TEXT";

        // Fallback to TEXT for unknown types.
        return "TEXT";
    }

    private static string SanitizeSheetName(string name)
    {
        // Excel sheet name rules:
        // - Max 31 characters
        // - Cannot contain: : \ / ? * [ ]
        // - Cannot be empty, or 'History'
        // - We'll also trim quotes and leading/trailing spaces
        if (string.IsNullOrWhiteSpace(name))
            return "Sheet1";

        var invalidChars = new[] { ':', '\\', '/', '?', '*', '[', ']' };
        var cleaned = new string(name.Where(c => !invalidChars.Contains(c)).ToArray()).Trim();

        if (string.IsNullOrEmpty(cleaned))
            cleaned = "Sheet1";

        if (cleaned.Equals("History", StringComparison.OrdinalIgnoreCase))
            cleaned = "History1";

        // Truncate to Excel's 31 character limit
        if (cleaned.Length > 31)
            cleaned = cleaned.Substring(0, 31);

        return cleaned;
    }
}

