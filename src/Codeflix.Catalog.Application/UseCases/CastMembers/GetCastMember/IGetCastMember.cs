using Codeflix.Catalog.Application.UseCases.CastMembers.Common;

using MediatR;

namespace Codeflix.Catalog.Application.UseCases.CastMembers.GetCastMember;
public interface IGetCastMember
    : IRequestHandler<GetCastMemberInput, CastMemberModelOutput>
{}
