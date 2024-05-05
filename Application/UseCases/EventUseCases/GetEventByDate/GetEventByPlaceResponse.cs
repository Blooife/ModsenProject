using Domain.Models.Entities;

namespace Application.UseCases.EventUseCases.GetEventByDate;

public class GetEventByDateResponse
{
    public IEnumerable<Event> events { get; set; }
}