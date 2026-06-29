using Codeflix.Catalog.Application;
using Codeflix.Catalog.Application.Exceptions;
using Codeflix.Catalog.Infra.Data.EF;
using Codeflix.Catalog.Infra.Data.EF.Repositories;
using Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMembers.Common;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using UseCase = Codeflix.Catalog.Application.UseCases.CastMembers.UpdateCastMember;

namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMembers.UpdateCastMember;

[Collection(nameof(CastMemberUseCasesBaseFixture))]
public class UpdateCastMemberTest(CastMemberUseCasesBaseFixture fixture)
{
    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Application", "UpdateCastMember - Use Cases")]
    public async Task Update()
    {
        var examples = fixture.GetExampleCastMembersList(10);
        var example = examples[5];
        var arrangeDbContext = fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(examples);
        await arrangeDbContext.SaveChangesAsync();
        var newName = fixture.GetValidName();
        var newType = fixture.GetRandomCastMemberType();
        var actDbContext = fixture.CreateDbContext(true);
        var repository = new CastMemberRepository(actDbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var uow = new UnitOfWork(
            actDbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );
        var useCase = new UseCase.UpdateCastMember(repository, uow);
        var input = new UseCase.UpdateCastMemberInput(example.Id, newName, newType);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(example.Id);
        output.Name.Should().Be(newName);
        output.Type.Should().Be(newType);
        var item = await fixture.CreateDbContext(true)
            .CastMembers
            .FindAsync(example.Id);
        item.Should().NotBeNull();
        item.Name.Should().Be(newName);
        item.Type.Should().Be(newType);
    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Integration/Application", "UpdateCastMember - Use Cases")]
    public async Task ThrowWhenNotFound()
    {
        var randomGuid = Guid.NewGuid();
        var newName = fixture.GetValidName();
        var newType = fixture.GetRandomCastMemberType();
        var actDbContext = fixture.CreateDbContext(true);
        var repository = new CastMemberRepository(actDbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var uow = new UnitOfWork(
            actDbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );
        var useCase = new UseCase.UpdateCastMember(repository, uow);
        var input = new UseCase.UpdateCastMemberInput(randomGuid, newName, newType);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"CastMember '{randomGuid}' not found.");
    }
}