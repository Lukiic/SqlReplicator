using Microsoft.Data.SqlClient;
using SQLReplicator.Domain.Services;

namespace SQLReplicator.Services.CommandExecutionServices
{
    public class ExecuteSqlCommandService : IExecuteSqlCommandService
    {
        private SqlConnection _connection;

        public ExecuteSqlCommandService(SqlConnection connection)
        {
            _connection = connection;
        }

        public void ExecuteCommand(string command)
        {
            SqlCommand sqlCommand = new SqlCommand(command, _connection);

            sqlCommand.ExecuteNonQuery();
        }
    }
}
