using Microsoft.AspNetCore.Identity;

namespace PayBill.UserManagement.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
}