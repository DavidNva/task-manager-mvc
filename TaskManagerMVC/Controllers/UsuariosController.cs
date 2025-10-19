using Microsoft.AspNetCore.Authentication;
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

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult>Login(LoginViewModel modelo)
        {
            if(!ModelState.IsValid)
            {
                return View(modelo);
            }

            var resultado = await _signInManager.PasswordSignInAsync(modelo.Email, modelo.Password, modelo.Recuerdame, lockoutOnFailure: false);//Por ahora no bloquea la cuenta si hay muchos intentos fallidos, ya en un tema de producción se puede activar
            //Tener en cuenta que lockoutOnFailure aplicaria cuando el usuario ha hecho este numero de intentos fallidos: _maxFailedAccessAttempts de IdentityOptions.Lockout, que por defecto es 5 y lo podemos ver en Startup.cs o Program.cs dependiendo de la version de .NET. Si no esta,
            //podemos configurarlo en Program.cs al agregar los servicios de identidad
            //ejemplo para hacerlo es asi: services.Configure<IdentityOptions>(options => { options.Lockout.MaxFailedAccessAttempts = 3; });

            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Inicio de sesión inválido");//Mensaje generico para no dar pistas sobre si el email o la contraseña son incorrectos, porque eso podria ser un riesgo de seguridad
                return View(modelo);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout2()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }

        //cual de los dos es mejor usar para el logout? //R: Ambos métodos de cierre de sesión son válidos y funcionan correctamente en ASP.NET Core Identity. Sin embargo, el método preferido es utilizar _signInManager.SignOutAsync() porque es más explícito y se adhiere mejor a la arquitectura de ASP.NET Core. Usar _signInManager.SignOutAsync() también garantiza que cualquier lógica adicional asociada con el cierre de sesión se maneje correctamente. Por lo tanto, se recomienda utilizar el primer método para una mejor claridad y mantenimiento del código. No obstante, ambos métodos logran el mismo resultado final de cerrar la sesión del usuario. Entonces , en resumen, es mejor usar _signInManager.SignOutAsync() por claridad y adherencia a las prácticas recomendadas de ASP.NET Core Identity. Pero ambos son correctos. La unica diferencia real es la claridad y adherencia a las prácticas recomendadas.

    }
}
