using MediatR;

namespace Application.UseCases.AuthUseCases.AssignRole;

public sealed record AssignRoleRequest: IRequest<AssignRoleResponse>
{
    public string Email { get; set; }
    public string Role { get; set; }
}