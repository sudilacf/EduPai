using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApplication1.Entities;
using WebApplication1.Enums;
using WebApplication1.Identity;

namespace WebApplication1.Data.Seed;

public sealed class DatabaseSeeder
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly DatabaseSeedOptions _options;

    public DatabaseSeeder(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<DatabaseSeedOptions> options)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
        _options = options.Value;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        // 1. Create all Identity roles first.
        await EnsureRoleAsync(Constants.Roles.RolePlatformAdmin);
        await EnsureRoleAsync(Constants.Roles.RoleSchoolAdmin);
        await EnsureRoleAsync(Constants.Roles.RoleSchoolStaff);
        await EnsureRoleAsync(Constants.Roles.RoleStudent);

        // 2. Only after roles exist, seed accounts and assign roles.
        foreach (var schoolConfig in _options.Schools)
        {
            var school = await GetOrCreateSchoolAsync(schoolConfig, ct);

            var admin = await GetOrCreateUserAsync(schoolConfig.Admin);
            var staff = await GetOrCreateUserAsync(schoolConfig.Staff);
            var student = await GetOrCreateUserAsync(schoolConfig.Student);

            await EnsureUserRoleAsync(admin, schoolConfig.Admin.Role);
            await EnsureUserRoleAsync(staff, schoolConfig.Staff.Role);
            await EnsureUserRoleAsync(student, schoolConfig.Student.Role);

            await EnsureMembershipAsync(
                school.Id,
                admin.Id,
                SchoolMemberRole.Admin,
                ct);

            await EnsureMembershipAsync(
                school.Id,
                staff.Id,
                SchoolMemberRole.Staff,
                ct);

            var studentProfile = await GetOrCreateStudentAsync(
                student,
                schoolConfig.Student.FullName,
                ct);

            await EnsureStudentSchoolAccountAsync(
                school.Id,
                studentProfile.Id,
                schoolConfig.Student.StudentNumber,
                ct);
        }
    }

    private static void ValidateRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            throw new InvalidOperationException(
                "Every seeded account must have a Role configured.");
        }
    }

    private static SchoolMemberRole ParseSchoolRole(string role)
    {
        if (!Enum.TryParse<SchoolMemberRole>(
                role, ignoreCase: true, out var parsedRole) ||
            !Enum.IsDefined(parsedRole))
        {
            throw new InvalidOperationException(
                $"'{role}' is not a valid school membership role.");
        }

        return parsedRole;
    }

    private async Task EnsureIdentityRoleAsync(string role)
    {
        if (await _roleManager.RoleExistsAsync(role))
            return;

        var result = await _roleManager.CreateAsync(
            new IdentityRole(role));

        if (!result.Succeeded)
        {
            var errors = string.Join(
                "; ",
                result.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"Failed to create Identity role '{role}': {errors}");
        }
    }
    
    private async Task EnsureRoleAsync(string roleName)
    {
        if (await _roleManager.RoleExistsAsync(roleName))
            return;

        var result = await _roleManager.CreateAsync(
            new IdentityRole(roleName));

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to seed role '{roleName}': " +
                string.Join("; ",
                    result.Errors.Select(e => e.Description)));
        }
    }

    private async Task EnsureUserRoleAsync(
        ApplicationUser user,
        string role)
    {
        if (await _userManager.IsInRoleAsync(user, role))
            return;

        var result = await _userManager.AddToRoleAsync(user, role);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                "; ",
                result.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"Failed to assign role '{role}' to " +
                $"'{user.Email}': {errors}");
        }
    }

    private async Task<School> GetOrCreateSchoolAsync(
        SeedSchoolOptions config,
        CancellationToken ct)
    {
        var school = await _db.Schools
            .FirstOrDefaultAsync(x => x.Name == config.Name, ct);

        if (school is not null)
            return school;

        school = new School
        {
            Name = config.Name,
            ContactEmail = config.ContactEmail,
            Status = SchoolStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _db.Schools.Add(school);
        await _db.SaveChangesAsync(ct);

        return school;
    }

    private async Task<ApplicationUser> GetOrCreateUserAsync(
        SeedUserOptions config)
    {
        var user = await _userManager.FindByEmailAsync(
            config.Email);

        if (user is not null)
            return user;

        user = new ApplicationUser
        {
            UserName = config.Email,
            Email = config.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(
            user, _options.DefaultPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                "; ",
                result.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"Failed to create user '{config.Email}': {errors}");
        }

        return user;
    }

    private async Task EnsureMembershipAsync(
        Guid schoolId,
        string userId,
        SchoolMemberRole role,
        CancellationToken ct)
    {
        var membership = await _db.SchoolMembers
            .FirstOrDefaultAsync(
                x => x.SchoolId == schoolId &&
                     x.UserId == userId,
                ct);

        if (membership is not null)
            return;

        _db.SchoolMembers.Add(new SchoolMember
        {
            SchoolId = schoolId,
            UserId = userId,
            Role = role,
            JoinedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync(ct);
    }

    private async Task<Student> GetOrCreateStudentAsync(
        ApplicationUser user,
        string fullName,
        CancellationToken ct)
    {
        var student = await _db.Students
            .FirstOrDefaultAsync(x => x.UserId == user.Id, ct);

        if (student is not null)
            return student;

        student = new Student
        {
            UserId = user.Id,
            FullName = fullName
        };

        _db.Students.Add(student);
        await _db.SaveChangesAsync(ct);

        return student;
    }

    private async Task EnsureStudentSchoolAccountAsync(
        Guid schoolId,
        Guid studentId,
        string studentNumber,
        CancellationToken ct)
    {
        var exists = await _db.StudentSchoolAccounts
            .AnyAsync(
                x => x.SchoolId == schoolId &&
                     x.StudentId == studentId,
                ct);

        if (exists)
            return;

        _db.StudentSchoolAccounts.Add(
            new StudentSchoolAccount
            {
                SchoolId = schoolId,
                StudentId = studentId,
                StudentNumber = studentNumber,
                VerificationStatus =
                    StudentVerificationStatus.Pending,
                RequestedAt = DateTime.UtcNow
            });

        await _db.SaveChangesAsync(ct);
    }
}