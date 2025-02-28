using SQLReplicator.Services.CommandPreparationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLReplicator.Tests.Services.CommandPreparationServices
{
    public class SqlSyntaxFormattingServiceTests
    {
        private List<string> attributes;
        private List<string> values;
        private List<string?> valuesWithNull;
        private string tableName;

        public SqlSyntaxFormattingServiceTests()
        {
            attributes = new List<string> { "Attr1", "Attr2", "Attr3" };
            values = new List<string> { "Value1", "Value2", "Value3" };
            valuesWithNull = new List<string?> { "Value1", null, "Value3" };
            tableName = "SampleName";
        }

        [Fact]
        public void GetInsertCommand_ReturnsStringThatContainsTableNameAndAttributes()
        {
            // Arrange

            // Act
            string returnedCommand = SqlSyntaxFormattingService.GetInsertCommand(tableName, attributes, values);

            // Assert
            Assert.Contains(tableName, returnedCommand);

            foreach(string attr in attributes)
            {
                Assert.Contains(attr, returnedCommand);
            }
        }

        [Fact]
        public void GetInsertCommand_ReturnsStringThatContainsNonNullValues()
        {
            // Arrange

            // Act
            string returnedCommand = SqlSyntaxFormattingService.GetInsertCommand(tableName, attributes, values);

            // Assert
            foreach (string value in values)
            {
                Assert.Contains(value, returnedCommand);
            }
        }

        [Fact]
        public void GetInsertCommand_ReturnsStringThatContainsNullForNullValues()
        {
            // Arrange

            // Act
            string returnedCommand = SqlSyntaxFormattingService.GetInsertCommand(tableName, attributes, valuesWithNull);

            // Assert
            Assert.Contains("NULL", returnedCommand);
        }

        [Fact]
        public void GetDeleteCommand_ReturnsStringThatContainsTableNameAndAttributes()
        {
            // Arrange

            // Act
            string returnedCommand = SqlSyntaxFormattingService.GetDeleteCommand(tableName, attributes, values);

            // Assert
            Assert.Contains(tableName, returnedCommand);

            foreach (string attr in attributes)
            {
                Assert.Contains(attr, returnedCommand);
            }
        }

        [Fact]
        public void GetDeleteCommand_ReturnsStringThatContainsNonNullValues()
        {
            // Arrange

            // Act
            string returnedCommand = SqlSyntaxFormattingService.GetDeleteCommand(tableName, attributes, values);

            // Assert
            foreach (string value in values)
            {
                Assert.Contains(value, returnedCommand);
            }
        }

        [Fact]
        public void GetDeleteCommand_ReturnsStringThatContainsNullForNullValues()
        {
            // Arrange

            // Act
            string returnedCommand = SqlSyntaxFormattingService.GetDeleteCommand(tableName, attributes, valuesWithNull);

            // Assert
            Assert.Contains("IS NULL", returnedCommand);
        }
    }
}
