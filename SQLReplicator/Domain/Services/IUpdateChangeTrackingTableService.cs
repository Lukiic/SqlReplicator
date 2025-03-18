namespace SQLReplicator.Domain.Services
{
    public interface IUpdateChangeTrackingTableService
    {
        public bool UpdateReplicatedBit(string tableName, int lastChangeID, string replicatedBitNum);
    }
}
