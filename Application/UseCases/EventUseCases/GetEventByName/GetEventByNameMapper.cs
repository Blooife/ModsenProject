using AutoMapper;
using Domain.Models.Entities;

namespace Application.UseCases.EventUseCases.GetEventByName;

public class GetEventByNameMapper: Profile
{
    public GetEventByNameMapper()
    {
        CreateMap<Event, GetEventByNameResponse>();
    }
}