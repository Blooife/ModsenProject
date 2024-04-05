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
    public async Task RegisterUserOnEvent(string userId, string eventId, CancellationToken cancellationToken)
    {
        await _dbContext.EventsUsers.AddAsync(new EventsUsers()
        {
            EventId = eventId,
            UserId = userId,
            RegistrationDate = DateTime.Now,
        }, cancellationToken);
    }
    
    public async Task<bool> UnregisterUserOnEvent(string userId, string eventId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.EventsUsers.FirstOrDefaultAsync(eu => eu.EventId == eventId && eu.UserId == userId, cancellationToken);
        if (entity == null) return false;
        _dbContext.EventsUsers.Remove(entity);
        return true;
    }
    
    public async Task<IEnumerable<Event>> GetAllUserEvents(string userId, CancellationToken cancellationToken)
    {
        var userEv = await _dbContext.EventsUsers
                .Where(eu => eu.UserId == userId)
                .Include(eu => eu.Event)
                .Select(eu => eu.Event)
                .ToListAsync(cancellationToken);

        return userEv;
       
    }
}