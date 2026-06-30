

using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.OleDb;

namespace Rentac.DbBackup
{
    public class Worker : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Worker> _logger;
        private SqlServerToAccessBackupService _service;
        private DbBackupRepository _respository;
        string _backupPath = @"D:\\Projects\\hm\\rentac_v1\\Api\\backup\";
        public HighPerformanceExporter _exporterService;
        public Worker(ILogger<Worker> logger,
            SqlServerToAccessBackupService service,
            DbBackupRepository dbBackupRepository,

            IConfiguration configuration)
        {
            _logger = logger;
            _service = service;
            _configuration = configuration;
            _respository = dbBackupRepository;
            string connection = _configuration.GetConnectionString("sqlCon");
            _service.setConnectionString(connection);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var backupFile = _backupPath + "rentac.accdb";
                while (!stoppingToken.IsCancellationRequested)
                {
                    var exportRequest = new ExportRequest();
                    var ViewNames = new List<string>();
                    //ViewNames.Add("vwbkp_products");
                    //ViewNames.Add("vwbkp_party");
                    ViewNames.Add("vwbkp_delChallan");
                    ViewNames.Add("vwbkp_retChallan");
                    ViewNames.Add("vwbkp_bills");
                    ViewNames.Add("vwbkp_quotations");
                    ViewNames.Add("vwbkp_ledgertxns");
                    ViewNames.Add("vwbkp_ewayBills");
                    ViewNames.Add("vwbkp_contracts");

                    ViewNames.Add("vwbkp_partyRates");
                    ViewNames.Add("vwbkp_purchase");

                    var currentBackupFile = _backupPath + $"Backup_{DateTime.Now:yyyyMMdd_HHmmss}.accdb";
                    File.Copy(backupFile, currentBackupFile);
                    exportRequest.CompanyId = 1111;
                    exportRequest.BackupCreatedBy = "System";

                    exportRequest.AccessFilePath = currentBackupFile;

                    var backupLog = new BackupLog
                    {
                        StartTime = DateTime.UtcNow,
                        Status = "InProgress",
                        SourceDatabase = _service.GetDatabaseNameFromConnectionString(),
                        TotalViews = ViewNames.Count,
                        RequestedBy = exportRequest.BackupCreatedBy,
                        MachineName = Environment.MachineName,
                        CompanyId = exportRequest.CompanyId,
                        CreatedOn = DateTime.Now,
                        Progress = 0,
                        GuId = Guid.NewGuid().ToString()
                    };

                    var created = await _respository.Create(backupLog);

                    foreach (var viewName in ViewNames)
                    {
                        var lstViews = new List<string>();
                        lstViews.Add(viewName);
                        exportRequest.ViewNames = lstViews;
                        var result = await _service.ExportViewsToAccessAsync(exportRequest, backupLog);
                        if (_logger.IsEnabled(LogLevel.Information))
                        {
                            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                        }
                        //pg.start(_configuration.GetConnectionString("sqlCon"), "", "");
                        await Task.Delay(5000, stoppingToken);
                    }
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }



      public  class BackupProgram
        {
            public void start(string sqlConnectionString, string accessConnectionString,string query)
            {
                //string sqlConnectionString = "Server=YOUR_SERVER;Database=YOUR_DB;Integrated Security=True;";
                // accessConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\path\to\your\database.accdb;";

                try
                {
                    // 1. Get data from SQL Server into a DataTable
                    DataTable dataTable = new DataTable();
                    using (SqlConnection sqlConn = new SqlConnection(sqlConnectionString))
                    {
                        sqlConn.Open();
                      //  string query = "SELECT * FROM YourSQLTable"; // Or use a stored procedure
                        using (SqlCommand cmd = new SqlCommand(query, sqlConn))
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dataTable);
                        }
                    }

                    // 2. Write DataTable to Access using OleDbDataAdapter
                    using (OleDbConnection accessConn = new OleDbConnection(accessConnectionString))
                    {
                        accessConn.Open();
                        using (OleDbTransaction transaction = accessConn.BeginTransaction()) // Use transaction for safety
                        {
                            using (OleDbDataAdapter accessAdapter = new OleDbDataAdapter())
                            {
                                // Create a command builder to generate INSERT commands automatically
                                using (OleDbCommandBuilder builder = new OleDbCommandBuilder(accessAdapter))
                                {
                                    accessAdapter.InsertCommand = builder.GetInsertCommand();
                                    accessAdapter.InsertCommand.Transaction = transaction;
                                    accessAdapter.InsertCommand.Connection = accessConn;

                                    // Map table name to the destination Access table
                                    accessAdapter.TableMappings.Add("Table", "YourAccessTable");

                                    // Perform the update (insert)
                                    int rowsInserted = accessAdapter.Update(dataTable);
                                    Console.WriteLine($"Inserted {rowsInserted} rows.");

                                    transaction.Commit();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }
    }
}
