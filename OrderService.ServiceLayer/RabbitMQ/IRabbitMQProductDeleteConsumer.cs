namespace OrderService.BusinessLayer.RabbitMQ
{
    public interface IRabbitMQProductDeleteConsumer
    {
        Task ConsumeAsync();
        void Dispose();
    }
}
