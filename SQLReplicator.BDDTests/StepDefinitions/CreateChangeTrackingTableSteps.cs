using System;
using Microsoft.Data.SqlClient;
using Reqnroll;
using SQLReplicator.Domain.Services;
using SQLReplicator.Services.ChangeTrackingServices;
using SQLReplicator.Services.CommandExecutionServices;

namespace SQLReplicator.BDDTests.StepDefinitions
{
    [Binding]
    public class CreateChangeTrackingTableSteps
    {
        private readonly string _connectionString = "Server=localhost\\SQLExpress;Database=DB4;Trusted_Connection=True;TrustServerCertificate=True;";
        private readonly SqlConnection _connection;
        private readonly ICreateChangeTrackingTableService _createChangeTrackingTable;

        public CreateChangeTrackingTableSteps()
        {
            _connection = new SqlConnection(_connectionString);
            _createChangeTrackingTable = new CreateChangeTrackingTableService(new ExecuteSqlCommandService(_connection));
        }


        [Given("source database does not have a change tracking table for table {string}")]
        public void GivenSourceDatabaseDoesNotHaveAChangeTrackingTableForTable(string tableName)
        {
            _connection.Open();

            string dropTableCommand = $@"
                IF EXISTS (SELECT * FROM sys.tables WHERE name = '{tableName}Changes')
                BEGIN
                    DROP TABLE {tableName}Changes;
                END";

            using var command = new SqlCommand(dropTableCommand, _connection);
            command.ExecuteNonQuery();
            _connection.Close();
        }

        [When("I run CreateChangeTrackingTable service for table {string} with key attributes:")]
        public void WhenIRunCreateChangeTrackingTableServiceForTableWithKeyAttributes(string tableName, Table table)
        {
            _connection.Open();
            List<string> keyAttributes = table.Rows.Select(row => row["AttributeName"]).ToList();

            _createChangeTrackingTable.CreateCTTable(tableName, keyAttributes);
            _connection.Close();
        }

        [Then("the source database should have change tracking table named {string}")]
        public void ThenTheSourceDatabaseShouldHaveChangeTrackingTableNamed(string tableName)
        {
            _connection.Open();

            string checkTableCommand = $@"
                                       SELECT COUNT(*)
                                       FROM sys.tables
                                       WHERE name = '{tableName}'";

            using var command = new SqlCommand(checkTableCommand, _connection);
            int tableCount = (int)command.ExecuteScalar();
            _connection.Close();

            Assert.Equal(1, tableCount);
        }

        [Then("the table named {string} should be empty")]
        public void ThenTheTableNamedShouldBeEmpty(string tableName)
        {
            _connection.Open();

            string checkTableCommand = $@"
                                       SELECT COUNT(*)
                                       FROM {tableName}";

            using var command = new SqlCommand(checkTableCommand, _connection);
            int rowsCount = (int)command.ExecuteScalar();
            _connection.Close();

            Assert.Equal(0, rowsCount);
        }
    }
}
