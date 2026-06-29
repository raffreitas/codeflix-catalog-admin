using Codeflix.Catalog.Infra.Data.EF.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeflix.Catalog.Infra.Data.EF.Configurations;

internal class VideosGenresConfiguration : IEntityTypeConfiguration<VideosGenres>
{
    public void Configure(EntityTypeBuilder<VideosGenres> builder)
        => builder.HasKey(relation => new { relation.GenreId, relation.VideoId });
}