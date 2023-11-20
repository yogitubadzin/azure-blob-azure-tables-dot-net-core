using RandomMessageApp.Application.Models;

namespace RandomMessageApp.Application.Services.Interfaces;

public interface IRandomMessagesService
{
    Task<List<RandomMessageMetadata>> GetMessagesAsync(DateTime from, DateTime to);

    Task<RandomMessageModel> GetMessageAsync(Guid messageId);
}
