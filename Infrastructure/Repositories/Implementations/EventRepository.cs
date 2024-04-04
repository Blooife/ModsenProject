using Application.Exceptions;
using Domain.Models.Entities;
using Domain.Repositories.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations;

public class EventRepository : GenericRepository<Event>, IEventRepository
{
    private readonly AppDbContext _dbContext;

    public EventRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Event>> GetByCategoryAsync(string category)
    {
        return await _dbContext.Events.Where(ev => ev.Category == category).ToArrayAsync();
    }

    public async Task<IEnumerable<Event>> GetByPlaceAsync(string place)
    {
        return await _dbContext.Events.Where(ev => ev.Place == place).ToArrayAsync();
    }

    public async Task<IEnumerable<Event>> GetByDateAsync(DateTime date)
    {
        return await _dbContext.Events.Where(ev => ev.Date.Date == date.Date).ToArrayAsync();
    }

    public async Task<Event?> GetByNameAsync(string name)
    {
        return _dbContext.Events.FirstOrDefault((ev => ev.Name == name));
    }
    
    public async Task UpdatePlacesLeftAsync(Event entity, int inc)
    {
        var usersCount = await _dbContext.EventsUsers.CountAsync(e=> e.EventId == entity.Id);
        usersCount += inc;
        entity.PlacesLeft = entity.MaxParticipants - usersCount;
        if (entity.PlacesLeft < 0)
        {
            throw new BadRequestException("you cant update participants count");
        }
        await UpdateAsync(entity);
    }
}