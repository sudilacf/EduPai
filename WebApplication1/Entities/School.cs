using WebApplication1.Enums;

namespace WebApplication1.Entities;

public class School
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactNumber { get; set; }
    public SchoolStatus Status { get; set; } = SchoolStatus.PendingApproval;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<SchoolMember> Members { get; set; }
        = new List<SchoolMember>();

    public ICollection<StudentSchoolAccount> StudentAccounts { get; set; }
        = new List<StudentSchoolAccount>();

    public ICollection<SchoolAdminInvitation> Invitations { get; set; }
        = new List<SchoolAdminInvitation>();

    public ICollection<SchoolPaymentConfiguration> PaymentConfigurations { get; set; }
        = new List<SchoolPaymentConfiguration>();
}