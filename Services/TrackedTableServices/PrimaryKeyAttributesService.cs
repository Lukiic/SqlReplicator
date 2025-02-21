using SQLReplicator.Domain.Interfaces;
using SQLReplicator.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            string query = $@"SELECT COLUMN_NAME 
                                FROM  INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
                                WHERE TABLE_NAME = '{tableName}' AND OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1;";

            IDataReaderWrapper dataReader = _executeSqlQueryService.ExecuteQuery(query);
            List<List<string>> listOfKeyAttributes = dataReader.ReadValues();   // Each inner list contains only one string, because query is selecting only one column

            dataReader.Dispose();
            return listOfKeyAttributes.Select(innerList => innerList[0]).ToList();
        }
    }
}
