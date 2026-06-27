using System.ComponentModel.DataAnnotations;

namespace carwash.Service.DTOs.Auth;

public class LoginRequest
{
    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
