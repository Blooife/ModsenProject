using Domain.Models.Entities;

namespace Application.UseCases.EventUseCases.GetAllEvents;

public class GetAllEventsResponse
{
    public IEnumerable<Event> Events { get; set; }
}