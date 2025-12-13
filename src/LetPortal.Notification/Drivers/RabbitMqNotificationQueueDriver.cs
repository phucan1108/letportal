using System.Text;
using LetPortal.Core.Utils;
using LetPortal.Notification.Entities;
using LetPortal.Notification.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LetPortal.Notification.Drivers;

internal sealed class RabbitMqNotificationQueueDriver(IOptions<NotificationOptions> options) : INotificationQueueDriver, IAsyncDisposable
{
    public string Driver => "RabbitMq";

    private readonly NotificationOptions _options = options.Value;
    private readonly SemaphoreSlim _startLock = new(1, 1);

    private IConnection? _pubConnection;
    private IChannel? _pubChannel;
    private IConnection? _consumerConnection;
    private IChannel? _consumerChannel;

    private volatile bool _isStarted;

    public async Task StartAsync()
    {
        if (_isStarted)
            return;

        await _startLock.WaitAsync();
        try
        {
            if (_isStarted)
                return;

            var factory = new ConnectionFactory
            {
                Uri = new Uri(_options.ConnectionString)
            };

            (_pubConnection, _consumerConnection) = (await factory.CreateConnectionAsync(), await factory.CreateConnectionAsync());
            _pubChannel = await _pubConnection.CreateChannelAsync();
            _consumerChannel = await _consumerConnection.CreateChannelAsync();

            // Declare queues for both channels
            await Task.WhenAll(
                DeclareQueueAsync(_pubChannel),
                DeclareQueueAsync(_consumerChannel)
            );

            _isStarted = true;
        }
        finally
        {
            _startLock.Release();
        }
    }

    private Task DeclareQueueAsync(IChannel channel) =>
        channel.QueueDeclareAsync(
            queue: _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

    public async Task PushAsync(IncomingNotificationMessage message)
    {
        ArgumentNullException.ThrowIfNull(_pubChannel);

        var body = Encoding.UTF8.GetBytes(ConvertUtil.SerializeObject(message));
        await _pubChannel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: _options.QueueName,
            mandatory: false,
            basicProperties: new BasicProperties(),
            body: body);
    }

    public async Task SubcribeAsync(Func<IncomingNotificationMessage, Task> proceed, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(_consumerChannel);
        ArgumentNullException.ThrowIfNull(proceed);

        var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var message = Encoding.UTF8.GetString(ea.Body.Span);
            var notificationMessage = ConvertUtil.DeserializeObject<IncomingNotificationMessage>(message);
            
            await proceed(notificationMessage);
            await Task.Delay(_options.DelayPullMessageInMs, cancellationToken);
            await _consumerChannel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken);
            
            cancellationToken.ThrowIfCancellationRequested();
        };

        await _consumerChannel.BasicConsumeAsync(
            queue: _options.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_consumerChannel is not null)
        {
            await _consumerChannel.CloseAsync();
            await _consumerChannel.DisposeAsync();
        }

        if (_pubChannel is not null)
        {
            await _pubChannel.CloseAsync();
            await _pubChannel.DisposeAsync();
        }

        if (_consumerConnection is not null)
        {
            await _consumerConnection.CloseAsync();
            await _consumerConnection.DisposeAsync();
        }

        if (_pubConnection is not null)
        {
            await _pubConnection.CloseAsync();
            await _pubConnection.DisposeAsync();
        }

        _startLock.Dispose();

        GC.SuppressFinalize(this);
    }
}
