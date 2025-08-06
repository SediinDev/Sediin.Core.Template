using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Sediin.Core.WebUi.Areas
{
#pragma warning disable
    public class AreaFolderConvention : IControllerModelConvention
    {
        private readonly Dictionary<string, string> _namespaceAreaMappings = new()
        {
            { ".Areas.Backend.", "Backend" },
            { ".Areas.Administrator.", "Administrator" }
        };

        public void Apply(ControllerModel controller)
        {
            string? controllerNamespace = controller.ControllerType.Namespace;

            if (string.IsNullOrEmpty(controllerNamespace))
                return;

            foreach (var mapping in _namespaceAreaMappings)
            {
                if (controllerNamespace.Contains(mapping.Key))
                {
                    controller.RouteValues["area"] = mapping.Value;
                    break;
                }
            }
        }
    }
}
