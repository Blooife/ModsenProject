using System.Reflection;
using Application.Mapper;
using Application.Models.Dtos;
using Application.Servicies.Implementations;
using Application.Servicies.Interfaces;
using Application.Validators;
using Domain.Models.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddTransient<AbstractValidator<EventRequestDto>, EventValidator>();
        services.AddTransient<AbstractValidator<RegistrationRequestDto>, UserValidator>();
        
        
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        return services;
    }
    
}