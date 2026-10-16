using System.Globalization;
using Library.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Api.Catalog;

/// <summary>
/// Catalogue des migrations EF Core de la bibliothèque Library et description du modèle de base de données.
/// Les migrations sont lues dans l'assembly de FullDbContext ; l'état appliqué vient de Migration.MigrationsHistory.
/// </summary>
public sealed class MigrationCatalog
{
    /// <summary>Identifiant EF de « base de données vide » (utilisable comme source ou cible d'un script).</summary>
    public const string InitialDatabase = "0";

    private readonly FullDbContext _db;

    public MigrationCatalog(FullDbContext db)
    {
        _db = db;
    }

    public bool IsKnownTarget(string? id)
        => id is null || id == InitialDatabase || GetAllIds().Contains(id, StringComparer.Ordinal);

    public async Task<IReadOnlyList<MigrationInfo>> ListAsync(CancellationToken ct)
    {
        var applied = await GetAppliedAsync(ct);
        return GetAllIds().Select(id => ToInfo(id, applied.Contains(id))).ToList();
    }

    public async Task<MigrationStatus> GetStatusAsync(CancellationToken ct)
    {
        var all = GetAllIds();
        var applied = await GetAppliedAsync(ct);
        var appliedKnown = all.Where(applied.Contains).ToList();

        return new MigrationStatus(
            Provider: _db.Database.ProviderName,
            Total: all.Count,
            Applied: appliedKnown.Count,
            Pending: all.Count - appliedKnown.Count,
            LastApplied: appliedKnown.LastOrDefault(),
            Latest: all.LastOrDefault(),
            ModelHasPendingChanges: _db.Database.HasPendingModelChanges());
    }

    public async Task<MigrationDetail?> GetAsync(string id, CancellationToken ct)
    {
        var all = GetAllIds();
        var index = all.IndexOf(id);
        if (index < 0)
        {
            return null;
        }

        var applied = await GetAppliedAsync(ct);
        var assembly = _db.GetService<IMigrationsAssembly>();
        var migration = assembly.CreateMigration(assembly.Migrations[id], _db.Database.ProviderName!);
        var previous = index > 0 ? all[index - 1] : null;
        var info = ToInfo(id, applied.Contains(id));

        return new MigrationDetail(
            Id: id,
            Name: info.Name,
            Timestamp: info.Timestamp,
            Applied: info.Applied,
            Previous: previous,
            UpOperations: migration.UpOperations.Select(Describe).ToList(),
            DownOperations: migration.DownOperations.Select(Describe).ToList(),
            UpSql: GenerateScript(previous, id, MigrationsSqlGenerationOptions.Default),
            DownSql: GenerateScript(id, previous ?? InitialDatabase, MigrationsSqlGenerationOptions.Default));
    }

    /// <summary>Génère le script SQL entre deux migrations (from absent = base vide, to absent = dernière migration).</summary>
    public string GenerateScript(string? from, string? to, MigrationsSqlGenerationOptions options)
        => _db.GetService<IMigrator>().GenerateScript(from, to, options);

    /// <summary>Décrit le modèle courant (schéma cible de Library) : tables, colonnes, clés et index.</summary>
    public IReadOnlyList<TableInfo> GetModel()
    {
        var relational = _db.GetService<IDesignTimeModel>().Model.GetRelationalModel();

        return relational.Tables
            .Where(t => !t.IsExcludedFromMigrations)
            .OrderBy(t => t.Schema, StringComparer.Ordinal)
            .ThenBy(t => t.Name, StringComparer.Ordinal)
            .Select(t => new TableInfo(
                Schema: t.Schema ?? string.Empty,
                Name: t.Name,
                Columns: t.Columns
                    .Select(c => new ColumnInfo(c.Name, c.StoreType, c.IsNullable))
                    .ToList(),
                PrimaryKey: t.PrimaryKey?.Columns.Select(c => c.Name).ToList() ?? [],
                ForeignKeys: t.ForeignKeyConstraints
                    .Select(fk => new ForeignKeyInfo(
                        fk.Name,
                        fk.Columns.Select(c => c.Name).ToList(),
                        Qualify(fk.PrincipalTable.Schema, fk.PrincipalTable.Name),
                        fk.PrincipalColumns.Select(c => c.Name).ToList(),
                        fk.OnDeleteAction.ToString()))
                    .OrderBy(fk => fk.Name, StringComparer.Ordinal)
                    .ToList(),
                Indexes: t.Indexes
                    .Select(i => new IndexInfo(i.Name, i.Columns.Select(c => c.Name).ToList(), i.IsUnique))
                    .OrderBy(i => i.Name, StringComparer.Ordinal)
                    .ToList()))
            .ToList();
    }

    public async Task<ApplyResult> ApplyAsync(string? target, bool dryRun, bool allowRollback, CancellationToken ct)
    {
        var all = GetAllIds();

        if (target is not null && target != InitialDatabase && !all.Contains(target, StringComparer.Ordinal))
        {
            return new ApplyResult(ApplyStatus.UnknownTarget, null, target, null, $"Unknown migration '{target}'.");
        }

        var applied = await GetAppliedAsync(ct);
        var lastApplied = all.LastOrDefault(applied.Contains);
        var effectiveTarget = target ?? all.LastOrDefault() ?? InitialDatabase;

        var sameState = effectiveTarget == InitialDatabase ? lastApplied is null : effectiveTarget == lastApplied;
        if (sameState)
        {
            return new ApplyResult(ApplyStatus.NothingToDo, lastApplied, effectiveTarget, null, "Database is already at the requested migration.");
        }

        var isRollback = lastApplied is not null
            && (effectiveTarget == InitialDatabase
                || string.CompareOrdinal(effectiveTarget, lastApplied) < 0);

        if (isRollback && !allowRollback)
        {
            return new ApplyResult(
                ApplyStatus.RollbackNotAllowed, lastApplied, effectiveTarget, null,
                "The target is older than the last applied migration. Pass allowRollback=true to confirm.");
        }

        if (dryRun)
        {
            var script = GenerateScript(lastApplied, effectiveTarget, MigrationsSqlGenerationOptions.Default);
            return new ApplyResult(ApplyStatus.DryRun, lastApplied, effectiveTarget, script, "Dry run: nothing was executed.");
        }

        if (_db.Database.HasPendingModelChanges())
        {
            return new ApplyResult(
                ApplyStatus.ModelHasPendingChanges, lastApplied, effectiveTarget, null,
                "The Library model differs from the last migration snapshot. Create a new migration first (dotnet ef migrations add).");
        }

        await _db.GetService<IMigrator>().MigrateAsync(effectiveTarget, ct);
        return new ApplyResult(ApplyStatus.Applied, lastApplied, effectiveTarget, null, "Migrations applied.");
    }

    private List<string> GetAllIds()
        => _db.GetService<IMigrationsAssembly>().Migrations.Keys
            .OrderBy(k => k, StringComparer.Ordinal)
            .ToList();

    private async Task<HashSet<string>> GetAppliedAsync(CancellationToken ct)
        => (await _db.Database.GetAppliedMigrationsAsync(ct)).ToHashSet(StringComparer.Ordinal);

    private static MigrationInfo ToInfo(string id, bool applied)
    {
        var separator = id.IndexOf('_');
        var name = separator > 0 ? id[(separator + 1)..] : id;

        DateTime? timestamp = null;
        if (separator == 14
            && DateTime.TryParseExact(
                id[..14], "yyyyMMddHHmmss", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var parsed))
        {
            timestamp = parsed;
        }

        return new MigrationInfo(id, name, timestamp, applied);
    }

    private static string Qualify(string? schema, string? name)
        => string.IsNullOrEmpty(schema) ? name ?? string.Empty : $"{schema}.{name}";

    private static OperationInfo Describe(MigrationOperation operation) => operation switch
    {
        EnsureSchemaOperation o => new("EnsureSchema", o.Name),
        CreateTableOperation o => new("CreateTable", Qualify(o.Schema, o.Name)),
        DropTableOperation o => new("DropTable", Qualify(o.Schema, o.Name)),
        RenameTableOperation o => new("RenameTable", $"{Qualify(o.Schema, o.Name)} -> {Qualify(o.NewSchema ?? o.Schema, o.NewName ?? o.Name)}"),
        AddColumnOperation o => new("AddColumn", $"{Qualify(o.Schema, o.Table)}.{o.Name}"),
        AlterColumnOperation o => new("AlterColumn", $"{Qualify(o.Schema, o.Table)}.{o.Name}"),
        DropColumnOperation o => new("DropColumn", $"{Qualify(o.Schema, o.Table)}.{o.Name}"),
        RenameColumnOperation o => new("RenameColumn", $"{Qualify(o.Schema, o.Table)}.{o.Name} -> {o.NewName}"),
        CreateIndexOperation o => new("CreateIndex", $"{Qualify(o.Schema, o.Table)}.{o.Name}"),
        DropIndexOperation o => new("DropIndex", $"{Qualify(o.Schema, o.Table)}.{o.Name}"),
        AddForeignKeyOperation o => new("AddForeignKey", $"{Qualify(o.Schema, o.Table)}.{o.Name}"),
        DropForeignKeyOperation o => new("DropForeignKey", $"{Qualify(o.Schema, o.Table)}.{o.Name}"),
        AddPrimaryKeyOperation o => new("AddPrimaryKey", $"{Qualify(o.Schema, o.Table)}.{o.Name}"),
        DropPrimaryKeyOperation o => new("DropPrimaryKey", $"{Qualify(o.Schema, o.Table)}.{o.Name}"),
        AddUniqueConstraintOperation o => new("AddUniqueConstraint", $"{Qualify(o.Schema, o.Table)}.{o.Name}"),
        DropUniqueConstraintOperation o => new("DropUniqueConstraint", $"{Qualify(o.Schema, o.Table)}.{o.Name}"),
        SqlOperation o => new("Sql", o.Sql.Length > 80 ? o.Sql[..80] + "..." : o.Sql),
        _ => new(operation.GetType().Name.Replace("Operation", string.Empty), null)
    };
}
