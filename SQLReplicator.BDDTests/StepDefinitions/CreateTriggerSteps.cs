using System;
using Microsoft.Data.SqlClient;
using Reqnroll;
using SQLReplicator.Domain.Services;
using SQLReplicator.Services.ChangeTrackingServices;
using SQLReplicator.Services.CommandExecutionServices;

namespace SQLReplicator.BDDTests.StepDefinitions
{
    [Binding]
    public class CreateTriggerSteps
    {
        private readonly string _connectionString = "Server=localhost\\SQLExpress;Database=DB4;Trusted_Connection=True;TrustServerCertificate=True;";
        private readonly SqlConnection _connection;
        private readonly ICreateTriggerService _createTriggerService;

        public CreateTriggerSteps()
        {
            _connection = new SqlConnection(_connectionString);
            _createTriggerService = new CreateTriggerService(new ExecuteSqlCommandService(_connection));
        }

        [Given("source database does not have a trigger for table {string}")]
        public void GivenSourceDatabaseDoesNotHaveATriggerForTable(string tableName)
        {
            _connection.Open();

            string dropTriggerCommand = $@"
                IF EXISTS (SELECT * FROM sys.objects WHERE type = 'TR' AND name = 'TrackChanges{tableName}')
                BEGIN
                    DROP TRIGGER TrackChanges{tableName};
                END";

            using var command = new SqlCommand(dropTriggerCommand, _connection);
            command.ExecuteNonQuery();
            _connection.Close();
        }

        [When("I run CreateTrigger service for table {string} with key attributes:")]
        public void WhenIRunCreateTriggerServiceForTableWithKeyAttributes(string tableName, Table table)
        {
            _connection.Open();
            List<string> keyAttributes = table.Rows.Select(row => row["AttributeName"]).ToList();

            _createTriggerService.CreateTrigger(tableName, keyAttributes);
            _connection.Close();
        }

        [Then("the source database should have trigger named {string}")]
        public void ThenTheSourceDatabaseShouldHaveTriggerNamed(string triggerName)
        {
            _connection.Open();

            string checkTriggerCommand = $@"
                                            SELECT COUNT(*)
                                            FROM sys.objects
                                            WHERE type = 'TR' AND name = '{triggerName}'";

            using var command = new SqlCommand(checkTriggerCommand, _connection);
            int triggerCount = (int)command.ExecuteScalar();
            _connection.Close();

            Assert.Equal(1, triggerCount); 
        }
    }
}
