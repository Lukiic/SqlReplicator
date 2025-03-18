namespace SQLReplicator.Domain.Services
{
    public interface IExecuteListOfCommandsService
    {
        public int ExecuteCommands(List<string> commands);  // Returns the count of commands that were not executed from the input list
    }
}
