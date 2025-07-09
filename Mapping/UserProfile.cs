namespace VirtualLibraryAPI.Mapping;

using AutoMapper;
using VirtualLibraryAPI.Dtos;
using VirtualLibraryAPI.Entities;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, EditUserDto>().ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.Ignore()); // Don't overwrite PK
        // Add .ForMember(...) if you need to ignore or map special fields
        CreateMap<EditUserDto, User>()
            .ForMember(dest => dest.Password, opt => opt.Ignore());
    }
}