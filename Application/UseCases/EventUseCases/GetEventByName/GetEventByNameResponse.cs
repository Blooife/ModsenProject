using Domain.Models.Entities;

namespace Application.UseCases.EventUseCases.GetEventByName;

public class GetEventByNameResponse
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public string Place { get; set; }
    public string Category { get; set; }
    public int MaxParticipants { get; set; }
    public string? Picture { get; set; }
    
    public int PlacesLeft { get; set; }
}