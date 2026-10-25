namespace WebApplication1.Data.Seed;

public sealed class DatabaseSeedOptions
{
    public string DefaultPassword { get; set; } = string.Empty;
    public List<SeedSchoolOptions> Schools { get; set; } = [];
}