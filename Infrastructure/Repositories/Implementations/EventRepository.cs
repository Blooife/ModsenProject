
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

    public async Task<IEnumerable<Event>> GetByCategoryAsync(string category, CancellationToken cancellationToken)
    {
        return await _dbContext.Events.Where(ev => ev.Category == category).ToArrayAsync(cancellationToken);
    }

    public async Task<IEnumerable<Event>> GetByPlaceAsync(string place, CancellationToken cancellationToken)
    {
        return await _dbContext.Events.Where(ev => ev.Place == place).ToArrayAsync(cancellationToken);
    }

    public async Task<IEnumerable<Event>> GetByDateAsync(DateTime date, CancellationToken cancellationToken)
    {
        return await _dbContext.Events.Where(ev => ev.Date.Date == date.Date).ToArrayAsync(cancellationToken);
    }

    public async Task<Event?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.Events.FirstOrDefaultAsync((ev => ev.Name == name), cancellationToken);
    }
    
    public async Task<bool> UpdatePlacesLeftAsync(Event entity, int inc, CancellationToken cancellationToken)
    {
        var usersCount = await _dbContext.EventsUsers.CountAsync(e=> e.EventId == entity.Id, cancellationToken);
        usersCount += inc;
        entity.PlacesLeft = entity.MaxParticipants - usersCount;
        if (entity.PlacesLeft < 0)
        {
            return false;
        }
        await UpdateAsync(entity, cancellationToken);
        return true;
    }

    public async Task<PagedList<Event>> GetPagedEventsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return PagedList<Event>.ToPagedList(FindAll().OrderBy(e=>e.Name),
            pageNumber,
            pageSize);
    }
}