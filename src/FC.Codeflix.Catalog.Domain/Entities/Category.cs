using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.Validations;

namespace FC.Codeflix.Catalog.Domain.Entities;

public class Category : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Category(string name, string description, bool isActive = true) : base()
    {
        Name = name;
        Description = description;
        IsActive = isActive;
        CreatedAt = DateTime.Now;

        Validate();
    }

    public void Activate()
    {
        IsActive = true;
        Validate();
    }

    public void Deactivate()
    {
        IsActive = false;
        Validate();
    }

    public void Update(string name, string? description = null)
    {
        Name = name;
        Description = description ?? Description;
        Validate();
    }

    private void Validate()
    {
        DomainValidation.NotNullOrEmpty(Name, fieldName: nameof(Name));
        DomainValidation.MinLength(Name, minLength: 3, fieldName: nameof(Name));
        DomainValidation.MaxLength(Name, maxLength: 255, fieldName: nameof(Name));
        DomainValidation.NotNull(Description, fieldName: nameof(Description));
        DomainValidation.MaxLength(Description, maxLength: 10_000, fieldName: nameof(Description));
    }
}