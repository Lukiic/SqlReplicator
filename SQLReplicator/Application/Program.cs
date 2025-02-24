using Microsoft.Data.SqlClient;
using Serilog;
using SQLReplicator.Domain.Services;
using SQLReplicator.Services.ChangeTrackingServices;
using SQLReplicator.Services.CommandExecutionServices;
using SQLReplicator.Services.CommandPreparationServices;
using SQLReplicator.Services.FileServices;
using SQLReplicator.Services.LoggerServices;
using SQLReplicator.Services.TrackedTableServices;

namespace SQLReplicator.Application
{
    public class Program
    {
        private static bool shouldRun = true;
        static void Main(string[] args)
        {
            LoggerService.InitializeLogger();
            Log.Information("Application is starting.");

            #region ValidatingApplicationArguments
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                Log.Fatal("Application cannot start: Missing required argument for file path.");
                return;
            }
            string filePath = args[0];
            #endregion

            #region ValidatingFileData
            List<string> dataFromFile = FileImportService.LoadData(filePath);

            if (dataFromFile.Count != 4)
            {
                Log.Fatal("Application cannot start: Missing or invalid configuration data in the file. Expected fields: Source server connection string, Destination server connection string, Table name, Instance ID (number of replicated bit).");
                return;
            }

            string srcConnectionString = dataFromFile[0];
            string destConnectionString = dataFromFile[1];
            string tableName = dataFromFile[2];
            string replicatedBitNum = dataFromFile[3];
            #endregion

            #region ConnectingToServers
            SqlConnection srcConnection = new SqlConnection(srcConnectionString);
            try
            {
                srcConnection.Open();
                Log.Information("Successfully established a connection to the source server.");
            }
            catch (Exception ex)
            {
                Log.Fatal("Application cannot start: Failed to establish a connection to the source server.");
                return;
            }

            SqlConnection destConnection = new SqlConnection(destConnectionString);
            try
            {
                destConnection.Open();
                Log.Information("Successfully established a connection to the destination server.");
            }
            catch (Exception)
            {
                srcConnection.Close();
                Log.Fatal("Application cannot start: Failed to establish a connection to the destination server.");
                return;
            }
            #endregion

            #region InitializingServices
            IExecuteSqlQueryService executeQueriesSrc = new ExecuteSqlQueryService(srcConnection);
            IExecuteSqlCommandService executeCommandsSrc = new ExecuteSqlCommandService(srcConnection);
            IExecuteSqlCommandService executeCommandsDest = new ExecuteSqlCommandService(destConnection);

            IPrimaryKeyAttributesService primaryKeyAttributesService = new PrimaryKeyAttributesService(executeQueriesSrc);
            IChangeTrackingDataService changeTrackingDataService = new ChangeTrackingDataService(executeQueriesSrc);
            ISqlCommandsGenerationService sqlCommandsGeneration = new SqlCommandsGenerationService();
            ITrackedDataToCommandsService trackedDataToCommands = new TrackedDataToCommandsService(changeTrackingDataService, sqlCommandsGeneration);
            IExecuteListOfCommandsService executeListOfCommands = new ExecuteListOfCommandsService(executeCommandsDest);

            IUpdateChangeTrackingTableService updateChangeTrackingTable = new UpdateChangeTrackingTableService(executeCommandsSrc);
            #endregion

            #region ReadingKeyAttributesOfInputTable
            List<string> keyAttributes = primaryKeyAttributesService.GetPrimaryKeyAttributes(tableName);
            int keyAttributesCount = keyAttributes.Count;
            #endregion

            #region SettingUpSourceServerForTrackingChanges
            ICreateChangeTrackingTableService createTableService = new CreateChangeTrackingTableService(executeCommandsSrc);
            ICreateTriggerService createTriggerService = new CreateTriggerService(executeCommandsSrc);

            if (!createTableService.CreateCTTable(tableName, keyAttributes))
            {
                Log.Fatal("Application cannot proceed: Failed to create Change Tracking table.");
                return;
            }

            if (!createTriggerService.CreateTrigger(tableName, keyAttributes))
            {
                Log.Fatal($"Application cannot proceed: Failed to create Change Tracking trigger.");
                return;
            }
            #endregion

            Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelKeyPressHandler);
            while (shouldRun)
            {
                #region GettingDmlCommandsToBeExecuted
                List<string> commandsForDestServer;
                string lastChangeID;
                (commandsForDestServer, lastChangeID) = trackedDataToCommands.GetCommandsAndLastChangeID(tableName, replicatedBitNum, keyAttributes);
                #endregion

                #region ExecutingCommandsOnDestinationServer
                executeListOfCommands.ExecuteCommands(commandsForDestServer);
                #endregion

                #region UpdatingReplicatedBitOfChangeTrackingTable
                if (!updateChangeTrackingTable.UpdateReplicatedBit(tableName, lastChangeID, replicatedBitNum))
                {
                    Log.Warning("Unable to update replicated bit in the Change Tracking table. The same data may be replicated again in the next execution if the issue persists.");
                }
                #endregion

                Log.Debug("Sleeping for 10 seconds before the next iteration.");
                Thread.Sleep(10_000);
            }

            #region ClosingServerConnections
            srcConnection.Close();
            Log.Information("Source server connection successfully closed.");
            destConnection.Close();
            Log.Information("Destination server connection successfully closed.");
            #endregion

            Log.Information("Application has finished executing and is shutting down.");
            LoggerService.CloseLogger();
        }

        // Handler for CTRL+C action stops the while-true loop
        private static void CancelKeyPressHandler(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            shouldRun = false;
            Log.Information("Termination signal received. Exiting main loop and preparing for shutdown.");
        }
    }
}
