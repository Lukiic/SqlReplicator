using SQLReplicator.Domain.Interfaces;
using SQLReplicator.Domain.Services;

namespace SQLReplicator.Services.TrackedTableServices
{
    public class TableNamesFetcherService : ITableNamesFetcherService
    {
        private IExecuteSqlQueryService _executeSqlQueryService;

        public TableNamesFetcherService(IExecuteSqlQueryService executeSqlQueryService)
        {
            _executeSqlQueryService = executeSqlQueryService;
        }

        public List<string> GetTableNames()
        {
            string query = $@"SELECT name 
                              FROM sys.tables 
                              WHERE name NOT LIKE '%Changes';";

            List<List<string>> listOfTableNames;
            using (IDataReaderWrapper dataReader = _executeSqlQueryService.ExecuteQuery(query))
            {
                listOfTableNames = dataReader.ReadValues();   // Each inner list contains only one string, because query is selecting only one column
            }
            return listOfTableNames.Select(innerList => innerList[0]).ToList();
        }
    }
}
