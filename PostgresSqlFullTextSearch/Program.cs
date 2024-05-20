using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using PostgresSqlFullTextSearch.DataAccess;
using PostgresSqlFullTextSearch.Services;

namespace PostgresSqlFullTextSearch
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //To use console app as a background service
            await CreateHostBuilder(args).Build().RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {

            /* //Manual Configuration of the Host
            // Build configuration
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Optionally bind to a strongly - typed object
            var appSettings = new AppSettings();
            configuration.GetSection("AppSettings").Bind(appSettings);
            */

            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    // Ensure the appsettings.json file is added to the configuration
                    config.SetBasePath(AppContext.BaseDirectory);
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddHostedService<App>();

                    // Register IConfiguration and other services
                    services.AddSingleton<IConfiguration>(context.Configuration);

                    services.AddDbContext<AppDbContext>();
                    services.AddScoped<ProductRepository>();
                })
                .ConfigureLogging(logging =>
                {
                    // Configure logging if necessary
                    logging.ClearProviders();
                    logging.AddConsole();
                }) //; Add this to run as default hosted service
                .UseWindowsService(); // Add this to run as a Windows Service;
        }
    }

}
