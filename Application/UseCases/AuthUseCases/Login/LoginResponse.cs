using Application.Models.Dtos;

namespace Application.UseCases.AuthUseCases.Login;

public class LoginResponse
{
    public UserDto User { get; set; }
    public string Token { get; set; }
}