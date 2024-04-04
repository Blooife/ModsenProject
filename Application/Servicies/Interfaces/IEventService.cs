using Application.Models.Dtos;
using Domain.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Servicies.Interfaces;

public interface IEventService
{
    Task<IEnumerable<Event>> GetAllAsync();

    Task<Event> GetByIdAsync(string id);

    Task<Event> CreateAsync(EventRequestDto dto);

    Task<Event> UpdateAsync(EventRequestDto dto);

    Task<Event> DeleteAsync(string id);
    
    Task<IEnumerable<Event>> GetByCategoryAsync(string category);
    Task<IEnumerable<Event>> GetByPlaceAsync(string place);
    Task<IEnumerable<Event>> GetByDateAsync(DateTime date);
    Task<Event> GetByNameAsync(string name);
    Task<IEnumerable<Event>> GetFilteredEventsAsync(FiltersRequestDto filters);
    Task<ResponseDto> UploadPictureAsync(IFormFile file, string eventId);
}