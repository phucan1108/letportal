namespace LetPortal.Core.Persistences
{
    public interface IPersistenceConnection<T>
    {
        void ReloadOptions(DatabaseOptions databaseOptions);

        T GetDatabaseConnection(string databaseName = null);
    }
}