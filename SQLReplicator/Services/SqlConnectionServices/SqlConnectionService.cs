using Microsoft.Data.SqlClient;
using Serilog;
using SQLReplicator.Application;
using SQLReplicator.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLReplicator.Services.SqlConnectionServices
{
    public class SqlConnectionService : ISqlConnectionService
    {
        private AppState appState;

        public SqlConnectionService(AppState appState)
        {
            this.appState = appState;
            AppTerminationHandler.SetupHandler(appState);   // CTRL+C Handler
        }

        public void OpenConnection(SqlConnection sqlConnection)
        {
            while (appState.ShouldRun)
            {
                try
                {
                    sqlConnection.Open();
                    Log.Information($"Successfully established a connection to the database server \"{sqlConnection.Database}\".");

                    break;
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, $"Application cannot start: Failed to establish a connection to the database server \"{sqlConnection.Database}\".");
                    Log.Debug("Sleeping for 10 seconds before trying again.");
                    Thread.Sleep(10_000);
                }
            }
        }

        public void CloseConnection(SqlConnection sqlConnection)
        {
            sqlConnection.Close();
            Log.Information($"Database server \"{sqlConnection.Database}\" connection successfully closed.");
        }
    }
}
