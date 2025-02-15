using SQLReplicator.Domain.Interfaces;
using SQLReplicator.Domain.Models;
using SQLReplicator.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLReplicator.Services.ChangeTrackingServices
{
    public class ChangeTrackingDataService : IChangeTrackingDataService
    {
        private IExecuteSqlCommandService _executeSqlCommandService;
        private IExecuteSqlQueryService _executeSqlQueryService;

        public ChangeTrackingDataService(IExecuteSqlCommandService executeSqlCommandService, IExecuteSqlQueryService executeSqlQueryService)
        {
            _executeSqlCommandService = executeSqlCommandService;
            _executeSqlQueryService = executeSqlQueryService;
        }

        public void DeleteData(string tableName)
        {
            string command = $"DELETE FROM {tableName}Changes";

            _executeSqlCommandService.ExecuteCommand(command);
        }

        public IDataReaderWrapper LoadData(string tableName)
        {
            string query = $"SELECT * FROM {tableName}Changes";

            return _executeSqlQueryService.ExecuteQuery(query);
        }
    }
}
