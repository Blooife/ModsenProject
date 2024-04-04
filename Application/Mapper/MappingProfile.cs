using Application.Models.Dtos;
using AutoMapper;
using Domain.Models.Entities;

namespace Application.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<EventRequestDto, Event>();
        //CreateMap<Event, ResponseDto<Event>>().ReverseMap();
        CreateMap<RegistrationRequestDto, ApplicationUser>()
            .ForMember(dest => dest.UserName, act => act.MapFrom(src => src.Email))
            .ForMember(dest => dest.NormalizedEmail, act => act.MapFrom(src => src.Email.ToUpper()));
        CreateMap<ApplicationUser, UserDto>();
    }
}