namespace SQLReplicator.Domain.Services
{
    public interface ITrackedDataToCommandsService
    {
        public (List<string>, string) GetCommandsAndLastChangeID(string tableName, string lastChangeID, List<string> keyAttributes);
    }
}
