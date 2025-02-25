using Moq;
using SQLReplicator.Domain.Services;
using SQLReplicator.Services.ChangeTrackingServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            bool retVal = _updateChangeTrackingTable.UpdateReplicatedBit("TableName", "LastChangeID", "ReplicatedBitNum");

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
            bool retVal = _updateChangeTrackingTable.UpdateReplicatedBit("TableName", "LastChangeID", "ReplicatedBitNum");

            // Assert
            Assert.False(retVal);
        }
    }
}
