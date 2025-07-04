// Mapping/BookProfile.cs
using AutoMapper;
using VirtualLibraryAPI.Dtos;
using VirtualLibraryAPI.Entities;

public class BookProfile : Profile
{
    public BookProfile()
    {
        CreateMap<Book, BookDto>();
        // If your DTO property names differ, you can .ForMember(...) here.
    }
}