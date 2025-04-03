using Microsoft.Data.SqlClient;

namespace SQLReplicator.BDDTests.StepDefinitions
{
    public static class ConnectionsContainer
    {
        private static Dictionary<string, SqlConnection> _dictionary = [];
        private readonly static string _connectionString = "Server=localhost\\SQLExpress;Database={0};Trusted_Connection=True;TrustServerCertificate=True;";

        public static void AddConnection(string dbName)
        {
            if (_dictionary.ContainsKey(dbName))
            {
                return;
            }

            var connection = new SqlConnection(string.Format(_connectionString, dbName));
            _dictionary.Add(dbName, connection);
        }

        public static SqlConnection GetConnection(string dbName)
        {
            return _dictionary[dbName];
        }
    }
}
