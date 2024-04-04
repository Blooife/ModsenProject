using Application.Exceptions;
using Domain.Models.Entities;
using Domain.Repositories.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations;

public class EventsUsersRepository : IEventsUsersRepository
{
    private readonly AppDbContext _dbContext;

    public EventsUsersRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
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
        var entity = await _dbContext.EventsUsers.FirstOrDefaultAsync(eu => eu.EventId == eventId && eu.UserId == userId);
        if (entity != null)
        {
            _dbContext.EventsUsers.Remove(entity);
        }
        else
        {
            throw new NotFoundException("users events not found", userId);
        }
        
    }
}