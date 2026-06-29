using Codeflix.Catalog.Application.UseCases.CastMembers.Common;

using MediatR;

namespace Codeflix.Catalog.Application.UseCases.CastMembers.UpdateCastMember;
public interface IUpdateCastMember 
    : IRequestHandler<UpdateCastMemberInput, CastMemberModelOutput>
{
}
