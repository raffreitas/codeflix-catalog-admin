using Codeflix.Catalog.Application;
using Codeflix.Catalog.Application.Exceptions;
using Codeflix.Catalog.Infra.Data.EF;
using Codeflix.Catalog.Infra.Data.EF.Repositories;
using Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMembers.Common;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using UseCase = Codeflix.Catalog.Application.UseCases.CastMembers.DeleteCastMember;

namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMembers.DeleteCastMember;

[Collection(nameof(CastMemberUseCasesBaseFixture))]
public class DeleteCastMemberTest(CastMemberUseCasesBaseFixture fixture)
{
    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Application", "DeleteCastMember - Use Cases")]
    public async Task Delete()
    {
        var example = fixture.GetExampleCastMember();
        var arrangeDbContext = fixture.CreateDbContext();
        await arrangeDbContext.AddAsync(example);
        await arrangeDbContext.SaveChangesAsync();
        var actDbContext = fixture.CreateDbContext(true);
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
        var useCase = new UseCase.DeleteCastMember(repository, unitOfWork);
        var input = new UseCase.DeleteCastMemberInput(example.Id);

        await useCase.Handle(input, CancellationToken.None);

        var assertDbContext = fixture.CreateDbContext(true);
        var list = await assertDbContext.CastMembers.AsNoTracking().ToListAsync();
        list.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Integration/Application", "DeleteCastMember - Use Cases")]
    public async Task ThrowWhenNotFound()
    {
        var actDbContext = fixture.CreateDbContext(true);
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
        var useCase = new UseCase.DeleteCastMember(repository, unitOfWork);
        var randomGuid = Guid.NewGuid();
        var input = new UseCase.DeleteCastMemberInput(randomGuid);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"CastMember '{randomGuid}' not found.");
    }
}