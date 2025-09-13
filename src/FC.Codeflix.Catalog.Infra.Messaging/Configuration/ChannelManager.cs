using RabbitMQ.Client;

namespace FC.Codeflix.Catalog.Infra.Messaging.Configuration;

public sealed class ChannelManager(IConnection connection)
{
    private readonly Lock _lock = new();
    private IModel? _channel;

    public IModel? GetChannel()
    {
        lock (_lock)
        {
            if (_channel == null || _channel.IsClosed)
            {
                _channel = connection.CreateModel();
                _channel.ConfirmSelect();
            }

            return _channel;
        }
    }
}