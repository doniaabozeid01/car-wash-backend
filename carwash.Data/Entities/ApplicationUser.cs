using Microsoft.AspNetCore.Identity;

namespace carwash.Data.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public int Points { get; set; }
    public string? QrCode { get; set; }
}
