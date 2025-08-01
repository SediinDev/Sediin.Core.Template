using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sediin.Core.WebUi.Areas.Administrator.Models;
using Sediin.Core.WebUi.Controllers;

namespace Sediin.Core.WebUi.Areas.Administrator.Controllers
{
    public class AdministratorController : ApplicationController
    {
        //private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AdministratorController> _logger;

        public AdministratorController(
            SignInManager<IdentityUser> signInManager,
            ILogger<AdministratorController> logger,
            EmailSender emailSender) : base(signInManager, emailSender)
        {
            //_signInManager = signInManager;
            _logger = logger;
            //_emailSender = emailSender;
        }

        // GET: AdministratorController
        public ActionResult Index()
        {
            return View();
        }

        // GET: BackendController/Details/5
        public ActionResult Details(int id)
        {
            return View("~/Areas/Administrator/Views/Details.cshtml", new AdministratorModel());
        }

        // GET: BackendController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BackendController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BackendController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BackendController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BackendController/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            return Json(new
            {
                isValid = true,
                message = "Il file è stato trovato oppure no"
            });
        }

        // POST: BackendController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBis(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
