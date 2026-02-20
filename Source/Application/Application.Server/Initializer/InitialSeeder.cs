using Library.Authentication;
using Library.Data;
using D = Library.Domain;
using R = Library.Repository;

namespace Application.Server.Initializer;

public static class InitialSeeder
{
    public static async Task SeedAsync(IServiceProvider services, CancellationToken ct = default)
    {
        using var scope = services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<FullDbContext>();

        R.Country country = new R.Country(dbContext);

        const string countryFrCodeAlpha2 = "FR";
        const string countryFrCodeAlpha3 = "FRA";
        const string countryFrName = "France";

        var frCountry = country.SearchByCodeAlpaha3Async(countryFrCodeAlpha3, ct).Result.Data;

        if (frCountry == null)
        {
            var tmp = country.CreateAsync(new D.Country
            {
                CodeAlpha2 = countryFrCodeAlpha2,
                CodeAlpha3 = countryFrCodeAlpha3,
                Name = countryFrName,
                Audit = new (),
                Active = new D.Active
                {
                    From = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            }, ct).Result.Data;

            if (tmp == null)
            {
                throw new Exception($"Error while creating {countryFrName}");
            }

            frCountry = tmp;
        }

        R.Member member = new R.Member(dbContext);

        const string adminName = "admin";
        const string adminEmail = "admin@local";
        const string adminPassword = "Admin123!";

        var adminMember = member.SearchByEmailAsync(adminEmail, ct).Result.Data;

        if (adminMember == null)
        {
            adminMember = member.CreateAsync(new D.Member
            {
                Addr = new D.Address
                {
                    Line1 = adminName,
                    PostalCode = adminName,
                    City = adminName,
                    CountryId = frCountry.Id,
                },
                Name = adminName,
                Email = adminEmail,
                HashedPassword = Encryptor.Hash(adminPassword),
                Audit = new (),
                Active = new D.Active
                {
                    From = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }

            }, ct).Result.Data;
        }
    }
}