using Serilog;
using SQLReplicator.Domain.Services;

namespace SQLReplicator.Services.CommandPreparationServices
{
    public class SqlCommandsGenerationService : ISqlCommandsGenerationService
    {
        /*
            Values: ID1, ID2, ..., IDn, Operation, ChangeID, IsReplicated1, IsReplicated2, Attr1, Attr2, AttrM
                    -----------------------------                                          -------------------
                    ChangeTracking table values                                            Tracked table values
        */
        /*
            Processing Change Tracking table data:
                1) Add command for corresponding DML operation
                2) Add command for deleting row from Change Tracking table (changes which are result of replication should not be tracked - prevent loop)
        */
        public List<string> GetCommands(string tableName, List<string> changeTrackingAttrs, List<string> trackedTableAttrs, List<string> keyAttributes, List<List<string>> listOfValues)
        {
            ValidateArguments(tableName, changeTrackingAttrs, trackedTableAttrs, keyAttributes, listOfValues);

            int numOfRows = listOfValues.Count;
            List<string> commands = new List<string>(2 * numOfRows);

            Log.Information($"{numOfRows} rows read from the Change Tracking table.");

            for (int i = 0; i < numOfRows; ++i)    // Reading all rows of change tracking table
            {
                List<string> values = listOfValues[i];
                char operation = values[keyAttributes.Count][0];

                switch (operation)
                {
                    case 'I':   // INSERT operation
                        List<string> trackedTableValues = values.Skip(changeTrackingAttrs.Count + 3).ToList();

                        ProcessInsertCommand(tableName, trackedTableAttrs, trackedTableValues, commands);
                        ProcessDeleteCommand($"{tableName}Changes", changeTrackingAttrs, values, commands);
                        break;

                    case 'D':   // DELETE operation
                        ProcessDeleteCommand(tableName, keyAttributes, values, commands);
                        ProcessDeleteCommand($"{tableName}Changes", changeTrackingAttrs, values, commands);
                        break;

                    default:
                        Log.Warning($"Unexpected value for 'Operation' column in Change Tracking table: {operation}. Row will be ignored");
                        break;
                }
            }

            if (numOfRows > 0)
            {
                Log.Information("Data from the Change Tracking table has been successfully imported.");
            }

            // Related commands INSERT/DELETE in tracked table and DELETE from Change Tracking table are joined in one string - because of transaction execution
            commands = commands.Chunk(2).Select(pair => string.Concat(pair)).ToList();

            return commands;
        }
        private void ValidateArguments(string tableName, List<string> changeTrackingAttrs, List<string> trackedTableAttrs, List<string> keyAttributes, List<List<string>> listOfValues)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException("Table name cannot be null, empty, or whitespace.", nameof(tableName));
            }

            if (changeTrackingAttrs == null || changeTrackingAttrs.Count == 0)
            {
                throw new ArgumentException("Change tracking attributes cannot be null or empty.", nameof(changeTrackingAttrs));
            }

            if (trackedTableAttrs == null || trackedTableAttrs.Count == 0)
            {
                throw new ArgumentException("Tracked table attributes cannot be null or empty.", nameof(trackedTableAttrs));
            }

            if (keyAttributes == null || keyAttributes.Count == 0)
            {
                throw new ArgumentException("Key attributes cannot be null or empty.", nameof(keyAttributes));
            }

            if (listOfValues == null)
            {
                throw new ArgumentException("List of values cannot be null.", nameof(listOfValues));
            }

            if (changeTrackingAttrs.Count != keyAttributes.Count + 1)
            {
                throw new ArgumentException("Change tracking attributes must have exactly one more element than key attributes.");
            }

            if (trackedTableAttrs.Count < keyAttributes.Count)
            {
                throw new ArgumentException("Tracked table attributes must have at least as many elements as key attributes.");
            }

            int expectedListSize = changeTrackingAttrs.Count + trackedTableAttrs.Count + 3;
            if (listOfValues.Any(values => values == null || values.Count != expectedListSize))
            {
                throw new ArgumentException($"Each inner list in listOfValues must contain exactly {expectedListSize} elements.");
            }
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
