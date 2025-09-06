using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Videos.CreateVideo;

public interface ICreateVideoUseCase : IRequestHandler<CreateVideoInput, CreateVideoOutput>
{
}