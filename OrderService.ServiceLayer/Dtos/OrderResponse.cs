using OrderService.RepositoryLayer.Entities;

namespace OrderService.BusinessLayer.Dtos
{
    public class OrderResponse
    {
        public Guid OrderID { get; set; }
        public Guid UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalBill { get; set; }
        public List<OrderItemsResponse> OrderItems { get; set; } = new List<OrderItemsResponse>();
    }
}
