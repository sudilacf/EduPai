using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels;

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

public sealed class RegisterViewModel
{
    [Required(ErrorMessage = "Full name is required")]
    [Display(Name = "Full Name")]
    public string? Fullname { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Please select your school")]
    [Display(Name = "School")]
    public Guid? SchoolId { get; set; }

    // Populated by the controller; never supplied by the user.
    public IEnumerable<SelectListItem> Schools { get; set; } = [];

    [Required(ErrorMessage = "Student ID Number is required")]
    [Display(Name = "Student ID Number")]
    public string? StudentIdNumber { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Please confirm your password")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    [Display(Name = "Confirm Password")]
    public string? ConfirmPassword { get; set; }
}