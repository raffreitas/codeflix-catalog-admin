using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMembers.ListCastMembers;

public record ListCastMembersInput
    : PaginatedListInput, IRequest<ListCastMembersOutput>
{
    public ListCastMembersInput(int page, int perPage, string search, string sort, SearchOrder dir)
        : base(page, perPage, search, sort, dir)
    {
    }

    public ListCastMembersInput()
        : base(1, 15, "", "", SearchOrder.Asc)
    {
    }
}