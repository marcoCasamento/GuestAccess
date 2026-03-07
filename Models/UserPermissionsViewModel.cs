namespace GuestAccess.Models;

public class UserPermissionsViewModel
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool CanOpenCancello { get; set; }
    public bool CanOpenCancelletto { get; set; }
    public bool CanOpenPortone { get; set; }
}
