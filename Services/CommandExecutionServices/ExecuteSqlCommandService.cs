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
        
        // Input string can have multiple commands, so transaction is used
        public void ExecuteCommand(string command)
        {
            using (SqlTransaction transaction = _connection.BeginTransaction())
            {
                try
                {
                    SqlCommand sqlCommand = new SqlCommand(command, _connection, transaction);
                    sqlCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
