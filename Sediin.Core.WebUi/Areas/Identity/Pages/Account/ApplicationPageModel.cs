using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sediin.Core.WebUi.Controllers;
//using Sediin.Core.WebUi.Pages.Application;

namespace Sediin.Core.WebUi.Areas.Identity.Pages.Account
{
    public class ApplicationPageModel : PageModel
    {
        protected readonly SignInManager<IdentityUser> _signInManager;
        //protected readonly ILogger<LoginModel> _logger;
        protected readonly EmailSender _emailSender;

        public ApplicationPageModel(
            SignInManager<IdentityUser> signInManager,
            //ILogger<LoginModel> logger,
            EmailSender emailSender)
        {
            _signInManager = signInManager;
            //_logger = logger;
            _emailSender = emailSender;
        }

    }
}
