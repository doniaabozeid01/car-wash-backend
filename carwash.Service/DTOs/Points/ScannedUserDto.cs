using carwash.Service.DTOs.Cars;

namespace carwash.Service.DTOs.Points;

public class ScannedUserDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public IReadOnlyList<UserCarDto> Cars { get; set; } = [];
}
