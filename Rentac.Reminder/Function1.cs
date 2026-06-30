using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Azure.Messaging.ServiceBus;
using System;
using System.Data;
using System.Globalization;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Reflection;

namespace Rentac.Reminder;

public class Function1
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    public Function1(ILoggerFactory loggerFactory, IConfiguration configuration)
    {
        _logger = loggerFactory.CreateLogger<Function1>();
        _configuration = configuration;
    }

    [Function("queueDismantleDueContractReminders1AM")]
    public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)
    {
        var dueDate = DateTime.Now.Date.AddDays(1);
        var queueName = _configuration["QueueName"] ?? "contractreminders";
        var contracts = new List<MessageDto>();
        var sqlConnectionString = _configuration["sqlCon"];

        if (string.IsNullOrWhiteSpace(sqlConnectionString))
        {
            throw new InvalidOperationException("Missing required configuration: sqlCon");
        }


        await using var sqlConnection = new SqlConnection(sqlConnectionString);
        try
        {
            await sqlConnection.OpenAsync();
            //dismantle due
            const string sql = @"SELECT T1.*,T2.rentacApiKey
                                 FROM [Contract] T1 INNER JOIN Company T2 ON T1.CompanyId = T2.CompanyId
                                 WHERE CAST([dismantleDueDate] AS date) > @dueDate and t2.rentacApiKey is not null";

            await using var command = new SqlCommand(sql, sqlConnection);
            command.Parameters.Add(new SqlParameter("@dueDate", SqlDbType.Date) { Value = dueDate });

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var contractId = GetIntValue(reader, "ContractId", "contractId", "Id", "id");
                if (!contractId.HasValue)
                {
                    continue;
                }

                var companyId = GetIntValue(reader, "CompanyId", "companyId");
                var dismantleDue = GetDateValue(reader, "dismantleDueDate") ?? dueDate;
                var apiKey = GetStringValue(reader, "rentacApiKey");
                if (apiKey == null)
                {
                    throw new InvalidOperationException("Invalid or empty API Key for :company:" + companyId);
                }
                //contracts.Add(new MessageDto
                //{
                //    MetaData = "1106," + contractId.Value,
                //    CompanyId = companyId,
                //    Type = "whatsapp",
                //    RentacApiKey = apiKey
                //});
            }
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Error in queueDismantleDueContractReminders function.");
        }

        try
        {

            //dismantle due
            const string sql = @"SELECT T1.*,T2.rentacApiKey
                                 FROM [Contract] T1 INNER JOIN Company T2 ON T1.CompanyId = T2.CompanyId
                                 WHERE CAST([installDueDate] AS date) = @dueDate and t2.rentacApiKey is not null";

            await using var command = new SqlCommand(sql, sqlConnection);
            command.Parameters.Add(new SqlParameter("@dueDate", SqlDbType.Date) { Value = dueDate });

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var contractId = GetIntValue(reader, "ContractId", "contractId", "Id", "id");
                if (!contractId.HasValue)
                {
                    continue;
                }

                var companyId = GetIntValue(reader, "CompanyId", "companyId");
                var dismantleDue = GetDateValue(reader, "installDueDate") ?? dueDate;
                var apiKey = GetStringValue(reader, "rentacApiKey");
                if (apiKey == null)
                {
                    throw new InvalidOperationException("Invalid or empty API Key for :company:" + companyId);
                }
                contracts.Add(new MessageDto
                {
                    MetaData = "1105," + contractId.Value,
                    CompanyId = companyId,
                    Type = "whatsapp",
                    RentacApiKey = apiKey
                });
            }
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Error in queueDismantleDueContractReminders function.");
        }



        if (contracts.Count == 0)
        {
            log.LogInformation("No contracts found with dismantle due date on {DueDate}.", dueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            return;
        }

        var serviceBusConnection = _configuration["azureServiceBus"];
        if (string.IsNullOrWhiteSpace(serviceBusConnection))
        {
            throw new InvalidOperationException("Missing required configuration: azureServiceBus");
        }

        await using var client = new ServiceBusClient(serviceBusConnection);
        await using ServiceBusSender sender = client.CreateSender(queueName);

        var sentCount = 0;
        foreach (var contract in contracts)
        {
            try
            {
                var body = JsonSerializer.Serialize(contract);
                await sender.SendMessageAsync(new ServiceBusMessage(body));
                sentCount++;
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to send contract reminder for Contract {MetaData}.", contract.MetaData);
            }
        }

        log.LogInformation(
            "Queued dismantle reminder messages. DueDate: {DueDate}, Found: {FoundCount}, Sent: {SentCount}, Queue: {QueueName}.",
            dueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            contracts.Count,
            sentCount,
            queueName);
    }

    //[Function("Function1")]
    //public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
    //{
    //    _logger.LogInformation("C# Timer trigger function executed at: {executionTime}", DateTime.Now);

    //    if (myTimer.ScheduleStatus is not null)
    //    {
    //        _logger.LogInformation("Next timer schedule at: {nextSchedule}", myTimer.ScheduleStatus.Next);
    //    }
    //} 

    [Function("delayedContractActivity1AM")]
    public async Task delayedContractActivity1AM([TimerTrigger("0 0 1 * * *")] TimerInfo myTimer, ILogger log)
    {
        try
        {
            var _notifyUrl = _configuration["NOTIFY_URL"] + "notify/CollectContractReminders";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_notifyUrl);
                var response = await client.PostAsync(_notifyUrl, null);
            }
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            Console.Write($"C# Timer trigger function executed at: {DateTime.Now}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    [Function("pickupReminder1AM")]
    public async Task pickupReminder1AM([TimerTrigger("0 0 1 * * *")] TimerInfo myTimer, ILogger log)
    {
        try
        {
            var _notifyUrl = _configuration["NOTIFY_URL"] + "notify/CollectPickupReminders";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_notifyUrl);
                var response = await client.PostAsync(_notifyUrl, null);
            }
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            Console.Write($"C# Timer trigger function executed at: {DateTime.Now}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }


    /// <summary>
    /// runs for templateCode 1108,1109
    /// </summary>
    /// <param name="message"></param>
    /// <param name="log"></param>
    /// <returns></returns>
    [Function("contractRemindersQueueTrigger")]
    public async Task contractRemindersQueueTrigger(
        [ServiceBusTrigger("contractreminders", Connection = "azureServiceBus")] string message,
        ILogger log)
    {
        try
        {
            _logger.LogInformation("Received contract reminder queue message: {Message}", message);
            string? token = null;
            string? xCompanyId = null;
            string? apiKey = null;
            using (var json = JsonDocument.Parse(message))
            {
                if (json.RootElement.TryGetProperty("Token", out var tokenProp))
                {
                    token = tokenProp.GetString();
                }
                if (json.RootElement.TryGetProperty("CompanyId", out var companyProp))
                {
                    xCompanyId = Convert.ToString(companyProp.GetInt32());
                }
                if (json.RootElement.TryGetProperty("RentacApiKey", out var apiKeyProp))
                {
                    apiKey = Convert.ToString(apiKeyProp.GetString());
                }
            }

            var notifyUrl = Environment.GetEnvironmentVariable("API_URL") + "api/notify/sendNotification";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(notifyUrl);
                using var request = new HttpRequestMessage(HttpMethod.Post, notifyUrl);
                request.Content = new StringContent(message, Encoding.UTF8, "application/json");
                if (!string.IsNullOrWhiteSpace(token))
                {
                    if (AuthenticationHeaderValue.TryParse(token, out var authHeader))
                    {
                        request.Headers.Authorization = authHeader;
                    }
                    else
                    {
                        request.Headers.TryAddWithoutValidation("Authorization", token);
                    }


                }
                request.Headers.TryAddWithoutValidation("ApiKey", apiKey);
                if (!string.IsNullOrWhiteSpace(xCompanyId))
                {
                    request.Headers.TryAddWithoutValidation("x-companyId", xCompanyId);
                }
                var response = await client.SendAsync(request);
                var b = await response.Content.ReadAsStringAsync();
                log.LogInformation(b);
                response.EnsureSuccessStatusCode();
            }

            log.LogInformation("Processed contract reminder queue message successfully.");
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Error while processing contract reminder queue message.");
            throw;
        }
    }

    private static int? GetIntValue(SqlDataReader reader, params string[] candidateColumns)
    {
        var value = GetValue(reader, candidateColumns);
        if (value == null || value == DBNull.Value)
        {
            return null;
        }

        return Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }
    private static String? GetStringValue(SqlDataReader reader, params string[] candidateColumns)
    {
        var value = GetValue(reader, candidateColumns);
        if (value == null || value == DBNull.Value)
        {
            return null;
        }

        return Convert.ToString(value);
    }
    private static DateTime? GetDateValue(SqlDataReader reader, params string[] candidateColumns)
    {
        var value = GetValue(reader, candidateColumns);
        if (value == null || value == DBNull.Value)
        {
            return null;
        }

        return Convert.ToDateTime(value, CultureInfo.InvariantCulture);
    }

    private static object? GetValue(SqlDataReader reader, params string[] candidateColumns)
    {
        foreach (var columnName in candidateColumns)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.GetValue(ordinal);
            }
            catch (IndexOutOfRangeException)
            {
                // Try next possible column alias.
            }
        }

        return null;
    }

    private sealed class MessageDto
    {
        public int? CompanyId { get; init; }
        public string MetaData { get; init; }
        public string Type { get; set; }

        public string RentacApiKey { get; set; }
    }
}