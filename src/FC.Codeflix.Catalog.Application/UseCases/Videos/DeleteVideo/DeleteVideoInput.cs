using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Videos.DeleteVideo;

public sealed record DeleteVideoInput(Guid VideoId) : IRequest;