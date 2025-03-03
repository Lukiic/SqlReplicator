using Moq;
using SQLReplicator.Domain.Services;
using SQLReplicator.Services.CommandExecutionServices;

namespace SQLReplicator.Tests.Services.CommandExecutionServices
{
    public class ExecuteListOfCommandsServiceTests
    {
        private ExecuteListOfCommandsService _executeListOfCommands;
        private Mock<IExecuteSqlCommandService> executeSqlCommandMock;

        public ExecuteListOfCommandsServiceTests()
        {
            executeSqlCommandMock = new Mock<IExecuteSqlCommandService>();
            executeSqlCommandMock.Setup(x => x.ExecuteCommand(It.IsAny<string>()));

            _executeListOfCommands = new ExecuteListOfCommandsService(executeSqlCommandMock.Object);
        }

        private List<string> GetSampleCommandList(int num)
        {
            List<string> commands = new List<string>();

            for (int i = 0; i < num; ++i)
            {
                commands.Add($"SampleCommand{i}");
            }

            return commands;
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public void ExecuteCommands_ForEachCommandCallsSqlExecuteCommand(int numOfCommands)
        {
            // Arrange
            List<string> commands = GetSampleCommandList(numOfCommands);

            // Act
            _executeListOfCommands.ExecuteCommands(commands);

            // Assert
            executeSqlCommandMock.Verify(x => x.ExecuteCommand(It.IsAny<string>()), Times.Exactly(numOfCommands));
        }
    }
}
