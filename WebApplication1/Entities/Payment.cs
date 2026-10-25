using WebApplication1.Enums;

namespace WebApplication1.Entities;

public class Payment
{
    public Guid Id { get; set; }

    public Guid StudentSchoolAccountId { get; set; }
    public StudentSchoolAccount StudentSchoolAccount { get; set; }
        = null!;

    public decimal Amount { get; set; }

    // For example, USDC.
    public string AssetCode { get; set; } = null!;

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public string? TransactionHash { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }

    public Receipt? Receipt { get; set; }

    public ICollection<PaymentAllocation> Allocations { get; set; }
        = new List<PaymentAllocation>();
}