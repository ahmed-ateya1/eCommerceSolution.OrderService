using MongoDB.Driver;
using OrderService.RepositoryLayer.Entities;
using System.Linq.Expressions;

namespace OrderService.DataAccessLayer.RepositoryContracts
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync(FilterDefinition<Order>?filter = null);
        Task<Order?> GetByAsync(FilterDefinition<Order> filter);
        Task<Order?> CreateAsync(Order order);
        Task<Order?> UpdateAsync(Order order);
        Task<bool> DeleteAsync(Guid orderID);
    }
}
