using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Repositories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;

using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

public class GenreRepository(CodeflixCatalogDbContext context) : IGenreRepository
{
    private DbSet<Genre> Genres => context.Set<Genre>();
    private DbSet<GenresCategories> GenresCategories => context.Set<GenresCategories>();

    public async Task Insert(Genre genre, CancellationToken cancellationToken = default)
    {
        await Genres.AddAsync(genre, cancellationToken);
        if (genre.Categories.Count > 0)
        {
            var relations = genre.Categories
                .Select(categoryId => new GenresCategories(genre.Id, categoryId));
            await GenresCategories.AddRangeAsync(relations, cancellationToken);
        }
    }

    public async Task<Genre> Get(Guid id, CancellationToken cancellationToken = default)
    {
        var genre = await Genres
                        .AsNoTracking()
                        .FirstOrDefaultAsync((x) => x.Id == id, cancellationToken)
                    ?? throw new NotFoundException($"Genre '{id}' not found.");

        var categoryIds = await GenresCategories
            .Where(x => x.GenreId == id)
            .Select(x => x.CategoryId)
            .ToListAsync(cancellationToken);
        categoryIds.ForEach(genre.AddCategory);
        return genre;
    }

    public Task Delete(Genre aggregate, CancellationToken cancellationToken = default)
    {
        var relations = GenresCategories.Where(x => x.GenreId == aggregate.Id);
        GenresCategories.RemoveRange(relations);
        Genres.Remove(aggregate);
        return Task.CompletedTask;
    }

    public async Task Update(Genre aggregate, CancellationToken cancellationToken = default)
    {
        Genres.Update(aggregate);
        var oldRelations = GenresCategories.Where(x => x.GenreId == aggregate.Id);
        GenresCategories.RemoveRange(oldRelations);
        if (aggregate.Categories.Count > 0)
        {
            var relations = aggregate.Categories
                .Select(categoryId => new GenresCategories(aggregate.Id, categoryId));
            await GenresCategories.AddRangeAsync(relations, cancellationToken);
        }
    }

    public async Task<SearchOutput<Genre>> Search(SearchInput input, CancellationToken cancellationToken = default)
    {
        var toSkip = (input.Page - 1) * input.PerPage;

        var total = await Genres.CountAsync(cancellationToken);

        var genres = await Genres
            .AsNoTracking()
            .Skip(toSkip)
            .Take(input.PerPage)
            .ToListAsync(cancellationToken);


        var genresIds = genres.Select(x => x.Id).ToList();
        var relations = await GenresCategories
            .AsNoTracking()
            .Where(relation => genresIds.Contains(relation.GenreId))
            .ToListAsync(cancellationToken: cancellationToken);

        var relationsByGenreIdsIdGroup = relations
            .GroupBy(x => x.GenreId)
            .ToList();

        relationsByGenreIdsIdGroup.ForEach(relationGroup =>
        {
            var genre = genres.Find(x => x.Id == relationGroup.Key);
            if (genre is not null)
                relationGroup
                    .ToList()
                    .ForEach(relation => genre.AddCategory(relation.CategoryId));
        });

        return new SearchOutput<Genre>(
            currentPage: input.Page,
            perPage: input.PerPage,
            total: total,
            genres
        );
    }
}