using System.Text;
using DotNet_portfolio.Hubs;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DotNet_portfolio.Services
{
    /// <summary>
    /// A background service that consumes messages from RabbitMQ and broadcasts them via SignalR.
    /// </summary>
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<RabbitMQConsumer> _logger;
        private const string QueueName = "notifications";

        public RabbitMQConsumer(
            IConfiguration configuration,
            IHubContext<NotificationHub> hubContext,
            ILogger<RabbitMQConsumer> logger
        )
        {
            _hubContext = hubContext;
            _logger = logger;
            try
            {
                var connectionString = configuration.GetConnectionString("RabbitMQ");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException(
                        "The RabbitMQ connection string 'ConnectionStrings:RabbitMQ' is missing or empty in the configuration."
                    );
                }

                var factory = new ConnectionFactory() { Uri = new Uri(connectionString) };
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _logger.LogInformation("RabbitMQ Consumer connected successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RabbitMQ Consumer failed to connect.");
                throw;
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var arguments = new Dictionary<string, object> { { "x-queue-type", "quorum" } };

            _channel.QueueDeclare(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: arguments
            );

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation("Received message from RabbitMQ: {Message}", message);

                await _hubContext.Clients.All.SendAsync(
                    "ReceiveMessage",
                    "RabbitMQ",
                    message,
                    stoppingToken
                );
            };

            _channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);
            _logger.LogInformation(
                "RabbitMQ Consumer started listening on queue '{QueueName}'.",
                QueueName
            );

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
