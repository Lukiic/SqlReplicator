using SQLReplicator.Domain.Interfaces;
using SQLReplicator.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public List<string> GetCommands(string tableName)
        {
            IDataReaderWrapper dataReader = _changeTrackingDataService.LoadData(tableName);
            List<string> attributes = dataReader.ReadAttributes();
            List<List<string>> listOfValues = dataReader.ReadValues();  // Inner list represents one row of Change Tracking table

            dataReader.Dispose();
            _changeTrackingDataService.DeleteData(tableName);   // Data has been loaded, so it is now deleted from table

            return _sqlCommandsGenerationService.GetCommands(tableName, attributes, listOfValues);
        }
    }
}
