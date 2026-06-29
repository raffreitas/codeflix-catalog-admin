using MediatR;

namespace Codeflix.Catalog.Application.UseCases.CastMembers.DeleteCastMember;
public interface IDeleteCastMember
    : IRequestHandler<DeleteCastMemberInput>
{ }
