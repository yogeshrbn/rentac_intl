using Renac.ReminderService;
using Serilog;


var builder = Host.CreateApplicationBuilder(args);

string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "service_log.txt");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(logPath,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14)
    .CreateLogger();


// Replace the default logging with Serilog
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

builder.Services.AddHostedService<Worker>();

builder.Services.ConfigureHttpClientDefaults(client =>
{
    client.ConfigureHttpClient(c =>
    {
        c.Timeout = TimeSpan.FromMinutes(5);
    });
});
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Rentac.ReminderService";
});

var host = builder.Build();
host.Run();