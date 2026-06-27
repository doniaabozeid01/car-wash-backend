using carwash.Service.DTOs.Points;

namespace carwash.Service.Interfaces;

public interface IPointsNotifier
{
    Task NotifyPointsUpdatedAsync(string userId, PointsUpdatedDto update);
}
