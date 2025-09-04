using FC.Codeflix.Catalog.Application.UseCases.Genres.Common;
using FC.Codeflix.Catalog.Domain.Repositories;

namespace FC.Codeflix.Catalog.Application.UseCases.Genres.GetGenre;

public class GetGenreUseCase : IGetGenreUseCase
{
    private readonly IGenreRepository _genreRepository;
    private readonly ICategoryRepository _categoryRepository;

    public GetGenreUseCase(IGenreRepository genreRepository, ICategoryRepository categoryRepository)
    {
        _genreRepository = genreRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<GenreModelOutput> Handle(GetGenreInput request, CancellationToken cancellationToken)
    {
        var genre = await _genreRepository.Get(request.Id, cancellationToken);
        var output = GenreModelOutput.FromGenre(genre);
        if (output.Categories.Count > 0)
        {
            var categories = (await _categoryRepository
                    .GetListByIds(output.Categories
                        .Select(x => x.Id).ToList(), cancellationToken))
                .ToDictionary(x => x.Id);
            foreach (var category in output.Categories)
                category.Name = categories[category.Id].Name;
        }

        return output;
    }
}