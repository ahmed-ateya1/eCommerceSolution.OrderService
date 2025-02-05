using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OrderService.BusinessLayer.Dtos;
using OrderService.BusinessLayer.ServiceContracts;
using OrderService.RepositoryLayer.Entities;

namespace OrderService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet("getall")]
        public async Task<IActionResult>GetAllOrders()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }
        [HttpGet("getBy/{orderID}")]
        public async Task<IActionResult> GetOrderByID(Guid orderID)
        {
            var filter = Builders<Order>.Filter.Eq(x => x.OrderID, orderID);
            var order = await _orderService.GetByAsync(filter);
            return Ok(order);
        }
        [HttpGet("searchByProductID/{productID}")]
        public async Task<IActionResult> SearchOrderByProductID(Guid productID)
        {
            var filter = Builders<Order>.Filter.ElemMatch(x => x.OrderItems, x => x.ProductID == productID);
            var order = await _orderService.GetByAsync(filter);
            return Ok(order);
        }
        [HttpGet("searchByDate/{orderDate}")]
        public async Task<IActionResult> SearchOrderByDate(DateTime orderDate)
        {
            var filter = Builders<Order>.Filter.Eq(x => x.OrderDate.ToString("yyyy-MM-dd"),
                orderDate.ToString("yyyy-MM-dd"));

            var order = await _orderService.GetByAsync(filter);
            return Ok(order);
        }
        [HttpGet("searchByUserID/{userID}")]
        public async Task<IActionResult> SearchOrderByUserID(Guid userID)
        {
            var filter = Builders<Order>.Filter.Eq(x => x.UserID, userID);
            var order = await _orderService.GetByAsync(filter);
            return Ok(order);
        }
        [HttpPost("createOrder")]
        public async Task<IActionResult> CreateOrder(OrderAddRequest orderRequest)
        {
            if (orderRequest == null)
            {
                return BadRequest("Invalid Order Data");
            }
            var order = await _orderService.CreateAsync(orderRequest);
            if (order == null)
            {
                return BadRequest("Error in adding product");
            }
            return Ok(order);
        }
        [HttpPut("updateOrder")]
        public async Task<IActionResult> UpdateOrder(OrderUpdateRequest orderRequest)
        {
            if (orderRequest == null)
            {
                return BadRequest("Invalid Order Data");
            }
            var order = await _orderService.UpdateAsync(orderRequest);
            if (order == null)
            {
                return BadRequest("Error in adding product");
            }
            return Ok(order);
        }
        [HttpDelete("deleteOrder/{orderID}")]
        public async Task<IActionResult> DeleteOrder(Guid orderID)
        {
            if(orderID == Guid.Empty)
            {
                return BadRequest("Invalid Order ID");
            }
            var result = await _orderService.DeleteAsync(orderID);
            if (!result)
            {
                return BadRequest("Error in deleting product");
            }
            return Ok(result);
        }
    }
}
