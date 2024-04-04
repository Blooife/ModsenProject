namespace Domain.Models.Entities;

public class Event //: BaseDomainEntity
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
    //Navigation properties
    public IEnumerable<EventsUsers> EventsUsers { get; set; }
    public IEnumerable<ApplicationUser> Users { get; set; }
    
}