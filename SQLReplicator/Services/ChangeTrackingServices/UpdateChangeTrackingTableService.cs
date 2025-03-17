using Serilog;
using SQLReplicator.Domain.Services;

namespace SQLReplicator.Services.ChangeTrackingServices
{
    public class UpdateChangeTrackingTableService : IUpdateChangeTrackingTableService
    {
        private IExecuteSqlCommandService _executeSqlCommandService;

        public UpdateChangeTrackingTableService(IExecuteSqlCommandService executeSqlCommandService)
        {
            _executeSqlCommandService = executeSqlCommandService;
        }

        public bool UpdateReplicatedBit(string tableName, string lastChangeID, string replicatedBitNum)
        {
            string command = $@"UPDATE {tableName}Changes
                                SET IsReplicated{replicatedBitNum} = 1
                                WHERE ChangeID <= {lastChangeID};";

            bool isUpdated = true;
            try
            {
                _executeSqlCommandService.ExecuteCommand(command);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, $"Failed to execute command: {command}");
                isUpdated = false;
            }

            return isUpdated;
        }
    }
}
