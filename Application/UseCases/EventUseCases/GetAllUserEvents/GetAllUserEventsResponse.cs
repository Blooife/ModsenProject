using Domain.Models.Entities;

namespace Application.UseCases.EventUseCases.GetAllUserEvents;

public class GetAllUserEventsResponse
{
    public IEnumerable<Event> events { get; set; }
}