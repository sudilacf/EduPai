namespace WebApplication1.Entities;

public class PaymentAllocation
{
    public Guid PaymentId { get; set; }
    public Payment Payment { get; set; } = null!;

    public Guid BalanceEntryId { get; set; }
    public BalanceEntry BalanceEntry { get; set; } = null!;

    public decimal Amount { get; set; }
}