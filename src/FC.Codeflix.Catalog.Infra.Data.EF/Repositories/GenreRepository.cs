using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Repositories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;

using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
public class GenreRepository(CodeflixCatalogDbContext context) : IGenreRepository
{
    private DbSet<Genre> _genres => context.Set<Genre>();
    private DbSet<GenresCategories> _genresCategories => context.Set<GenresCategories>();

    public async Task Insert(Genre genre, CancellationToken cancellationToken = default)
    {
        await _genres.AddAsync(genre, cancellationToken);
        if (genre.Categories.Count > 0)
        {
            var relations = genre.Categories
                .Select(categoryId => new GenresCategories(genre.Id, categoryId));
            await _genresCategories.AddRangeAsync(relations, cancellationToken);
        }
    }

    public async Task<Genre> Get(Guid id, CancellationToken cancellationToken = default)
    {
        var genre = await _genres
            .AsNoTracking()
            .FirstOrDefaultAsync((x) => x.Id == id, cancellationToken)
                ?? throw new NotFoundException($"Genre '{id}' not found.");

        var categoryIds = await _genresCategories
            .Where(x => x.GenreId == id)
            .Select(x => x.CategoryId)
            .ToListAsync(cancellationToken);
        categoryIds.ForEach(genre.AddCategory);
        return genre;
    }

    public Task Delete(Genre aggregate, CancellationToken cancellationToken = default)
    {
        var relations = _genresCategories.Where(x => x.GenreId == aggregate.Id);
        _genresCategories.RemoveRange(relations);
        _genres.Remove(aggregate);
        return Task.CompletedTask;
    }

    public async Task Update(Genre aggregate, CancellationToken cancellationToken = default)
    {
        _genres.Update(aggregate);
        var oldRelations = _genresCategories.Where(x => x.GenreId == aggregate.Id);
        _genresCategories.RemoveRange(oldRelations);
        if (aggregate.Categories.Count > 0)
        {
            var relations = aggregate.Categories
                .Select(categoryId => new GenresCategories(aggregate.Id, categoryId));
            await _genresCategories.AddRangeAsync(relations, cancellationToken);
        }
    }

    public async Task<SearchOutput<Genre>> Search(SearchInput input, CancellationToken cancellationToken = default)
    {
        var genres = await _genres.AsNoTracking().ToListAsync(cancellationToken);
        return new SearchOutput<Genre>(
            currentPage: input.Page,
            perPage: input.PerPage,
            total: genres.Count,
            genres
        );
    }
}
