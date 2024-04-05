using Application.Models.Dtos;
using Domain.Models.Entities;

namespace Application.Servicies.Interfaces;

public interface IAuthService
{
    Task<ResponseDto> RegisterAsync(RegistrationRequestDto registrationRequestDto, CancellationToken cancellationToken);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto, CancellationToken cancellationToken);
    Task<ResponseDto> AssignRoleAsync(string name, string roleName, CancellationToken cancellationToken);
    Task<LoginResponseDto> RefreshToken(string refrToken, CancellationToken cancellationToken);
    Task<ResponseDto> CreateRoleAsync(string roleName);

}