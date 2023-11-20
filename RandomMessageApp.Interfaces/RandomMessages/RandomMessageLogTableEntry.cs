using Azure;
using Azure.Data.Tables;

namespace RandomMessageApp.Interfaces.RandomMessages;

public sealed record RandomMessageLogTableEntry : ITableEntity
{
    public string PartitionKey { get; set; }

    public string RowKey { get; set; }

    public DateTimeOffset? Timestamp { get; set; }

    public ETag ETag { get; set; }

    public string FileName { get; set; }

    public string Result { get; set; }
}
