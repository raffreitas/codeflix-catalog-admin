using RabbitMQ.Client;

namespace FC.Codeflix.Catalog.Infra.Messaging.Configuration;

public sealed class ChannelManager(IConnection connection) : IAsyncDisposable
{
    private readonly SemaphoreSlim _channelLock = new(1, 1);
    private readonly SemaphoreSlim _publishLock = new(1, 1);
    private IChannel? _channel;

    public async Task<IChannel> GetChannelAsync(CancellationToken cancellationToken = default)
    {
        await _channelLock.WaitAsync(cancellationToken);
        try
        {
            if (_channel is not null && _channel.IsOpen)
                return _channel;

            if (_channel is not null)
                await _channel.DisposeAsync();

            var channelOptions = new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true
            );

            _channel = await connection.CreateChannelAsync(channelOptions, cancellationToken);

            return _channel;
        }
        finally
        {
            _channelLock.Release();
        }
    }

    public async Task<IDisposable> AcquirePublishLockAsync(CancellationToken cancellationToken = default)
    {
        await _publishLock.WaitAsync(cancellationToken);
        return new Releaser(_publishLock);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            if (_channel.IsOpen)
                await _channel.CloseAsync();

            await _channel.DisposeAsync();
        }

        _channelLock.Dispose();
        _publishLock.Dispose();
    }

    private sealed class Releaser(SemaphoreSlim semaphore) : IDisposable
    {
        public void Dispose() => semaphore.Release();
    }
}