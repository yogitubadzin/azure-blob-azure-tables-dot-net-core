namespace RandomMessageApp.Storage.Interfaces;

public interface ITableStorage
{
    Task UpsertEntityAsync<T>(T entity)
        where T : ITableEntity;
}
