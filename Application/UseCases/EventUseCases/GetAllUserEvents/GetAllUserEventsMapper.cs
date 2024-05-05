using AutoMapper;
using Domain.Models.Entities;

namespace Application.UseCases.EventUseCases.GetAllUserEvents;

public class GetAllUserEventsMapper: Profile
{
    public GetAllUserEventsMapper()
    {
        CreateMap<Event, GetAllUserEventsResponse>();
    }
}