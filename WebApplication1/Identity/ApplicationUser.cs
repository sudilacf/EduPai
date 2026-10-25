using Microsoft.AspNetCore.Identity;
using WebApplication1.Entities;

namespace WebApplication1.Identity;

public class ApplicationUser : IdentityUser
{
    public Student? Student { get; set; }

    public ICollection<SchoolMember> SchoolMemberships { get; set; } = [];
}