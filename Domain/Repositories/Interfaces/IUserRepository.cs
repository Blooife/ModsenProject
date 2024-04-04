using Domain.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace Domain.Repositories.Interfaces;

public interface IUserRepository : IGenericRepository<ApplicationUser>
{
    Task<ApplicationUser?> GetByEmailAsync(string email);
    Task<IdentityResult> RegisterAsync(ApplicationUser user, string password);

    Task<ApplicationUser?> GetByNameAsync(string name);
    Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    Task<IList<string>> GetRolesAsync(ApplicationUser user);
    Task<bool> RoleExistsAsync(string roleName);
    Task CreateRoleAsync(string roleName);
    Task AddToRoleAsync(ApplicationUser user, string roleName);
    Task RegisterUserOnEvent(string userId, string eventId);
    Task<IEnumerable<Event>> GetAllUserEvents(string userId);
    Task<ApplicationUser?> GetByRefreshTokenAsync(string token);
    Task UnregisterUserOnEvent(string userId, string eventId);
}