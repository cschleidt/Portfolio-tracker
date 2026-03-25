using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortfolioTracker.Application.Interfaces;
using PortfolioTracker.Infrastructure.BackgroundServices;
using PortfolioTracker.Infrastructure.Hubs;
using PortfolioTracker.Infrastructure.Persistence;
using PortfolioTracker.Infrastructure.Repositories;
using PortfolioTracker.Infrastructure.Services;

namespace PortfolioTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString) || connectionString.Contains("Data Source", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlite(connectionString ?? "Data Source=portfoliotracker.db"));
        }
        else
        {
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseNpgsql(connectionString));
        }

        services.AddScoped<IInvestorRepository, InvestorRepository>();
        services.AddScoped<IPortfolioRepository, PortfolioRepository>();
        services.AddScoped<IPortfolioChangeRepository, PortfolioChangeRepository>();
        services.AddScoped<INotificationService, SignalRNotificationService>();

        services.AddHttpClient<IPortfolioScraperService, PortfolioScraperService>(client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddSignalR();
        services.AddHostedService<PortfolioSyncBackgroundService>();

        return services;
    }
}
