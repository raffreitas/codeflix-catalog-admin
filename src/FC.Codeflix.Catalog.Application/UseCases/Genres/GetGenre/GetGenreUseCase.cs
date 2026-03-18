using FC.Codeflix.Catalog.Application.UseCases.Genres.Common;
using FC.Codeflix.Catalog.Domain.Repositories;

namespace FC.Codeflix.Catalog.Application.UseCases.Genres.GetGenre;

public class GetGenreUseCase(IGenreRepository genreRepository, ICategoryRepository categoryRepository)
    : IGetGenreUseCase
{
    public async Task<GenreModelOutput> Handle(GetGenreInput request, CancellationToken cancellationToken)
    {
        var genre = await genreRepository.Get(request.Id, cancellationToken);
        var output = GenreModelOutput.FromGenre(genre);
        if (output.Categories.Count > 0)
        {
            var categories = (await categoryRepository
                    .GetListByIds(output.Categories
                        .Select(x => x.Id).ToList(), cancellationToken))
                .ToDictionary(x => x.Id);
            foreach (var category in output.Categories)
                category.Name = categories[category.Id].Name;
        }

        return output;
    }
}