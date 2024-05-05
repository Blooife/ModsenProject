using AutoMapper;
using Domain.Models.Entities;

namespace Application.UseCases.EventUseCases.GetEventByPlace;

public class GetEventByPlaceMapper: Profile
{
    public GetEventByPlaceMapper()
    {
        CreateMap<Event, GetEventByPlaceResponse>();
    }
}