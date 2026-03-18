using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.CastMembers.Common;
using FC.Codeflix.Catalog.Domain.Repositories;

using DomainEntity = FC.Codeflix.Catalog.Domain.Entities;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMembers.CreateCastMember;

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