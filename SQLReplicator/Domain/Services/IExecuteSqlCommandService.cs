namespace SQLReplicator.Domain.Services
{
    public interface IExecuteSqlCommandService
    {
        public void ExecuteCommand(string command);
    }
}
