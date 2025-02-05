namespace OrderService.BusinessLayer.Dtos
{
    public class OrderItemsResponse
    {
        public Guid ProductID { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
