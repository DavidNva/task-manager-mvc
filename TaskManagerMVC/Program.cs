using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using TaskManagerMVC;
using TaskManagerMVC.Services;

var builder = WebApplication.CreateBuilder(args);

//Vamos a crear un filtro global para que todas las paginas requieran autenticacion
var politicaUsuariosAutenticados = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();


// Add services to the container.
builder.Services.AddControllersWithViews(opciones =>
{
    opciones.Filters.Add(new AuthorizeFilter(politicaUsuariosAutenticados));
});//Agregamos el filtro global a todas las paginas para que requieran autenticacion, a excepcion de las que tengan el atributo [AllowAnonymous]

builder.Services.AddDbContext<ApplicationDBContext>(opciones => opciones.UseSqlServer("name=DefaultConnection"));
builder.Services.AddAuthentication();
builder.Services.AddIdentity<IdentityUser, IdentityRole>(opciones =>
{
    opciones.SignIn.RequireConfirmedAccount = false;//Para que no pida confirmacion de cuenta al registrar un nuevo usuario.
}).AddEntityFrameworkStores<ApplicationDBContext>()
.AddDefaultTokenProviders()
.AddErrorDescriber<MensajesDeErrorIdentity>();
//AddDefaultTokenProviders:Esto es para que use los tokens por defecto para cosas como resetear contraseñas, etc. y el ApplicationDBContext: application db context que creamos antes es el que usara para almacenar los usuarios y roles.

//Para agregar mensajes en español en las validaciones de identidad, que tenemos la clase MensajesDeErrorIdentity en Models, lo que haremos es agregar un servicio que use esa clase como proveedor de mensajes de error de identity core


builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, opciones =>
{
    opciones.LoginPath = "/Usuarios/Login";//Redirige a esta ruta cuando no este autenticado
    opciones.LogoutPath = "/Usuarios/Login";//Redirige a esta ruta cuando cierre sesion
    opciones.AccessDeniedPath = "/Usuarios/Login";//Redirige a esta ruta cuando no tenga permisos para acceder a una pagina
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
