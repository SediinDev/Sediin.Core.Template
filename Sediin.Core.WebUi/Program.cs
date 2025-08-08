using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Sediin.Core.DataAccess.Abstract;
using Sediin.Core.DataAccess.Data;
using Sediin.Core.DataAccess.Repository;
using Sediin.Core.Helpers.Html;
using Sediin.Core.Identity.Abstract;
using Sediin.Core.Identity.Data;
using Sediin.Core.Identity.Entities;
using Sediin.Core.Identity.Mapping;
using Sediin.Core.Identity.Repository;
using Sediin.Core.Mvc.Helpers.Middleware;
using Sediin.Core.TemplateConfiguration;
using Sediin.Core.WebUi.Areas;
using Sediin.Core.WebUi.Controllers;
using Sediin.Core.WebUi.Filters;
using Serilog;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

//--------------------------------------------------------
//  DATABASE CONFIGURATION
//--------------------------------------------------------
builder.Services.AddDbContext<SediinCoreIdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SediinCoreIdentityConnection")));

builder.Services.AddDbContext<SediinCoreDataAccessDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SediinCoreDataAccessConnection")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//--------------------------------------------------------
//  IDENTITY
//--------------------------------------------------------
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
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Authentication/Login/";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

//--------------------------------------------------------
//  DEPENDENCY INJECTION
//--------------------------------------------------------
builder.Services.AddScoped<IUnitOfWorkDataAccess, UnitOfWorkDataAccess>();
builder.Services.AddScoped<IUnitOfWorkIdentity, UnitOfWorkIdentity>();
builder.Services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddSingleton<IBaseConfiguration>(provider =>
{
    var env = provider.GetRequiredService<IWebHostEnvironment>();
    var pathToJson = Path.Combine(env.WebRootPath, "json", "config.json");
    return new BaseConfiguration(pathToJson);
});

//--------------------------------------------------------
//  AUTO MAPPER
//--------------------------------------------------------
//builder.Services.SediinIdentityAutoMapper();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//--------------------------------------------------------
//  SESSION, CACHE, LOCALIZATION
//--------------------------------------------------------
builder.Services.AddMemoryCache();
builder.Services.AddSession();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("it-IT") };
    options.DefaultRequestCulture = new RequestCulture("it-IT");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

//--------------------------------------------------------
//  LOGGING
//--------------------------------------------------------
builder.Host.UseSerilog((ctx, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration));

//--------------------------------------------------------
//  MVC, RAZOR, FILTERS, AREAS
//--------------------------------------------------------
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<HandleAjaxErrorAttribute>();
    options.Filters.Add<NoCacheAttribute>();
    options.Conventions.Add(new AreaFolderConvention());
});

builder.Services.AddRazorPages();

//--------------------------------------------------------
//  APP BUILD
//--------------------------------------------------------
var app = builder.Build();

app.UseRequestLocalization();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseMiddleware<QueryDecryptMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

//--------------------------------------------------------
//  ROUTING
//--------------------------------------------------------
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authentication}/{action=Login}/{id?}");

app.MapRazorPages();

app.Run();
