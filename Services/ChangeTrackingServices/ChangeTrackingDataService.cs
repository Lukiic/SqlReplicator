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
        public IDataReaderWrapper LoadData(string tableName, string lastChangeID, List<string> keyAttributes)
        {
            string keysFormat = string.Join(" AND ", keyAttributes.Zip(keyAttributes, (k1, k2) => $"{tableName}Changes.{k1} = {tableName}.{k2}"));

            string query = @$"SELECT *
                           FROM {tableName}Changes LEFT OUTER JOIN {tableName} ON {keysFormat}
                           WHERE ChangeID > {lastChangeID}
                           ORDER BY ChangeID ASC";   // SELECT without ORDER BY may not give us proper order

            return _executeSqlQueryService.ExecuteQuery(query);
        }
    }
}
