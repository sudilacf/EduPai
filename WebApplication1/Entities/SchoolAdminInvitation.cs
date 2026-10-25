namespace WebApplication1.Entities;

public class SchoolAdminInvitation
{
    public Guid Id { get; set; }

    public Guid SchoolId { get; set; }
    public School School { get; set; } = null!;

    public string Email { get; set; } = null!;

    // Store a hash of the token, never the raw invitation token.
    public string TokenHash { get; set; } = null!;

    public string InvitedByUserId { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
}