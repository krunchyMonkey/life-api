using Life.Domain.Aggregates;

namespace Life.Domain.Repositories
{
    /// <summary>
    /// Repository interface for Board aggregate
    /// </summary>
    public interface IBoardRepository
    {
        Task<string> CreateAsync(Board board, CancellationToken cancellationToken = default);
        Task<Board?> GetAsync(string id, CancellationToken cancellationToken = default);
        Task SaveSnapshotAsync(string id, long generation, Board board, CancellationToken cancellationToken = default);
    }
}