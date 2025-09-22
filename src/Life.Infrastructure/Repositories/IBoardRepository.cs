using Life.Domain.Aggregates;

namespace Life.Infrastructure.Repositories
{
    public interface IBoardRepository
    {
        Task<string> CreateAsync(Board b, CancellationToken ct);
        Task<Board?> GetAsync(string id, CancellationToken ct);
        Task SaveSnapshotAsync(string id, long generation, Board b, CancellationToken ct);
    }
}
