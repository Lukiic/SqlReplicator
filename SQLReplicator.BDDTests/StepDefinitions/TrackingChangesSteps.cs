using System;
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
        private readonly string _connectionString = "Server=localhost\\SQLExpress;Database=DB4;Trusted_Connection=True;TrustServerCertificate=True;";
        private readonly SqlConnection _connection;

        public TrackingChangesSteps()
        {
            _connection = new SqlConnection(_connectionString);
            IExecuteSqlCommandService executeSqlCommand = new ExecuteSqlCommandService(_connection);
        }

        /*
            If provided row already exists in the table, it is deleted first.
            This ensures that INSERT command executes, and the new row is inserted as expected.
        */
        [When("I insert new row in table {string} with values:")]
        public void WhenIInsertNewRowInTableWithValues(string tableName, Table table)
        {
            _connection.Open();

            List<string> attributes = table.Header.ToList();
            List<string> rowValues = table.Rows[0].Values.ToList();

            // Format for DELETE
            IEnumerable<string> attrsAssignValsFormat = attributes.Zip(rowValues, (a, v) => $"{a} = '{v}'");
            string conditionFormat = string.Join(" AND ", attrsAssignValsFormat);

            // Format for INSERT
            string attributesFormat = string.Join(", ", attributes);
            string valuesFormat = string.Join(", ", rowValues.Select(v => $"'{v}'"));

            string deleteCommandSyntax = $"DELETE FROM {tableName} WHERE {conditionFormat};";
            string insertCommandSyntax = $"INSERT INTO {tableName} ({attributesFormat}) VALUES ({valuesFormat});";

            using var deleteCommand = new SqlCommand(deleteCommandSyntax, _connection);
            deleteCommand.ExecuteNonQuery();

            using var insertCommand = new SqlCommand(insertCommandSyntax, _connection);
            insertCommand.ExecuteNonQuery();

            _connection.Close();
        }

        [Then("the table {string} should have row with values:")]
        public void ThenTheTableShouldHaveRowWithValues(string tableName, Table table)
        {
            _connection.Open();

            List<string> attributes = table.Header.ToList();
            List<string> rowValues = table.Rows[0].Values.ToList();

            IEnumerable<string> attrsAssignValsFormat = attributes.Zip(rowValues, (a, v) => $"{a} = '{v}'");
            string conditionFormat = string.Join(" AND ", attrsAssignValsFormat);

            string checkRow = $@"
                            SELECT COUNT(*)
                            FROM {tableName}
                            WHERE {conditionFormat};";

            using var command = new SqlCommand(checkRow, _connection);
            int rowsCount = (int)command.ExecuteScalar();
            _connection.Close();

            Assert.Equal(1, rowsCount);
        }
    }
}
