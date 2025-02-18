namespace SQLReplicator.Domain.Services
{
    public interface IExecuteListOfCommandsService
    {
        public void ExecuteCommands(List<string> commands);
    }
}
