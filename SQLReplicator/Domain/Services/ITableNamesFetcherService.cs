namespace SQLReplicator.Domain.Services
{
    public interface ITableNamesFetcherService
    {
        public List<string> GetTableNames();
    }
}
