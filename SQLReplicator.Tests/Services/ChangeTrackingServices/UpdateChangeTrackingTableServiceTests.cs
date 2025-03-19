using Moq;
using SQLReplicator.Domain.Services;
using SQLReplicator.Services.ChangeTrackingServices;

namespace SQLReplicator.Tests.Services.ChangeTrackingServices
{
    public class UpdateChangeTrackingTableServiceTests
    {
        private UpdateChangeTrackingTableService _updateChangeTrackingTable;
        Mock<IExecuteSqlCommandService> executeSqlCommandMock;

        public UpdateChangeTrackingTableServiceTests()
        {
            executeSqlCommandMock = new Mock<IExecuteSqlCommandService>();
        }

        [Fact]
        public void UpdateReplicatedBit_ReturnsTrueIfThereIsNoException()
        {
            // Arrange
            executeSqlCommandMock.Setup(x => x.ExecuteCommand(It.IsAny<string>()));

            _updateChangeTrackingTable = new UpdateChangeTrackingTableService(executeSqlCommandMock.Object);

            // Act
            bool retVal = _updateChangeTrackingTable.UpdateReplicatedBit("TableName", 0, "ReplicatedBitNum");

            // Assert
            Assert.True(retVal);
        }

        [Fact]
        public void UpdateReplicatedBit_ReturnsFalseIfThereIsException()
        {
            // Arrange
            executeSqlCommandMock.Setup(x => x.ExecuteCommand(It.IsAny<string>())).Throws<Exception>();

            _updateChangeTrackingTable = new UpdateChangeTrackingTableService(executeSqlCommandMock.Object);

            // Act
            bool retVal = _updateChangeTrackingTable.UpdateReplicatedBit("TableName", 0, "ReplicatedBitNum");

            // Assert
            Assert.False(retVal);
        }
    }
}
