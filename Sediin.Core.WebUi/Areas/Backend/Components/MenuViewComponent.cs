using Microsoft.AspNetCore.Mvc;
using Sediin.Core.DataAccess;
using Sediin.Core.DataAccess.Entities;
using Sediin.Core.Helpers.Cache;
using Sediin.Core.Identity;
using System.Collections.Generic;


namespace Sediin.Core.WebUi.Areas.Backend.Components
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IUnitOfWorkDataAccess _unitOfWorkDataAccess;
        private readonly IUnitOfWorkIdentity _unitOfWorkIdentity;


        public MenuViewComponent(IUnitOfWorkDataAccess unitOfWorkDataAccess, IUnitOfWorkIdentity unitOfWorkIdentity)
        {
            _unitOfWorkDataAccess = unitOfWorkDataAccess;
            _unitOfWorkIdentity = unitOfWorkIdentity;
        }

        public IEnumerable<Menu> Menu
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return null;
                }

                var cached = HttpContext.Session.GetObject<IEnumerable<Menu>>("Menu");
                if (cached != null)
                    return cached;

                IEnumerable<Menu> _allMenu = _unitOfWorkDataAccess.Menu.GetAll().Result;

                var role = _unitOfWorkIdentity.AuthService.GetUserRole(User.Identity.Name).Result;

                var menu = _allMenu.Where(x => x.Ruoli
                    .Where(a => a.Ruolo == role).Count() > 0);

                List<Menu> _menuPadri = new List<Menu>();

                foreach (var item in menu)
                {
                    if (item.CodmenuPadre.GetValueOrDefault() > 1)
                    {
                        GetPadre(item.CodmenuPadre.GetValueOrDefault());
                    }
                }


                void GetPadre(int codPadre)
                {
                    var _padre = _allMenu.FirstOrDefault(x => x.Codmenu == codPadre);
                    if (_padre != null)
                    {
                        _menuPadri.Add(_padre);

                        if (_padre.CodmenuPadre.GetValueOrDefault() > 1)
                        {
                            GetPadre(_padre.CodmenuPadre.GetValueOrDefault());
                        }
                    }
                }

                var _newenu = menu.Union(_menuPadri).Distinct();

                HttpContext.Session.SetObject("Menu", _newenu);
                return _newenu;
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
                    return View("~/Areas/Backend/Views/Shared/Components/Menu/SideMenu.cshtml", Menu);
                case "navmenu":
                    return View("~/Areas/Backend/Views/Shared/Components/Menu/NavMenu.cshtml", Menu);
                case "homemenu":
                    return View("~/Areas/Backend/Views/Shared/Components/Menu/HomeMenu.cshtml", Menu);

                default:
                    menu = await _unitOfWorkDataAccess.Menu.GetAll();
                    return View("Default", menu);
            }
        }
    }
}
