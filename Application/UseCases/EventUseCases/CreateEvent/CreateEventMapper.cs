using AutoMapper;
using Domain.Models.Entities;

namespace Application.UseCases.EventUseCases.CreateEvent;

public class CreateEventMapper: Profile
{
    public CreateEventMapper()
    {
        CreateMap<CreateEventRequest, Event>();
        CreateMap<Event, CreateEventResponse>();
    }
}