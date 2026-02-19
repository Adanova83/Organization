namespace Library.Repository
{
    public class RepositoryResponseRange<RepositoryEntity>
    {
        public IReadOnlyList<RepositoryEntity> Data { get; set; } = default!;
        public int FilteredCount { get; set; }
        public int TotalCount { get; set; }
    }
}
