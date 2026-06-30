using DbUp;
using DbUp.Engine;
using DbUp.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DbMigration
{
    public class DbMigrationService
    {
        public static void MigrateDatabase(string connectionString, IDictionary<string, string> variables)
        {

            EnsureDatabase.For.SqlDatabase(connectionString);

            var upgrader = DeployChanges.To
                .SqlDatabase(connectionString)
                  .WithVariables(variables)
                  .WithScriptsEmbeddedInAssembly(
                      Assembly.GetExecutingAssembly(), x => x.StartsWith("DbUpMigration.Db."),
                      new SqlScriptOptions { ScriptType = ScriptType.RunOnce, RunGroupOrder = 0 })
                  .WithScriptsEmbeddedInAssembly(
                      Assembly.GetExecutingAssembly(), x => x.StartsWith("DbUpMigration.PreRun."),
                      new SqlScriptOptions { ScriptType = ScriptType.RunOnce, RunGroupOrder = 1 })
                     .WithScriptsEmbeddedInAssembly(
                      Assembly.GetExecutingAssembly(), x => x.StartsWith("DbUpMigration.Views."),
                      new SqlScriptOptions { ScriptType = ScriptType.RunOnce, RunGroupOrder = 2 })
                  .WithScriptsEmbeddedInAssembly(
                      Assembly.GetExecutingAssembly(), x => x.StartsWith("DbUpMigration.PostRun."),
                      new SqlScriptOptions { ScriptType = ScriptType.RunOnce, RunGroupOrder = 3 })
                  .WithScriptsEmbeddedInAssembly(
                      Assembly.GetExecutingAssembly(), x => x.StartsWith("DbUpMigration.Functions."),
                      new SqlScriptOptions { ScriptType = ScriptType.RunOnce, RunGroupOrder = 43 })
                  .WithScriptsEmbeddedInAssembly(
                      Assembly.GetExecutingAssembly(), x => x.StartsWith("DbUpMigration.SP."),
                      new SqlScriptOptions { ScriptType = ScriptType.RunOnce, RunGroupOrder = 5 })

                  .JournalToSqlTable("dbo", "schemaversion")

                 //.JournalTo(new NullJournal())
                 .WithVariablesEnabled()
                //   .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                //.LogToAutodetectedLog()

                .WithTransactionPerScript()
                .Build();
            var scripts = upgrader.GetDiscoveredScripts();
            var sc1 = upgrader.GetScriptsToExecute();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                string msg = result.ErrorScript.Name;
                throw new Exception("Database migration failed:-" + msg, result.Error);
            }

        }
    }
}
