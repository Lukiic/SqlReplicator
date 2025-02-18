namespace SQLReplicator.Domain.Services
{
    public interface ISqlCommandsGenerationService
    {
        public List<string> GetCommands(string tableName, List<string> attributes, List<List<string>> listOfValues);
    }
}
