using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class ChatMessage : IChatMessage
{
    private readonly FullDbContext _dbContext;

    public ChatMessage(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.ChatMessage>> CreateAsync(Domain.ChatMessage domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.ChatMessages.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.ChatMessage>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public Task DeleteAsync(Domain.ChatMessage domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.ChatMessages.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.ChatMessage domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.ChatMessages.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.ChatMessage>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.ChatMessages
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.ChatMessage>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.ChatMessage>> SearchAsync(Guid senderId, DateOnly dateFrom, DateOnly dateTo, CancellationToken ct)
    {
        IQueryable<Entity.ChatMessage> q = _dbContext.ChatMessages.AsNoTracking();

        q = q.Where(item => item.SenderId == senderId)
            .OrderBy(item => EF.Functions.Collate(item.SendDate, "C"));

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.ChatMessage>()
        {
            Data = entities
             .Select(Mapper.ChatMessage.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.ChatMessages.CountAsync(ct)
        };
    }
}
