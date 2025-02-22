using SQLReplicator.Domain.Interfaces;

namespace SQLReplicator.Domain.Services
{
    public interface IChangeTrackingDataService
    {
        public IDataReaderWrapper LoadData(string tableName, string lastChangeID, List<string> keyAttributes);
    }
}
