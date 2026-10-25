using WebApplication1.Enums;
using WebApplication1.Identity;

namespace WebApplication1.Entities;

public class BalanceEntry
{
    public Guid Id { get; set; }

    public Guid StudentSchoolAccountId { get; set; }
    public StudentSchoolAccount StudentSchoolAccount { get; set; }
        = null!;

    public BalanceEntryType Type { get; set; }

    // Positive amount; the entry type determines its accounting effect.
    public decimal Amount { get; set; }

    public string CurrencyCode { get; set; } = "PHP";
    public string Description { get; set; } = null!;

    public string RecordedByUserId { get; set; } = null!;
    public ApplicationUser RecordedByUser { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PaymentAllocation> Allocations { get; set; }
        = new List<PaymentAllocation>();
}