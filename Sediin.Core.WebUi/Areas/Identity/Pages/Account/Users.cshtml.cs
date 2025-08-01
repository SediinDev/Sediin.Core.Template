using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
//using Sediin.Core.WebUi.Pages.Application;
using Sediin.Core.WebUi.Areas.Identity.Data;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Sediin.Core.WebUi.Controllers;

namespace Sediin.Core.WebUi.Areas.Identity.Pages.Account
{
    public class IdentityUserExt : IdentityUser
    {
        public List<string> Roles { get; set; } = new List<string>();
    }

    public class UserListModel : ApplicationPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<UserListModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AuthenticationDbContext _context;

        public UserListModel(UserManager<IdentityUser> userManager, IUserStore<IdentityUser> userStore, RoleManager<IdentityRole> roleManager, ILogger<UserListModel> logger, AuthenticationDbContext context,
            SignInManager<IdentityUser> signInManager, EmailSender emailSender) : base(signInManager, emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _logger = logger;
            _roleManager = roleManager;
            _context = context;
        }

        public List<IdentityUserExt> UsersList { get; set; } = new List<IdentityUserExt>();
        public List<SelectListItem> RolesList { get; set; } = new List<SelectListItem>();
        public string Role { get; set; } = string.Empty;

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }

        internal void GetList()
        {
            RolesList = _roleManager.Roles.
                 Select(s => new SelectListItem { Value = s.Name.ToString(), Text = s.Name }).ToList();
            RolesList?.Insert(0, new SelectListItem("-- Nessun ruolo --", ""));

            var users = _userManager.Users.ToList();
            UsersList = new List<IdentityUserExt>();
            foreach (IdentityUser user in users)
            {
                IdentityUserExt ext = new IdentityUserExt()
                {
                    AccessFailedCount = user.AccessFailedCount,
                    ConcurrencyStamp = user.ConcurrencyStamp,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    Id = user.Id,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd,
                    NormalizedEmail = user.NormalizedEmail,
                    NormalizedUserName = user.NormalizedUserName,
                    PasswordHash = user.PasswordHash,
                    PhoneNumber = user.PhoneNumber,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    SecurityStamp = user.SecurityStamp,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    UserName = user.UserName,
                };

                ext.Roles = _userManager.GetRolesAsync(user).Result.ToList();

                UsersList.Add(ext);
            }
        }

        public void OnGet()
        {
            GetList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            Dictionary<string, string> listValues = new Dictionary<string, string>();
            foreach (string key in Request.Form.Keys)
            {
                listValues.Add(key, Request.Form[key]);
            }

            if (listValues.ContainsKey("DeleteUser"))
            {
                string value = string.Empty;
                listValues.TryGetValue("UserName", out value);

                var res = await DeleteUser(value);
            }
            else if (listValues.ContainsKey("RemoveRoleUser"))
            {
                string UserName = string.Empty;
                listValues.TryGetValue("UserName", out UserName);

                string Role = string.Empty;
                listValues.TryGetValue("RemoveRoleUser", out Role);

                var res = await RemoveRoleUser(UserName, Role);
            }
            else if (listValues.ContainsKey("AddRoleUser"))
            {
                string UserName = string.Empty;
                listValues.TryGetValue("UserName", out UserName);

                string Role = string.Empty;
                listValues.TryGetValue("Role", out Role);
                if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Role))
                {
                    var res = await AddRoleUser(UserName, Role);
                }
            }
            /*
            else
            {
                returnUrl ??= Url.Content("~/");
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                if (ModelState.IsValid)
                {
                    var user = CreateUser();

                    await _userStore.SetUserNameAsync(user, Input.Username, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                    user.EmailConfirmed = true;

                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(Input.Role))
                        {
                            var res = await AddRoleUser(user.UserName, Input.Role);
                        }

                        _logger.LogInformation("User created a new account with password.");

                        if (User.Identity.Name != "Administrator")
                        {
                            var userId = await _userManager.GetUserIdAsync(user);
                            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                            var callbackUrl = Url.Page(
                                "/Account/ConfirmEmail",
                                pageHandler: null,
                                values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                                protocol: Request.Scheme);

                            await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                            if (_userManager.Options.SignIn.RequireConfirmedAccount)
                            {
                                return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                            }
                            else
                            {
                                await _signInManager.SignInAsync(user, isPersistent: false);
                                return LocalRedirect(returnUrl);
                            }
                        }
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            */

            GetList();
            // If we got this far, something failed, redisplay form
            return Page();
        }


        public async Task<IActionResult> OnPostDeleteUser(string UserName)
        {
            var x = await DeleteUser(UserName);
            GetList();
            return Page();
        }
        private async Task<IdentityResult> DeleteUser(string UserName)
        {
            var result = new IdentityResult();
            var user = await _userManager.FindByNameAsync(UserName);
            if (user != null)
            {
                result = await _userManager.DeleteAsync(user);
                var userId = await _userManager.GetUserIdAsync(user);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Unexpected error occurred deleting user.");
                }

                await _signInManager.SignOutAsync();
                _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);
            }

            return result;
        }

        public async Task<IActionResult> OnPostAddRoleUser(string UserName, string Role)
        {
            var x = await AddRoleUser(UserName, Role);
            GetList();

            return Page();
        }
        private async Task<IdentityResult> AddRoleUser(string UserName, string Role)
        {
            IdentityResult roleresult = new IdentityResult();
            if (!string.IsNullOrEmpty(Role))
            {
                IdentityUser user = await _userManager.FindByNameAsync(UserName);
                IdentityUserExt userExt = new IdentityUserExt()
                {
                    AccessFailedCount = user.AccessFailedCount,
                    ConcurrencyStamp = user.ConcurrencyStamp,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    Id = user.Id,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd,
                    NormalizedEmail = user.NormalizedEmail,
                    NormalizedUserName = user.NormalizedUserName,
                    PasswordHash = user.PasswordHash,
                    PhoneNumber = user.PhoneNumber,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    SecurityStamp = user.SecurityStamp,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    UserName = user.UserName,
                };
                userExt.Roles = _userManager.GetRolesAsync(user).Result.ToList();
                if (!userExt.Roles.Contains(Role))
                {
                    roleresult = await _userManager.AddToRoleAsync(user, Role);
                }
            }

            return roleresult;
        }

        public async Task<IActionResult> OnPostRemoveRoleUser(string UserName, string Role)
        {
            var x = await RemoveRoleUser(UserName, Role);
            GetList();

            return Page();
        }
        private async Task<IdentityResult> RemoveRoleUser(string UserName, string Role)
        {
            IdentityResult roleresult = new IdentityResult();
            if (!string.IsNullOrEmpty(Role))
            {
                IdentityUser User = await _userManager.FindByNameAsync(UserName);
                IdentityUserExt UserExt = new IdentityUserExt()
                {
                    AccessFailedCount = User.AccessFailedCount,
                    ConcurrencyStamp = User.ConcurrencyStamp,
                    Email = User.Email,
                    EmailConfirmed = User.EmailConfirmed,
                    Id = User.Id,
                    LockoutEnabled = User.LockoutEnabled,
                    LockoutEnd = User.LockoutEnd,
                    NormalizedEmail = User.NormalizedEmail,
                    NormalizedUserName = User.NormalizedUserName,
                    PasswordHash = User.PasswordHash,
                    PhoneNumber = User.PhoneNumber,
                    PhoneNumberConfirmed = User.PhoneNumberConfirmed,
                    SecurityStamp = User.SecurityStamp,
                    TwoFactorEnabled = User.TwoFactorEnabled,
                    UserName = User.UserName,
                };
                UserExt.Roles = _userManager.GetRolesAsync(User).Result.ToList();
                if (UserExt.Roles.Contains(Role))
                {
                    roleresult = await _userManager.RemoveFromRoleAsync(User, Role);
                }
            }

            return roleresult;
        }

    }
}
