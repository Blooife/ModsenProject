
namespace Domain.Models.Entities;

public class EventsUsers
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public string EventId { get; set; }
    public Event Event { get; set; }
    public DateTime RegistrationDate { get; set; }
}