using Microsoft.Data.SqlClient;

namespace SQLReplicator.Domain.Services
{
    public interface ISqlConnectionService
    {
        public void OpenConnection(SqlConnection sqlConnection);

        public void CloseConnection(SqlConnection sqlConnection);
    }
}
