using Microsoft.Extensions.Options;
using H = Library.Handler;
using Do = Library.Domain;

namespace Api.Security;

public static class AdminSeeder
{
    /// <summary>
    /// Crée, s'il n'existe pas, le membre du premier administrateur (Admin:Email) et le pays « France » dont
    /// son adresse a besoin. Ne modifie jamais un membre existant. À appeler après l'application des migrations.
    /// </summary>
    public static async Task SeedAdminMemberAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AdminOptions>>().Value;

        if (string.IsNullOrWhiteSpace(options.Email))
        {
            return;
        }

        try
        {
            var members = scope.ServiceProvider.GetRequiredService<H.Member>();
            var countries = scope.ServiceProvider.GetRequiredService<H.Country>();
            var ct = app.Lifetime.ApplicationStopping;

            var existing = await members.SearchByEmailAsync(options.Email, ct);
            if (existing.Data is not null)
            {
                return;
            }

            var country = (await countries.SearchByCodeAlpaha2Async("FR", ct)).Data;
            if (country is null)
            {
                var created = await countries.CreateAsync(
                    new Do.Country
                    {
                        CodeAlpha2 = "FR",
                        CodeAlpha3 = "FRA",
                        Name = "France",
                        Active = new Do.Active(),
                        Audit = new Do.Audit()
                    },
                    ct);
                country = created.Data!;
            }

            // L'adresse est obligatoire en base : valeurs provisoires, à corriger ensuite par PUT /api/me.
            await members.CreateAsync(
                new Do.Member
                {
                    Firstname = options.Firstname,
                    Lastname = string.IsNullOrWhiteSpace(options.Lastname) ? "Administrateur" : options.Lastname,
                    Email = options.Email,
                    RGBColor = "FFFFFFFF",
                    Addr = new Do.Address("À renseigner", "À renseigner", "00000") { CountryId = country.Id },
                    Active = new Do.Active(),
                    Audit = new Do.Audit()
                },
                ct);

            logger.LogInformation("Administrator member created for {Email}.", options.Email);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Could not create the administrator member (are the migrations applied?).");
        }
    }
}
