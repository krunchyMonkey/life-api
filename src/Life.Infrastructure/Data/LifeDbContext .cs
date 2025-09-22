using Life.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Life.Infrastructure.Data
{
    public sealed class LifeDbContext : DbContext
    {
        public DbSet<BoardEntity> Boards => Set<BoardEntity>();
        public DbSet<SnapshotEntity> Snapshots => Set<SnapshotEntity>();

        public LifeDbContext(DbContextOptions<LifeDbContext> o) : base(o) { }

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<BoardEntity>().HasKey(x => x.Id);
            b.Entity<SnapshotEntity>().HasKey(x => x.Id);
            b.Entity<SnapshotEntity>().HasIndex(x => new { x.BoardId, x.Generation }).IsUnique();
        }
    }
}
