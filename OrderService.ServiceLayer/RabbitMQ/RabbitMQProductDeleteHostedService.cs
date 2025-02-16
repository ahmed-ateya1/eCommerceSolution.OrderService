using Microsoft.Extensions.Hosting;

namespace OrderService.BusinessLayer.RabbitMQ
{
    public class RabbitMQProductDeleteHostedService : IHostedService
    {
        private readonly IRabbitMQProductDeleteConsumer _consumer;

        public RabbitMQProductDeleteHostedService(IRabbitMQProductDeleteConsumer consumer)
        {
            _consumer = consumer;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _consumer.ConsumeAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer.Dispose();
            return Task.CompletedTask;
        }
    }
}
