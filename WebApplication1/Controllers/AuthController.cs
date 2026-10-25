using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Contracts;
using WebApplication1.Data;
using WebApplication1.Enums;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers;

public class AuthController : Controller
{
    
    private readonly IAuthService _authService;
    private readonly ApplicationDbContext _dbContext;

    public AuthController(IAuthService authService, ApplicationDbContext dbContext)
    {
        _authService = authService;
        _dbContext = dbContext;
    }

    [HttpGet]
    public IActionResult Login()
    {
        var model = new LoginViewModel();
        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        var model = new RegisterViewModel();
        
        LoadSchoolsAsync(model).Wait();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.RegisterAsync(model);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        TempData["SuccessMessage"] =
            "Registration submitted. Your school account is pending verification.";

        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.LoginAsync(model);

        if (result.Succeeded)
            return RedirectToAction("Index", "Home");

        if (result.IsLockedOut)
            ModelState.AddModelError(
                string.Empty, "Your account is temporarily locked.");
        else if (result.IsNotAllowed)
            ModelState.AddModelError(
                string.Empty, "You are not allowed to sign in yet.");
        else
            ModelState.AddModelError(
                string.Empty, "Incorrect email or password.");

        return View(model);
    }
    
    private async Task LoadSchoolsAsync(RegisterViewModel model)
    {
        model.Schools = await _dbContext.Schools
            .AsNoTracking()
            .Where(s => s.Status == SchoolStatus.Active)
            .OrderBy(s => s.Name)
            .Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            })
            .ToListAsync();
    }
}