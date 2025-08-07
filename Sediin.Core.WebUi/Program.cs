using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Sediin.Core.DataAccess.Abstract;
using Sediin.Core.DataAccess.Data;
using Sediin.Core.DataAccess.Repository;
using Sediin.Core.Helpers.Html;
using Sediin.Core.Identity.Abstract;
using Sediin.Core.Identity.Data;
using Sediin.Core.Identity.Entities;
using Sediin.Core.Identity.Mapping;
using Sediin.Core.Identity.Repository;
using Sediin.Core.WebUi.Areas;
using Sediin.Core.WebUi.Controllers;
using Sediin.Core.WebUi.Filters;
using Serilog;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// ?? Connessioni DB
var identityConn = builder.Configuration.GetConnectionString("SediinCoreIdentityConnection");
var dataConn = builder.Configuration.GetConnectionString("SediinCoreDataAccessConnection");

// ?? DbContexts
builder.Services.AddDbContext<SediinCoreIdentityDbContext>(options =>
    options.UseSqlServer(identityConn));

builder.Services.AddDbContext<SediinCoreDataAccessDbContext>(options =>
    options.UseSqlServer(dataConn));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ?? Identity
builder.Services.AddDefaultIdentity<SediinIdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<SediinCoreIdentityDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.LoginPath = "/Authentication/Login/";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// Email
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Dependency Injection
builder.Services.AddScoped<IUnitOfWorkDataAccess, UnitOfWorkDataAccess>();
builder.Services.AddScoped<IUnitOfWorkIdentity, UnitOfWorkIdentity>();
builder.Services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Caching & Sessione
builder.Services.AddMemoryCache();
builder.Services.AddSession();

// Logging Serilog
builder.Host.UseSerilog((ctx, cfg) =>
    cfg.ReadFrom.Configuration(ctx.Configuration));

// Razor, MVC, Filtri
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<HandleAjaxErrorAttribute>();
    options.Filters.Add<NoCacheAttribute>();
    options.Conventions.Add(new AreaFolderConvention());
});

builder.Services.AddRazorPages();

// Registra profili AutoMapper dalla class library
builder.Services.SediinIdentityAutoMapper();


// ------------------------------------
// BUILD APP
// ------------------------------------
var app = builder.Build();


var supportedCultures = new[] { new CultureInfo("it-IT") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("it-IT"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

// ?? Middleware custom
app.UseMiddleware<Sediin.Core.Mvc.Helpers.Middleware.QueryDecryptMiddleware>();

// ?? Errori
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ?? HTTPS, statici, routing
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

// ?? Autenticazione & Autorizzazione
app.UseAuthentication();
app.UseAuthorization();

// ?? Routing
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authentication}/{action=Login}/{id?}");

app.MapRazorPages();

app.Run();
