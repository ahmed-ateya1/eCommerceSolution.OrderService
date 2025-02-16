namespace OrderService.BusinessLayer.RabbitMQ
{
    public record ProductNameUpdateMessage(Guid ProductID, string NewName);
}
