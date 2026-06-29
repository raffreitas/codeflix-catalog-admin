using Codeflix.Catalog.Domain.SeedWork;

namespace Codeflix.Catalog.Domain.Events;

public sealed record VideoUploadedEvent(Guid ResourceId, string FilePath) : DomainEvent;