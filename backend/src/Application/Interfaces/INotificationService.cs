using PortfolioTracker.Domain.Entities;

namespace PortfolioTracker.Application.Interfaces;

public interface INotificationService
{
    Task NotifyPortfolioChangedAsync(PortfolioChange change, string investorName, CancellationToken cancellationToken = default);
}
