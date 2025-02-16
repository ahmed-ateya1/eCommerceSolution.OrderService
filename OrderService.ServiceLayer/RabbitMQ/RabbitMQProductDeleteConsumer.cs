
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace OrderService.BusinessLayer.RabbitMQ
{
    public class RabbitMQProductDeleteConsumer : IRabbitMQProductDeleteConsumer , IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RabbitMQProductDeleteConsumer> _logger;

        public RabbitMQProductDeleteConsumer(IConfiguration configuration, ILogger<RabbitMQProductDeleteConsumer> logger)
        {
            _configuration = configuration;
            _logger = logger;
            InitializeAsync().GetAwaiter().GetResult();
        }
        private IChannel _channel;
        private IConnection _connection;

        private async Task InitializeAsync()
        {
            string hostname = _configuration["RabbitMQ_HostName"]!;
            string username = _configuration["RabbitMQ_UserName"]!;
            string password = _configuration["RabbitMQ_PASSWORD"]!;
            string port = _configuration["RabbitMQ_Port"]!;

            var connectionFactory = new ConnectionFactory
            {
                HostName = hostname,
                UserName = username,
                Password = password,
                Port = Convert.ToInt32(port)
            };

            _connection = await connectionFactory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
        }
        public async Task ConsumeAsync()
        {
            if (_channel == null)
            {
                throw new InvalidOperationException("channel can't be null");
            }
            string exchangeName = _configuration["RabbitMQ_Products_Exchange"]!;
            string queueName = "orders.product.delete.queue";
            string routingKey = "product.delete";

            //create Exchange
            await _channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Direct,
                durable: true
                );

            //create queue
            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false
                );

            //Binding queue with exchange
            await _channel.QueueBindAsync(
                queue: queueName,
                exchange: exchangeName,
                routingKey: routingKey
                );

            //create consumer
            var consumer = new AsyncEventingBasicConsumer(_channel);

            //Subscribe in Event to consume queue
            consumer.ReceivedAsync += async (sender, args) =>
            {
                var msgBytes = args.Body.ToArray();
                var msgBodyJson = Encoding.UTF8.GetString(msgBytes);
                
                var message = JsonSerializer.Deserialize<ProductDeleteMessage>(msgBodyJson);

                if(message != null)
                {
                    _logger.LogInformation($"product with id = {message.ProductID} and name = {message.DeletedName} is Deleted");
                }
            };

            await _channel.BasicConsumeAsync(
                    queue: queueName,
                    consumer: consumer,
                    autoAck: false
                  );
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
