using Azure.Storage.Blobs;
using RandomMessageApp.Core.CommonServices.Interfaces;
using RandomMessageApp.Core.Exceptions;
using RandomMessageApp.Storage.Configuration;
using RandomMessageApp.Storage.Services.Interfaces;

namespace RandomMessageApp.Storage.Services.Implementation;

public class FileStorage : IFileStorage
{
    private readonly StorageSettings _storageSettings;
    private readonly IJsonSerializationService _jsonSerializationService;

    public FileStorage(
        StorageSettings storageSettings,
        IJsonSerializationService jsonSerializationService)
    {
        _storageSettings = storageSettings;
        _jsonSerializationService = jsonSerializationService;
    }

    public async Task UploadFileAsync(string filePath, object obj)
    {
        var json = _jsonSerializationService.Serialize(obj);
        var content = System.Text.Encoding.UTF8.GetBytes(json);

        await using var memoryStream = new MemoryStream(content);

        memoryStream.Seek(0, SeekOrigin.Begin);

        var blob = await GetBlobAsync(filePath);
        await blob.UploadAsync(memoryStream, true);
    }

    public async Task<T> ReadFileAsync<T>(string filePath)
    {
        var blob = await GetBlobAsync(filePath);

        var download = await blob.DownloadAsync();
        await using var stream = download.Value.Content;

        return _jsonSerializationService.Deserialize<T>(stream);
    }

    private async Task<BlobClient> GetBlobAsync(string filePath, bool checkExistence = false)
    {
        var container = await GetContainerAsync();
        var blob = container.GetBlobClient(filePath);

        if (checkExistence)
        {
            var blobExists = await blob.ExistsAsync();
            if (!blobExists)
            {
                throw new InfrastructureException($"File '{filePath}' does not exist.");
            }
        }

        return blob;
    }

    private async Task<BlobContainerClient> GetContainerAsync()
    {
        var storageAccount = new BlobServiceClient(_storageSettings.ConnectionString);

        var containerClient = storageAccount.GetBlobContainerClient(_storageSettings.ContainerName.ToLower());

        var containerExists = await containerClient.ExistsAsync();

        if (containerExists)
        {
            return containerClient;
        }

        await containerClient.CreateIfNotExistsAsync();

        return containerClient;
    }
}
