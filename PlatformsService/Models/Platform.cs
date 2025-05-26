using System.ComponentModel.DataAnnotations;

namespace PlatformsService.Models;

public record Platform(string Name, string Publisher, string Cost)
{
    [Key]
    [Required]
    public int Id;

    [Required]
    public string Name = Name;

    [Required]
    public string Publisher = Publisher;

    [Required]
    public string Cost = Cost;
}