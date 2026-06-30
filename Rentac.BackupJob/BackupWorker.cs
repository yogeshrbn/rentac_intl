using Azure.Messaging.ServiceBus;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO.Compression;

namespace Rentac.BackupJob;

internal sealed class BackupWorker : BackgroundService
{
    private readonly IConfiguration _config;

    public BackupWorker(IConfiguration config)
    {
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var sqlConnectionString = _config["Backup:SqlConnectionString"]
                                  ?? throw new InvalidOperationException("Backup:SqlConnectionString is not configured.");

        var excelOutputFolder = _config["Backup:ExcelOutputFolder"]
                                ?? throw new InvalidOperationException("Backup:ExcelOutputFolder is not configured.");

        var sbConnectionString = _config["ServiceBus:ConnectionString"]
                                 ?? throw new InvalidOperationException("ServiceBus:ConnectionString is not configured.");

        var sbQueueName = _config["ServiceBus:QueueName"]
                          ?? throw new InvalidOperationException("ServiceBus:QueueName is not configured.");

        // List of views/tables to backup (source → destination name)
        var jobs = new[]
        {
            new BackupJob("vwbkp_delChallan", "vwbkp_delChallan"),
            new BackupJob("vwbkp_products", "vwbkp_products"),
            new BackupJob("vwbkp_party", "vwbkp_party"),
            new BackupJob("vwbkp_partyRates", "vwbkp_partyRates"),
            new BackupJob("vwbkp_retChallan", "vwbkp_retChallan"),
            new BackupJob("vwbkp_quotations", "vwbkp_quotations"),
            new BackupJob("vwbkp_ledgertxns", "vwbkp_ledgertxns"),
            new BackupJob("vwbkp_bills", "vwbkp_bills")
        };

        await using var client = new ServiceBusClient(sbConnectionString);
        var processor = client.CreateProcessor(sbQueueName, new ServiceBusProcessorOptions
        {
            MaxConcurrentCalls = 1,
            AutoCompleteMessages = false
        });

        processor.ProcessMessageAsync += async args =>
        {
            if (stoppingToken.IsCancellationRequested)
                return;
            string guId = "";
            string strCompanyId = "";
            string body = args.Message.Body.ToString();
            var msgData = body.Split(",");

            if (msgData == null || msgData.Length < 2)
            {
                await args.DeadLetterMessageAsync(args.Message, "InvalidData", $"Body '{body}' is not a valid int.");
                return;
            }
            guId = msgData[0];

            strCompanyId = msgData[1];
            if (!int.TryParse(strCompanyId, out int companyId))
            {
                // Dead-letter or abandon invalid messages
                await args.DeadLetterMessageAsync(args.Message, "InvalidCompanyId", $"Body '{body}' is not a valid int.");
                return;
            }

            try
            {
                var company = await Program.LoadCompanyByIdAsync(sqlConnectionString, companyId);
                if (company is null)
                {
                    await args.DeadLetterMessageAsync(args.Message, "CompanyNotFound", $"CompanyId {companyId} not found.");
                    return;
                }

                Console.WriteLine();
                Console.WriteLine($"=== Company: {company.CompanyName} (Id={company.CompanyId}) ===");

                var companyFolderName = Program.SanitizePathSegment(company.CompanyName);
                var companyFolder = Path.Combine(excelOutputFolder, companyFolderName);
                Directory.CreateDirectory(companyFolder);

                foreach (var job in jobs)
                {
                    Console.WriteLine();
                    Console.WriteLine($"--- Exporting: {job.SourceTable} → {job.DestinationTable} ---");

                    var excelPath = Path.Combine(companyFolder, $"{job.DestinationTable}.xlsx");

                    string companyFilter = $"CompanyId = {company.CompanyId}";
                    string? baseWhere = job.WhereClause;
                    string? finalWhere = string.IsNullOrWhiteSpace(baseWhere)
                        ? companyFilter
                        : $"{baseWhere} AND {companyFilter}";

                    var exportConfig = new ExportConfiguration
                    {
                        SqlServerConnectionString = sqlConnectionString,
                        ExcelFilePath = excelPath,
                        SourceTableName = job.SourceTable,
                        DestinationTableName = job.DestinationTable,
                        BatchSize = 100,
                        WhereClause = finalWhere
                    };

                    var exporter = new SqlToAccessExporter(exportConfig);

                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    int rows = await exporter.ExportAsync();
                    sw.Stop();

                    Console.WriteLine($"Done: {rows:N0} rows in {sw.Elapsed:mm\\:ss\\.ff} (avg {(rows / Math.Max(sw.Elapsed.TotalSeconds, 0.001)):N0} rows/s)");
                }

                // After all exports for this company, compress the company folder into a zip file
                string zipFileName = $"{companyFolderName}{companyId}.zip";
                var zipPath = Path.Combine(excelOutputFolder, zipFileName);
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }

                ZipFile.CreateFromDirectory(companyFolder, zipPath, CompressionLevel.Optimal, includeBaseDirectory: false);
                Console.WriteLine($"Compressed folder '{companyFolder}' to '{zipPath}'.");

                // Delete the uncompressed company folder after successful compression
                Directory.Delete(companyFolder, recursive: true);
                Console.WriteLine($"Deleted uncompressed folder '{companyFolder}'.");

                // Update backup status in BackupLogs table for this company
                  await Program.UpdateBackupStatusAsync(sqlConnectionString, zipFileName, guId, "Completed");

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error processing message for CompanyId={body}: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();

                await args.AbandonMessageAsync(args.Message);
            }
        };

        processor.ProcessErrorAsync += args =>
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Service Bus error: {args.Exception.Message}");
            Console.WriteLine(args.Exception.StackTrace);
            Console.ResetColor();
            return Task.CompletedTask;
        };

        await processor.StartProcessingAsync(stoppingToken);

        try
        {
            // Keep the processor alive until the service is stopping
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (TaskCanceledException)
        {
            // Expected on shutdown
        }
        finally
        {
            await processor.StopProcessingAsync();
            await processor.DisposeAsync();
        }
    }
}

