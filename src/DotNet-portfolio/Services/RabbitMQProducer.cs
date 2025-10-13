using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace DotNet_portfolio.Services
{
    /// <summary>
    /// A service to publish messages to a RabbitMQ queue.
    /// </summary>
    public class RabbitMQProducer : IMessageProducer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQProducer> _logger;
        private const string QueueName = "notifications";

        public RabbitMQProducer(IConfiguration configuration, ILogger<RabbitMQProducer> logger)
        {
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
                _channel.QueueDeclare(
                    queue: QueueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
                _logger.LogInformation("Successfully connected to RabbitMQ.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to RabbitMQ.");
                throw;
            }
        }

        public void SendMessage<T>(T message)
        {
            var jsonString = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonString);

            _channel.BasicPublish(
                exchange: string.Empty,
                routingKey: QueueName,
                basicProperties: null,
                body: body
            );

            _logger.LogInformation("Sent message to RabbitMQ: {Message}", jsonString);
        }
    }
}
