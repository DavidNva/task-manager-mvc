using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManagerMVC.Models;

namespace TaskManagerMVC.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public UsuariosController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(RegistroViewModel modelo)
        {
            if(!ModelState.IsValid)
            {
                return View(modelo);
            }
            var usuario = new IdentityUser { Email = modelo.Email, UserName = modelo.Email };

            var resultado = await _userManager.CreateAsync(usuario, modelo.Password);

            if (resultado.Succeeded)
            {
                await _signInManager.SignInAsync(usuario, isPersistent: true);//Mantener la sesión iniciada
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach(var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);//Esto agrega los errores al modelo para mostrarlos en la vista, que serian tipo asp-validation-summary
                }
                return View(modelo);
            }
        }
    }
}
