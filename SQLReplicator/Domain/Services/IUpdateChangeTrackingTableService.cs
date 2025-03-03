namespace SQLReplicator.Domain.Services
{
    public interface IUpdateChangeTrackingTableService
    {
        public bool UpdateReplicatedBit(string tableName, string lastChangeID, string replicatedBitNum);
    }
}
