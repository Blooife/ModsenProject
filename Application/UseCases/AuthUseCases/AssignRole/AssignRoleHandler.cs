using Application.Exceptions;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.UseCases.AuthUseCases.AssignRole;

public class AssignRoleHandler: IRequestHandler<AssignRoleRequest, AssignRoleResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public AssignRoleHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<AssignRoleResponse> Handle(AssignRoleRequest request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByNameAsync(request.Email, cancellationToken);
        if (user != null)
        {
            var isRoleExist = await _unitOfWork.UserRepository.RoleExistsAsync(request.Role.ToUpper());
            if (!isRoleExist)
            {
                throw new AssignRoleException(ExceptionMessages.RoleNotExists);
            }
            await _unitOfWork.UserRepository.AddToRoleAsync(user, request.Role.ToUpper());
            return new AssignRoleResponse()
            {
                Message = "Role assigned successfully",
            };
        }
        throw new AssignRoleException(ExceptionMessages.ErrorAssigningRole);
    }
}