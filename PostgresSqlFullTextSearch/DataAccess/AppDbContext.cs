using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using PostgresSqlFullTextSearch.Models;

namespace PostgresSqlFullTextSearch.DataAccess
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public AppDbContext() : base()
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public AppDbContext(DbContextOptions options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                IConfigurationRoot configuration = builder.Build();
                string connectionString = configuration.GetConnectionString("DefaultConnection")!;
                connectionString = connectionString.Replace("{DatabaseName}", configuration.GetConnectionString("DatabaseName"));

                optionsBuilder.UseNpgsql(connectionString);
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasGeneratedTsVectorColumn(
                p => p.SearchVector,
                "simple",  // Text search config
                p => new { p.Name, p.Description })  // Included properties
                .HasIndex(p => p.SearchVector)
                .HasMethod("GIN"); // Index method on the search vector (GIN or GIST)

            //modelBuilder.HasPostgresExtension("pg_trgm");
            //modelBuilder.Entity<Product>()
            //    .HasIndex(x => x.Description)
            //    .HasMethod("GIN")// Index method on the search vector (GIN or GIST)
            //    .HasOperators("gin_trgm_ops"); // Index operator class
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
    }
}