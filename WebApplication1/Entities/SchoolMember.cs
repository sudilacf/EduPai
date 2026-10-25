using System.ComponentModel.DataAnnotations;
using WebApplication1.Enums;
using WebApplication1.Identity;

namespace WebApplication1.Entities;

public sealed class SchoolMember
{
    public Guid SchoolId { get; set; }
    public School School { get; set; } = null!;

    [MaxLength(64)]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public SchoolMemberRole Role { get; set; }

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}