using AutoMapper;
using Domain.Models.Entities;

namespace Application.UseCases.EventUseCases.GetEventByCategory;

public class GetEventByCategoryMapper: Profile
{
    public GetEventByCategoryMapper()
    {
        CreateMap<Event, GetEventByCategoryResponse>();
    }
}