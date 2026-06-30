using Microsoft.AspNetCore.Identity;

namespace carwash.Data.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? QrCode { get; set; }
    public ICollection<UserCar> Cars { get; set; } = [];
}
