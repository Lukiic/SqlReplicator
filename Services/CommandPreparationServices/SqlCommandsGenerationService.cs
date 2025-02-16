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
        public List<string> GetCommands(string tableName, List<string> changeTrackingAttributes, List<List<string>> listOfValues)
        {
            List<string> commands = new List<string>();
            List<string> trackedTableAttributes = changeTrackingAttributes.GetRange(0, changeTrackingAttributes.Count - 1); // Tracked table doesn't have 'Operation' column, so it's removed

            int numOfRows = listOfValues.Count;
            Log.Information($"There are {numOfRows} new rows on {tableName} Change Tracking table.");

            for (int i = 0; i < numOfRows; ++i)    // Reading all rows of change tracking table
            {
                List<string> values = listOfValues[i];
                char operation = values.ElementAt(values.Count - 1).ElementAt(0);

                switch(operation)
                {
                    case 'I':   // INSERT operation
                        ProcessInsertCommand(tableName, changeTrackingAttributes, trackedTableAttributes, values, commands);
                        break;

                    case 'D':   // DELETE operation
                        ProcessDeleteCommand(tableName, changeTrackingAttributes, trackedTableAttributes, values, commands);
                        break;

                    case 'U':   // Update operation -> Always 2 rows in Change Tracking table. One with new, and other with old values
                        if (i+1 >= numOfRows)
                        {
                            Log.Warning("Unexpected behaviour with UPDATE operation in Change Tracking table.");
                            break;
                        }

                        ++i;
                        List<string> oldValues = listOfValues[i];
                        ProcessUpdateCommand(tableName, changeTrackingAttributes, trackedTableAttributes, values, commands, oldValues);
                        break;

                    default:
                        Log.Warning("Unexpected data in Change Tracking table: [" + string.Join(", ", values) + "]");
                        break;
                }
            }

            return commands;
        }

        /*
            Processing Change Tracking table data:
                1) Add command for corresponding DML operation
                2) Add command for deleting row from Change Tracking table (changes which are result of replication should not be tracked - prevent loop)
        */
        private void ProcessInsertCommand(string tableName, List<string> changeTrackingAttributes, List<string> trackedTableAttributes, List<string> values, List<string> commands)
        {
            commands.Add(SqlSyntaxFormattingService.GetInsertCommand(tableName, trackedTableAttributes, values));

            commands.Add(SqlSyntaxFormattingService.GetDeleteCommand($"{tableName}Changes", changeTrackingAttributes, values));

            Log.Information("INSERT operation tracked.");
        }

        private void ProcessDeleteCommand(string tableName, List<string> changeTrackingAttributes, List<string> trackedTableAttributes, List<string> values, List<string> commands)
        {
            commands.Add(SqlSyntaxFormattingService.GetDeleteCommand(tableName, trackedTableAttributes, values));

            commands.Add(SqlSyntaxFormattingService.GetDeleteCommand($"{tableName}Changes", changeTrackingAttributes, values));

            Log.Information("DELETE operation tracked.");
        }

        private void ProcessUpdateCommand(string tableName, List<string> changeTrackingAttributes, List<string> trackedTableAttributes, List<string> values, List<string> commands, List<string> oldValues)
        {
            if (oldValues.Count == 0 || oldValues.ElementAt(oldValues.Count - 1).ElementAt(0) != 'O')
            {
                Log.Warning("Unexpected behaviour with UPDATE operation in Change Tracking table.");
                return;
            }

            commands.Add(SqlSyntaxFormattingService.GetUpdateCommand(tableName, trackedTableAttributes, values, oldValues));

            commands.Add(SqlSyntaxFormattingService.GetDeleteCommand($"{tableName}Changes", changeTrackingAttributes, values));
            commands.Add(SqlSyntaxFormattingService.GetDeleteCommand($"{tableName}Changes", changeTrackingAttributes, oldValues));

            Log.Information("UPDATE operation tracked.");
        }    
    }
}
