using AutoMapper;
using RandomMessageApp.Application.Models;
using RandomMessageApp.Interfaces.RandomMessages;

namespace RandomMessageApp.WebApi.Mapping;

public class RandomMessagesProfile : Profile
{
    public RandomMessagesProfile()
    {
        CreateMap<RandomMessage, RandomMessageModel>();
        CreateMap<RandomMessageEntry, RandomMessageEntryModel>();
    }
}
