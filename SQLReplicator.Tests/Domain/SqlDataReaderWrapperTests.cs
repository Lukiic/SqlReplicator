using Moq;
using SQLReplicator.Domain.Models;
using System.Data;

namespace SQLReplicator.Tests.Domain
{
    public class SqlDataReaderWrapperTests
    {
        private SqlDataReaderWrapper _sqlDataReaderWrapper;
        Mock<IDataReader> dataReaderMock;

        public SqlDataReaderWrapperTests()
        {
            dataReaderMock = new Mock<IDataReader>();
        }

        [Fact]
        public void Dispose_ShouldCallCloseAndDisposeOnce()
        {
            // Arrange
            dataReaderMock.Setup(x => x.Close());
            dataReaderMock.Setup(x => x.Dispose());

            _sqlDataReaderWrapper = new SqlDataReaderWrapper(dataReaderMock.Object);

            // Act
            _sqlDataReaderWrapper.Dispose();

            // Assert
            dataReaderMock.Verify(x => x.Close(), Times.Once);
            dataReaderMock.Verify(x => x.Dispose(), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(1000)]
        [InlineData(100000)]
        public void ReadAttributes_ReturnsListWithExpectedNumberOfElements(int expected)
        {
            // Arrange
            dataReaderMock.Setup(x => x.FieldCount).Returns(expected);
            dataReaderMock.Setup(x => x.GetName(It.IsAny<int>())).Returns("SampleAttribute");

            _sqlDataReaderWrapper = new SqlDataReaderWrapper(dataReaderMock.Object);

            // Act
            List<string> returnedList = _sqlDataReaderWrapper.ReadAttributes();

            // Assert
            Assert.NotNull(returnedList);
            Assert.Equal(expected, returnedList.Count);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        public void ReadAttributes_ReturnsListThatContainsExpectedAttributes(int numOfAttributes)
        {
            // Arrange
            dataReaderMock.Setup(x => x.FieldCount).Returns(numOfAttributes);

            for (int i = 0; i < numOfAttributes; ++i)
            {
                dataReaderMock.Setup(x => x.GetName(i)).Returns($"SampleAttribute{i}");
            }

            _sqlDataReaderWrapper = new SqlDataReaderWrapper(dataReaderMock.Object);

            // Act
            List<string> returnedList = _sqlDataReaderWrapper.ReadAttributes();

            // Assert
            for (int i = 0; i < numOfAttributes; ++i)
            {
                Assert.Contains($"SampleAttribute{i}", returnedList);
            }
        }

        [Fact]
        public void ReadValues_ReturnsEmptyList()
        {
            // Arrange
            dataReaderMock.Setup(x => x.Read()).Returns(false);
            _sqlDataReaderWrapper = new SqlDataReaderWrapper(dataReaderMock.Object);

            // Act
            List<List<string>> returnedList = _sqlDataReaderWrapper.ReadValues();

            // Assert
            Assert.NotNull(returnedList);
            Assert.Empty(returnedList);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public void ReadValues_ReturnsListWithExpectedNumberOfElements(int expected)
        {
            // Arrange
            int numOfCalls = 0;
            dataReaderMock.Setup(x => x.Read()).Returns(() => { return ++numOfCalls <= expected; });

            dataReaderMock.Setup(x => x.FieldCount).Returns(expected);
            dataReaderMock.Setup(x => x.IsDBNull(It.IsAny<int>())).Returns(true);

            _sqlDataReaderWrapper = new SqlDataReaderWrapper(dataReaderMock.Object);

            // Act
            List<List<string?>> returnedList = _sqlDataReaderWrapper.ReadValues();

            // Assert
            Assert.NotNull(returnedList);
            Assert.Equal(expected, returnedList.Count);

            for (int i = 0; i < expected; ++i)
            {
                Assert.Equal(expected, returnedList[i].Count);  // Every inner list should have expected number of elements too
            }
        }

        [Fact]
        public void ReadValues_ReturnsListThatContainsNullForNullValues()
        {
            // Arrange
            const int innerListCount = 5;

            int numOfCalls = 0;
            dataReaderMock.Setup(x => x.Read()).Returns(() => { return ++numOfCalls <= 1; });   // Only one inner list

            dataReaderMock.Setup(x => x.FieldCount).Returns(innerListCount);
            dataReaderMock.Setup(x => x.IsDBNull(It.IsAny<int>())).Returns(true);

            _sqlDataReaderWrapper = new SqlDataReaderWrapper(dataReaderMock.Object);

            // Act
            List<List<string?>> returnedList = _sqlDataReaderWrapper.ReadValues();
            List<string?> innerList = returnedList[0];

            // Assert
            Assert.NotNull(returnedList);
            Assert.NotNull(innerList);

            for (int i = 0; i < innerListCount; ++i)
            {
                Assert.Null(innerList[i]);
            }
        }

        [Fact]
        public void ReadValues_ReturnsListThatContainsValidStringsForNonNullValues()
        {
            // Arrange
            const int innerListCount = 5;

            int numOfCalls = 0;
            dataReaderMock.Setup(x => x.Read()).Returns(() => { return ++numOfCalls <= 1; });   // Only one inner list

            dataReaderMock.Setup(x => x.FieldCount).Returns(innerListCount);
            dataReaderMock.Setup(x => x.IsDBNull(It.IsAny<int>())).Returns(false);
            dataReaderMock.Setup(x => x.GetValue(It.IsAny<int>())).Returns("SampleValue");

            _sqlDataReaderWrapper = new SqlDataReaderWrapper(dataReaderMock.Object);

            // Act
            List<List<string?>> returnedList = _sqlDataReaderWrapper.ReadValues();
            List<string?> innerList = returnedList[0];

            // Assert
            Assert.NotNull(returnedList);
            Assert.NotNull(innerList);

            for (int i = 0; i < innerListCount; ++i)
            {
                Assert.NotNull(innerList[i]);
            }
        }
    }
}
