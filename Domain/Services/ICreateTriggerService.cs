namespace SQLReplicator.Domain.Services
{
    public interface ICreateTriggerService
    {
        public bool CreateTrigger(string tableName);
    }
}
