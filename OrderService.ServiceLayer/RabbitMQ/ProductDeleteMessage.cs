namespace OrderService.BusinessLayer.RabbitMQ
{
    public record ProductDeleteMessage(Guid ProductID, string DeletedName);
}
