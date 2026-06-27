using carwash.Service.DTOs.Auth;
using carwash.Service.DTOs.Common;

namespace carwash.Service.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request);
    Task<ServiceResult<UserProfileDto>> GetProfileAsync(string userId);
}
