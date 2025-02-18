namespace SQLReplicator.Domain.Services
{
    public interface ICreateChangeTrackingTableService
    {
        public bool CreateCTTable(string tableName);
    }
}
