// Mapping/CartProfile.cs
using AutoMapper;
using VirtualLibraryAPI.Dtos;
using VirtualLibraryAPI.Entities;

public class CartProfile : Profile
{
    public CartProfile()
    {
        CreateMap<CartItem, CartItemDto>()
            .ForMember(d => d.BookName,  o => o.MapFrom(s => s.Book.Name))
            .ForMember(d => d.BookPrice, o => o.MapFrom(s => s.Book.Price));

        // Map from Add/Update DTOs into entity
        CreateMap<AddCartItemDto, CartItem>();
        CreateMap<UpdateCartItemDto, CartItem>();
    }
}