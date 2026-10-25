using System.ComponentModel.DataAnnotations;
using WebApplication1.Identity;

namespace WebApplication1.Entities;

public class Student
{
    public Guid Id { get; set; }

    [MaxLength(64)]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    [MaxLength(32)]
    public string FullName { get; set; } = null!;

    public ICollection<StudentSchoolAccount> SchoolAccounts { get; set; }
        = new List<StudentSchoolAccount>();
}