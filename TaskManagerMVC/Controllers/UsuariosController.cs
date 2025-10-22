using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
            if (!ModelState.IsValid)
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
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);//Esto agrega los errores al modelo para mostrarlos en la vista, que serian tipo asp-validation-summary
                }
                return View(modelo);
            }
        }

        [AllowAnonymous]
        public IActionResult Login(string mensaje = null)
        {
            if (mensaje is not null)
            {
                ViewData["mensaje"] = mensaje;
            }
         
            return View();
            //Puede ser confuso que si arriba llega null el mensaje, y abajo hacemos el chequeo de null, pero es para que si llega un mensaje desde otra accion (como en el caso de login externo) lo muestre en la vista. 
            //Para que lo entiendas, en la vista Login.cshtml tenemos esto: @if(ViewData["mensaje"] != null) { <div class="alert alert-danger">@ViewData["mensaje"]</div> } y como se llena ese mensaje ? Pues desde aqui, si llega un mensaje por parametro, lo asignamos a ViewData["mensaje"] para que la vista lo pueda mostrar
            //Aunque pareciera que nunca llegaria un mensaje por parametro, si puede llegar, por ejemplo, desde la accion RegistrarUsuarioExterno cuando hay un error con el proveedor externo
            //y como es que se pasa por parametr si nunca en la vista Login.cshtml hay un link o formulario que envie un mensaje? Pues porque en la accion RegistrarUsuarioExterno hacemos esto: return RedirectToAction(nameof(Login), new { mensaje }); esto redirige a la accion Login y le pasa el mensaje como parametro, es decir ese segundo argumento del RedirectToAction es un objeto anonimo que tiene una propiedad mensaje, que es la que se recibe en la accion Login
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            if (!ModelState.IsValid)
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



        [AllowAnonymous]
        [HttpGet]
        public ChallengeResult ExternalLogin(string proveedor, string returnUrl = null)
        {//Significa que vamos a redirigir al usuario a un proveedor externo de autenticación
            var redirectUrl = Url.Action("RegistrarUsuarioExterno", values: new { returnUrl });
            var propiedades = _signInManager
                .ConfigureExternalAuthenticationProperties(proveedor, redirectUrl);//Configura las propiedades necesarias para la autenticación externa, como la URL de redirección después de la autenticación
            return new ChallengeResult(proveedor, propiedades);
        }

        [AllowAnonymous]
        public async Task<IActionResult> RegistrarUsuarioExterno(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");//Si no hay returnUrl, redirige a la raiz del sitio
            var mensaje = "";
            if(remoteError != null)
            {
                mensaje = $"Error de proveedor externo: {remoteError}";
                return RedirectToAction("login", new { mensaje });

                //la diferencia entre hacer esto (nameof(Login) y hacer RedirectToAction("Login") es que el primero es mas seguro en terminos de mantenimiento del codigo, porque si se cambia el nombre del metodo Login, el nameof(Login) se actualiza automaticamente, mientras que "Login" no. Entonces, usar nameof(Login) ayuda a evitar errores tipograficos y facilita el mantenimiento del codigo a largo plazo, de alli en mas es cuestion de preferencia personal o del equipo de desarrollo.
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                mensaje = "Error cargando la información del proveedor externo.";
                return RedirectToAction(nameof(Login), new { mensaje });
            }

            var resultadoLoginExterno = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);//Estamos haciendo el login con el proveedor externo. Con el bypassTwoFactor: true estamos diciendo que no queremos que se aplique la autenticación de dos factores en este caso
            //Ya la cuenta existe
            if (resultadoLoginExterno.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            string email = "";
            if(info.Principal.HasClaim(c => c.Type==ClaimTypes.Email))
            {
                email = info.Principal.FindFirstValue(ClaimTypes.Email);//Obtenemos el email del usuario desde el proveedor externo, como Google o Facebook. El finalValue es el valor del claim, que en este caso es el email y dice first porque puede haber varios claims del mismo tipo
            }
            else
            {
                mensaje = "Error leyendo el mail del usuario del proveedor";
                return RedirectToAction(nameof(Login), new { mensaje });
            }

            var usuario = new IdentityUser { UserName = email, Email = email };
            var resultadoCreacionUsuario = await _userManager.CreateAsync(usuario);
            if(!resultadoCreacionUsuario.Succeeded)
            {
                //mensaje = "Error creando el usuario";
                mensaje = resultadoCreacionUsuario.Errors.First().Description;
                return RedirectToAction(nameof(Login), new { mensaje });
            }
            var resultadoAgregarLogin = await _userManager.AddLoginAsync(usuario, info);//Asociamos el login externo con el usuario que acabamos de crear

            if(resultadoAgregarLogin.Succeeded)
            {
                await _signInManager.SignInAsync(usuario, isPersistent: true, info.LoginProvider);//Hacemos el login del usuario que acabamos de crear, con el proveedor externo
                return LocalRedirect(returnUrl);
            }
            mensaje = "Ha ocurrido un error agregando el login";
            return RedirectToAction(nameof(Login), new { mensaje });//Redirigimos al login con el mensaje de error
        }
    }
}
