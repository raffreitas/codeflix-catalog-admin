using FC.Codeflix.Catalog.Application.UseCases.CastMembers.Common;
using FC.Codeflix.Catalog.Domain.Repositories;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMembers.GetCastMember;

public class GetCastMember : IGetCastMember
{
    private readonly ICastMemberRepository _repository;

    public GetCastMember(ICastMemberRepository repository)
        => _repository = repository;

    public async Task<CastMemberModelOutput> Handle(
        GetCastMemberInput request,
        CancellationToken cancellationToken
    )
    {
        var castMember = await _repository.Get(request.Id, cancellationToken);
        return CastMemberModelOutput.FromCastMember(castMember);
    }
}