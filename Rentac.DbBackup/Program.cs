 
using Rentac.DbBackup;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<DbBackupRepository>();
builder.Services.AddSingleton<SqlServerToAccessBackupService>();
 


builder.Services.AddHostedService<Worker>();
var host = builder.Build();
host.Run();
