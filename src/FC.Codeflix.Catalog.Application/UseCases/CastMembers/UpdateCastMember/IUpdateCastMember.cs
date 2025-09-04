using FC.Codeflix.Catalog.Application.UseCases.CastMembers.Common;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMembers.UpdateCastMember;
public interface IUpdateCastMember 
    : IRequestHandler<UpdateCastMemberInput, CastMemberModelOutput>
{
}
