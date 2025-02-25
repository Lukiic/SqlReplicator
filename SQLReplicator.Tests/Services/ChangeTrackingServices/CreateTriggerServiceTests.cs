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
    public class CreateTriggerServiceTests
    {
        private CreateTriggerService _createTriggerService;
        Mock<IExecuteSqlCommandService> executeSqlCommandMock;

        public CreateTriggerServiceTests()
        {
            executeSqlCommandMock = new Mock<IExecuteSqlCommandService>();
        }

        [Fact]
        public void CreateTrigger_ReturnsTrueIfThereIsNoException()
        {
            // Arrange
            executeSqlCommandMock.Setup(x => x.ExecuteCommand(It.IsAny<string>()));

            _createTriggerService = new CreateTriggerService(executeSqlCommandMock.Object);

            // Act
            bool retVal = _createTriggerService.CreateTrigger("TableName", new List<string>());

            // Assert
            Assert.True(retVal);
        }

        [Fact]
        public void CreateTrigger_ReturnsFalseIfThereIsException()
        {
            // Arrange
            executeSqlCommandMock.Setup(x => x.ExecuteCommand(It.IsAny<string>())).Throws<Exception>();

            _createTriggerService = new CreateTriggerService(executeSqlCommandMock.Object);

            // Act
            bool retVal = _createTriggerService.CreateTrigger("TableName", new List<string>());

            // Assert
            Assert.False(retVal);
        }
    }
}
