using AutoMapper;
using OrderService.BusinessLayer.Dtos;
using OrderService.RepositoryLayer.Entities;

namespace OrderService.BusinessLayer.MappingProfile
{
    public class OrderItemsConfig : Profile
    {
        public OrderItemsConfig()
        {
            CreateMap<OrderItemsAddRequest, OrderItems>()
                .ReverseMap();
            CreateMap<OrderItems, OrderItemsResponse>()
                .ReverseMap();
        }
    }   
}
