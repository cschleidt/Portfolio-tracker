using Microsoft.AspNetCore.SignalR;
using PortfolioTracker.Application.Interfaces;
using PortfolioTracker.Domain.Entities;
using PortfolioTracker.Infrastructure.Hubs;

namespace PortfolioTracker.Infrastructure.Services;

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
        => _hubContext = hubContext;

    public async Task NotifyPortfolioChangedAsync(PortfolioChange change, string investorName, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.All.SendAsync("PortfolioChanged", new
        {
            change.Id,
            change.PortfolioId,
            InvestorName = investorName,
            ChangeType = change.ChangeType.ToString(),
            change.Ticker,
            change.Description,
            change.OldValue,
            change.NewValue,
            change.DetectedAt
        }, cancellationToken);
    }
}
