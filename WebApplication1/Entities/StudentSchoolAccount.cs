using WebApplication1.Enums;

namespace WebApplication1.Entities;

public class StudentSchoolAccount
{
    public Guid Id { get; set; }

    public Guid SchoolId { get; set; }
    public School School { get; set; } = null!;

    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public string StudentNumber { get; set; } = null!;

    public StudentVerificationStatus VerificationStatus { get; set; }
        = StudentVerificationStatus.Pending;

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? VerifiedAt { get; set; }

    public string? VerifiedByUserId { get; set; }
    public string? RejectionReason { get; set; }

    public ICollection<BalanceEntry> BalanceEntries { get; set; }
        = new List<BalanceEntry>();

    public ICollection<Payment> Payments { get; set; }
        = new List<Payment>();
}