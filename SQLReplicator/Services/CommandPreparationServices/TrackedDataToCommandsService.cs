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

        public (List<string>, int) GetCommandsAndLastChangeID(string tableName, string replicatedBitNum, List<string> keyAttributes)
        {
            IDataReaderWrapper dataReader;
            int keyAttrsCount = keyAttributes.Count;
            int lastChangeID = 0;
            List<string> attributes;
            List<List<string?>> listOfValues;

            try
            {
                using (dataReader = _changeTrackingDataService.LoadData(tableName, replicatedBitNum, keyAttributes))
                {
                    attributes = dataReader.ReadAttributes();
                    // Attributes: ID1, ID2, ..., IDn, Operation, ChangeID, IsReplicated1, IsReplicated2, Attr1, Attr2, AttrM

                    listOfValues = dataReader.ReadValues();  // Inner list represents one row of table with "attributes"
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Unable to read data from the Change Tracking table.");
                return (new List<string>(), lastChangeID);
            }

            List<string> changeTrackingAttrs = attributes.Take(keyAttrsCount + 1).ToList();
            // ChangeTracking attributes: ID1, ID2, ..., IDn, Operation     - Without 'ChangeID' and 'IsReplicated' attributes

            List<string> trackedTableAttrs = attributes.Skip(keyAttrsCount + 4).ToList();
            // Tracked table attributes: Attr1, Attr2, AttrM

            List<string> commands = _sqlCommandsGenerationService.GetCommands(tableName, changeTrackingAttrs, trackedTableAttrs, keyAttributes, listOfValues);

            if (listOfValues.Count != 0)
            {
                lastChangeID = int.Parse(listOfValues.Last()[changeTrackingAttrs.Count]);   // 'ChangeID' value for last row
            }

            return (commands, lastChangeID);
        }
    }
}
