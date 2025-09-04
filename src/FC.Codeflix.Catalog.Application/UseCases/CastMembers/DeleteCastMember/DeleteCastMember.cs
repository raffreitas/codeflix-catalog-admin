using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repositories;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMembers.DeleteCastMember;

public class DeleteCastMember : IDeleteCastMember
{
    private readonly ICastMemberRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCastMember(
        ICastMemberRepository repository,
        IUnitOfWork unitOfWork
    )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        DeleteCastMemberInput request,
        CancellationToken cancellationToken
    )
    {
        var castMember = await _repository.Get(request.Id, cancellationToken);
        await _repository.Delete(castMember, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
    }
}