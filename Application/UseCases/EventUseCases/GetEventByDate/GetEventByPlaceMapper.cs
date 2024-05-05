using AutoMapper;
using Domain.Models.Entities;

namespace Application.UseCases.EventUseCases.GetEventByDate;

public class GetEventByDateMapper: Profile
{
    public GetEventByDateMapper()
    {
        CreateMap<Event, GetEventByDateResponse>();
    }
}