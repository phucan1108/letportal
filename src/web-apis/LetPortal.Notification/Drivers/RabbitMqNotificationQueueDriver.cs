using System.Text;
using LetPortal.Core.Utils;
using LetPortal.Notification.Entities;
using LetPortal.Notification.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LetPortal.Notification.Drivers
{
    class RabbitMqNotificationQueueDriver : INotificationQueueDriver, IDisposable
    {
        public string Driver => "RabbitMq";

        private readonly IOptions<NotificationOptions> _options;

        private IConnection _pubConnection;

        private IModel _pubChannel;

        private IConnection _consumerConnection;

        private IModel _consumerChannel;

        public RabbitMqNotificationQueueDriver(IOptions<NotificationOptions> options)
        {
            _options = options;
            ConnectionFactory factory = new ConnectionFactory
            {
                Uri = new Uri(_options.Value.ConnectionString)
            };
            _pubConnection = factory.CreateConnection();
            _consumerConnection = factory.CreateConnection();
            _pubChannel = _pubConnection.CreateModel();
            _pubChannel.QueueDeclare(queue: _options.Value.QueueName,
                                 durable: true, // Ensure the message won't be lost when RabbitMq is stopped
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            _consumerChannel = _consumerConnection.CreateModel();
            _consumerChannel.QueueDeclare(queue: _options.Value.QueueName,
                                 durable: true, // Ensure the message won't be lost when RabbitMq is stopped
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            InitRabbitConnection();
        }

        public Task Push(IncomingNotificationMessage message)
        {
            var rabbitMessage = ConvertUtil.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(rabbitMessage);
            _pubChannel.BasicPublish(exchange: "",
                                routingKey: _options.Value.QueueName,
                                basicProperties: null,
                                body: body);
            return Task.CompletedTask;
        }

        public Task Subcribe(Func<IncomingNotificationMessage, Task> proceed, CancellationToken cancellationToken)
        {
            var consumer = new EventingBasicConsumer(_consumerChannel);
            consumer.Received += (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var notificationMessage = ConvertUtil.DeserializeObject<IncomingNotificationMessage>(message);
                proceed.Invoke(notificationMessage);
                Thread.Sleep(_options.Value.DelayPullMessageInMs); // Sleep in 300ms
                _consumerChannel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                cancellationToken.ThrowIfCancellationRequested();
            };
            _consumerChannel.BasicConsume(queue: _options.Value.QueueName,
                                 autoAck: false,
                                 consumer: consumer);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _consumerConnection.Close();
            _pubConnection.Close();
        }

        private void InitRabbitConnection()
        {
            
        }
    }
}
