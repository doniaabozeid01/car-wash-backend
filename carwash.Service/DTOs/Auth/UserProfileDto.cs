using carwash.Service.DTOs.Cars;

namespace carwash.Service.DTOs.Auth;

public class UserProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? QrCodeBase64 { get; set; }
    public IReadOnlyList<UserCarDto>? Cars { get; set; }
}
