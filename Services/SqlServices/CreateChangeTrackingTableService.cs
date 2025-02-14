using Serilog;
using SQLReplicator.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLReplicator.Services.SqlServices
{
    public class CreateChangeTrackingTableService : ICreateChangeTrackingTableService
    {
        private IExecuteSqlCommandService executeSqlCommandService;

        public CreateChangeTrackingTableService(IExecuteSqlCommandService executeSqlCommandService)
        {
            this.executeSqlCommandService = executeSqlCommandService;
        }

        /*
            This method creates new table for tracking changes of table {tableName}.
            Table for tracking changes is made of all columns of the first table, with one additional column for operation type.
        */
        public bool CreateCTTable(string tableName)
        {
            string command = $@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}Changes')
                BEGIN
                    SELECT * INTO {tableName}Changes FROM {tableName} WHERE 1 = 0;
                    ALTER TABLE {tableName}Changes ADD Operation CHAR(1);
                END;";

            bool isCreated = true;
            try
            {
                executeSqlCommandService.ExecuteCommand(command);
            }
            catch (Exception)
            {
                isCreated = false;
            }

            return isCreated;
        }
    }
}
