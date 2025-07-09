// Mapping/BookProfile.cs
using AutoMapper;
using VirtualLibraryAPI.Dtos;
using VirtualLibraryAPI.Entities;

public class BookProfile : Profile
{
    public BookProfile()
    {
        CreateMap<Book, BookDto>();
        CreateMap<BookDto, Book>()
            // often you don’t want to overwrite the PK on updates:
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        // If your DTO property names differ, you can .ForMember(...) here.
    }
}