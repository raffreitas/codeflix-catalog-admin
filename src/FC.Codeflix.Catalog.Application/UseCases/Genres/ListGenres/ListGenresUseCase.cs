using FC.Codeflix.Catalog.Domain.Repositories;

namespace FC.Codeflix.Catalog.Application.UseCases.Genres.ListGenres;

public class ListGenresUseCase : IListGenresUseCase
{
    private readonly IGenreRepository _genreRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ListGenresUseCase(IGenreRepository genreRepository, ICategoryRepository categoryRepository)
    {
        _genreRepository = genreRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ListGenresOutput> Handle(ListGenresInput request, CancellationToken cancellationToken)
    {
        var searchOutput = await _genreRepository.Search(request.ToSearchInput(), cancellationToken);
        var output = ListGenresOutput.FromSearchOutput(searchOutput);
        
        var relatedCategoriesIds = output.Items
            .SelectMany(item => item.Categories)
            .Select(category => category.Id)
            .Distinct()
            .ToList();
        
        if (relatedCategoriesIds.Count > 0)
        {
            var categories = await _categoryRepository.GetListByIds(relatedCategoriesIds, cancellationToken);
            output.FillWithCategoryNames(categories);
        }
        
        return output;
    }
}