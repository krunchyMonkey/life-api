using Life.Infrastructure.Data;
using Life.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Life.Infrastructure.Seeding;

public class DatabaseSeeder
{
    private readonly LifeDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(LifeDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Ensure the database is created
            await _context.Database.MigrateAsync();
            _logger.LogInformation("Database migrations applied successfully");

            // Check if we already have data
            if (await _context.Boards.AnyAsync())
            {
                _logger.LogInformation("Database already contains data. Skipping seeding.");
                return;
            }

            _logger.LogInformation("Starting database seeding...");

            // Seed initial data
            await SeedSampleBoards();

            await _context.SaveChangesAsync();
            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private async Task SeedSampleBoards()
    {
        // Create a sample Game of Life pattern - Glider
        var gliderBoard = new BoardEntity
        {
            Id = Guid.NewGuid().ToString(),
            Width = 10,
            Height = 10,
            CreatedUtc = DateTimeOffset.UtcNow
        };

        // Add some snapshots for the glider pattern
        var gliderSnapshots = new List<SnapshotEntity>
        {
            new SnapshotEntity
            {
                Id = Guid.NewGuid().ToString(),
                BoardId = gliderBoard.Id,
                Generation = 0,
                Data = System.Text.Encoding.UTF8.GetBytes("[[1,0],[2,1],[0,2],[1,2],[2,2]]") // Glider pattern
            }
        };

        // Create a sample blinker pattern
        var blinkerBoard = new BoardEntity
        {
            Id = Guid.NewGuid().ToString(),
            Width = 5,
            Height = 5,
            CreatedUtc = DateTimeOffset.UtcNow
        };

        var blinkerSnapshots = new List<SnapshotEntity>
        {
            new SnapshotEntity
            {
                Id = Guid.NewGuid().ToString(),
                BoardId = blinkerBoard.Id,
                Generation = 0,
                Data = System.Text.Encoding.UTF8.GetBytes("[[1,2],[2,2],[3,2]]") // Blinker pattern (oscillator)
            }
        };

        // Add to context
        await _context.Boards.AddRangeAsync(gliderBoard, blinkerBoard);
        await _context.Snapshots.AddRangeAsync(gliderSnapshots);
        await _context.Snapshots.AddRangeAsync(blinkerSnapshots);

        _logger.LogInformation("Added sample boards: Glider and Blinker patterns");
    }
}