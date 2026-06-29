using Codeflix.Catalog.Infra.Data.EF;

using Microsoft.EntityFrameworkCore;

using DomainEntity = Codeflix.Catalog.Domain.Entities;

namespace Codeflix.Catalog.EndToEndTests.Api.CastMembers.Common;

public class CastMemberPersistence(CodeflixCatalogDbContext context)
{
    public async Task InsertList(List<DomainEntity.CastMember> castMember)
    {
        await context.AddRangeAsync(castMember);
        await context.SaveChangesAsync();
    }

    public async Task<DomainEntity.CastMember?> GetById(Guid id)
        => await context.CastMembers.AsNoTracking()
            .FirstOrDefaultAsync(castMember => castMember.Id == id);
}