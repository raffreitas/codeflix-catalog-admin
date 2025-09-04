using FC.Codeflix.Catalog.Application.UseCases.CastMembers.Common;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMembers.GetCastMember;
public interface IGetCastMember
    : IRequestHandler<GetCastMemberInput, CastMemberModelOutput>
{}
