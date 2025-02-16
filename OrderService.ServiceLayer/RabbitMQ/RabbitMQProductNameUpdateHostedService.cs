using Microsoft.Extensions.Hosting;

namespace OrderService.BusinessLayer.RabbitMQ
{
    public class RabbitMQProductNameUpdateHostedService : IHostedService
    {
        private readonly IRabbitMQProductNameUpdateConcumer _consumer;

        public RabbitMQProductNameUpdateHostedService(IRabbitMQProductNameUpdateConcumer consumer)
        {
            _consumer = consumer;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _consumer.Consume();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer.Dispose();
            return Task.CompletedTask;
        }
    }
}
