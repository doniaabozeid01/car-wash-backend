using carwash.Hubs;
using carwash.Service.DTOs.Points;
using carwash.Service.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace carwash.Services;

public class SignalRPointsNotifier : IPointsNotifier
{
    private readonly IHubContext<PointsHub> _hubContext;

    public SignalRPointsNotifier(IHubContext<PointsHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyPointsUpdatedAsync(string userId, PointsUpdatedDto update)
    {
        return _hubContext.Clients
            .Group(PointsHub.GetUserGroup(userId))
            .SendAsync("PointsUpdated", update);
    }
}
