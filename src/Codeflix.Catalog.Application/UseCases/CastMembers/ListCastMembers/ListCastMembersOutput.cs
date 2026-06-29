using Codeflix.Catalog.Application.Common;
using Codeflix.Catalog.Application.UseCases.CastMembers.Common;

using Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

using DomainEntity = Codeflix.Catalog.Domain.Entities;

namespace Codeflix.Catalog.Application.UseCases.CastMembers.ListCastMembers;

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