using Library.Authentication;
using Library.Data;
using H = Library.Handler;
using R = Library.Repository;

namespace Intranet;

public static class InitialSeeder
{
    public static async Task SeedAsync(IServiceProvider services, CancellationToken ct = default)
    {
        using var scope = services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<FullDbContext>();

        H.Country country = new(scope.ServiceProvider.GetRequiredService<R.ICountry>());

        const string countryFrCodeAlpha2 = "FR";
        const string countryFrCodeAlpha3 = "FRA";
        const string countryFrName = "France";

        var frCountry = country.SearchByCodeAlpaha3Async(countryFrCodeAlpha3, ct).Result.Data;

        if (frCountry == null)
        {
            var tmp = country.CreateAsync(new()
            {
                CodeAlpha2 = countryFrCodeAlpha2,
                CodeAlpha3 = countryFrCodeAlpha3,
                Name = countryFrName,
                Audit = new(),
                Active = new()
                {
                    From = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            }, ct).Result.Data;

            if (tmp == null)
            {
                throw new($"Error while creating {countryFrName}");
            }

            frCountry = tmp;
        }

        H.Member member = new(scope.ServiceProvider.GetRequiredService<R.IMember>());

        const string adminName = "admin";
        const string adminEmail = "admin@local";
        const string adminPassword = "Admin123!";

        var adminMember = member.SearchByEmailAsync(adminEmail, ct).Result.Data;

        if (adminMember == null)
        {
            adminMember = member.CreateAsync(new()
            {
                Addr = new()
                {
                    Line1 = adminName,
                    PostalCode = adminName,
                    City = adminName,
                    CountryId = frCountry.Id,
                },
                Name = adminName,
                Email = adminEmail,
                HashedPassword = Encryptor.Hash(adminPassword),
                Audit = new(),
                Active = new()
                {
                    From = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            }, ct).Result.Data;
        }

        H.Role group = new(scope.ServiceProvider.GetRequiredService<R.IRole>());
        H.RoleMember groupMember = new(scope.ServiceProvider.GetRequiredService<R.IRoleMember>());

        const string groupAdminCode = "Admin";
        const string groupAdminName = "Administrateur";

        var adminGroup = group.SearchByCodeAsync(groupAdminCode, ct).Result.Data;

        if (adminGroup == null)
        {
            adminGroup = group.CreateAsync(new()
            {
                Code = groupAdminCode,
                Name = groupAdminName,
                Audit = new(),
                Active = new()
                {
                    From = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            }, ct).Result.Data;

            await groupMember.CreateAsync(new ()
            {
                RoleId = adminGroup!.Id,
                MemberId = adminMember!.Id,
                Audit = new(),
                Active = new()
                {
                    From = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            }, ct);
        }

        H.Permission permissionHandler = new(scope.ServiceProvider.GetRequiredService<R.IPermission>());
        H.RolePermission rolePermissionHandler = new(scope.ServiceProvider.GetRequiredService<R.IRolePermission>());

        const string permissionCode = "permission";

        foreach (var permissionValue in Permissions.All)
        {
            var permission = permissionHandler.SearchByCodeAsync(permissionCode, permissionValue, ct).Result.Data;

            if (permission == null)
            {
                permission = permissionHandler.CreateAsync(new()
                {
                    Type = permissionCode,
                    Value = permissionValue,
                    Audit = new(),
                    Active = new()
                    {
                        From = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    }
                }, ct).Result.Data;

                await rolePermissionHandler.CreateAsync(new ()
                {
                    RoleId = adminGroup!.Id,
                    PermissionId = permission!.Id,
                    Audit = new(),
                    Active = new()
                    {
                        From = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    }                    
                }, ct);
            }
        }
    }
}