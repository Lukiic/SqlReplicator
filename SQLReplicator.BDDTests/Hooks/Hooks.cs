using Microsoft.Data.SqlClient;
using SQLReplicator.BDDTests.StepDefinitions;

namespace SQLReplicator.BDDTests.Hooks
{
    [Binding]
    public class Hooks
    {
        [BeforeScenario]
        public void SetupDatabase()
        {
            ConnectionsContainer.AddConnection("SourceDB");
            ConnectionsContainer.AddConnection("DestinationDB");

            SqlConnection srcConnection = ConnectionsContainer.GetConnection("SourceDB");
            SqlConnection destConnection = ConnectionsContainer.GetConnection("DestinationDB");
            srcConnection.Open();
            destConnection.Open();

            string createTableOrders = $@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
                BEGIN
                    CREATE TABLE Orders (
                        OrderID INT NOT NULL,
                        ProductID INT NOT NULL,
                        CustomerName VARCHAR(255) NOT NULL,
                        Quantity INT NOT NULL,
                        PRIMARY KEY (OrderID, ProductID)
                    );
                END;";

            string makeTableOrdersEmpty = $"DELETE FROM Orders;";

            ExecuteSqlCommand(srcConnection, createTableOrders);
            ExecuteSqlCommand(srcConnection, makeTableOrdersEmpty);

            ExecuteSqlCommand(destConnection, createTableOrders);
            ExecuteSqlCommand(destConnection, makeTableOrdersEmpty);

            srcConnection.Close();
            destConnection.Close();
        }

        private void ExecuteSqlCommand(SqlConnection connection, string commandSyntax)
        {
            using var command = new SqlCommand(commandSyntax, connection);
            command.ExecuteNonQuery();
        }
    }
}
