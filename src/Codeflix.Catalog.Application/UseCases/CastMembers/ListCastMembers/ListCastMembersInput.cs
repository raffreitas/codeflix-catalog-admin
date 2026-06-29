using Codeflix.Catalog.Application.Common;

using Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

using MediatR;

namespace Codeflix.Catalog.Application.UseCases.CastMembers.ListCastMembers;

public record ListCastMembersInput(int Page, int PerPage, string Search, string Sort, SearchOrder Dir)
    : PaginatedListInput(Page, PerPage, Search, Sort, Dir), IRequest<ListCastMembersOutput>
{
    public ListCastMembersInput()
        : this(1, 15, "", "", SearchOrder.Asc)
    {
    }
}