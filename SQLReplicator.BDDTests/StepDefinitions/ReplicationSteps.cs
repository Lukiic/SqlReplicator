using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Reqnroll;
using SQLReplicator.Domain.Services;
using SQLReplicator.Services.ChangeTrackingServices;
using SQLReplicator.Services.CommandExecutionServices;
using SQLReplicator.Services.CommandPreparationServices;

namespace SQLReplicator.BDDTests.StepDefinitions
{
    [Binding]
    public class ReplicationSteps
    {
        private readonly string _connectionStringSrc = "Server=localhost\\SQLExpress;Database=DB4;Trusted_Connection=True;TrustServerCertificate=True;";
        private readonly SqlConnection _srcConnection;
        private readonly string _connectionStringDest = "Server=localhost\\SQLExpress;Database=DB5;Trusted_Connection=True;TrustServerCertificate=True;";
        private readonly SqlConnection _destConnection;

        private readonly ITrackedDataToCommandsService _trackedDataToCommands;
        private readonly IExecuteListOfCommandsService _executeListOfCommands;

        public ReplicationSteps()
        {
            _srcConnection = new SqlConnection(_connectionStringSrc);
            _destConnection = new SqlConnection(_connectionStringDest);

            _trackedDataToCommands = new TrackedDataToCommandsService(new ChangeTrackingDataService(new ExecuteSqlQueryService(_srcConnection)), new SqlCommandsGenerationService());
            _executeListOfCommands = new ExecuteListOfCommandsService(new ExecuteSqlCommandService(_destConnection));
        }

        [Given("destination database has an empty {string} table")]
        public void GivenDestinationDatabaseHasAnEmptyTable(string tableName)
        {
            _destConnection.Open();

            string deleteCommand = $"DELETE FROM {tableName};";
            using var command = new SqlCommand(deleteCommand, _destConnection);
            command.ExecuteNonQuery();

            _destConnection.Close();
        }


        [When("I run services for generating and executing commands on table {string} with key attributes:")]
        public void WhenIRunServicesForGeneratingAndExecutingCommandsOnTableWithKeyAttributes(string tableName, Table table)
        {
            _srcConnection.Open();
            _destConnection.Open();

            List<string> keyAttributes = table.Rows.Select(row => row["AttributeName"]).ToList();
            List<string> commandsForDestServer;
            int lastChangeID;

            (commandsForDestServer, lastChangeID) = _trackedDataToCommands.GetCommandsAndLastChangeID(tableName, "1", keyAttributes);

            _executeListOfCommands.ExecuteCommands(commandsForDestServer);

            _srcConnection.Close();
            _destConnection.Close();
        }


        [Then("the table {string} of destination server should have row with values:")]
        public void ThenTheTableOfDestinationServerShouldHaveRowWithValues(string tableName, Table table)
        {
            _destConnection.Open();
            List<string> attributes = table.Header.ToList();
            List<string> rowValues = table.Rows[0].Values.ToList();

            IEnumerable<string> attrsAssignValsFormat = attributes.Zip(rowValues, (a, v) => $"{a} = '{v}'");
            string conditionFormat = string.Join(" AND ", attrsAssignValsFormat);

            string checkRow = $@"
                            SELECT COUNT(*)
                            FROM {tableName}
                            WHERE {conditionFormat};";

            using var command = new SqlCommand(checkRow, _destConnection);
            int rowsCount = (int)command.ExecuteScalar();
            _destConnection.Close();

            Assert.Equal(1, rowsCount);
        }
    }
}
