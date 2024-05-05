using Domain.Models.Entities;

namespace Application.UseCases.EventUseCases.GetFilteredEvents;

public class GetFilteredEventsResponse
{
    public IEnumerable<Event> events { get; set; }
}