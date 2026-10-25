using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Contracts;
using WebApplication1.Data;
using WebApplication1.Data.Seed;
using WebApplication1.Identity;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services
    .AddOptions<DatabaseSeedOptions>()
    .Bind(builder.Configuration.GetSection("DatabaseSeed"))
    .ValidateOnStart();

builder.Services.AddScoped<DatabaseSeeder>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;

    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedPhoneNumber = false;

    options.Lockout = new()
    {
        AllowedForNewUsers = true,
        DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5),
        MaxFailedAccessAttempts = 5,
    };
})
.AddDefaultTokenProviders()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// if (app.Environment.IsDevelopment())
// {
//     using var scope = app.Services.CreateScope();
//
//     var db = scope.ServiceProvider
//         .GetRequiredService<ApplicationDbContext>();
//
//     await db.Database.MigrateAsync();
//
//     var seeder = scope.ServiceProvider
//         .GetRequiredService<DatabaseSeeder>();
//
//     await seeder.SeedAsync();
// }

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();