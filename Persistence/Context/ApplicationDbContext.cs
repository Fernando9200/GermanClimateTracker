using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Persistence.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<WeatherData> WeatherData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherData>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.City).IsRequired();
                entity.Property(e => e.Temperature).IsRequired();
                entity.Property(e => e.Humidity).IsRequired();
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.WindSpeed).IsRequired();
                entity.Property(e => e.Timestamp).IsRequired();
            });
        }
    }
}