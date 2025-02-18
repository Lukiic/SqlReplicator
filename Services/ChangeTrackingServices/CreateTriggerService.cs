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

        public bool CreateTrigger(string tableName)
        {
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
												SELECT *, ''I''
												FROM inserted;
											END

											-- UPDATE Operation
											IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted)
											BEGIN
												INSERT INTO {tableName}Changes
												SELECT *, ''U''						-- U -> New updated values
												FROM inserted;

												INSERT INTO {tableName}Changes
												SELECT *, ''O''						-- O -> Old values (before update)
												FROM deleted;
											END

											-- DELETE operation
											IF EXISTS (SELECT * FROM deleted) AND NOT EXISTS (SELECT * FROM inserted)
											BEGIN
												INSERT INTO {tableName}Changes
												SELECT *, ''D''
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
