using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Sediin.Core.DataAccess.Abstract;
using Sediin.Core.DataAccess.Data;
using Sediin.Core.DataAccess.Repository;
using Sediin.Core.Helpers.Html;
using Sediin.Core.Identity.Abstract;
using Sediin.Core.Identity.Data;
using Sediin.Core.Identity.Entities;
using Sediin.Core.Identity.Repository;
using Sediin.Core.WebUi.Areas;
using Sediin.Core.WebUi.Controllers;
using Sediin.Core.WebUi.Filters;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURAZIONI CONNECTION STRING ---
var identityConn = builder.Configuration.GetConnectionString("SediinCoreIdentityConnection");
var dataConn = builder.Configuration.GetConnectionString("SediinCoreDataAccessConnection");

// --- DB CONTEXTS ---
builder.Services.AddDbContext<SediinCoreIdentityDbContext>(options =>
    options.UseSqlServer(identityConn));

builder.Services.AddDbContext<SediinCoreDataAccessDbContext>(options =>
    options.UseSqlServer(dataConn));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// --- IDENTITY ---
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
    options.User.RequireUniqueEmail = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    //options.LoginPath = "/Identity/Account/Login";
    options.LoginPath = "/Authentication/Login/";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

// --- DEPENDENCY INJECTION ---
builder.Services.AddScoped<IUnitOfWorkDataAccess, UnitOfWorkDataAccess>();
builder.Services.AddScoped<IUnitOfWorkIdentity, UnitOfWorkIdentity>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();



builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddMemoryCache();
builder.Services.AddSession();

// --- LOGGING (Serilog) ---
builder.Host.UseSerilog((ctx, cfg) =>
    cfg.ReadFrom.Configuration(ctx.Configuration));

// --- CONTROLLERS / VIEWS / RAZOR ---
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// --- CONFIGURAZIONE FILE EXTRA ---
builder.Configuration.AddJsonFile("Config/Menu.json", optional: true, reloadOnChange: true);


// Aggiungi filtro globale
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<HandleAjaxErrorAttribute>();
});


builder.Services.AddControllersWithViews(options =>
{
    options.Conventions.Add(new AreaFolderConvention());
});

var app = builder.Build();

// --- MIDDLEWARE ---
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();


// --- ROUTING ---
//Importante, aggiungere in AreaFolderConvention.cs area a mano 
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authentication}/{action=Login}/{id?}");

app.MapRazorPages();

app.Run();
