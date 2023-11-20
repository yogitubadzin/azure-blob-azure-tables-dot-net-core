using Azure.Data.Tables;

namespace RandomMessageApp.Storage.Services.Interfaces;

public interface ITableStorage
{
    Task UpsertEntityAsync<T>(T entity)
        where T : ITableEntity;

    Task<List<T>> GetEntitiesAsync<T>(DateTime from, DateTime to)
        where T : class, ITableEntity, new();
}
