namespace RandomMessageApp.Storage.Services.Interfaces;

public interface IFileStorage
{
    Task UploadFileAsync(string filePath, object obj);

    Task<T> ReadFileAsync<T>(string filePath);
}
