using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.Validations;

namespace FC.Codeflix.Catalog.Domain.Entities;

public class CastMember : AggregateRoot
{
    public string Name { get; private set; }
    public CastMemberType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public CastMember(string name, CastMemberType type)
    {
        Name = name;
        Type = type;
        CreatedAt = DateTime.Now;
        Validate();
    }

    public void Update(string name, CastMemberType type)
    {
        Name = name;
        Type = type;
        Validate();
    }

    private void Validate()
        => DomainValidation.NotNullOrEmpty(Name, nameof(Name));

    public override string ToString()
    {
        return $"[Id] = {Id}, [Name] = {Name}, [CreatedAt] = {CreatedAt:HHmmfffffff}";
    }
}