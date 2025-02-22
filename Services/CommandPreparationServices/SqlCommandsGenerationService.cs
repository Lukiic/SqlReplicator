using Serilog;
using SQLReplicator.Domain.Services;

namespace SQLReplicator.Services.CommandPreparationServices
{
    public class SqlCommandsGenerationService : ISqlCommandsGenerationService
    {
        /*
            Values: ID1, ID2, ..., IDn, Operation, ChangeID, Attr1, Attr2, AttrM
                    -----------------------------            -------------------
                    ChangeTracking table values              Tracked table values
        */
        /*
            Processing Change Tracking table data:
                1) Add command for corresponding DML operation
                2) Add command for deleting row from Change Tracking table (changes which are result of replication should not be tracked - prevent loop)
        */
        public List<string> GetCommands(string tableName, List<string> changeTrackingAttrs, List<string> trackedTableAttrs, List<string> keyAttributes, List<List<string>> listOfValues)
        {
            List<string> commands = new List<string>();

            int numOfRows = listOfValues.Count;
            Log.Information($"There are {numOfRows} new rows in {tableName} Change Tracking table.");

            for (int i = 0; i < numOfRows; ++i)    // Reading all rows of change tracking table
            {
                List<string> values = listOfValues[i];
                char operation = values[keyAttributes.Count][0];

                switch (operation)
                {
                    case 'I':   // INSERT operation
                        List<string> trackedTableValues = values.Skip(changeTrackingAttrs.Count + 1).ToList();

                        ProcessInsertCommand(tableName, trackedTableAttrs, trackedTableValues, commands);
                        ProcessDeleteCommand($"{tableName}Changes", changeTrackingAttrs, values, commands);
                        break;

                    case 'D':   // DELETE operation
                        ProcessDeleteCommand(tableName, keyAttributes, values, commands);
                        ProcessDeleteCommand($"{tableName}Changes", changeTrackingAttrs, values, commands);
                        break;

                    default:
                        Log.Warning("Unexpected data in Change Tracking table: [" + string.Join(", ", values) + "]");
                        break;
                }
            }

            if (numOfRows > 0)
            {
                Log.Information($"Data from {tableName} Change Tracking table successfully imported.");
            }

            return commands;
        }
        
        private void ProcessInsertCommand(string tableName, List<string> attributes, List<string> values, List<string> commands)
        {
            commands.Add(SqlSyntaxFormattingService.GetInsertCommand(tableName, attributes, values));
        }

        private void ProcessDeleteCommand(string tableName, List<string> attributes, List<string> values, List<string> commands)
        {
            commands.Add(SqlSyntaxFormattingService.GetDeleteCommand(tableName, attributes, values));
        }
    }
}
