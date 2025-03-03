namespace SQLReplicator.Domain.Services
{
    public interface IPrimaryKeyAttributesService
    {
        public List<string> GetPrimaryKeyAttributes(string tableName);
    }
}
