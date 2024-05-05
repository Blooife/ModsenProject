using AutoMapper;
using Domain.Models.Entities;

namespace Application.UseCases.EventUseCases.UpdateEvent;

public class UpdateEventMapper: Profile
{
    public UpdateEventMapper()
    {
        CreateMap<UpdateEventRequest, Event>();
        CreateMap<Event, UpdateEventResponse>();
    }
}