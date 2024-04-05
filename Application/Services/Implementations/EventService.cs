using Application.Exceptions;
using Application.Models.Dtos;
using Application.Servicies.Interfaces;
using AutoMapper;
using Domain.Models.Entities;
using Domain.Repositories.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ValidationException = FluentValidation.ValidationException;

namespace Application.Services.Implementations;

public class EventService : IEventService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly AbstractValidator<EventRequestDto> _validation;

    public EventService(IUnitOfWork unitOfWork, IMapper mapper, AbstractValidator<EventRequestDto> validator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validation = validator;
    }
    
    public async Task<IEnumerable<Event>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _unitOfWork.EventRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Event> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var ev = await _unitOfWork.EventRepository.GetByIdAsync(id, cancellationToken);
        if (ev == null)
        {
            throw new NotFoundException("Event", id);
        }
        return ev;
    }

    public async Task<Event> CreateAsync(EventRequestDto dto, CancellationToken cancellationToken)
    {
        var result =  await _validation.ValidateAsync(dto, cancellationToken);
        if (!result.IsValid)
        {
            throw new Exceptions.ValidationException(result);
        }
        dto.PlacesLeft = dto.MaxParticipants;
        var entity = _mapper.Map<Event>(dto);
        var res =await _unitOfWork.EventRepository.CreateAsync(entity, cancellationToken);
        
        await _unitOfWork.Save();
        
        return res;
    }

    public async Task<Event> UpdateAsync(EventRequestDto dto, CancellationToken cancellationToken)
    {
        var result =  await _validation.ValidateAsync(dto, cancellationToken);
        if (!result.IsValid)
        {
            throw new ValidationException("Invalid data for event");
        }
        var entity = _mapper.Map<Event>(dto);
        if (! await _unitOfWork.EventRepository.Exists(entity.Id, cancellationToken))
        {
            throw new NotFoundException("Event", entity.Id);
        }

        var updateResult = await _unitOfWork.EventRepository.UpdatePlacesLeftAsync(entity, 0, cancellationToken);

        if (!updateResult)
        {
            throw new BadRequestException("you cant update participants count");
        }
        
        await _unitOfWork.Save();
        
        return entity;
    }

    public async Task<Event> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.EventRepository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException("Event", id);
        }
        await _unitOfWork.EventRepository.DeleteAsync(entity, cancellationToken);
        
        await _unitOfWork.Save();
        return entity;
    }

    public async Task<IEnumerable<Event>> GetByCategoryAsync(string category, CancellationToken cancellationToken)
    {
        return await _unitOfWork.EventRepository.GetByCategoryAsync(category, cancellationToken);
    }

    public async Task<IEnumerable<Event>> GetByPlaceAsync(string place, CancellationToken cancellationToken)
    {
        return  await _unitOfWork.EventRepository.GetByPlaceAsync(place, cancellationToken);
    }

    public async Task<IEnumerable<Event>> GetByDateAsync(DateTime date, CancellationToken cancellationToken)
    {
        return await _unitOfWork.EventRepository.GetByDateAsync(date, cancellationToken);
    }
    
    public async Task<IEnumerable<Event>> GetFilteredEventsAsync(FiltersRequestDto filters, CancellationToken cancellationToken)
    {
        IEnumerable<Event> res = Enumerable.Empty<Event>();
        IEnumerable<Event> res1 = Enumerable.Empty<Event>();
        if (!string.IsNullOrEmpty(filters.category) && !string.IsNullOrEmpty(filters.place))
        {
            res = await _unitOfWork.EventRepository.GetByCategoryAsync(filters.category, cancellationToken);
            res1 = await _unitOfWork.EventRepository.GetByPlaceAsync(filters.place, cancellationToken);
            return res.Intersect(res1);
        }
        if(!string.IsNullOrEmpty(filters.category))
        {
            return await _unitOfWork.EventRepository.GetByCategoryAsync(filters.category, cancellationToken);
        }
        if(!string.IsNullOrEmpty(filters.place))
        {
            return await _unitOfWork.EventRepository.GetByPlaceAsync(filters.place, cancellationToken);
        }

        return res;
    }

    public async Task<Event> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        var ev =  await _unitOfWork.EventRepository.GetByNameAsync(name, cancellationToken);
        if (ev == null)
        {
            throw new NotFoundException("Event", name);
        }
        return ev;
    }
    
    public async Task<ResponseDto> UploadPictureAsync(IFormFile file, string eventId, CancellationToken cancellationToken)
    {
        var fileExtension = Path.GetExtension(file.FileName);

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", };

        if (allowedExtensions.Contains(fileExtension.ToLowerInvariant()))
        {
            var parentDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            var path = Path.Combine(parentDirectory,"frontapp/public/pictures", eventId+file.FileName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream, cancellationToken);
        }
        else
        {
            throw new ImageUploadException("Wrong extension");
        }
        return new ResponseDto() { Message = eventId+file.FileName};
    }
    
    public async Task<ResponseDto> RegisterUserOnEvent(string userId, string eventId, CancellationToken cancellationToken)
    {
        var ev = await _unitOfWork.EventRepository.GetByIdAsync(eventId, cancellationToken);
        if (ev == null)
        {
            throw new NotFoundException("Event", eventId);
        }

        if (ev.PlacesLeft < 1)
        {
            throw new ArgumentException("No places left");
        }
        if (!await _unitOfWork.UserRepository.Exists(userId, cancellationToken))
        {
            throw new NotFoundException("User", userId);
        }
        await _unitOfWork.EventsUsersRepository.RegisterUserOnEvent(userId, eventId, cancellationToken);
        await _unitOfWork.EventRepository.UpdatePlacesLeftAsync(ev, 1, cancellationToken);
        await _unitOfWork.Save();
        
        return new ResponseDto { Message = "User successfully registered on event" };
    }
        
    public async Task<ResponseDto> UnregisterUserOnEvent(string userId, string eventId, CancellationToken cancellationToken)
    {
        var ev = await _unitOfWork.EventRepository.GetByIdAsync(eventId, cancellationToken);
        if (ev == null)
        {
            throw new NotFoundException("Event", eventId);
        }
        if (!await _unitOfWork.UserRepository.Exists(userId, cancellationToken))
        {
            throw new NotFoundException("User", userId);
        }
        var unregisterResult = await _unitOfWork.EventsUsersRepository.UnregisterUserOnEvent(userId, eventId, cancellationToken);
        if (!unregisterResult)
        {
            throw new NotFoundException("users events for ", userId);
        }
        await _unitOfWork.EventRepository.UpdatePlacesLeftAsync(ev, -1, cancellationToken);
        
        await _unitOfWork.Save();
        return new ResponseDto { Message = "User successfully unregistered on event" };
    }
    
    public async Task<IEnumerable<Event>> GetAllUserEvents(string userId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.EventsUsersRepository.GetAllUserEvents(userId, cancellationToken);
    }

    public async Task<PagedList<Event>> GetPagedEvents(PageParams pageParams, CancellationToken cancellationToken)
    {
        return await _unitOfWork.EventRepository.GetPagedEventsAsync(pageParams.PageNumber, pageParams.PageSize,
            cancellationToken);
    }
}