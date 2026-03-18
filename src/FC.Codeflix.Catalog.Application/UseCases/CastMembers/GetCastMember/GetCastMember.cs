using FC.Codeflix.Catalog.Application.UseCases.CastMembers.Common;
using FC.Codeflix.Catalog.Domain.Repositories;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMembers.GetCastMember;

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