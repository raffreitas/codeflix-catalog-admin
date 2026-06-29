using Codeflix.Catalog.Application.Interfaces;

using Codeflix.Catalog.Domain.Events;
using Codeflix.Catalog.Domain.SeedWork;

namespace Codeflix.Catalog.Application.EventHandlers;

public sealed class SentToEncoderEventHandler(IMessageProducer messageProducer)
    : IDomainEventHandler<VideoUploadedEvent>
{
    public async Task HandleAsync(VideoUploadedEvent @event, CancellationToken cancellationToken = default)
        => await messageProducer.SendMessageAsync(@event, cancellationToken);
}