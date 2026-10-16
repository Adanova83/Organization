namespace Api.Catalog;

public sealed record MigrationInfo(
    string Id,
    string Name,
    DateTime? Timestamp,
    bool Applied);

public sealed record OperationInfo(string Type, string? Target);

public sealed record MigrationDetail(
    string Id,
    string Name,
    DateTime? Timestamp,
    bool Applied,
    string? Previous,
    IReadOnlyList<OperationInfo> UpOperations,
    IReadOnlyList<OperationInfo> DownOperations,
    string UpSql,
    string DownSql);

public sealed record MigrationStatus(
    string? Provider,
    int Total,
    int Applied,
    int Pending,
    string? LastApplied,
    string? Latest,
    bool ModelHasPendingChanges);

public sealed record ColumnInfo(string Name, string StoreType, bool Nullable);

public sealed record ForeignKeyInfo(
    string Name,
    IReadOnlyList<string> Columns,
    string PrincipalTable,
    IReadOnlyList<string> PrincipalColumns,
    string OnDelete);

public sealed record IndexInfo(string Name, IReadOnlyList<string> Columns, bool Unique);

public sealed record TableInfo(
    string Schema,
    string Name,
    IReadOnlyList<ColumnInfo> Columns,
    IReadOnlyList<string> PrimaryKey,
    IReadOnlyList<ForeignKeyInfo> ForeignKeys,
    IReadOnlyList<IndexInfo> Indexes);

public enum ApplyStatus
{
    Applied,
    DryRun,
    NothingToDo,
    UnknownTarget,
    RollbackNotAllowed,
    ModelHasPendingChanges
}

public sealed record ApplyResult(
    ApplyStatus Status,
    string? From,
    string? To,
    string? Script,
    string Message);
