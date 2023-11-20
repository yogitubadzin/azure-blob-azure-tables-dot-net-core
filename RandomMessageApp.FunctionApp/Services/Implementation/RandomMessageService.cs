using System.Threading.Tasks;
using Azure;
using RandomMessageApp.Core.CommonServices.Interfaces;
using RandomMessageApp.FunctionApp.Configuration;
using RandomMessageApp.FunctionApp.Models;
using RandomMessageApp.FunctionApp.Services.Interfaces;
using RandomMessageApp.Interfaces.RandomMessages;
using RandomMessageApp.Storage.Services.Interfaces;

namespace RandomMessageApp.FunctionApp.Services.Implementation;

public class RandomMessageService : IRandomMessageService
{
    private readonly RandomMessageServiceSettings _randomMessageServiceSettings;
    private readonly IHttpClientService _httpClientService;
    private readonly IFileStorage _fileStorage;
    private readonly ITableStorage _tableStorage;
    private readonly IDateTimeService _dateTimeService;
    private readonly ITableStoragePrimaryKeyGenerator _tableStoragePrimaryKeyGenerator;

    public RandomMessageService(
        RandomMessageServiceSettings randomMessageServiceSettings,
        IHttpClientService httpClientService,
        IFileStorage fileStorage,
        ITableStorage tableStorage,
        IDateTimeService dateTimeService,
        ITableStoragePrimaryKeyGenerator tableStoragePrimaryKeyGenerator)
    {
        _randomMessageServiceSettings = randomMessageServiceSettings;
        _httpClientService = httpClientService;
        _fileStorage = fileStorage;
        _tableStorage = tableStorage;
        _dateTimeService = dateTimeService;
        _tableStoragePrimaryKeyGenerator = tableStoragePrimaryKeyGenerator;
    }

    public async Task RunAsync()
    {
        var url = _randomMessageServiceSettings.Url;
        var messageResult = await _httpClientService.GetAsync<RandomMessage>(url);
        var (partitionKey, rowKey) = _tableStoragePrimaryKeyGenerator.Generate();

        if (messageResult.Result != null)
        {
            var fileName = $"{rowKey}.json";
            await _fileStorage.UploadFileAsync(fileName, messageResult.Result);

            var randomMessageLogTableEntry = CreateRandomMessageLogTableEntry(partitionKey, rowKey, fileName, RandomMessageResult.Success);
            await _tableStorage.UpsertEntityAsync(randomMessageLogTableEntry);
        }
        else
        {
            var randomMessageLogTableEntry = CreateRandomMessageLogTableEntry(partitionKey, rowKey, null, RandomMessageResult.Failure);
            await _tableStorage.UpsertEntityAsync(randomMessageLogTableEntry);
        }
    }

    private RandomMessageLogTableEntry CreateRandomMessageLogTableEntry(
        string partitionKey,
        string rowKey,
        string fileName,
        RandomMessageResult randomMessageResult)
    {
        return new RandomMessageLogTableEntry
        {
            PartitionKey = partitionKey,
            RowKey = rowKey,
            FileName = fileName,
            Result = randomMessageResult.ToString(),
            Timestamp = _dateTimeService.UtcNow,
            ETag = ETag.All
        };
    }
}
