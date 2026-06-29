using Codeflix.Catalog.UnitTests.Common;

using Codeflix.Catalog.Domain.Enum;

using DomainEntity = Codeflix.Catalog.Domain.Entities;


namespace Codeflix.Catalog.UnitTests.Application.CastMembers.Common;

public class CastMemberUseCasesBaseFixture
    : BaseFixture
{
    public DomainEntity.CastMember GetExampleCastMember()
        => new(GetValidName(), GetRandomCastMemberType());

    public string GetValidName()
        => Faker.Name.FullName();

    public CastMemberType GetRandomCastMemberType()
        => (CastMemberType)(new Random()).Next(1, 2);
}