using Microsoft.AspNetCore.Identity;

namespace GuestAccess.Models;

public class ApplicationUser : IdentityUser
{
    public bool CanOpenCancello { get; set; }
    public bool CanOpenCancelletto { get; set; }
    public bool CanOpenPortone { get; set; }
}
