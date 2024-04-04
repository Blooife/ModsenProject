using Application.Models.Dtos;
using Application.Servicies.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/events")]
public class EventController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventController(IEventService eventService)
    {
        _eventService = eventService;
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetEvents()
    {
        var events = await _eventService.GetAllAsync();
        return Ok(events);
    }
    
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetEventById(string id)
    {
        var ev = await _eventService.GetByIdAsync(id);
        return Ok(ev);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateEvent([FromBody] EventRequestDto post)
    {
        var createdEvent = await _eventService.CreateAsync(post);
        return CreatedAtAction(nameof(CreateEvent), new { id = createdEvent.Id }, createdEvent);
    }

    [HttpPut]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateEvent([FromBody] EventRequestDto post)
    {
        var updatedEvent = await _eventService.UpdateAsync(post);
        return Ok(updatedEvent);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteEvent(string id)
    {
        await _eventService.DeleteAsync(id);
        return NoContent();
    }
    
    [HttpGet("getByName/{name}")]
    [Authorize]
    public async Task<IActionResult> GetEventByName(string name)
    {
        var ev = await _eventService.GetByNameAsync(name);
        return Ok(ev);
    }
    
    [HttpGet("getByCategory/{category}")]
    [Authorize]
    public async Task<IActionResult> GetEventsByCategory(string category)
    {
        var ev = await _eventService.GetByCategoryAsync(category);
        return Ok(ev);
    }
    
    [HttpGet("getByPlace/{place}")]
    [Authorize]
    public async Task<IActionResult> GetEventsByPlace(string place)
    {
        var ev = await _eventService.GetByPlaceAsync(place);
        return Ok(ev);
    }
    
    [HttpGet("getByDate/{date}")]
    [Authorize]
    public async Task<IActionResult> GetEventsByDate(DateTime date)
    {
        var ev = await _eventService.GetByDateAsync(date);
        return Ok(ev);
    }
    
    [HttpPost("getFilteredEvents")]
    [Authorize]
    public async Task<IActionResult> GetFilteredEvents([FromBody] FiltersRequestDto filters)
    {
        var events = await _eventService.GetFilteredEventsAsync(filters);
        return Ok(events);
    }
    
    [HttpPost("upload")]
    [Authorize(Roles = "ADMIN")]
    public async Task<object> UploadPicture([FromForm] IFormFile file, [FromForm]string eventId)
    {
        var response = await _eventService.UploadPictureAsync(file, eventId);
        return Ok(response.Message);
    }
}