using MongoDB.Driver;
using OrderService.BusinessLayer.Dtos;
using OrderService.RepositoryLayer.Entities;

namespace OrderService.BusinessLayer.ServiceContracts
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateAsync(OrderAddRequest? orderRequest);
        Task<OrderResponse> UpdateAsync(OrderUpdateRequest? orderRequest);
        Task<bool> DeleteAsync(Guid orderId);
        Task<OrderResponse> GetByAsync(FilterDefinition<Order> filter);
        Task<IEnumerable<OrderResponse>> GetAllAsync(FilterDefinition<Order>? filter = null);
    }
}
