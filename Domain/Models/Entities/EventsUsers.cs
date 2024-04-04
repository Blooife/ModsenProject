using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Entities;

public class EventsUsers
{
    [Key]
    public string Id { get; set; }
    [ForeignKey("UserId")]
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    [ForeignKey("EventId")]
    public string EventId { get; set; }
    public Event Event { get; set; }
    public DateTime RegistrationDate { get; set; }
}