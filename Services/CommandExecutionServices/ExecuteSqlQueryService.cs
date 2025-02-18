using Microsoft.Data.SqlClient;
using SQLReplicator.Domain.Interfaces;
using SQLReplicator.Domain.Models;
using SQLReplicator.Domain.Services;

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

            return new SqlDataReaderWrapper(reader);
        }
    }
}
