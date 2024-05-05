using AutoMapper;
using Domain.Models.Entities;

namespace Application.UseCases.EventUseCases.DeleteEvent;

public class DeleteEventMapper: Profile
{
    public DeleteEventMapper()
    {
        CreateMap<Event, DeleteEventResponse>();
    }
}