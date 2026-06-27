using carwash.Service.DTOs.Common;
using carwash.Service.DTOs.Points;

namespace carwash.Service.Interfaces;

public interface IPointsService
{
    Task<ServiceResult<ScannedUserDto>> GetUserByQrCodeAsync(string qrCode);
    Task<ServiceResult<ScannedUserDto>> ApplyPointsAsync(ApplyPointsRequest request);
}
