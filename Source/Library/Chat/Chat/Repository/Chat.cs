using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class Chat : IChat
{
    private readonly FullDbContext _dbContext;

    public Chat(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.Chat>> CreateAsync(Domain.Chat domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.Chats.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.Chat>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public Task DeleteAsync(Domain.Chat domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.Chats.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.Chat domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.Chats.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.Chat>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.Chats
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.Chat>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.Chat>> SearchAsync(Guid id, DateOnly dateFrom, DateOnly dateTo, CancellationToken ct)
    {
        IQueryable<Entity.Chat> q = _dbContext.Chats.AsNoTracking();

        q = q.Where(item => item.Id == id);

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.Chat>()
        {
            Data = entities
             .Select(Mapper.Chat.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.ChatMessages.CountAsync(ct)
        };
    }
}
