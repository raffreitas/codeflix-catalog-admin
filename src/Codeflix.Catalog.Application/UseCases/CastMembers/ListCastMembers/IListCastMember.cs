using MediatR;

namespace Codeflix.Catalog.Application.UseCases.CastMembers.ListCastMembers;
public interface IListCastMembers
    : IRequestHandler<ListCastMembersInput, ListCastMembersOutput>
{
}
