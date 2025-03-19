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
        public IDataReaderWrapper LoadData(string tableName, string replicatedBitNum, List<string> keyAttributes)
        {
            ValidateArguments(tableName, replicatedBitNum, keyAttributes);

            string keysFormat = string.Join(" AND ", keyAttributes.Zip(keyAttributes, (k1, k2) => $"{tableName}Changes.{k1} = {tableName}.{k2}"));

            string query = @$"SELECT *
                           FROM {tableName}Changes LEFT OUTER JOIN {tableName} ON {keysFormat}
                           WHERE IsReplicated{replicatedBitNum} = 0
                           ORDER BY ChangeID ASC";   // SELECT without ORDER BY may not give us proper order

            return _executeSqlQueryService.ExecuteQuery(query);
        }

        private void ValidateArguments(string tableName, string replicatedBitNum, List<string> keyAttributes)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException("Table name cannot be null or empty.", nameof(tableName));
            }

            if (string.IsNullOrWhiteSpace(replicatedBitNum) || !int.TryParse(replicatedBitNum, out _))
            {
                throw new ArgumentException("Replicated bit number must be a valid non-empty integer string.", nameof(replicatedBitNum));
            }

            if (keyAttributes == null || keyAttributes.Count == 0)
            {
                throw new ArgumentException("Key attributes list cannot be null or empty.", nameof(keyAttributes));
            }
        }
    }
}
