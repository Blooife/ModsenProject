using Application.Models.Dtos;
using AutoMapper;
using Domain.Models.Entities;

namespace Application.UseCases.AuthUseCases.Login;

public class LoginMapper: Profile
{
    public LoginMapper()
    {
        CreateMap<ApplicationUser, UserDto>().ReverseMap();
    }
}