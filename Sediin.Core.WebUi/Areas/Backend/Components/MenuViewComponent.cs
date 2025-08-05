using Microsoft.AspNetCore.Mvc;
using Sediin.Core.DataAccess.Abstract;
using Sediin.Core.DataAccess.Entities;
using Sediin.Core.Helpers.Cache;


namespace Sediin.Core.WebUi.Areas.Backend.Components
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IUnitOfWorkDataAccess _unitOfWork;

        public MenuViewComponent(IUnitOfWorkDataAccess unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Menu> Menu
        {
            get
            {
                var cached = HttpContext.Session.GetObject<IEnumerable<Menu>>("Menu");
                if (cached != null)
                    return cached;

                var menu = _unitOfWork.Menu.GetAll(includeProperts: "Ruoli").ToList();
                HttpContext.Session.SetObject("Menu", menu);
                return menu;
            }
            set
            {
                HttpContext.Session.SetObject("Menu", value.ToList());
            }
        }

        public async Task<IViewComponentResult> InvokeAsync(string tipo)
        {
            IEnumerable<Menu> menu;

            switch (tipo.ToLower())
            {
                case "sidemenu":
                    return View("SideMenu", Menu);

                case "navmenu":
                    return View("NavMenu", Menu);

                default:
                    menu = _unitOfWork.Menu.GetAll(includeProperts: "Ruoli");
                    return View("Default", menu);
            }
        }
    }
}
