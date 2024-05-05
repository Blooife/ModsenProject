using MediatR;

namespace Application.UseCases.AuthUseCases.Login;

public sealed record LoginRequest: IRequest<LoginResponse>
{
    public string Email { get; set; }
    public string Password { get; set; }
}