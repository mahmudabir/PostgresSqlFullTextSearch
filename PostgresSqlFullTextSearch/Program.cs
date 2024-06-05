using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using PostgresSqlFullTextSearch.DataAccess;
using PostgresSqlFullTextSearch.Models;
using PostgresSqlFullTextSearch.Services;

namespace PostgresSqlFullTextSearch
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            //To use console app
            IHost host = await CreateHostBuilderAsync<Startup>(args);
            IServiceProvider services = host.Services;

            var configuration = services.GetRequiredService<IConfiguration>();


            try
            {
                await DataMigartionAsync(services);

                var app = services.GetRequiredService<Startup>();
                var cts = new CancellationTokenSource();
                var next = app.StartAsync(cts.Token);

                await next;
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("Operation was cancelled");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void ConfigureServices(HostBuilderContext context, IServiceCollection services, IConfiguration configuration)
        {
            //Add you services
            services.AddDbContext<AppDbContext>();
            services.AddScoped<ProductRepository>();
            services.Configure<AppConfiguration>(configuration.GetSection("AppConfiguration"));
        }

        private static async Task DataMigartionAsync(IServiceProvider serviceProvider)
        {
            #region DB Seed

            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;

                if (Convert.ToBoolean(services.GetRequiredService<IConfiguration>().GetConnectionString("AutomaticMigration")))
                {
                    return;
                }

                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    //Auto migrate if database exists
                    var dbContext = services.GetRequiredService<AppDbContext>();
                    if (await dbContext.Database.CanConnectAsync())
                    {
                        var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync()).ToList();
                        if (pendingMigrations.Any())
                        {
                            //Migrate Database as the database is already there
                            await dbContext.Database.MigrateAsync();
                        }
                    }
                    else
                    {
                        var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync()).ToList();
                        if (pendingMigrations.Any())
                        {
                            //First Migrate then ensure Created to avoid database errors
                            await dbContext.Database.MigrateAsync();

                            //Ensures that Database is created
                            await dbContext.Database.EnsureCreatedAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }

            #endregion DB Seed
        }

        private static Task<IHost> CreateHostBuilderAsync<TApp>(string[] args) where TApp : class, IHostedService
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

            var hostBuilder = Host.CreateDefaultBuilder(args)
                .UseWindowsService() // Add this to run as a Windows Service;
                .ConfigureAppConfiguration((context, config) =>
                {
                    // Ensure the appsettings.json file is added to the configuration
                    config.SetBasePath(AppContext.BaseDirectory);
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    // Register IConfiguration and other services
                    services.AddSingleton(context.Configuration);
                    services.AddSingleton<TApp>(); // services.AddHostedService<TApp>(); Add this to run as default hosted service

                    ConfigureServices(context, services, context.Configuration);
                })
                .ConfigureLogging(logging =>
                {
                    // Configure logging if necessary
                    logging.ClearProviders();
                    logging.AddConsole();
                }); // Add this to run as default hosted service

            var host = hostBuilder.Build(); //.RunAsync(); for hosted/Background service

            return Task.FromResult(host);
        }
    }
}