using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMembers.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

using DomainEntity = FC.Codeflix.Catalog.Domain.Entities;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMembers.ListCastMembers;

public record ListCastMembersOutput(int Page, int PerPage, int Total, IReadOnlyList<CastMemberModelOutput> Items)
    : PaginatedListOutput<CastMemberModelOutput>(Page, PerPage, Total, Items)
{
    public static ListCastMembersOutput FromSearchOutput(
        SearchOutput<DomainEntity.CastMember> searchOutput
    ) => new(
        searchOutput.CurrentPage,
        searchOutput.PerPage,
        searchOutput.Total,
        searchOutput.Items
            .Select(castmember
                => CastMemberModelOutput.FromCastMember(castmember))
            .ToList()
            .AsReadOnly()
    );
}