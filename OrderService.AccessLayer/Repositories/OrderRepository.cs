using MongoDB.Driver;
using OrderService.DataAccessLayer.RepositoryContracts;
using OrderService.RepositoryLayer.Entities;
using System.Collections;

namespace OrderService.DataAccessLayer.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IMongoCollection<Order> _orderCollection;
        private readonly string collectionName = "orders";
        public OrderRepository(IMongoDatabase mongoDatabase)
        {
            _orderCollection = mongoDatabase.GetCollection<Order>(collectionName);
        }
        public async Task<Order?> CreateAsync(Order order)
        {
            order.OrderID = Guid.NewGuid();
            order._id = Guid.NewGuid();
            foreach (var item in order.OrderItems)
            {
                item._id = Guid.NewGuid();
            }
            await _orderCollection.InsertOneAsync(order);
            return order;
        }

        public async Task<bool> DeleteAsync(Guid orderID)
        {
            var filter = Builders<Order>.Filter.Eq(order => order.OrderID, orderID);

            var existingOrder = (await _orderCollection.FindAsync(filter)).FirstOrDefault();
            if (existingOrder == null)
            {
                return false;
            }

            var result = await _orderCollection.DeleteOneAsync(filter);

            return result.DeletedCount > 0; 
        }


        public async Task<IEnumerable<Order>> GetAllAsync(FilterDefinition<Order>? filter = null)
        {
            filter ??= FilterDefinition<Order>.Empty;
            return (await _orderCollection.FindAsync(filter)).ToList();
        }

        public async Task<Order?> GetByAsync(FilterDefinition<Order> filter)
        {
            return (await _orderCollection.FindAsync(filter)).FirstOrDefault();
        }

        public async Task<Order?> UpdateAsync(Order order)
        {
            var filter = Builders<Order>.Filter.Eq(order => order.OrderID,order.OrderID);

            var existingOrder = (await _orderCollection.FindAsync(filter)).FirstOrDefault();
            if (existingOrder == null)
            {
                return null;
            }
            await _orderCollection.ReplaceOneAsync(filter, order);
            return order;
        }
    }
}
