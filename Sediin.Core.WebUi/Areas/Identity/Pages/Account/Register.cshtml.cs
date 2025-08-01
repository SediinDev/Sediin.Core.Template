// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Sediin.Core.Identity.Data;
using Sediin.Core.WebUi.Controllers;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace Sediin.Core.WebUi.Areas.Identity.Pages.Account
{
    public class RegisterModel : ApplicationPageModel
    {
        //private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        //private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SediinCoreIdentityDbContext _context;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            EmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            SediinCoreIdentityDbContext context) : base(signInManager, emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            //_signInManager = signInManager;
            _logger = logger;
            //_emailSender = emailSender;
            _roleManager = roleManager;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required]
            [Display(Name = "Username")]
            public string Username { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            
            //public List<IdentityUserExt> UsersList { get; set; } = new List<IdentityUserExt>();            
            public List<SelectListItem> RolesList { get; set; } = new List<SelectListItem>();
            public string Role { get; set; }
        }


        /*
        internal void GetList()
        {
            this.Input = new InputModel();
            this.Input.RolesList = _roleManager.Roles.
                 Select(s => new SelectListItem { Value = s.Name.ToString(), Text = s.Name }).ToList();
            this.Input.RolesList?.Insert(0, new SelectListItem("-- Nessun ruolo --", ""));

            var users = _userManager.Users.ToList();
            this.Input.UsersList = new List<IdentityUserExt>();
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
                
                this.Input.UsersList.Add(ext);
            }

        }
        */
        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            //GetList();

            this.Input = new InputModel();
            this.Input.RolesList = _roleManager.Roles.
                 Select(s => new SelectListItem { Value = s.Name.ToString(), Text = s.Name }).ToList();
            this.Input.RolesList?.Insert(0, new SelectListItem("-- Nessun ruolo --", ""));
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            Dictionary<string, string> listValues = new Dictionary<string, string>();
            foreach (string key in Request.Form.Keys)
            {
                listValues.Add(key, Request.Form[key]);
            }

            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                var userExists = await _userManager.FindByEmailAsync(Input.Email);
                if (userExists != null)
                {
                    if (userExists.NormalizedUserName != user.NormalizedUserName)
                    {
                        ModelState.AddModelError(string.Empty, "Can't create the user: Email already registered: " + Input.Email);
                        return Page();
                        //throw new InvalidOperationException($"Can't create the user: Email already registered: " +  Input.Email);
                    }
                }

                await _userStore.SetUserNameAsync(user, Input.Username, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                user.EmailConfirmed = false;
                
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
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
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

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
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
