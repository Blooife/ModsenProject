using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models.Entities;


public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    
    public string? RefreshToken { get; set; }
    
    public DateTime? RefreshTokenEndDate { get; set; }
    
    // navigation properties
    public IEnumerable<EventsUsers> EventsUsers { get; set; }
    public IEnumerable<Event> Events { get; set; }
}