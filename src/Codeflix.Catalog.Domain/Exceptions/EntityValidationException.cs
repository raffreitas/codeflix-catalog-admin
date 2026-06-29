using Codeflix.Catalog.Domain.Validations;

namespace Codeflix.Catalog.Domain.Exceptions;

public class EntityValidationException(
    string? message,
    IReadOnlyCollection<ValidationError>? errors = null
) : Exception(message)
{
    public IReadOnlyCollection<ValidationError>? Errors { get; } = errors;
}