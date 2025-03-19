using SQLReplicator.Domain.Interfaces;
using SQLReplicator.Domain.Services;

namespace SQLReplicator.Services.TrackedTableServices
{
    public class PrimaryKeyAttributesService : IPrimaryKeyAttributesService
    {
        private IExecuteSqlQueryService _executeSqlQueryService;

        public PrimaryKeyAttributesService(IExecuteSqlQueryService executeSqlQueryService)
        {
            _executeSqlQueryService = executeSqlQueryService;
        }

        public List<string> GetPrimaryKeyAttributes(string tableName)
        {
            ValidateArguments(tableName);

            string query = $@"SELECT COLUMN_NAME 
                              FROM  INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
                              WHERE TABLE_NAME = '{tableName}' AND OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1;";

            List<List<string>> listOfKeyAttributes;
            using (IDataReaderWrapper dataReader = _executeSqlQueryService.ExecuteQuery(query))
            {
                listOfKeyAttributes = dataReader.ReadValues();   // Each inner list contains only one string, because query is selecting only one column
            }
            return listOfKeyAttributes.Select(innerList => innerList[0]).ToList();
        }

        private void ValidateArguments(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException("Table name cannot be null or empty.", nameof(tableName));
            }
        }
    }
}
