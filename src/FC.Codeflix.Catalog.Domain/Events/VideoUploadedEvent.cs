using FC.Codeflix.Catalog.Domain.SeedWork;

namespace FC.Codeflix.Catalog.Domain.Events;

public sealed record VideoUploadedEvent(Guid ResourceId, string FilePath) : DomainEvent;