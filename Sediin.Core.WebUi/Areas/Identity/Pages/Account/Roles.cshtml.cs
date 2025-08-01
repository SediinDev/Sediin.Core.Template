// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Sediin.Core.WebUi.Areas.Identity.Data;
using Sediin.Core.WebUi.Areas.Identity.Pages.Account;
using Sediin.Core.WebUi.Data;

namespace Sediin.Core.WebUi.Areas.Identity.Pages.Account
{
    public class RoleModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AuthenticationDbContext _context;

        public RoleModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            AuthenticationDbContext context
            )
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
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
            [Display(Name = "Ruolo")]
            public string RoleName { get; set; }

            public List<IdentityRole> RoleList { get; set; } = new List<IdentityRole>();
        }

        internal void GetRoleList()
        {
            var roles = _roleManager.Roles.ToList();

            this.Input = new InputModel();
            this.Input.RoleList = roles;
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // creazione amministratore
            await CreateRoleUser("Amministratore", "Administrator", "c.galletti@sediin.it", "Passw0rd.");

            //var roleStore = new RoleStore<IdentityRole>(_context);
            //var roleMngr = new RoleManager<IdentityRole>(roleStore);
            GetRoleList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            Dictionary<string, string> listValues = new Dictionary<string, string>();
            foreach (string key in Request.Form.Keys)
            {
                listValues.Add(key, Request.Form[key]);
            }

            if (listValues.ContainsKey("CreateRole"))
            {
                string value = string.Empty;
                listValues.TryGetValue("Input.RoleName", out value);
                await CreateRole(value);
            }
            else if (listValues.ContainsKey("DeleteRole"))
            {
                string value = string.Empty;
                listValues.TryGetValue("RoleName", out value);
                await DeleteRole(value);
            }

            GetRoleList();

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

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }

        private async Task CreateRole(string RoleName)
        {
            bool x = await _roleManager.RoleExistsAsync(RoleName);
            if (!x)
            {
                var role = new IdentityRole();
                role.Name = RoleName;
                await _roleManager.CreateAsync(role);
            }
        }

        public async Task<IActionResult> OnPostDeleteRole(string RoleName)
        {
            return await DeleteRole(RoleName);
        }
        private async Task<IActionResult> DeleteRole(string RoleName)
        {
            var role = await _roleManager.FindByNameAsync(RoleName);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }
            GetRoleList();

            return Page();
        }



        private async Task CreateRoleUser(string RoleName, string UserName, string Email, string UserPassord)
        {
            bool x = await _roleManager.RoleExistsAsync(RoleName);
            if (!x)
            {
                // first we create Admin role
                var role = new IdentityRole();
                role.Name = RoleName;
                await _roleManager.CreateAsync(role);

                //Here we create a Admin super user who will maintain the website                   

                var user = CreateUser();
                //var result = await _userManager.CreateAsync(user, Input.Password);


                user.UserName = UserName;
                user.Email = Email;

                IdentityResult chkUser = await _userManager.CreateAsync(user, UserPassord);

                //Add default User to Role Admin    
                if (chkUser.Succeeded)
                {
                    var result1 = await _userManager.AddToRoleAsync(user, RoleName);
                }
            }
        }
    }
}
