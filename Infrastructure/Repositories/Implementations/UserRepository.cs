using Domain.Models.Entities;
using Domain.Repositories.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations;

public class UserRepository : GenericRepository<ApplicationUser>, IUserRepository
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public UserRepository(AppDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) : base(dbContext)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string email)
    {
        return _dbContext.Users.FirstOrDefault(u => u.Email == email);
    }

    public async Task<IdentityResult> RegisterAsync(ApplicationUser user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<ApplicationUser?> GetByNameAsync(string name)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == name);
    }
    
    public async Task<ApplicationUser?> GetByRefreshTokenAsync(string token)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(x => x.RefreshToken == token);
    }

    public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<bool> RoleExistsAsync(string roleName)
    {
        return await _roleManager.RoleExistsAsync(roleName);
    }

    public async Task CreateRoleAsync(string roleName)
    {
        await _roleManager.CreateAsync(new IdentityRole(roleName));
    }
    
    public async Task RegisterUserOnEvent(string userId, string eventId)
    {
        await _dbContext.EventsUsers.AddAsync(new EventsUsers()
        {
            EventId = eventId,
            UserId = userId,
            RegistrationDate = DateTime.Now,
        });
    }
    
    public async Task UnregisterUserOnEvent(string userId, string eventId)
    {
        _dbContext.EventsUsers.Remove(new EventsUsers()
        {
            EventId = eventId,
            UserId = userId,
        });
    }
    
    public async Task<IEnumerable<Event>> GetAllUserEvents(string userId)
    {
        try
        {
            var userEv = await _dbContext.EventsUsers
                .Where(eu => eu.UserId == userId)
                .Include(eu => eu.Event)
                .Select(eu => eu.Event)
                .ToListAsync();

            return userEv;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task AddToRoleAsync(ApplicationUser user, string roleName)
    {
        await _userManager.AddToRoleAsync(user, roleName);
    }
}