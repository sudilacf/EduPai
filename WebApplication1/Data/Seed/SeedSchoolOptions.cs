namespace WebApplication1.Data.Seed;

public sealed class SeedSchoolOptions
{
    public string Name { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;

    public SeedUserOptions Admin { get; set; } = new();
    public SeedUserOptions Staff { get; set; } = new();
    public SeedStudentOptions Student { get; set; } = new();
}