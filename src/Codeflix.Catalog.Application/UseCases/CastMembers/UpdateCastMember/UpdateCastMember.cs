using Codeflix.Catalog.Application.Interfaces;
using Codeflix.Catalog.Application.UseCases.CastMembers.Common;

using Codeflix.Catalog.Domain.Repositories;

namespace Codeflix.Catalog.Application.UseCases.CastMembers.UpdateCastMember;

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