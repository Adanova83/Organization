using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class ChatMember : IChatMember
{
    private readonly FullDbContext _dbContext;

    public ChatMember(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.ChatMember>> CreateAsync(Domain.ChatMember domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.ChatMembers.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.ChatMember>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public async Task DeleteAsync(Domain.ChatMember domain, CancellationToken ct)
    {
        var entity = await _dbContext.ChatMembers.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.ChatMembers.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.ChatMember domain, CancellationToken ct)
    {
        var entity = await _dbContext.ChatMembers.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.Entry(entity).CurrentValues.SetValues(domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.ChatMember>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.ChatMembers
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.ChatMember>()
        {
            Data = entity?.EntityToDomain()
        };
    }
}
