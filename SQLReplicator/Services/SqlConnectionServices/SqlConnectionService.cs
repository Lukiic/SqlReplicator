using Microsoft.Data.SqlClient;
using Serilog;
using SQLReplicator.Application;
using SQLReplicator.Domain.Services;

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
            ValidateArguments(sqlConnection);

            while (appState.ShouldRun)
            {
                try
                {
                    sqlConnection.Open();
                    Log.Debug($"Successfully established a connection to the database server \"{sqlConnection.Database}\".");

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
            ValidateArguments(sqlConnection);

            sqlConnection.Close();
            Log.Debug($"Database server \"{sqlConnection.Database}\" connection successfully closed.");
        }

        private void ValidateArguments(SqlConnection sqlConnection)
        {
            if (sqlConnection == null)
            {
                throw new ArgumentNullException(nameof(sqlConnection), "SqlConnection cannot be null.");
            }

            if (string.IsNullOrEmpty(sqlConnection.ConnectionString))
            {
                throw new ArgumentException("The connection string cannot be null or empty.", nameof(sqlConnection));
            }
        }
    }
}
