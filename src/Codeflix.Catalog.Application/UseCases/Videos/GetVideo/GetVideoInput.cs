using Codeflix.Catalog.Application.UseCases.Videos.Common;

using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Videos.GetVideo;

public sealed record GetVideoInput(Guid VideoId) : IRequest<VideoModelOutput>;