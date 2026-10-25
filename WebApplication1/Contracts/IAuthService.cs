using Microsoft.AspNetCore.Identity;
using WebApplication1.ViewModels;

namespace WebApplication1.Contracts;

public interface IAuthService
{
    Task<IdentityResult> RegisterAsync(RegisterViewModel model);

    Task<SignInResult> LoginAsync(LoginViewModel model);

    Task LogoutAsync();
}