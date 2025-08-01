using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sediin.Core.WebUi.Controllers
{
    public class ApplicationController : Controller
    {
        protected readonly SignInManager<IdentityUser> _signInManager;
        //protected readonly ILogger<LoginModel> _logger;
        protected readonly EmailSender _emailSender;

        public ApplicationController(
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

