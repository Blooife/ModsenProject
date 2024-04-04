using Application.Models.Dtos;
using Domain.Models.Entities;

namespace Application.Servicies.Interfaces;

public interface IAuthService
{
    Task<ResponseDto> RegisterAsync(RegistrationRequestDto registrationRequestDto);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto);
    Task<ResponseDto> AssignRoleAsync(string name, string roleName);
    Task<ResponseDto> RegisterUserOnEvent(string userId, string eventId);
    Task<IEnumerable<Event>> GetAllUserEvents(string userId);
    Task<LoginResponseDto> RefreshToken(string refrToken);
    Task<ResponseDto> UnregisterUserOnEvent(string userId, string eventId);
}