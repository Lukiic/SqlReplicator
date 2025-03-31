using SQLReplicator.Services.CommandPreparationServices;
using System.Text.RegularExpressions;

namespace SQLReplicator.Tests.Services.CommandPreparationServices
{
    public class SqlCommandsGenerationServiceTests
    {
        /*
            Values: ID1, ID2, ..., IDn, Operation, ChangeID, IsReplicated1, IsReplicated2, Attr1, Attr2, AttrM
                    -----------------------------                                          -------------------
                    ChangeTracking table values                                            Tracked table values
        */

        private SqlCommandsGenerationService _sqlCommandsGenerationService;
        private string tableName;
        private List<string> changeTrackingAttrs;
        private List<string> trackedTableAttrs;
        private List<string> keyAttributes;
        private List<List<string>> listOfValues;
        private List<List<string>> listOfWrongValues;

        public SqlCommandsGenerationServiceTests()
        {
            _sqlCommandsGenerationService = new SqlCommandsGenerationService();

            tableName = "SampleName";
            changeTrackingAttrs = new List<string>() { "ID1", "ID2", "IDn", "Operation" };
            trackedTableAttrs = new List<string>() { "Attr1", "Attr2", "AttrM" };
            keyAttributes = new List<string>() { "ID1", "ID2", "IDn" };
            listOfValues = new List<List<string>>() {
                new List<string>() { "1", "2", "3", "I", "99", "0", "0", "Attr1Val", "Attr2Val", "Attr3Val" },
                new List<string>() { "4", "5", "6", "D", "99", "0", "0", "Attr1Val", "Attr2Val", "Attr3Val" },
                new List<string>() { "7", "8", "9", "U", "99", "0", "0", "Attr1Val", "Attr2Val", "Attr3Val" }
            };
            listOfWrongValues = new List<List<string>>() {
                new List<string>() { "1", "2", "3", "M", "99", "0", "0", "Attr1Val", "Attr2Val", "Attr3Val" },
                new List<string>() { "4", "5", "6", "F", "99", "0", "0", "Attr1Val", "Attr2Val", "Attr3Val" },
                new List<string>() { "7", "8", "9", "O", "99", "0", "0", "Attr1Val", "Attr2Val", "Attr3Val" }
            };
        }

        [Fact]
        public void GetCommands_EachStringContainsTwoCommands()
        {
            // Arrange
            string regexPattern = @".+;.+";     // Command ; Command

            // Act
            List<string> commands = _sqlCommandsGenerationService.GetCommands(tableName, changeTrackingAttrs, trackedTableAttrs, keyAttributes, listOfValues);

            // Assert
            foreach (string command in commands)
            {
                bool isMatch = Regex.IsMatch(command, regexPattern);
                Assert.True(isMatch);
            }
        }

        [Fact]
        public void GetCommands_EachStringContainsChangeTrackingDeletionCommand()
        {
            // Arrange

            // Act
            List<string> commands = _sqlCommandsGenerationService.GetCommands(tableName, changeTrackingAttrs, trackedTableAttrs, keyAttributes, listOfValues);

            // Assert
            foreach (string command in commands)
            {
                Assert.Contains($"DELETE FROM {tableName}Changes", command);
            }
        }

        [Fact]
        public void GetCommands_ReturnsEmptyListForUnexpectedOperationValues()
        {
            // Arrange

            // Act
            List<string> commands = _sqlCommandsGenerationService.GetCommands(tableName, changeTrackingAttrs, trackedTableAttrs, keyAttributes, listOfWrongValues);

            // Assert
            Assert.Empty(commands);
        }
    }
}
