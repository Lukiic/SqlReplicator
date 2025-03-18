namespace SQLReplicator.Domain.Services
{
    public interface ITrackedDataToCommandsService
    {
        public (List<string>, int) GetCommandsAndLastChangeID(string tableName, string replicatedBitNum, List<string> keyAttributes);
    }
}
