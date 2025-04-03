using Microsoft.Data.SqlClient;
using SQLReplicator.Domain.Services;
using SQLReplicator.Services.ChangeTrackingServices;
using SQLReplicator.Services.CommandExecutionServices;

namespace SQLReplicator.BDDTests.StepDefinitions
{
    [Binding]
    public class CreateTriggerSteps
    {
        private ICreateTriggerService _createTriggerService;

        [Given("database {string} does not have a trigger for table {string}")]
        public void GivenDatabaseDoesNotHaveATriggerForTable(string dbName, string tableName)
        {
            SqlConnection connection = ConnectionsContainer.GetConnection(dbName);
            connection.Open();

            string dropTriggerCommand = $@"
                IF EXISTS (SELECT * FROM sys.objects WHERE type = 'TR' AND name = 'TrackChanges{tableName}')
                BEGIN
                    DROP TRIGGER TrackChanges{tableName};
                END";

            using var command = new SqlCommand(dropTriggerCommand, connection);
            command.ExecuteNonQuery();
            connection.Close();
        }

        [When("I run CreateTrigger service on database {string} for table {string} with key attributes:")]
        public void WhenIRunCreateTriggerServiceOnDatabaseForTableWithKeyAttributes(string dbName, string tableName, Table keyAttrs)
        {
            SqlConnection connection = ConnectionsContainer.GetConnection(dbName);
            connection.Open();

            List<string> keyAttributes = keyAttrs.Rows.Select(row => row["AttributeName"]).ToList();

            _createTriggerService = new CreateTriggerService(new ExecuteSqlCommandService(connection));
            _createTriggerService.CreateTrigger(tableName, keyAttributes);

            connection.Close();
        }

        [Then("the database {string} should have trigger named {string}")]
        public void ThenTheDatabaseShouldHaveTriggerNamed(string dbName, string triggerName)
        {
            SqlConnection connection = ConnectionsContainer.GetConnection(dbName);
            connection.Open();

            string checkTriggerCommand = $@"
                                         SELECT COUNT(*)
                                         FROM sys.objects
                                         WHERE type = 'TR' AND name = '{triggerName}'";

            using var command = new SqlCommand(checkTriggerCommand, connection);
            int triggerCount = (int)command.ExecuteScalar();
            connection.Close();

            Assert.Equal(1, triggerCount);
        }
    }
}
