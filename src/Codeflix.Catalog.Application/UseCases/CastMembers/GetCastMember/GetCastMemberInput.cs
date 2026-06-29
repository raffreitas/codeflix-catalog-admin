using Codeflix.Catalog.Application.UseCases.CastMembers.Common;

using MediatR;

namespace Codeflix.Catalog.Application.UseCases.CastMembers.GetCastMember;
public class GetCastMemberInput(Guid id) : IRequest<CastMemberModelOutput>
{
    public Guid Id { get; private set; } = id;
}
