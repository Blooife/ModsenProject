using MediatR;

namespace Application.UseCases.AuthUseCases.CreateRole;

public sealed record CreateRoleRequest(string roleName): IRequest<CreateRoleResponse>
{
}