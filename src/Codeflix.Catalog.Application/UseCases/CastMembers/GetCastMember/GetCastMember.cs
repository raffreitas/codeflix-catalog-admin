using Codeflix.Catalog.Application.UseCases.CastMembers.Common;

using Codeflix.Catalog.Domain.Repositories;

namespace Codeflix.Catalog.Application.UseCases.CastMembers.GetCastMember;

public class GetCastMember(ICastMemberRepository repository) : IGetCastMember
{
    public async Task<CastMemberModelOutput> Handle(
        GetCastMemberInput request,
        CancellationToken cancellationToken
    )
    {
        var castMember = await repository.Get(request.Id, cancellationToken);
        return CastMemberModelOutput.FromCastMember(castMember);
    }
}