using Life.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Life.Infrastructure.Repositories
{
    public interface IBoardRepository
    {
        Task<string> CreateAsync(Board b, CancellationToken ct);
        Task<Board?> GetAsync(string id, CancellationToken ct);
        Task SaveSnapshotAsync(string id, long generation, Board b, CancellationToken ct);
    }
}
