
using Application.Mapper;
using Application.Models.Dtos;
using Application.Services.Implementations;
using Application.Servicies.Interfaces;
using Application.Validators;
using FluentValidation;
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