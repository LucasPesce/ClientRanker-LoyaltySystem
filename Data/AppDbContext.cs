using Microsoft.EntityFrameworkCore;
using Client_Ranker.Models; 

namespace Client_Ranker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
        {
            this.Database.Migrate();
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<AppConfig> Configurations { get; set; }
        public DbSet<MonthlySummary> MonthlySummaries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=loyalty_database.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.DocumentId)
                .IsUnique();
        }
    }
}