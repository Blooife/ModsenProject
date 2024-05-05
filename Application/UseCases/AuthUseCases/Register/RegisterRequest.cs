using MediatR;

namespace Application.UseCases.AuthUseCases.Register;

public sealed record RegisterRequest: IRequest<RegisterResponse>
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public string Password { get; set; }
}