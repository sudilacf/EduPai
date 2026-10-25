using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Entities;

public class Receipt
{
    public Guid Id { get; set; }

    public Guid PaymentId { get; set; }
    public Payment Payment { get; set; } = null!;

    [MaxLength(64)]
    public string ReceiptNumber { get; set; } = null!;

    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
}