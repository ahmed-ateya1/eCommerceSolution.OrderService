﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace OrderService.BusinessLayer.RabbitMQ
{
    public class RabbitMQProductNameUpdateConcumer : IRabbitMQProductNameUpdateConcumer , IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RabbitMQProductNameUpdateConcumer> _logger;

        public RabbitMQProductNameUpdateConcumer(IConfiguration configuration, ILogger<RabbitMQProductNameUpdateConcumer> logger)
        {
            _configuration = configuration;
            InitializeAsync().GetAwaiter().GetResult();
            _logger = logger;
        }

        private IConnection _connection;
        private IChannel _channel;
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
        public async Task Consume()
        {
            if (_channel == null)
                throw new InvalidOperationException("RabbitMQ connection is not initialized.");

            string routingKey = "product.update.name";
            string queueName = "Order.product.update.name.queue";
            string exchangeName = _configuration["RabbitMQ_Products_Exchange"]!;

            //create exchange
            await _channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Direct,
                durable: true
                );

            //create message queue
            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                arguments: null
                );

            //Binding exchange with queue

            await _channel.QueueBindAsync(
                queue: queueName,
                exchange: exchangeName,
                routingKey: routingKey
                );

            //Consume Message from queue

            var consumer = new AsyncEventingBasicConsumer(_channel);

            //handle the receivee message
            consumer.ReceivedAsync += async (sender, args) =>
            {
                var msgBodyBytes = args.Body.ToArray();
                var msgJson = Encoding.UTF8.GetString(msgBodyBytes);

                if(msgJson != null)
                {
                    var message = JsonSerializer.Deserialize<ProductNameUpdateMessage>(msgJson);
                    _logger.LogInformation($"Product Name is updated {message.ProductID} and new Name is : {message.NewName}");

                }
            };

            await _channel.BasicConsumeAsync(
                queue: queueName,
                consumer: consumer,
                autoAck: true
                );
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
