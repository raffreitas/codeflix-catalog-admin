using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.CastMembers.Common;
using FC.Codeflix.Catalog.Domain.Repositories;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMembers.UpdateCastMember;

public class UpdateCastMember(ICastMemberRepository repository, IUnitOfWork unitOfWork) : IUpdateCastMember
{
    public async Task<CastMemberModelOutput> Handle(
        UpdateCastMemberInput input,
        CancellationToken cancellationToken
    )
    {
        var castmember = await repository.Get(input.Id, cancellationToken);
        castmember.Update(input.Name, input.Type);
        await repository.Update(castmember, cancellationToken);
        await unitOfWork.Commit(cancellationToken);
        return CastMemberModelOutput.FromCastMember(castmember);
    }
}