using AutoMapper;
using OrderService.BusinessLayer.Dtos;
using OrderService.RepositoryLayer.Entities;

namespace OrderService.BusinessLayer.MappingProfile
{
    public class OrderConfig : Profile
    {
        public OrderConfig()
        {
            CreateMap<OrderAddRequest, Order>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems)) 
                .ForMember(dest=>dest.OrderID,opt=>opt.MapFrom(src=> Guid.NewGuid()))
                .ReverseMap();

            CreateMap<OrderUpdateRequest, Order>()
                .ForMember(dest=>dest.OrderItems , opt=>opt.MapFrom(src=>src.OrderItems))
                .ReverseMap();

            CreateMap<Order,OrderResponse>()
                .ForMember(dest => dest.TotalBill, opt => opt.MapFrom(src => src.OrderItems.Sum(x => x.Quantity * x.UnitPrice)))
                .ReverseMap();

        }
    }
}
