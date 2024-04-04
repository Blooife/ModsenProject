using Domain.Models.Entities;
using Domain.Repositories.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Repositories.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private IEventRepository _eventRepository;
    private IUserRepository _userRepository;
    private IEventsUsersRepository _eventsUsersRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;


    public UnitOfWork(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _dbContext = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public IEventRepository EventRepository => 
        _eventRepository ??= new EventRepository(_dbContext);
    public IUserRepository UserRepository => 
        _userRepository ??= new UserRepository(_dbContext, _userManager, _roleManager);
    public IEventsUsersRepository EventsUsersRepository => 
        _eventsUsersRepository ??= new EventsUsersRepository(_dbContext);
        
    public void Dispose()
    {
        _dbContext.Dispose();
        GC.SuppressFinalize(this);
    }


    public async Task Save() 
    {
        await _dbContext.SaveChangesAsync();
    }
}