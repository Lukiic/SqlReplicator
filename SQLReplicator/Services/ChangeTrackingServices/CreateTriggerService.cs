using SQLReplicator.Domain.Services;

namespace SQLReplicator.Services.ChangeTrackingServices
{
    public class CreateTriggerService : ICreateTriggerService
    {
        private IExecuteSqlCommandService _executeSqlCommandService;

        public CreateTriggerService(IExecuteSqlCommandService executeSqlCommandService)
        {
            _executeSqlCommandService = executeSqlCommandService;
        }

        public bool CreateTrigger(string tableName, List<string> keyAttributes)
        {
			string keyAttributesCSV = string.Join(',', keyAttributes);
            string command = $@"IF NOT EXISTS (SELECT * FROM sys.triggers WHERE name = 'TrackChanges{tableName}')
								BEGIN
									EXEC('
										CREATE TRIGGER TrackChanges{tableName}
										ON {tableName}
										AFTER INSERT, UPDATE, DELETE
										AS
										BEGIN
											-- INSERT Operation
											IF EXISTS (SELECT * FROM inserted) AND NOT EXISTS (SELECT * FROM deleted)
											BEGIN
												INSERT INTO {tableName}Changes
												SELECT {keyAttributesCSV}, ''I'', 0, 0						-- IsReplicated bits - default 0
												FROM inserted;
											END

											-- UPDATE Operation
											IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted)
											BEGIN
												INSERT INTO {tableName}Changes
												SELECT {keyAttributesCSV}, ''D'', 0, 0						-- Deleting old values
												FROM deleted;

												INSERT INTO {tableName}Changes
												SELECT {keyAttributesCSV}, ''I'', 0, 0						-- Inserting new (updated) values
												FROM inserted;
											END

											-- DELETE operation
											IF EXISTS (SELECT * FROM deleted) AND NOT EXISTS (SELECT * FROM inserted)
											BEGIN
												INSERT INTO {tableName}Changes
												SELECT {keyAttributesCSV}, ''D'', 0, 0
												FROM deleted;
											END
										END
									')
								END;";

            bool isCreated = true;
            try
            {
                _executeSqlCommandService.ExecuteCommand(command);
            }
            catch (Exception)
            {
                isCreated = false;
            }

            return isCreated;
        }
    }
}
