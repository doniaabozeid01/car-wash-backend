using System.ComponentModel.DataAnnotations;
using carwash.Data.Entities;

namespace carwash.Service.DTOs.Cars;

public class CreateUserCarRequest
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string CarType { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string PlateNumber { get; set; } = string.Empty;

    [Required]
    public CarSize Size { get; set; }
}
