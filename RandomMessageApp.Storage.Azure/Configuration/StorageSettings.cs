namespace RandomMessageApp.Storage.Azure.Configuration;

public class StorageSettings
{
    public string ConnectionString { get; set; }

    public string ContainerName { get; set; }

    public string TableName { get; set; }
}
