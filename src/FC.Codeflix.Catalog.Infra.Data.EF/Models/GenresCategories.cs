using System.ComponentModel.DataAnnotations;

using FC.Codeflix.Catalog.Domain.Entities;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Models;
public class GenresCategories
{
    public GenresCategories(Guid genreId, Guid categoryId)
    {
        GenreId = genreId;
        CategoryId = categoryId;
    }

    public Guid GenreId { get; set; }
    public Guid CategoryId { get; set; }

    public Genre? Genre { get; set; }
    public Category? Category { get; set; }
}
