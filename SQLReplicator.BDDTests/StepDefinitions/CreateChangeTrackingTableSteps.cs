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
        private ICreateChangeTrackingTableService _createChangeTrackingTable;

        [Given("database {string} does not have a change tracking table for table {string}")]
        public void GivenDatabaseDoesNotHaveAChangeTrackingTableForTable(string dbName, string tableName)
        {
            SqlConnection connection = ConnectionsContainer.GetConnection(dbName);
            connection.Open();

            string dropTableCommand = $@"
                IF EXISTS (SELECT * FROM sys.tables WHERE name = '{tableName}Changes')
                BEGIN
                    DROP TABLE {tableName}Changes;
                END";

            using var command = new SqlCommand(dropTableCommand, connection);
            command.ExecuteNonQuery();
            connection.Close();
        }

        [When("I run CreateChangeTrackingTable service on database {string} for table {string} with key attributes:")]
        public void WhenIRunCreateChangeTrackingTableServiceOnDatabaseForTableWithKeyAttributes(string dbName, string tableName, Table keyAttrs)
        {
            SqlConnection connection = ConnectionsContainer.GetConnection(dbName);
            connection.Open();

            List<string> keyAttributes = keyAttrs.Rows.Select(row => row["AttributeName"]).ToList();

            _createChangeTrackingTable = new CreateChangeTrackingTableService(new ExecuteSqlCommandService(connection));
            _createChangeTrackingTable.CreateCTTable(tableName, keyAttributes);

            connection.Close();
        }

        [Then("the database {string} should have change tracking table named {string}")]
        public void ThenTheDatabaseShouldHaveChangeTrackingTableNamed(string dbName, string tableName)
        {
            SqlConnection connection = ConnectionsContainer.GetConnection(dbName);
            connection.Open();

            string checkTableCommand = $@"
                                       SELECT COUNT(*)
                                       FROM sys.tables
                                       WHERE name = '{tableName}'";

            using var command = new SqlCommand(checkTableCommand, connection);
            int tableCount = (int)command.ExecuteScalar();
            connection.Close();

            Assert.Equal(1, tableCount);
        }

        [Then("table named {string} in database {string} should be empty")]
        public void ThenTableNamedInDatabaseShouldBeEmpty(string tableName, string dbName)
        {
            SqlConnection connection = ConnectionsContainer.GetConnection(dbName);
            connection.Open();

            string checkTableCommand = $@"
                                       SELECT COUNT(*)
                                       FROM {tableName}";

            using var command = new SqlCommand(checkTableCommand, connection);
            int rowsCount = (int)command.ExecuteScalar();
            connection.Close();

            Assert.Equal(0, rowsCount);
        }
    }
}
