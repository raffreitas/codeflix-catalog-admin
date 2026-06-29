using Codeflix.Catalog.Application.UseCases.Videos.Common;

using Codeflix.Catalog.Domain.Enum;

using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Videos.UpdateMediaStatus;

public sealed record UpdateMediaStatusInput(
    Guid VideoId,
    MediaStatus Status,
    string? EncodedPath = null,
    string? ErrorMessage = null
) : IRequest<VideoModelOutput>;