using Moq;
using SQLReplicator.Domain.Services;
using SQLReplicator.Services.ChangeTrackingServices;

namespace SQLReplicator.Tests.Services.ChangeTrackingServices
{
    public class CreateChangeTrackingTableServiceTests
    {
        private CreateChangeTrackingTableService _createChangeTrackingTable;
        Mock<IExecuteSqlCommandService> executeSqlCommandMock;

        public CreateChangeTrackingTableServiceTests()
        {
            executeSqlCommandMock = new Mock<IExecuteSqlCommandService>();
        }

        [Fact]
        public void CreateCTTable_ReturnsTrueIfThereIsNoException()
        {
            // Arrange
            executeSqlCommandMock.Setup(x => x.ExecuteCommand(It.IsAny<string>()));

            _createChangeTrackingTable = new CreateChangeTrackingTableService(executeSqlCommandMock.Object);

            // Act
            bool retVal = _createChangeTrackingTable.CreateCTTable("TableName", new List<string> { "ID" });

            // Assert
            Assert.True(retVal);
        }

        [Fact]
        public void CreateCTTable_ReturnsFalseIfThereIsException()
        {
            // Arrange
            executeSqlCommandMock.Setup(x => x.ExecuteCommand(It.IsAny<string>())).Throws<Exception>();

            _createChangeTrackingTable = new CreateChangeTrackingTableService(executeSqlCommandMock.Object);

            // Act
            bool retVal = _createChangeTrackingTable.CreateCTTable("TableName", new List<string>() { "ID" });

            // Assert
            Assert.False(retVal);
        }
    }
}
