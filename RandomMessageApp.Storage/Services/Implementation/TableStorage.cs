using Azure.Data.Tables;
using RandomMessageApp.Core.CommonServices.Interfaces;
using RandomMessageApp.Storage.Configuration;
using RandomMessageApp.Storage.Services.Interfaces;

namespace RandomMessageApp.Storage.Services.Implementation;

public class TableStorage : ITableStorage
{
    private readonly StorageSettings _storageSettings;
    private readonly IPartitionKeyGenerator _partitionKeyGenerator;

    public TableStorage(
        StorageSettings storageSettings,
        IPartitionKeyGenerator partitionKeyGenerator)
    {
        _storageSettings = storageSettings;
        _partitionKeyGenerator = partitionKeyGenerator;
    }

    public async Task UpsertEntityAsync<T>(T entity)
        where T : ITableEntity
    {
        var tableClient = await GetTableClientAsync();
        await tableClient.UpsertEntityAsync(entity);
    }

    public async Task<List<T>> GetEntitiesAsync<T>(DateTime from, DateTime to)
        where T : class, ITableEntity, new()
    {
        var tableClient = await GetTableClientAsync();
        var partitionKeyFrom = _partitionKeyGenerator.Generate(from);
        var partitionKeyTo = _partitionKeyGenerator.Generate(to.AddHours(1));
        var filter = $"(PartitionKey le '{partitionKeyFrom}') and(PartitionKey gt '{partitionKeyTo}')";
        var queryResult = tableClient.QueryAsync<T>(filter);
        var queriedEntities = new List<T>();

        await foreach (var page in queryResult.AsPages())
        {
            queriedEntities.AddRange(page.Values);
        }

        var filteredResult = queriedEntities.Where(x => from <= x.Timestamp && to >= x.Timestamp)
            .OrderByDescending(x => x.Timestamp)
            .ToList();

        return filteredResult;
    }

    private async Task<TableClient> GetTableClientAsync()
    {
        var serviceClient = new TableServiceClient(_storageSettings.ConnectionString);

        var tableClient = serviceClient.GetTableClient(_storageSettings.TableName);
        await tableClient.CreateIfNotExistsAsync();
        return tableClient;
    }
}
