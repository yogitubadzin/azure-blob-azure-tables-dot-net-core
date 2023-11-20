using Azure.Data.Tables;
using RandomMessageApp.Storage.Azure.Configuration;
using RandomMessageApp.Storage.Interfaces;

namespace RandomMessageApp.Storage.Azure;

public class TableStorage : ITableStorage
{
    private readonly StorageSettings _storageSettings;

    public TableStorage(StorageSettings storageSettings)
    {
        _storageSettings = storageSettings;
    }

    public async Task UpsertEntityAsync<T>(T entity)
        where T : ITableEntity
    {
        var tableClient = await GetTableClient();
        await tableClient.UpsertEntityAsync(entity);
    }

    private async Task<TableClient> GetTableClient()
    {
        var serviceClient = new TableServiceClient(_storageSettings.ConnectionString);

        var tableClient = serviceClient.GetTableClient(_storageSettings.TableName);
        await tableClient.CreateIfNotExistsAsync();
        return tableClient;
    }
}
