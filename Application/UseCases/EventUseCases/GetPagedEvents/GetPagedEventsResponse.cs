using Domain.Models.Entities;

namespace Application.UseCases.EventUseCases.GetPagedEvents;

public class GetPagedEventsResponse
{
    public PagedList<Event> events { get; set; }
}