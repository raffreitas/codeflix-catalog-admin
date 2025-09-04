using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMembers.ListCastMembers;
public interface IListCastMembers
    : IRequestHandler<ListCastMembersInput, ListCastMembersOutput>
{
}
