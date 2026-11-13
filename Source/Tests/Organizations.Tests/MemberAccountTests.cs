using Api.Accounts;
using Library.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Do = Library.Domain;
using En = Library.Entity;

namespace Organizations.Tests;

/// <summary>Comptes des membres dans Authentik, pilotés depuis l'intranet (faux annuaire en mémoire).</summary>
public class MemberAccountTests
{
    private sealed class FakeDirectory : IDirectoryClient
    {
        public readonly Dictionary<string, string> GroupIds = new() { ["membres"] = "g-membres", ["animateurs"] = "g-animateurs" };
        public readonly Dictionary<int, (DirectoryUser User, HashSet<string> Groups, string? Password)> Users = [];
        private int _next = 100;

        public DirectoryUser Add(string username, string email, bool superuser = false)
        {
            var user = new DirectoryUser(_next++, username, username, email, true, superuser, []);
            Users[user.Id] = (user, [], null);
            return user;
        }

        private DirectoryUser Snapshot(int id) => Users[id].User with { GroupIds = Users[id].Groups.ToList() };

        public Task<DirectoryUser?> GetUserAsync(int id, CancellationToken ct) => Task.FromResult(Users.ContainsKey(id) ? Snapshot(id) : null);

        public Task<DirectoryUser?> FindUserByEmailAsync(string email, CancellationToken ct)
            => Task.FromResult(Users.Values.Where(u => string.Equals(u.User.Email, email, StringComparison.OrdinalIgnoreCase)).Select(u => Snapshot(u.User.Id)).FirstOrDefault());

        public Task<bool> UsernameExistsAsync(string username, CancellationToken ct) => Task.FromResult(Users.Values.Any(u => u.User.Username == username));

        public Task<DirectoryUser> CreateUserAsync(string username, string name, string email, string path, CancellationToken ct)
        {
            var user = new DirectoryUser(_next++, username, name, email, true, false, []);
            Users[user.Id] = (user, [], null);
            return Task.FromResult(user);
        }

        public Task UpdateUserAsync(int id, string name, string email, CancellationToken ct)
        {
            Users[id] = Users[id] with { User = Users[id].User with { Name = name, Email = email } };
            return Task.CompletedTask;
        }

        public Task SetActiveAsync(int id, bool isActive, CancellationToken ct)
        {
            Users[id] = Users[id] with { User = Users[id].User with { IsActive = isActive } };
            return Task.CompletedTask;
        }

        public Task SetPasswordAsync(int id, string password, CancellationToken ct)
        {
            Users[id] = Users[id] with { Password = password };
            return Task.CompletedTask;
        }

        public Task<string> GetGroupIdAsync(string name, CancellationToken ct) => Task.FromResult(GroupIds[name]);

        public Task AddToGroupAsync(string groupId, int userId, CancellationToken ct)
        {
            Users[userId].Groups.Add(groupId);
            return Task.CompletedTask;
        }

        public Task RemoveFromGroupAsync(string groupId, int userId, CancellationToken ct)
        {
            Users[userId].Groups.Remove(groupId);
            return Task.CompletedTask;
        }
    }

    private static (AccountService Service, Library.Handler.Member Members, FakeDirectory Directory, FullDbContext Db) Setup()
    {
        var db = new FullDbContext(new DbContextOptionsBuilder<FullDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var members = new Library.Handler.Member(new Library.Repository.Member(db));
        var directory = new FakeDirectory();
        var options = Options.Create(new DirectoryOptions { BaseUrl = "https://auth.example.org", Token = "token" });
        return (new AccountService(directory, options, members, NullLogger<AccountService>.Instance), members, directory, db);
    }

    private static async Task<Do.Member> AddMemberAsync(FullDbContext db, Library.Handler.Member members, string firstname, string lastname, string email)
    {
        var country = new En.Country { Id = Guid.NewGuid(), CodeAlpha2 = "FR", CodeAlpha3 = "FRA", Name = "France", Active = new En.Active(), Audit = new En.Audit() };
        db.Add(country);
        var member = new En.Member
        {
            Id = Guid.NewGuid(), Firstname = firstname, Lastname = lastname, Email = email,
            Addr = new En.Address { Line1 = "1 rue", City = "Toulon", PostalCode = "83000", CountryId = country.Id },
            Active = new En.Active(), Audit = new En.Audit()
        };
        db.Add(member);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        return (await members.SearchAsync(member.Id, CancellationToken.None)).Data!;
    }

    [Fact]
    public void Username_is_firstname_dot_lastname_without_accents()
    {
        Assert.Equal("eloise.de-la-tour", AccountService.BaseUsername("Éloïse", "De La Tour"));
        Assert.Equal("dupont", AccountService.BaseUsername(null, "Dupont"));
        Assert.Matches("^[a-z2-9]{4}-[a-z2-9]{4}-[a-z2-9]{4}$", AccountService.NewPassword());
    }

    [Fact]
    public async Task Create_makes_an_animator_account_with_a_temporary_password_and_a_unique_username()
    {
        var (service, members, directory, db) = Setup();
        directory.Add("marie.dupont", "autre@example.org");
        var member = await AddMemberAsync(db, members, "Marie", "Dupont", "marie@example.org");

        var account = await service.CreateAsync(member, isAnimator: true, CancellationToken.None);

        Assert.Equal("marie.dupont2", account.Username);
        Assert.True(account.IsAnimator);
        Assert.NotNull(account.TemporaryPassword);
        var stored = directory.Users.Values.Single(u => u.User.Username == "marie.dupont2");
        Assert.Equal(account.TemporaryPassword, stored.Password);
        Assert.Equal(["g-animateurs", "g-membres"], stored.Groups.Order());
        Assert.Equal(stored.User.Id.ToString(), (await members.SearchAsync(member.Id, CancellationToken.None)).Data!.DirectoryAccountId);

        await Assert.ThrowsAsync<AccountConflictException>(async () =>
            await service.CreateAsync((await members.SearchAsync(member.Id, CancellationToken.None)).Data!, false, CancellationToken.None));
    }

    [Fact]
    public async Task Existing_admin_account_with_same_email_is_linked_but_never_modified()
    {
        var (service, members, directory, db) = Setup();
        var admin = directory.Add("ClementB", "clement@example.org", superuser: true);
        var member = await AddMemberAsync(db, members, "Clément", "B", "Clement@example.org");

        var account = await service.CreateAsync(member, isAnimator: true, CancellationToken.None);

        Assert.True(account.IsProtected);
        Assert.Null(account.TemporaryPassword);
        Assert.Empty(directory.Users[admin.Id].Groups);
        Assert.Null(directory.Users[admin.Id].Password);

        member = (await members.SearchAsync(member.Id, CancellationToken.None)).Data!;
        await Assert.ThrowsAsync<AccountConflictException>(() => service.ResetPasswordAsync(member, CancellationToken.None));
    }

    [Fact]
    public async Task Sync_copies_the_name_and_deactivates_the_account_when_the_member_is_deleted()
    {
        var (service, members, directory, db) = Setup();
        var member = await AddMemberAsync(db, members, "Paul", "Martin", "paul@example.org");
        await service.CreateAsync(member, isAnimator: false, CancellationToken.None);
        member = (await members.SearchAsync(member.Id, CancellationToken.None)).Data!;
        var id = int.Parse(member.DirectoryAccountId!);

        member.Firstname = "Pierre";
        await members.EditAsync(member, CancellationToken.None);
        await service.SyncAsync(member, CancellationToken.None);
        Assert.Equal("Pierre Martin", directory.Users[id].User.Name);

        await members.DeleteAsync(member, CancellationToken.None);
        await service.SyncAsync(member, CancellationToken.None);
        Assert.False(directory.Users[id].User.IsActive);
    }
}
