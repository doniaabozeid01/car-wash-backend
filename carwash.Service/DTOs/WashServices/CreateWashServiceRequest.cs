using System.ComponentModel.DataAnnotations;

namespace carwash.Service.DTOs.WashServices;

public class CreateWashServiceRequest
{
    [Required]
    [MaxLength(100)]
    public string NameAr { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string NameEn { get; set; } = string.Empty;

    [Required]
    public int Points { get; set; }
}
