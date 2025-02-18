using Serilog;
using SQLReplicator.Domain.Interfaces;
using SQLReplicator.Domain.Services;

namespace SQLReplicator.Services.CommandPreparationServices
{
    public class TrackedDataToCommandsService : ITrackedDataToCommandsService
    {
        private IChangeTrackingDataService _changeTrackingDataService;
        private ISqlCommandsGenerationService _sqlCommandsGenerationService;

        public TrackedDataToCommandsService(IChangeTrackingDataService changeTrackingDataService, ISqlCommandsGenerationService sqlCommandsGenerationService)
        {
            _changeTrackingDataService = changeTrackingDataService;
            _sqlCommandsGenerationService = sqlCommandsGenerationService;
        }

        public (List<string>, string) GetCommandsAndLastChangeID(string tableName, string lastChangeID)
        {
            IDataReaderWrapper dataReader;
            try
            {
                dataReader = _changeTrackingDataService.LoadData(tableName, lastChangeID);
            }
            catch (Exception)
            {
                Log.Warning($"Failed to load data from {tableName}Changes table");
                return (new List<string>(), lastChangeID);
            }

            List<string> attributes = dataReader.ReadAttributes();
            List<List<string>> listOfValues = dataReader.ReadValues();  // Inner list represents one row of Change Tracking table

            dataReader.Dispose();

            List<string> commands = _sqlCommandsGenerationService.GetCommands(tableName, attributes, listOfValues);

            if (listOfValues.Count != 0)
            {
                lastChangeID = listOfValues.Last().Last();   // Last column of last row -> last ChangeID that is processed
            }

            return (commands, lastChangeID);
        }
    }
}
