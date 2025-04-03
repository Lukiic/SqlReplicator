using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Xml.Linq;
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
        private List<string> _commandsToBeExecuted = [];

        [When("I run service for generating commands on database {string} for table {string} with key attributes:")]
        public void WhenIRunServiceForGeneratingCommandsOnDatabaseForTableWithKeyAttributes(string dbName, string tableName, Table keyAttrs)
        {
            SqlConnection connection = ConnectionsContainer.GetConnection(dbName);
            connection.Open();

            List<string> keyAttributes = keyAttrs.Rows.Select(row => row["AttributeName"]).ToList();

            TrackedDataToCommandsService trackedDataToCommands = new TrackedDataToCommandsService(new ChangeTrackingDataService(new ExecuteSqlQueryService(connection)), new SqlCommandsGenerationService());

            (_commandsToBeExecuted, _) = trackedDataToCommands.GetCommandsAndLastChangeID(tableName, "1", keyAttributes);

            connection.Close();
        }

        [When("I run service for executing generated commands on database {string}")]
        public void WhenIRunServiceForExecutingGeneratedCommandsOnDatabase(string dbName)
        {
            SqlConnection connection = ConnectionsContainer.GetConnection(dbName);
            connection.Open();

            ExecuteListOfCommandsService executeListOfCommands = new ExecuteListOfCommandsService(new ExecuteSqlCommandService(connection));

            executeListOfCommands.ExecuteCommands(_commandsToBeExecuted);

            connection.Close();
        }
    }
}
