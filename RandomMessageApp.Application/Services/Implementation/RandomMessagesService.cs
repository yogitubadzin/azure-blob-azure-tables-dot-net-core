using AutoMapper;
using RandomMessageApp.Application.Models;
using RandomMessageApp.Application.Services.Interfaces;
using RandomMessageApp.Interfaces.RandomMessages;
using RandomMessageApp.Storage.Services.Interfaces;

namespace RandomMessageApp.Application.Services.Implementation;

public class RandomMessagesService : IRandomMessagesService
{
    private readonly ITableStorage _tableStorage;
    private readonly IFileStorage _fileStorage;
    private readonly IMapper _mapper;

    public RandomMessagesService(
        ITableStorage tableStorage,
        IFileStorage fileStorage,
        IMapper mapper)
    {
        _tableStorage = tableStorage;
        _fileStorage = fileStorage;
        _mapper = mapper;
    }

    public async Task<List<RandomMessageMetadata>> GetMessagesAsync(DateTime from, DateTime to)
    {
        var entries = await _tableStorage.GetEntitiesAsync<RandomMessageLogTableEntry>(from, to);

        var randomMessages = new List<RandomMessageMetadata>();
        foreach (var entry in entries)
        {
            randomMessages.Add(new RandomMessageMetadata
            {
                Id = Guid.Parse(entry.RowKey),
                DateTime = entry.Timestamp.Value.DateTime,
                Result = entry.Result
            });
        }

        return randomMessages;
    }

    public async Task<RandomMessageModel> GetMessageAsync(Guid messageId)
    {
        var fileName = $"{messageId}.json";

        var message = await _fileStorage.ReadFileAsync<RandomMessage>(fileName);

        return _mapper.Map<RandomMessageModel>(message);
    }
}
