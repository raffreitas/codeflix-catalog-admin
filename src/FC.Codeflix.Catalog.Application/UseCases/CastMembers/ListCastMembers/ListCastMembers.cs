using FC.Codeflix.Catalog.Domain.Repositories;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMembers.ListCastMembers;

public class ListCastMembers : IListCastMembers
{
    private readonly ICastMemberRepository _repository;

    public ListCastMembers(ICastMemberRepository repository)
        => _repository = repository;

    public async Task<ListCastMembersOutput> Handle(
        ListCastMembersInput request,
        CancellationToken cancellationToken
    )
    {
        var searchOutput = await _repository.Search(request.ToSearchInput(), cancellationToken);
        return ListCastMembersOutput.FromSearchOutput(searchOutput);
    }
}