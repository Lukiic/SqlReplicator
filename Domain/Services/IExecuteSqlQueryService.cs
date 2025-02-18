using SQLReplicator.Domain.Interfaces;

namespace SQLReplicator.Domain.Services
{
    public interface IExecuteSqlQueryService
    {
        public IDataReaderWrapper ExecuteQuery(string query);
    }
}
