using SQLReplicator.Domain.Interfaces;

namespace SQLReplicator.Domain.Services
{
    public interface IChangeTrackingDataService
    {
        public IDataReaderWrapper LoadData(string tableName, string replicatedBitNum, List<string> keyAttributes);
    }
}
