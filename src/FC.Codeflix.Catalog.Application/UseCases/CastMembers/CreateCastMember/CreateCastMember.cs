using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.CastMembers.Common;
using FC.Codeflix.Catalog.Domain.Repositories;

using DomainEntity = FC.Codeflix.Catalog.Domain.Entities;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMembers.CreateCastMember;

public class CreateCastMember : ICreateCastMember
{
    private readonly ICastMemberRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCastMember(ICastMemberRepository repository, IUnitOfWork unitOfWork)
        => (_repository, _unitOfWork) = (repository, unitOfWork);

    public async Task<CastMemberModelOutput> Handle(CreateCastMemberInput request, CancellationToken cancellationToken)
    {
        var castMember = new DomainEntity.CastMember(request.Name, request.Type);
        await _repository.Insert(castMember, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        return CastMemberModelOutput.FromCastMember(castMember);
    }
}