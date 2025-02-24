namespace SQLReplicator.Domain.Interfaces
{
    public interface IDataReaderWrapper : IDisposable
    {
        public List<string> ReadAttributes();
        public List<List<string?>> ReadValues();
    }
}
