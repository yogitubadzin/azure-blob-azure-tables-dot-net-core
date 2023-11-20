using System;
using RandomMessageApp.Core.CommonServices.Interfaces;
using RandomMessageApp.FunctionApp.Services.Interfaces;

namespace RandomMessageApp.FunctionApp.Services.Implementation;

public class TableStoragePrimaryKeyGenerator : ITableStoragePrimaryKeyGenerator
{
    private readonly IDateTimeService _dateTimeService;
    private readonly IPartitionKeyGenerator _partitionKeyGenerator;

    public TableStoragePrimaryKeyGenerator(
        IDateTimeService dateTimeService,
        IPartitionKeyGenerator partitionKeyGenerator)
    {
        _dateTimeService = dateTimeService;
        _partitionKeyGenerator = partitionKeyGenerator;
    }

    public (string, string) Generate()
    {
        var now = _dateTimeService.UtcNow;
        var partitionKey = _partitionKeyGenerator.Generate(now);
        var rowKey = Guid.NewGuid().ToString();

        return (partitionKey, rowKey);
    }
}
