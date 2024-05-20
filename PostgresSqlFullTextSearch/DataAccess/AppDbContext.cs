using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using PostgresSqlFullTextSearch.Models;

namespace PostgresSqlFullTextSearch.DataAccess
{
    public class AppDbContext : DbContext
    {
        //public readonly string _connectionString = "Server=localhost;Port=5432;Database=postgres;User ID=postgres;Password=Password12;";
        public DbSet<Product> Products { get; set; }

        public AppDbContext() : base()
        {

        }

        public AppDbContext(DbContextOptions options) : base(options)
        {

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
                "english",  // Text search config
                p => new { p.Name, p.Description })  // Included properties
                .HasIndex(p => p.SearchVector)
                .HasMethod("GIN"); // Index method on the search vector (GIN or GIST)
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
    }
}
