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

        public bool UpdateReplicatedBit(string tableName, int lastChangeID, string replicatedBitNum)
        {
            ValidateArguments(tableName, lastChangeID, replicatedBitNum);

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

        private void ValidateArguments(string tableName, int lastChangeID, string replicatedBitNum)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException("Table name cannot be null or empty.", nameof(tableName));
            }

            if (lastChangeID < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lastChangeID), "Last change ID cannot be negative.");
            }

            if (string.IsNullOrWhiteSpace(replicatedBitNum))
            {
                throw new ArgumentException("Replicated bit number cannot be null or empty.", nameof(replicatedBitNum));
            }
        }
    }
}
