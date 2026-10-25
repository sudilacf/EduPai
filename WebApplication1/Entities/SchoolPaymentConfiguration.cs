namespace WebApplication1.Entities;

public class SchoolPaymentConfiguration
{
    public Guid Id { get; set; }

    public Guid SchoolId { get; set; }
    public School School { get; set; } = null!;

    public string Network { get; set; } = null!;
    public string ReceiverWalletAddress { get; set; } = null!;
    public string? ProviderDeploymentId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}