using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PayBill.UserManagement.Identity;

public class MembershipDbContext : IdentityDbContext<ApplicationUser>
{
    public MembershipDbContext(DbContextOptions<MembershipDbContext> options) : base(options)
    {
        // Leave it empty.
    }
}