namespace OrderService.BusinessLayer.Dtos
{
    public class ProductDto
    {
        public Guid ProductID { get; set; }
        public string ProductName { get; set; }
        public double UnitPrice { get; set; }
        public int QuantityInStock { get; set; }
        public string Category { get; set; }
    }
}
