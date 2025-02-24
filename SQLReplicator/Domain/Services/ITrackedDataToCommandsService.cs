namespace SQLReplicator.Domain.Services
{
    public interface ITrackedDataToCommandsService
    {
        public (List<string>, string) GetCommandsAndLastChangeID(string tableName, string replicatedBitNum, List<string> keyAttributes);
    }
}
