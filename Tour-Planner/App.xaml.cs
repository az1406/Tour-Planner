using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using Tour_Planner.db;
using Tour_Planner.ViewModels;

namespace Tour_Planner
{
    public partial class App : Application
    {
        public IHost Host { get; }

        public App()
        {
            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                    Console.WriteLine($"Using connection string: {connectionString}");
                    services.AddDbContext<TourPlannerDbContext>(options =>
                        options.UseNpgsql(connectionString));
                    services.AddLogging(configure => configure.AddConsole());

                    services.AddTransient<MainViewModel>();
                    services.AddTransient<SearchViewModel>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                await Host.StartAsync();
                var logger = Host.Services.GetRequiredService<ILogger<App>>();
                logger.LogInformation("Application started successfully.");
                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during application startup: {ex.Message}");
                throw;
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await Host.StopAsync();
            base.OnExit(e);
        }
    }
}