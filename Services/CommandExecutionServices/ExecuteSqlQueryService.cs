using Microsoft.Data.SqlClient;
using SQLReplicator.Domain.Interfaces;
using SQLReplicator.Domain.Models;
using SQLReplicator.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLReplicator.Services.CommandExecutionServices
{
    public class ExecuteSqlQueryService : IExecuteSqlQueryService
    {
        private SqlConnection _connection;

        public ExecuteSqlQueryService(SqlConnection connection)
        {
            _connection = connection;
        }

        public IDataReaderWrapper ExecuteQuery(string query)
        {
            SqlCommand sqlCommand = new SqlCommand(query, _connection);
            SqlDataReader reader = sqlCommand.ExecuteReader();

            SqlDataReaderWrapper dataReader = new SqlDataReaderWrapper(reader);

            return dataReader;
        }
    }
}
