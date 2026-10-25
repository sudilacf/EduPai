using WebApplication1.Contracts;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Enums;
using WebApplication1.Identity;
using WebApplication1.ViewModels;

namespace WebApplication1.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public sealed class AuthService : IAuthService
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthService(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _db = db;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IdentityResult> RegisterAsync(
        RegisterViewModel model)
    {
        var email = model.Email!.Trim();
        var studentNumber = model.StudentIdNumber!.Trim();

        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Description = "An account with this email already exists."
                });
        }

        if (model.SchoolId is null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Description = "Please select a school."
            });
        }

        var school = await _db.Schools
            .FirstOrDefaultAsync(s =>
                s.Id == model.SchoolId.Value &&
                s.Status == SchoolStatus.Active);

        if (school is null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Description = "The selected school does not exist or is unavailable."
            });
        }

        var studentNumberExists = await _db.StudentSchoolAccounts
            .AnyAsync(a =>
                a.SchoolId == school.Id &&
                a.StudentNumber == studentNumber);

        if (studentNumberExists)
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Description = "This student ID is already registered with the school."
                });
        }

        await using var transaction =
            await _db.Database.BeginTransactionAsync();

        try
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = false
            };

            var createResult = await _userManager.CreateAsync(
                user, model.Password!);

            if (!createResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return createResult;
            }

            // The Student role must already exist in AspNetRoles.
            if (!await _userManager.IsInRoleAsync(user, "Student"))
            {
                var roleResult = await _userManager.AddToRoleAsync(
                    user, "Student");

                if (!roleResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return roleResult;
                }
            }

            var student = new Student
            {
                UserId = user.Id,
                FullName = model.Fullname!.Trim()
            };

            _db.Students.Add(student);

            var schoolAccount = new StudentSchoolAccount
            {
                SchoolId = school.Id,
                Student = student,
                StudentNumber = studentNumber,
                VerificationStatus = StudentVerificationStatus.Pending,
                RequestedAt = DateTime.UtcNow
            };

            _db.StudentSchoolAccounts.Add(schoolAccount);

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return IdentityResult.Success;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<SignInResult> LoginAsync(LoginViewModel model)
    {
        var email = model.Email!.Trim();

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return SignInResult.Failed;

        return await _signInManager.PasswordSignInAsync(
            user,
            model.Password!,
            isPersistent: false,
            lockoutOnFailure: true);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}