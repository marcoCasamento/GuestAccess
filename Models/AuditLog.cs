using System.ComponentModel.DataAnnotations;

namespace GuestAccess.Models;

public class AuditLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string UserEmail { get; set; } = string.Empty;

    [Required]
    public string Action { get; set; } = string.Empty;

    public string? Details { get; set; }

    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public string IpAddress { get; set; } = string.Empty;
}
