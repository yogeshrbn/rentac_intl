using DbUp;
using DbUp.Engine;
using DbUp.Support;
using System.Reflection;

namespace DbMigration
{
    public class DbMigrationService
    {
        public static void MigrateDatabase(string connectionString, IDictionary<string, string> variables)
        {

            //  EnsureDatabase.For.SqlDatabase(connectionString);
            EnsureDatabase.For.SqlDatabase(connectionString);
            var upgrader = DeployChanges.To
                .SqlDatabase(connectionString)
                  .WithVariables(variables)
                  .WithScriptsEmbeddedInAssembly(
                      Assembly.GetExecutingAssembly(), x => x.StartsWith("HMS.AlwaysDbUpMigration.Db."),
                      new SqlScriptOptions { ScriptType = ScriptType.RunOnce, RunGroupOrder = 0 })
                  .WithScriptsEmbeddedInAssembly(
                      Assembly.GetExecutingAssembly(), x => x.StartsWith("HMS.AlwaysDbUpMigration.PreRun."),
                      new SqlScriptOptions { ScriptType = ScriptType.RunOnce, RunGroupOrder = 0 })
                  .WithScriptsEmbeddedInAssembly(
                      Assembly.GetExecutingAssembly(), x => x.StartsWith("HMS.AlwaysDbUpMigration.PostRun."),
                      new SqlScriptOptions { ScriptType = ScriptType.RunOnce, RunGroupOrder = 2 })
                  .WithScriptsEmbeddedInAssembly(
                      Assembly.GetExecutingAssembly(), x => x.StartsWith("HMS.AlwaysDbUpMigration.Functions."),
                      new SqlScriptOptions { ScriptType = ScriptType.RunAlways, RunGroupOrder = 3 })
                  .WithScriptsEmbeddedInAssembly(
                      Assembly.GetExecutingAssembly(), x => x.StartsWith("HMS.AlwaysDbUpMigration.SP."),
                      new SqlScriptOptions { ScriptType = ScriptType.RunAlways, RunGroupOrder = 5 })
                  .WithScriptsEmbeddedInAssembly(
                      Assembly.GetExecutingAssembly(), x => x.StartsWith("HMS.AlwaysDbUpMigration.Views."),
                      new SqlScriptOptions { ScriptType = ScriptType.RunAlways, RunGroupOrder = 6 })
                  .JournalToSqlTable("dbo", "schemaversion")

                 //.JournalTo(new NullJournal())
                 .WithVariablesEnabled()
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                //.LogToAutodetectedLog()

                .WithTransactionPerScript()
                .Build();
            var scripts = upgrader.GetDiscoveredScripts();
            var sc1 = upgrader.GetScriptsToExecute();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                throw new Exception("Database migration failed", result.Error);
            }

        }
    }
}
