using Serilog;
using SQLReplicator.Domain.Interfaces;
using SQLReplicator.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLReplicator.Services.CommandPreparationServices
{
    public class SqlCommandsGenerationService : ISqlCommandsGenerationService
    {
        private IChangeTrackingDataService _changeTrackingDataService;

        public SqlCommandsGenerationService(IChangeTrackingDataService changeTrackingDataService)
        {
            _changeTrackingDataService = changeTrackingDataService;
        }

        public List<string> GetCommands(string tableName)
        {
            List<string> commands = new List<string>();

            IDataReaderWrapper dataReader = _changeTrackingDataService.LoadData(tableName);
            List<string> attributes = dataReader.ReadAttributes();
            List<List<string>> listOfValues = dataReader.ReadValues();      // Rows of Change Tracking table

            dataReader.Dispose();
            _changeTrackingDataService.DeleteData(tableName);   // Data has been loaded, so it is now deleted from table


            attributes.RemoveAt(attributes.Count - 1);  // Removing 'Operation' attribute (attribute of Change Tracking table, not of the table whose changes are tracked)

            int numOfRows = listOfValues.Count;
            Log.Information($"There are {numOfRows} new rows on {tableName} Change Tracking table.");

            for (int i = 0; i < numOfRows; ++i)    // Reading all rows of change tracking table
            {
                List<string> values = listOfValues[i];
                char operation = values.ElementAt(values.Count - 1).ElementAt(0);   // 'Operation' is the last column of Change Tracking table
                values.RemoveAt(values.Count - 1);  // Removing value for 'Operation' attribute

                switch(operation)
                {
                    case 'I':   // INSERT operation
                        ProcessInsertCommand(tableName, attributes, values, commands);
                        break;

                    case 'D':   // DELETE operation
                        ProcessDeleteCommand(tableName, attributes, values, commands);
                        break;

                    case 'U':   // Update operation -> Always 2 rows in Change Tracking table. One with new, and other with old values
                        if (i+1 >= numOfRows)
                        {
                            Log.Warning("Unexpected behaviour with UPDATE operation in Change Tracking table.");
                            break;
                        }

                        ++i;
                        List<string> oldValues = listOfValues[i];
                        ProcessUpdateCommand(tableName, attributes, values, commands, oldValues);
                        break;

                    default:
                        Log.Warning("Unexpected data in Change Tracking table: [" + string.Join(", ", values) + "]");
                        break;
                }
            }

            return commands;
        }

        private void ProcessInsertCommand(string tableName, List<string> attributes, List<string> values, List<string> commands)
        {
            commands.Add(SqlSyntaxFormattingService.GetInsertCommand(tableName, attributes, values));
            Log.Information("INSERT operation tracked.");
        }

        private void ProcessDeleteCommand(string tableName, List<string> attributes, List<string> values, List<string> commands)
        {
            commands.Add(SqlSyntaxFormattingService.GetDeleteCommand(tableName, attributes, values));
            Log.Information("DELETE operation tracked.");
        }

        private void ProcessUpdateCommand(string tableName, List<string> attributes, List<string> values, List<string> commands, List<string> oldValues)
        {
            if (oldValues.Count == 0 || oldValues.ElementAt(oldValues.Count - 1).ElementAt(0) != 'O')
            {
                Log.Warning("Unexpected behaviour with UPDATE operation in Change Tracking table.");
                return;
            }

            oldValues.RemoveAt(oldValues.Count - 1);
            commands.Add(SqlSyntaxFormattingService.GetUpdateCommand(tableName, attributes, values, oldValues));
            Log.Information("UPDATE operation tracked.");
        }
    }
}
