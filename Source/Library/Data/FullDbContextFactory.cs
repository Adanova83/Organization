using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Library.Data;

public class FullDbContextFactory
    : IDesignTimeDbContextFactory<FullDbContext>
{
    public FullDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FullDbContext>();

        optionsBuilder.UseNpgsql(
            "Host=127.0.0.1;Port=5432;Database=adanova;Username=adanova;Password=adanova"
        );

        return new FullDbContext(optionsBuilder.Options);
    }
}