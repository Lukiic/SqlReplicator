using Microsoft.Data.SqlClient;
using Serilog;
using SQLReplicator.Domain.Services;
using SQLReplicator.Services.ChangeTrackingServices;
using SQLReplicator.Services.CommandExecutionServices;
using SQLReplicator.Services.CommandPreparationServices;
using SQLReplicator.Services.FileServices;
using SQLReplicator.Services.LoggerServices;
using SQLReplicator.Services.SqlConnectionServices;
using SQLReplicator.Services.TrackedTableServices;
using System.Transactions;

namespace SQLReplicator.Application
{
    public class Program
    {
        private static AppState appState = new AppState();
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

            if (dataFromFile.Count != 3)
            {
                Log.Fatal("Application cannot start: Missing or invalid configuration data in the file. Expected fields: Source server connection string, Destination server connection string, Instance ID (number of replicated bit).");
                return;
            }

            string srcConnectionString = dataFromFile[0];
            string destConnectionString = dataFromFile[1];
            string replicatedBitNum = dataFromFile[2];
            #endregion

            #region ConnectingToServers
            ISqlConnectionService sqlConnectionService = new SqlConnectionService(appState);

            SqlConnection srcConnection = new SqlConnection(srcConnectionString);
            SqlConnection destConnection = new SqlConnection(destConnectionString);

            sqlConnectionService.OpenConnection(srcConnection);
            sqlConnectionService.OpenConnection(destConnection);
            #endregion

            #region InitializingServices
            IExecuteSqlQueryService executeQueriesSrc = new ExecuteSqlQueryService(srcConnection);
            IExecuteSqlCommandService executeCommandsSrc = new ExecuteSqlCommandService(srcConnection);
            IExecuteSqlCommandService executeCommandsDest = new ExecuteSqlCommandService(destConnection);

            IPrimaryKeyAttributesService primaryKeyAttributesService = new PrimaryKeyAttributesService(executeQueriesSrc);
            ITableNamesFetcherService tableNamesFetcher = new TableNamesFetcherService(executeQueriesSrc);

            IChangeTrackingDataService changeTrackingDataService = new ChangeTrackingDataService(executeQueriesSrc);
            ISqlCommandsGenerationService sqlCommandsGeneration = new SqlCommandsGenerationService();
            ITrackedDataToCommandsService trackedDataToCommands = new TrackedDataToCommandsService(changeTrackingDataService, sqlCommandsGeneration);
            IExecuteListOfCommandsService executeListOfCommands = new ExecuteListOfCommandsService(executeCommandsDest);

            IUpdateChangeTrackingTableService updateChangeTrackingTable = new UpdateChangeTrackingTableService(executeCommandsSrc);

            ICreateChangeTrackingTableService createTableService = new CreateChangeTrackingTableService(executeCommandsSrc);
            ICreateTriggerService createTriggerService = new CreateTriggerService(executeCommandsSrc);
            #endregion

            #region GettingAllTableNames
            List<string> tableNames = tableNamesFetcher.GetTableNames();
            #endregion

            List<List<string>> keyAttrsForEachTable = new List<List<string>>();

            foreach (string tableName in tableNames)
            {
                #region ReadingKeyAttributesOfInputTable
                List<string> keyAttributes = primaryKeyAttributesService.GetPrimaryKeyAttributes(tableName);
                keyAttrsForEachTable.Add(keyAttributes);
                #endregion

                #region SettingUpSourceServerForTrackingChanges
                if (!createTableService.CreateCTTable(tableName, keyAttributes))
                {
                    Log.Fatal($"Application cannot proceed: Failed to create Change Tracking table for \"{tableName}\" table.");
                    return;
                }

                if (!createTriggerService.CreateTrigger(tableName, keyAttributes))
                {
                    Log.Fatal($"Application cannot proceed: Failed to create Change Tracking trigger for \"{tableName}\" table.");
                    return;
                }
                #endregion
            }

            #region ClosingServerConnections
            sqlConnectionService.CloseConnection(srcConnection);
            sqlConnectionService.CloseConnection(destConnection);
            #endregion

            while (appState.ShouldRun)
            {
                for (int i = 0; i < tableNames.Count; ++i)
                {
                    string tableName = tableNames[i];
                    List<string> keyAttributes = keyAttrsForEachTable[i];
                    Log.Information($"Processing \"{tableName}\" table.");

                    TransactionManager.ImplicitDistributedTransactions = true;
                    using (TransactionScope scope = new TransactionScope()) // Distributed transaction
                    {
                        #region OpeningServerConnections
                        sqlConnectionService.OpenConnection(srcConnection);
                        sqlConnectionService.OpenConnection(destConnection);
                        #endregion

                        #region GettingDmlCommandsToBeExecuted
                        List<string> commandsForDestServer;
                        int lastChangeID;
                        (commandsForDestServer, lastChangeID) = trackedDataToCommands.GetCommandsAndLastChangeID(tableName, replicatedBitNum, keyAttributes);
                        #endregion

                        #region ExecutingCommandsOnDestinationServer
                        int unexecutedCommandsCount = executeListOfCommands.ExecuteCommands(commandsForDestServer);
                        #endregion

                        #region UpdatingReplicatedBitOfChangeTrackingTable
                        int changeID = lastChangeID - unexecutedCommandsCount;

                        if (!updateChangeTrackingTable.UpdateReplicatedBit(tableName, changeID, replicatedBitNum))
                        {
                            Log.Warning("Unable to update replicated bit in the Change Tracking table. The same data may be replicated again in the next execution if the issue persists.");
                        }
                        else
                        {
                            scope.Complete();   // Transaction is completed only if ReplicatedBit is updated
                        }
                        #endregion
                    }

                    #region ClosingServerConnections
                    sqlConnectionService.CloseConnection(srcConnection);
                    sqlConnectionService.CloseConnection(destConnection);
                    #endregion
                }

                Log.Debug("Sleeping for 10 seconds before the next iteration.");
                Thread.Sleep(10_000);
            }

            Log.Information("Application has finished executing and is shutting down.");
            LoggerService.CloseLogger();
        }
    }
}
