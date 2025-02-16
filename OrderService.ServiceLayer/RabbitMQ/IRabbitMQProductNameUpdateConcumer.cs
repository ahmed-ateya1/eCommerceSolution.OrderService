namespace OrderService.BusinessLayer.RabbitMQ
{
    public interface IRabbitMQProductNameUpdateConcumer
    {
        Task Consume();
        void Dispose();
    }
}
