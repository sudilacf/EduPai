using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels;

public sealed class LoginViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
}