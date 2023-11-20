namespace RandomMessageApp.Storage.Interfaces;

public interface IFileStorage
{
    Task UploadFileAsync(string filePath, object obj);
}
