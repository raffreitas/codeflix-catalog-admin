using Codeflix.Catalog.Application.Interfaces;

using Codeflix.Catalog.Domain.Repositories;

namespace Codeflix.Catalog.Application.UseCases.CastMembers.DeleteCastMember;

public class DeleteCastMember(
    ICastMemberRepository repository,
    IUnitOfWork unitOfWork)
    : IDeleteCastMember
{
    public async Task Handle(
        DeleteCastMemberInput request,
        CancellationToken cancellationToken
    )
    {
        var castMember = await repository.Get(request.Id, cancellationToken);
        await repository.Delete(castMember, cancellationToken);
        await unitOfWork.Commit(cancellationToken);
    }
}