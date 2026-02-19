using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class ChatMessageRecipient : IChatMessageRecipient
{
    private readonly FullDbContext _dbContext;

    public ChatMessageRecipient(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.ChatMessageRecipient>> CreateAsync(Domain.ChatMessageRecipient domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.ChatMessageRecipients.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.ChatMessageRecipient>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public Task DeleteAsync(Domain.ChatMessageRecipient domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.ChatMessageRecipients.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.ChatMessageRecipient domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.ChatMessageRecipients.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.ChatMessageRecipient>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.ChatMessageRecipients
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.ChatMessageRecipient>()
        {
            Data = entity?.EntityToDomain()
        };
    }
}
