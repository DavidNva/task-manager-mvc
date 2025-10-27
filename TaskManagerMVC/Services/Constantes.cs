using Microsoft.AspNetCore.Mvc.Rendering;

namespace TaskManagerMVC.Services
{
    public class Constantes
    {
        public const string RolAdmin = "admin";
        public const string RolUsuario = "usuario";

        public static readonly SelectListItem[] CulturasUISoportadas = new SelectListItem[]
        {
            new SelectListItem { Value = "es", Text = "Español" },
            new SelectListItem { Value = "en", Text = "English" }
            //new SelectListItem { Value = "fr", Text = "Français" },
            //new SelectListItem { Value = "de", Text = "Deutsch" },
            //new SelectListItem { Value = "it", Text = "Italiano" }
        };
    }
}
