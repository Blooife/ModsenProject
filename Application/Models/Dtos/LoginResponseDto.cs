namespace Application.Models.Dtos;

public class LoginResponseDto
{
    public UserDto User { get; set; }
    public string Token { get; set; }
    public IList<string>? Role { get; set; }
}