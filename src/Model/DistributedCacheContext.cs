using Microsoft.EntityFrameworkCore;

namespace SqlDistributedCache.Model
{
    public class DistributedCacheContext: DbContext
    {
        public DbSet<EntityItem> EntityItem { get; set; }

        public DistributedCacheContext(DbContextOptions<DistributedCacheContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EntityItem>(entity=> {
                entity
                    .Property(p => p.Timestamp)
                    .IsRowVersion();

                entity
                    .HasKey(p => p.Id);
            });
        }
    }
}
