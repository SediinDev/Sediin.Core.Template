using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Sediin.Core.WebUi.Areas
{
#pragma warning disable
    public class AreaFolderConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.Namespace.Contains(".Areas.Backend."))
            {
                controller.RouteValues["area"] = "Backend";
            }
        }
    }
}
