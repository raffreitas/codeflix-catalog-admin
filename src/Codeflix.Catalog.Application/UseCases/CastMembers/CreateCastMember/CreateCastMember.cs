using Codeflix.Catalog.Application.Interfaces;
using Codeflix.Catalog.Application.UseCases.CastMembers.Common;

using Codeflix.Catalog.Domain.Repositories;

using DomainEntity = Codeflix.Catalog.Domain.Entities;

namespace Codeflix.Catalog.Application.UseCases.CastMembers.CreateCastMember;

public class CreateCastMember(ICastMemberRepository repository, IUnitOfWork unitOfWork) : ICreateCastMember
{
    public async Task<CastMemberModelOutput> Handle(CreateCastMemberInput request, CancellationToken cancellationToken)
    {
        var castMember = new DomainEntity.CastMember(request.Name, request.Type);
        await repository.Insert(castMember, cancellationToken);
        await unitOfWork.Commit(cancellationToken);
        return CastMemberModelOutput.FromCastMember(castMember);
    }
}