using Microsoft.Data.SqlClient;
using Serilog;
using SQLReplicator.Domain.Interfaces;
using SQLReplicator.Domain.Services;
using SQLReplicator.Services.FileServices;
using SQLReplicator.Services.LoggerServices;
using SQLReplicator.Services.SqlServices;
using System.Data.SqlClient;

namespace SQLReplicator.Application
{
    public class Program
    {
        static void Main(string[] args)
        {
            LoggerService.InitializeLogger();
            Log.Information("Application has started.");

            #region ValidatingApplicationArguments
            if (args.Length != 1)
            {
                Log.Error("Wrong number of application arguments.");
                Console.WriteLine("Expected: filePath");
                return;
            }
            string filePath = args[0];
            #endregion

            #region ValidatingFileData
            List<string> dataFromFile = FileImportService.LoadData(filePath);

            if (dataFromFile.Count != 3)
            {
                Log.Error("File doesn't include necessary data.");
                Console.WriteLine("Expected: Source server connection string, destination server connection string, Table name");
                return;
            }

            string srcConnectionString = dataFromFile[0];
            string destConnectionString = dataFromFile[1];
            string tableName = dataFromFile[2];
            #endregion

            #region ConnectingToServers
            SqlConnection srcConnection = new SqlConnection(srcConnectionString);
            try
            {
                srcConnection.Open();
                Log.Information("Connection to source server is opened.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Connection to source server can't be opened.");
                return;
            }

            SqlConnection destConnection = new SqlConnection(destConnectionString);
            try
            {
                destConnection.Open();
                Log.Information("Connection to destination server is opened.");
            }
            catch (Exception)
            {
                srcConnection.Close();
                Log.Error("Connection to destination server can't be opened.");
                return;
            }
            #endregion

            IExecuteSqlQueryService executeSqlQueryService = new ExecuteSqlQueryService(srcConnection);

            #region ClosingServerConnections
            srcConnection.Close();
            Log.Information("Connection to source server is closed.");
            destConnection.Close();
            Log.Information("Connection to destination server is closed.");
            #endregion

            Log.Information("Application has finished replication.");
            LoggerService.CloseLogger();
        }
    }
}
