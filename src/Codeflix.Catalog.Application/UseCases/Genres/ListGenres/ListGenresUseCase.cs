using Codeflix.Catalog.Domain.Repositories;

namespace Codeflix.Catalog.Application.UseCases.Genres.ListGenres;

public class ListGenresUseCase(IGenreRepository genreRepository, ICategoryRepository categoryRepository)
    : IListGenresUseCase
{
    public async Task<ListGenresOutput> Handle(ListGenresInput request, CancellationToken cancellationToken)
    {
        var searchOutput = await genreRepository.Search(request.ToSearchInput(), cancellationToken);
        var output = ListGenresOutput.FromSearchOutput(searchOutput);
        
        var relatedCategoriesIds = output.Items
            .SelectMany(item => item.Categories)
            .Select(category => category.Id)
            .Distinct()
            .ToList();
        
        if (relatedCategoriesIds.Count > 0)
        {
            var categories = await categoryRepository.GetListByIds(relatedCategoriesIds, cancellationToken);
            output.FillWithCategoryNames(categories);
        }
        
        return output;
    }
}