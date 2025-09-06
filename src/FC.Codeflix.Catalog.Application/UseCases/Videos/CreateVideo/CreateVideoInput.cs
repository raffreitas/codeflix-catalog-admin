using FC.Codeflix.Catalog.Application.UseCases.Videos.Common;
using FC.Codeflix.Catalog.Domain.Enum;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Videos.CreateVideo;

public sealed record CreateVideoInput(
    string Title,
    string Description,
    int YearLaunched,
    int Duration,
    bool Opened,
    bool Published,
    Rating Rating,
    IReadOnlyCollection<Guid>? CategoriesIds = null,
    IReadOnlyCollection<Guid>? GenresIds = null,
    IReadOnlyCollection<Guid>? CastMembersIds = null,
    FileInput? Thumb = null,
    FileInput? Banner = null,
    FileInput? ThumbHalf = null,
    FileInput? Media = null,
    FileInput? Trailer = null
) : IRequest<CreateVideoOutput>;