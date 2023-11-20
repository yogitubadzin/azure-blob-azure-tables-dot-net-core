using Microsoft.AspNetCore.Mvc;
using RandomMessageApp.Application.Models;
using RandomMessageApp.Application.Services.Interfaces;

namespace RandomMessageApp.WebApi.Controllers;

[ApiController]
[Route("[controller]/messages")]
public class RandomMessagesController : ControllerBase
{
    private readonly IRandomMessagesService _randomMessagesService;

    public RandomMessagesController(IRandomMessagesService randomMessagesService)
    {
        _randomMessagesService = randomMessagesService;
    }

    [HttpGet]
    public async Task<List<RandomMessageMetadata>> Get(DateTime from, DateTime to)
    {
        return await _randomMessagesService.GetMessagesAsync(from, to);
    }

    [HttpGet("{messageId}")]
    public async Task<RandomMessageModel> GetMessage(Guid messageId)
    {
        return await _randomMessagesService.GetMessageAsync(messageId);
    }
}
