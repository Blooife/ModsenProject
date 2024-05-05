using Domain.Models.Entities;

namespace Application.UseCases.EventUseCases.GetEventByCategory;

public class GetEventByCategoryResponse
{
    public IEnumerable<Event> events { get; set; }
}