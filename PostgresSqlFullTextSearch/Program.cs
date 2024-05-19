using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using PostgresSqlFullTextSearch.DataAccess;
using PostgresSqlFullTextSearch.Services;

namespace PostgresSqlFullTextSearch
{
    internal class Program
    {
        public readonly static string _connectionString = "Server=localhost;Port=5432;Database=postgres;User ID=postgres;Password=Password12;";
        private static IServiceProvider? _serviceProvider;

        static async Task Main(string[] args)
        {
            _serviceProvider = new ServiceCollection()
                                    .AddDbContext<AppDbContext>()
                                    .AddScoped<ProductRepository>()
                                    .BuildServiceProvider();

            await new Test(_serviceProvider).RunAsync();
        }
    }
}
