namespace SQLReplicator.Domain.Services
{
    public interface ISqlCommandsGenerationService
    {
        public List<string> GetCommands(string tableName, List<string> changeTrackingAttrs, List<string> trackedTableAttrs, List<string> keyAttributes, List<List<string?>> listOfValues);
    }
}
