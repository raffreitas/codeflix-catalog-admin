using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Videos.DeleteVideo;

public interface IDeleteVideoUseCase : IRequestHandler<DeleteVideoInput>{}