namespace RandomMessageApp.Storage.Configuration;

public sealed record StorageSettings
{
    public string ConnectionString { get; set; }

    public string ContainerName { get; set; }

    public string TableName { get; set; }
}
