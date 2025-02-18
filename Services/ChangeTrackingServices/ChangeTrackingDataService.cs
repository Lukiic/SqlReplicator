using SQLReplicator.Domain.Interfaces;
using SQLReplicator.Domain.Services;

namespace SQLReplicator.Services.ChangeTrackingServices
{
    public class ChangeTrackingDataService : IChangeTrackingDataService
    {
        private IExecuteSqlQueryService _executeSqlQueryService;

        public ChangeTrackingDataService(IExecuteSqlQueryService executeSqlQueryService)
        {
            _executeSqlQueryService = executeSqlQueryService;
        }
        public IDataReaderWrapper LoadData(string tableName, string lastChangeID)
        {
            string query = $"SELECT * FROM {tableName}Changes WHERE ChangeID > {lastChangeID} ORDER BY ChangeID ASC";   // SELECT without ORDER BY may not give us proper order

            return _executeSqlQueryService.ExecuteQuery(query);
        }
    }
}
