using Microsoft.Data.SqlClient;
using SQLReplicator.Domain.Models;
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
        public void ExecuteCommand(string commands)
        {
            ValidateArguments(commands);

            using (DbTransactionManager transactionManager = new DbTransactionManager(_connection))
            {
                try
                {
                    SqlCommand sqlCommand = new SqlCommand(commands, _connection, transactionManager.Transaction);
                    sqlCommand.ExecuteNonQuery();
                    transactionManager.Commit();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void ValidateArguments(string commands)
        {
            if (string.IsNullOrWhiteSpace(commands))
            {
                throw new ArgumentException("Command cannot be null, empty, or whitespace.", nameof(commands));
            }
        }
    }
}
