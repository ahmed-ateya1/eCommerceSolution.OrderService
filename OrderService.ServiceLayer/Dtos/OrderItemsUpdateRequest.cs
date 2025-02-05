namespace OrderService.BusinessLayer.Dtos
{
    public class OrderItemsUpdateRequest
    {
        public Guid ProductID { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
