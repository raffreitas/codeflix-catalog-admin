namespace FC.Codeflix.Catalog.Domain.Validations;

public sealed class NotificationValidationHandler : ValidationHandler
{
    private readonly List<ValidationError> _errors = [];

    public IReadOnlyCollection<ValidationError> Errors => _errors.AsReadOnly();

    public bool HasErrors() => _errors.Count != 0;

    public override void HandleError(ValidationError validationError)
        => _errors.Add(validationError);
}