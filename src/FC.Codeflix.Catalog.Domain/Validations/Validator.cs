namespace FC.Codeflix.Catalog.Domain.Validations;

public abstract class Validator(ValidationHandler handler)
{
    protected ValidationHandler _handler = handler;

    public abstract void Validate();
}