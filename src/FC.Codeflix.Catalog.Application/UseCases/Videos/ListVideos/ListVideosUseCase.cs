using FC.Codeflix.Catalog.Application.UseCases.Videos.Common;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Repositories;

namespace FC.Codeflix.Catalog.Application.UseCases.Videos.ListVideos;

public sealed class ListVideosUseCase(
    IVideoRepository videoRepository,
    ICategoryRepository categoryRepository,
    IGenreRepository genreRepository
) : IListVideosUseCase
{
    public async Task<ListVideosOutput> Handle(ListVideosInput request, CancellationToken cancellationToken)
    {
        var result = await videoRepository.Search(request.ToSearchInput(), cancellationToken);

        IReadOnlyList<Category>? categories = null;
        var relatedCategoriesIds = result.Items.SelectMany(video => video.Categories).Distinct().ToList();
        if (relatedCategoriesIds.Count > 0)
            categories = await categoryRepository.GetListByIds(relatedCategoriesIds, cancellationToken);

        IReadOnlyList<Genre>? genres = null;
        var relatedGenresIds = result.Items.SelectMany(item => item.Genres).Distinct().ToList();
        if (relatedGenresIds.Count > 0)
            genres = await genreRepository.GetListByIds(relatedGenresIds, cancellationToken);

        var output = new ListVideosOutput(
            result.CurrentPage,
            result.PerPage,
            result.Total,
            result.Items.Select(item => VideoModelOutput.FromVideo(item, categories, genres)).ToList());

        return output;
    }
}