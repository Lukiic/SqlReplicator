﻿using Serilog;
using SQLReplicator.Domain.Services;

namespace SQLReplicator.Services.ChangeTrackingServices
{
    public class CreateChangeTrackingTableService : ICreateChangeTrackingTableService
    {
        private IExecuteSqlCommandService _executeSqlCommandService;

        public CreateChangeTrackingTableService(IExecuteSqlCommandService executeSqlCommandService)
        {
            _executeSqlCommandService = executeSqlCommandService;
        }

        /*
            This method creates new table for tracking changes of table {tableName}.
            Table for tracking changes is made of all columns of the first table, with one additional column for operation type.
        */
        public bool CreateCTTable(string tableName, List<string> keyAttributes)
        {
            ValidateArguments(tableName, keyAttributes);

            string command = $@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}Changes')
                BEGIN
                    SELECT TOP 0 {string.Join(',', keyAttributes)} INTO {tableName}Changes FROM {tableName}
                    ALTER TABLE {tableName}Changes ADD Operation CHAR(1), ChangeID BIGINT IDENTITY(1,1), IsReplicated1 BIT, IsReplicated2 BIT;
                END;";

            bool isCreated = true;
            try
            {
                _executeSqlCommandService.ExecuteCommand(command);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"Failed to execute command: {command}");
                isCreated = false;
            }

            return isCreated;
        }

        private void ValidateArguments(string tableName, List<string> keyAttributes)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException("Table name cannot be null or empty.", nameof(tableName));
            }

            if (keyAttributes == null || keyAttributes.Count == 0)
            {
                throw new ArgumentException("Key attributes list cannot be null or empty.", nameof(keyAttributes));
            }

            if (keyAttributes.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException("Key attributes cannot contain null or empty values.", nameof(keyAttributes));
            }
        }
    }
}
