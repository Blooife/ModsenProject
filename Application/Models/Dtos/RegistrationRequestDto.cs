namespace Application.Models.Dtos;

public class RegistrationRequestDto
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public string Password { get; set; }
}