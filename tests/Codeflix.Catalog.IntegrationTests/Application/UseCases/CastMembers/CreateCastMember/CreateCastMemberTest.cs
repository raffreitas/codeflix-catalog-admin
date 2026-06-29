using Codeflix.Catalog.Application;
using Codeflix.Catalog.Infra.Data.EF;
using Codeflix.Catalog.Infra.Data.EF.Repositories;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using UseCase = Codeflix.Catalog.Application.UseCases.CastMembers.CreateCastMember;

namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMembers.CreateCastMember;

[Collection(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTest(CreateCastMemberTestFixture fixture)
{
    [Fact(DisplayName = nameof(CreateCastMember))]
    [Trait("Integration/Application", "CreateCastMember - Use Cases")]
    public async Task CreatCastMember()
    {
        var actDbContext = fixture.CreateDbContext();
        var repository = new CastMemberRepository(actDbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            actDbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );
        var useCase = new UseCase.CreateCastMember(repository, unitOfWork);
        var input = new UseCase.CreateCastMemberInput(
            fixture.GetValidName(),
            fixture.GetRandomCastMemberType()
        );

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Type.Should().Be(input.Type);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        var assertDbContext = fixture.CreateDbContext(true);
        var castMembers = await assertDbContext.CastMembers.AsNoTracking().ToListAsync();
        castMembers.Should().HaveCount(1);
        var castMemberFromDb = castMembers[0];
        castMemberFromDb.Name.Should().Be(input.Name);
        castMemberFromDb.Type.Should().Be(input.Type);
        castMemberFromDb.Id.Should().Be(output.Id);
    }
}