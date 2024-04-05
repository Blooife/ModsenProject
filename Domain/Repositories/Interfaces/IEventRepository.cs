using Domain.Models.Entities;

namespace Domain.Repositories.Interfaces;

public interface IEventRepository : IGenericRepository<Event>
{
    Task<IEnumerable<Event>> GetByCategoryAsync(string category, CancellationToken cancellationToken);
    Task<IEnumerable<Event>> GetByPlaceAsync(string place, CancellationToken cancellationToken);
    Task<IEnumerable<Event>> GetByDateAsync(DateTime date, CancellationToken cancellationToken);
    Task<Event?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<bool> UpdatePlacesLeftAsync(Event entity, int inc, CancellationToken cancellationToken);
    Task<PagedList<Event>> GetPagedEventsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
}