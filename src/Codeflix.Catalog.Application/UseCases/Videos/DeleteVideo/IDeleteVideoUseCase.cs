using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Videos.DeleteVideo;

public interface IDeleteVideoUseCase : IRequestHandler<DeleteVideoInput>{}