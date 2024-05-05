using AutoMapper;
using Domain.Models.Entities;

namespace Application.UseCases.EventUseCases.GetEventById;

public class GetEventByIdMapper: Profile
{
    public GetEventByIdMapper()
    {
        CreateMap<Event, GetEventByIdResponse>();
    }
}