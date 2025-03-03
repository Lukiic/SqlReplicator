using Moq;
using SQLReplicator.Domain.Interfaces;
using SQLReplicator.Domain.Services;
using SQLReplicator.Services.CommandPreparationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLReplicator.Tests.Services.CommandPreparationServices
{
    public class TrackedDataToCommandsServiceTests
    {
        private TrackedDataToCommandsService _trackedDataToCommandsService;
        private Mock<IChangeTrackingDataService> changeTrackingDataMock;
        private Mock<ISqlCommandsGenerationService> sqlCommandsGenerationMock;

        public TrackedDataToCommandsServiceTests()
        {
            changeTrackingDataMock = new Mock<IChangeTrackingDataService>();
            sqlCommandsGenerationMock = new Mock<ISqlCommandsGenerationService>();
        }

        [Fact]
        public void GetCommandsAndLastChangeID_ReturnsEmptyListWhenLoadingDataFails()
        {
            // Arrange
            changeTrackingDataMock
                .Setup(x => x.LoadData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .Throws<Exception>();

            _trackedDataToCommandsService = new TrackedDataToCommandsService(changeTrackingDataMock.Object, sqlCommandsGenerationMock.Object);

            List<string> commands;
            string lastChangeId;
            
            // Act

            (commands, lastChangeId) = _trackedDataToCommandsService.GetCommandsAndLastChangeID("TableName", "0", new List<string>());

            // Assert
            Assert.Empty(commands);
            Assert.Equal("0", lastChangeId);
        }

        [Fact]
        public void GetCommandsAndLastChangeID_CallsDisposeOfDataReader()
        {
            // Arrange
            Mock<IDataReaderWrapper> dataReaderMock = new Mock<IDataReaderWrapper>();
            dataReaderMock.Setup(x => x.Dispose());
            dataReaderMock.Setup(x => x.ReadAttributes()).Returns(new List<string>());
            dataReaderMock.Setup(x => x.ReadValues()).Returns(new List<List<string?>>());

            changeTrackingDataMock
                .Setup(x => x.LoadData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .Returns(dataReaderMock.Object);

            _trackedDataToCommandsService = new TrackedDataToCommandsService(changeTrackingDataMock.Object, sqlCommandsGenerationMock.Object);

            // Act
            _trackedDataToCommandsService.GetCommandsAndLastChangeID("TableName", "0", new List<string>());

            // Assert
            dataReaderMock.Verify(x => x.Dispose(), Times.Once);
        }
    }
}
