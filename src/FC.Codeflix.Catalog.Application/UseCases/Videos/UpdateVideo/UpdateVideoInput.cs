using FC.Codeflix.Catalog.Application.UseCases.Videos.Common;
using FC.Codeflix.Catalog.Domain.Enum;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Videos.UpdateVideo;

public record UpdateVideoInput(
    Guid VideoId,
    string Title,
    string Description,
    int YearLaunched,
    bool Opened,
    bool Published,
    int Duration,
    Rating Rating,
    List<Guid>? GenresIds = null,
    List<Guid>? CategoriesIds = null,
    List<Guid>? CastMembersIds = null,
    FileInput? Banner = null,
    FileInput? Thumb = null,
    FileInput? ThumbHalf = null
) : IRequest<VideoModelOutput>;