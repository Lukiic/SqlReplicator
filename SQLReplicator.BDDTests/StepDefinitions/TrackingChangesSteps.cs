using System;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;
using Reqnroll;
using SQLReplicator.Domain.Services;
using SQLReplicator.Services.ChangeTrackingServices;
using SQLReplicator.Services.CommandExecutionServices;

namespace SQLReplicator.BDDTests.StepDefinitions
{
    [Binding]
    public class TrackingChangesSteps
    {
        [Given("database {string} has a trigger and an empty change tracking table for table {string} with key attributes:")]
        public void GivenDatabaseHasATriggerAndAnEmptyChangeTrackingTableForTableWithKeyAttributes(string dbName, string tableName, Table keyAttrs)
        {
            ConnectionsContainer.AddConnection(dbName);
            SqlConnection connection = ConnectionsContainer.GetConnection(dbName);
            connection.Open();

            List<string> keyAttributes = keyAttrs.Rows.Select(row => row["AttributeName"]).ToList();

            var createTriggerService = new CreateTriggerService(new ExecuteSqlCommandService(connection));
            createTriggerService.CreateTrigger(tableName, keyAttributes);

            var createChangeTrackingTable = new CreateChangeTrackingTableService(new ExecuteSqlCommandService(connection));
            createChangeTrackingTable.CreateCTTable(tableName, keyAttributes);

            string deleteCommand = $"DELETE FROM {tableName}Changes;";
            using var command = new SqlCommand(deleteCommand, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }

        /*
            If provided row already exists in the table, it is deleted first.
            This ensures that INSERT command executes, and the new row is inserted as expected.
        */
        [When("I insert new row in database {string} table {string} with values:")]
        public void WhenIInsertNewRowInDatabaseTableWithValues(string dbName, string tableName, Table rowData)
        {
            SqlConnection connection = ConnectionsContainer.GetConnection(dbName);
            connection.Open();

            string deleteCommandSyntax = GetDeleteCommand(tableName, rowData);
            string insertCommandSyntax = GetInsertCommand(tableName, rowData);

            using var deleteCommand = new SqlCommand(deleteCommandSyntax, connection);
            deleteCommand.ExecuteNonQuery();

            using var insertCommand = new SqlCommand(insertCommandSyntax, connection);
            insertCommand.ExecuteNonQuery();

            connection.Close();
        }

        [Then("the table {string} in database {string} should have row with values:")]
        public void ThenTheTableInDatabaseShouldHaveRowWithValues(string tableName, string dbName, DataTable rowData)
        {
            SqlConnection connection = ConnectionsContainer.GetConnection(dbName);
            connection.Open();

            List<string> attributes = rowData.Header.ToList();
            List<string> rowValues = rowData.Rows[0].Values.ToList();

            IEnumerable<string> attrsAssignValsFormat = attributes.Zip(rowValues, (a, v) => $"{a} = '{v}'");
            string conditionFormat = string.Join(" AND ", attrsAssignValsFormat);

            string checkRow = $@"
                              SELECT COUNT(*)
                              FROM {tableName}
                              WHERE {conditionFormat};";

            using var command = new SqlCommand(checkRow, connection);
            int rowsCount = (int)command.ExecuteScalar();
            connection.Close();

            Assert.Equal(1, rowsCount);
        }

        /*
           Provided row is inserted first.
           This ensures that DELETE command executes.
        */
        [When("I delete existing row in database {string} table {string} with values:")]
        public void WhenIDeleteExistingRowInDatabaseTableWithValues(string dbName, string tableName, Table rowValues)
        {
            SqlConnection connection = ConnectionsContainer.GetConnection(dbName);
            connection.Open();

            string insertCommandSyntax = GetInsertCommand(tableName, rowValues);
            string deleteCommandSyntax = GetDeleteCommand(tableName, rowValues);

            using var insertCommand = new SqlCommand(insertCommandSyntax, connection);
            insertCommand.ExecuteNonQuery();

            using var deleteCommand = new SqlCommand(deleteCommandSyntax, connection);
            deleteCommand.ExecuteNonQuery();

            connection.Close();
        }

        [When("I update existing row in database {string} table {string} with values:")]
        public void WhenIUpdateExistingRowInDatabaseTableWithValues(string dbName, string tableName, Table rowValues)
        {
            SqlConnection connection = ConnectionsContainer.GetConnection(dbName);
            connection.Open();

            string updateCommandSyntax = GetUpdateCommand(tableName, rowValues);

            using var updateCommand = new SqlCommand(updateCommandSyntax, connection);
            updateCommand.ExecuteNonQuery();

            connection.Close();
        }

        private string GetUpdateCommand(string tableName, Table table)
        {
            List<string> attributes = table.Header.ToList();
            List<string> rowValues = table.Rows[0].Values.ToList();

            IEnumerable<string> attrsAssignValsFormat = attributes.Zip(rowValues, (a, v) => $"{a} = '{v}'");
            string csvFormat = string.Join(", ", attrsAssignValsFormat);
            string conditionFormat = string.Join(" AND ", attrsAssignValsFormat);

            return $"UPDATE {tableName} SET {csvFormat} WHERE {conditionFormat};";  // No row values are changed, but this operation still counts as an update
        }

        private string GetDeleteCommand(string tableName, Table table)
        {
            List<string> attributes = table.Header.ToList();
            List<string> rowValues = table.Rows[0].Values.ToList();

            IEnumerable<string> attrsAssignValsFormat = attributes.Zip(rowValues, (a, v) => $"{a} = '{v}'");
            string conditionFormat = string.Join(" AND ", attrsAssignValsFormat);

            return $"DELETE FROM {tableName} WHERE {conditionFormat};";
        }

        private string GetInsertCommand(string tableName, Table table)
        {
            List<string> attributes = table.Header.ToList();
            List<string> rowValues = table.Rows[0].Values.ToList();

            string attributesFormat = string.Join(", ", attributes);
            string valuesFormat = string.Join(", ", rowValues.Select(v => $"'{v}'"));

            return $"INSERT INTO {tableName} ({attributesFormat}) VALUES ({valuesFormat});";
        }
    }
}
