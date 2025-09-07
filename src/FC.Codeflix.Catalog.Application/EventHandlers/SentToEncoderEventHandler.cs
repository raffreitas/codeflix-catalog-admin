using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Events;
using FC.Codeflix.Catalog.Domain.SeedWork;

namespace FC.Codeflix.Catalog.Application.EventHandlers;

public sealed class SentToEncoderEventHandler(IMessageProducer messageProducer)
    : IDomainEventHandler<VideoUploadedEvent>
{
    public async Task HandleAsync(VideoUploadedEvent @event, CancellationToken cancellationToken = default)
        => await messageProducer.SendMessageAsync(@event, cancellationToken);
}