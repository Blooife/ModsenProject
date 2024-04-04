using System.Runtime.InteropServices.JavaScript;
using Application.Exceptions;
using Application.Models.Dtos;
using Application.Servicies.Interfaces;
using AutoMapper;
using Domain.Models.Entities;
using Domain.Repositories.Interfaces;
using EntityFramework.Exceptions.Common;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ValidationException = FluentValidation.ValidationException;

namespace Application.Servicies.Implementations;

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
    
    public async Task<IEnumerable<Event>> GetAllAsync()
    {
        return await _unitOfWork.EventRepository.GetAllAsync();
    }

    public async Task<Event> GetByIdAsync(string id)
    {
        var ev = await _unitOfWork.EventRepository.GetByIdAsync(id);
        if (ev == null)
        {
            throw new NotFoundException("Event", id);
        }
        return ev;
    }

    public async Task<Event> CreateAsync(EventRequestDto dto)
    {
        var result =  await _validation.ValidateAsync(dto);
        if (!result.IsValid)
        {
            throw new Exceptions.ValidationException(result);
        }
        dto.PlacesLeft = dto.MaxParticipants;
        var entity = _mapper.Map<Event>(dto);
        var res =await _unitOfWork.EventRepository.CreateAsync(entity);
        try
        {
            await _unitOfWork.Save();
        }
        catch(DbUpdateException ex)
        {
            throw new DbException("Db exception");
        }
        return res;
    }

    public async Task<Event> UpdateAsync(EventRequestDto dto)
    {
        var result =  await _validation.ValidateAsync(dto);
        if (!result.IsValid)
        {
            throw new ValidationException("Invalid data for event");
        }
        var entity = _mapper.Map<Event>(dto);
        if (! await _unitOfWork.EventRepository.Exists(entity.Id))
        {
            throw new NotFoundException("Event", entity.Id);
        }

        await _unitOfWork.EventRepository.UpdatePlacesLeftAsync(entity, 0);
        
        try
        {
            await _unitOfWork.Save();
        }
        catch(DbUpdateException ex)
        {
            throw new DbException("db exception");
        }
        return entity;
    }

    public async Task<Event> DeleteAsync(string id)
    {
        var entity = await _unitOfWork.EventRepository.GetByIdAsync(id);
        if (entity == null)
        {
            throw new NotFoundException("Event", id);
        }
        await _unitOfWork.EventRepository.DeleteAsync(entity);
        try
        {
            await _unitOfWork.Save();
        }
        catch(DbUpdateException ex)
        {
            throw new DbException("db exception");
        }
        return entity;
    }

    public async Task<IEnumerable<Event>> GetByCategoryAsync(string category)
    {
        return await _unitOfWork.EventRepository.GetByCategoryAsync(category);
    }

    public async Task<IEnumerable<Event>> GetByPlaceAsync(string place)
    {
        return  await _unitOfWork.EventRepository.GetByPlaceAsync(place);
    }

    public async Task<IEnumerable<Event>> GetByDateAsync(DateTime date)
    {
        return await _unitOfWork.EventRepository.GetByDateAsync(date);
    }
    
    public async Task<IEnumerable<Event>> GetFilteredEventsAsync(FiltersRequestDto filters)
    {
        IEnumerable<Event> res = Enumerable.Empty<Event>();
        IEnumerable<Event> res1 = Enumerable.Empty<Event>();
        if (!string.IsNullOrEmpty(filters.category) && !string.IsNullOrEmpty(filters.place))
        {
            res = await _unitOfWork.EventRepository.GetByCategoryAsync(filters.category);
            res1 = await _unitOfWork.EventRepository.GetByPlaceAsync(filters.place);
            return res.Intersect(res1);
        }
        if(!string.IsNullOrEmpty(filters.category))
        {
            return await _unitOfWork.EventRepository.GetByCategoryAsync(filters.category);
        }
        if(!string.IsNullOrEmpty(filters.place))
        {
            return await _unitOfWork.EventRepository.GetByPlaceAsync(filters.place);
        }

        return res;
    }

    public async Task<Event> GetByNameAsync(string name)
    {
        var ev =  await _unitOfWork.EventRepository.GetByNameAsync(name);
        if (ev == null)
        {
            throw new NotFoundException("Event", name);
        }
        return ev;
    }
    
    public async Task<ResponseDto> UploadPictureAsync(IFormFile file, string eventId)
    {
        var fileExtension = Path.GetExtension(file.FileName);

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", };

        if (allowedExtensions.Contains(fileExtension.ToLowerInvariant()))
        {
            var parentDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            var path = Path.Combine(parentDirectory,"frontapp/public/pictures", eventId+file.FileName);

            try
            {
                using var stream = new FileStream(path, FileMode.Create);
                await file.CopyToAsync(stream);
            }
            catch(Exception ex)
            {
                throw new ImageUploadException("Failed upload image");
            }
        }
        return new ResponseDto() { Message = eventId+file.FileName};
    }
}