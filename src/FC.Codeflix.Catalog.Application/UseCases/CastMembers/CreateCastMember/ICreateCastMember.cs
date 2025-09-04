using FC.Codeflix.Catalog.Application.UseCases.CastMembers.Common;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMembers.CreateCastMember;

public interface ICreateCastMember
    : IRequestHandler<CreateCastMemberInput, CastMemberModelOutput>
{
}
