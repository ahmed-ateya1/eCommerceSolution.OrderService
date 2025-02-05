namespace OrderService.BusinessLayer.Dtos
{
    public class OrderUpdateRequest
    {
        public Guid OrderID { get; set; }
        public Guid UserID { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemsUpdateRequest> OrderItems { get; set; } = new List<OrderItemsUpdateRequest>();
    }
}
