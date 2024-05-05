using Domain.Models.Entities;

namespace Application.UseCases.EventUseCases.GetEventByPlace;

public class GetEventByPlaceResponse
{
    public IEnumerable<Event> events { get; set; }
}