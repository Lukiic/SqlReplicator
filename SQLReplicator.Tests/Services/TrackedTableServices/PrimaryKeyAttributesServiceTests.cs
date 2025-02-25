using Moq;
using SQLReplicator.Domain.Interfaces;
using SQLReplicator.Domain.Services;
using SQLReplicator.Services.TrackedTableServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace SQLReplicator.Tests.Services.TrackedTableServices
{
    public class PrimaryKeyAttributesServiceTests
    {
        private PrimaryKeyAttributesService _primaryKeyAttributesService;
        private Mock<IExecuteSqlQueryService> executeSqlQueryMock;
        private Mock<IDataReaderWrapper> dataReaderMock;

        public PrimaryKeyAttributesServiceTests()
        {
            dataReaderMock = new Mock<IDataReaderWrapper>();
            executeSqlQueryMock = new Mock<IExecuteSqlQueryService>();

            dataReaderMock.Setup(x => x.ReadValues()).Returns(GetSampleList());
            dataReaderMock.Setup(x => x.Dispose());

            executeSqlQueryMock.Setup(x => x.ExecuteQuery(It.IsAny<string>())).Returns(dataReaderMock.Object);

            _primaryKeyAttributesService = new PrimaryKeyAttributesService(executeSqlQueryMock.Object);
        }

        private List<List<string>> GetSampleList()
        {
            return new List<List<string>> { new List<string> { "ID1" }, new List<string> { "ID2" } };
        }

        [Fact]
        public void GetPrimaryKeyAttributes_CallsDisposeOfDataReader()
        {
            // Arrange

            // Act
            _primaryKeyAttributesService.GetPrimaryKeyAttributes("TableName");

            // Assert
            dataReaderMock.Verify(x => x.Dispose(), Times.Once);
        }

        [Fact]
        public void GetPrimaryKeyAttributes_ReturnsNonEmptyListOfStrings()
        {
            // Arrange

            // Act
            List<string> returnedList = _primaryKeyAttributesService.GetPrimaryKeyAttributes("TableName");

            // Assert
            Assert.NotNull(returnedList);
            Assert.NotEmpty(returnedList);

            foreach(string key in returnedList)
            {
                Assert.NotNull(key);
            }
        }
    }
}
