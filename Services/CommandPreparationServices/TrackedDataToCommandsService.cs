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

        public (List<string>, string) GetCommandsAndLastChangeID(string tableName, string lastChangeID, List<string> keyAttributes)
        {
            IDataReaderWrapper dataReader;
            int keyAttrsCount = keyAttributes.Count;

            try
            {
                dataReader = _changeTrackingDataService.LoadData(tableName, lastChangeID, keyAttributes);
            }
            catch (Exception)
            {
                Log.Warning($"Failed to load data from {tableName}Changes table");
                return (new List<string>(), lastChangeID);
            }

            List<string> attributes = dataReader.ReadAttributes();
            // Attributes: ID1, ID2, ..., IDn, Operation, ChangeID, Attr1, Attr2, AttrM

            List<List<string>> listOfValues = dataReader.ReadValues();  // Inner list represents one row of table with "attributes"

            List<string> changeTrackingAttrs = attributes.Take(keyAttrsCount + 1).ToList();
            // ChangeTracking attributes: ID1, ID2, ..., IDn, Operation     - Without 'ChangeID' attribute

            List<string> trackedTableAttrs = attributes.Skip(keyAttrsCount + 2).ToList();
            // Tracked table attributes: Attr1, Attr2, AttrM

            dataReader.Dispose();

            List<string> commands = _sqlCommandsGenerationService.GetCommands(tableName, changeTrackingAttrs, trackedTableAttrs, keyAttributes, listOfValues);

            if (listOfValues.Count != 0)
            {
                lastChangeID = listOfValues.Last()[changeTrackingAttrs.Count];   // 'ChangeID' value for last row
            }

            return (commands, lastChangeID);
        }
    }
}
