using Intranet;
using Intranet.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using D = Library.Data;
using H = Library.Handler;
using R = Library.Repository;


// Loading .env file
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

AppSettings.Initialize(builder.Configuration);

// Configure OrganizationDbContext with PostgreSQL
var connectionString = AppSettings.GetAdanovaConnectionString();
if (string.IsNullOrEmpty(connectionString))
    throw new InvalidOperationException("Connection string 'Adanova' not found.");

// Add Database to the services
builder.Services.AddDbContext<D.FullDbContext>(options =>
    options
        .UseNpgsql(
            connectionString,
            npgsqlOptions =>
            {
                npgsqlOptions.CommandTimeout(60);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);
                npgsqlOptions.MigrationsHistoryTable("MigrationsHistory", "Migration");
            }
        )
        .EnableDetailedErrors()
);

// Configure Authentication and Authorization
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/accessdenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    }
);

const string permissionCode = "Permission";
builder.Services.AddAuthorization(options =>
    {
        foreach (var permission in Permissions.All)
        {
            options.AddPolicy(permission, policy => policy.RequireClaim(permissionCode, permission));
        }
    }
);
builder.Services.AddCascadingAuthenticationState();

// Add Database Respositories
builder.Services.AddScoped<R.IAccessEvent, R.AccessEvent>();
builder.Services.AddScoped<R.IActivity, R.Activity>();
builder.Services.AddScoped<R.IActivitySession, R.ActivitySession>();
builder.Services.AddScoped<R.IActivitySessionSlot, R.ActivitySessionSlot>();
builder.Services.AddScoped<R.IActivitySessionSlotAnimation, R.ActivitySessionSlotAnimation>();
builder.Services.AddScoped<R.IActivitySessionSlotSubscription, R.ActivitySessionSlotSubscription>();
builder.Services.AddScoped<R.IBadge, R.Badge>();
builder.Services.AddScoped<R.IBadgeAssignation, R.BadgeAssignation>();
builder.Services.AddScoped<R.IChat, R.Chat>();
builder.Services.AddScoped<R.IChatMember, R.ChatMember>();
builder.Services.AddScoped<R.IChatMessage, R.ChatMessage>();
builder.Services.AddScoped<R.IChatMessageRecipient, R.ChatMessageRecipient>();
builder.Services.AddScoped<R.IContribution, R.Contribution>();
builder.Services.AddScoped<R.ICountry, R.Country>();
builder.Services.AddScoped<R.IDocument, R.Document>();
builder.Services.AddScoped<R.IEvent, R.Event>();
builder.Services.AddScoped<R.IEventAnimation, R.EventAnimation>();
builder.Services.AddScoped<R.IEventSubscription, R.EventSubscription>();
builder.Services.AddScoped<R.IRole, R.Role>();
builder.Services.AddScoped<R.IRoleMember, R.RoleMember>();
builder.Services.AddScoped<R.IRolePermission, R.GroupPermission>();
builder.Services.AddScoped<R.IHoliday, R.Holiday>();
builder.Services.AddScoped<R.IMember, R.Member>();
builder.Services.AddScoped<R.INote, R.Note>();
builder.Services.AddScoped<R.INoteLink, R.NoteLink>();
builder.Services.AddScoped<R.IOrganization, R.Organization>();
builder.Services.AddScoped<R.IOrganizationSession, R.OrganizationSession>();
builder.Services.AddScoped<R.IProject, R.Project>();
builder.Services.AddScoped<R.IProjectSlot, R.ProjectSlot>();
builder.Services.AddScoped<R.IProjectSlotAnimation, R.ProjectSlotAnimation>();
builder.Services.AddScoped<R.IProjectSlotSubscription, R.ProjectSlotSubscription>();
builder.Services.AddScoped<R.IPermission, R.Permission>();
builder.Services.AddScoped<R.IPermissionMember, R.PermissionMember>();
builder.Services.AddScoped<R.ISpace, R.Space>();

// Add Database Handler
builder.Services.AddScoped<H.AccessEvent>();
builder.Services.AddScoped<H.Activity>();
builder.Services.AddScoped<H.ActivitySession>();
builder.Services.AddScoped<H.ActivitySessionSlot>();
builder.Services.AddScoped<H.ActivitySessionSlotAnimation>();
builder.Services.AddScoped<H.ActivitySessionSlotSubscription>();
builder.Services.AddScoped<H.Badge>();
builder.Services.AddScoped<H.BadgeAssignation>();
builder.Services.AddScoped<H.Chat>();
builder.Services.AddScoped<H.ChatMember>();
builder.Services.AddScoped<H.ChatMessage>();
builder.Services.AddScoped<H.ChatMessageRecipient>();
builder.Services.AddScoped<H.Contribution>();
builder.Services.AddScoped<H.Country>();
builder.Services.AddScoped<H.Document>();
builder.Services.AddScoped<H.Event>();
builder.Services.AddScoped<H.EventAnimation>();
builder.Services.AddScoped<H.EventSubscription>();
builder.Services.AddScoped<H.Holiday>();
builder.Services.AddScoped<H.Member>();
builder.Services.AddScoped<H.Note>();
builder.Services.AddScoped<H.NoteLink>();
builder.Services.AddScoped<H.Organization>();
builder.Services.AddScoped<H.OrganizationSession>();
builder.Services.AddScoped<H.Permission>();
builder.Services.AddScoped<H.PermissionMember>();
builder.Services.AddScoped<H.Project>();
builder.Services.AddScoped<H.ProjectSlot>();
builder.Services.AddScoped<H.ProjectSlotAnimation>();
builder.Services.AddScoped<H.ProjectSlotSubscription>();
builder.Services.AddScoped<H.Role>();
builder.Services.AddScoped<H.RoleMember>();
builder.Services.AddScoped<H.RolePermission>();
builder.Services.AddScoped<H.Space>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Apply pending migrations at startup
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var fullDbContext = scope.ServiceProvider.GetRequiredService<D.FullDbContext>();

    // Retry logic for applying migrations
    const int maxRetryCount = 5;
    int retryAttempts = 0;

    // Loop until migrations are applied or max retries reached
    while (retryAttempts < maxRetryCount)
    {
        try
        {
            // Increment retry attempts
            retryAttempts++;
            // Check for pending migrations
            logger.LogInformation("Checking for pending database migrations.");
            var pendingMigrations = fullDbContext.Database.GetPendingMigrations();
            if (pendingMigrations.Any())
            {
                // Apply migrations
                logger.LogInformation("Applying database migrations...");
                fullDbContext.Database.Migrate();
                logger.LogInformation("Database migrations applied successfully.");
            }
            else
            {
                // No pending migrations
                logger.LogInformation("No pending database migrations found.");
            }
            // Exit the loop if migration is successful
            break;
        }
        catch (Exception ex)
        {
            // Log the exception and retry
            logger.LogWarning(ex, "An error occurred while migrating the database. Attempt {RetryAttempt} of {MaxRetryCount}", retryAttempts, maxRetryCount);
            if (retryAttempts == maxRetryCount)
            {
                logger.LogError(ex, "Max retry attempts reached. Migration failed.");
                throw; // Rethrow the exception after max attempts
            }
            Thread.Sleep(2000); // Wait before retrying
        }
    }
}

await InitialSeeder.SeedAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
