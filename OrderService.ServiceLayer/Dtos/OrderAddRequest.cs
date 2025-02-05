namespace OrderService.BusinessLayer.Dtos
{
    public class OrderAddRequest
    {
        public Guid UserID { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemsAddRequest> OrderItems { get; set; } = new List<OrderItemsAddRequest>();
    }
}
