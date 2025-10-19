using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagerMVC;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDBContext>(opciones => opciones.UseSqlServer("name=DefaultConnection"));
builder.Services.AddAuthentication();
builder.Services.AddIdentity<IdentityUser, IdentityRole>(opciones =>
{
    opciones.SignIn.RequireConfirmedAccount = false;//Para que no pida confirmacion de cuenta al registrar un nuevo usuario.
}).AddEntityFrameworkStores<ApplicationDBContext>()
.AddDefaultTokenProviders();//Esto es para que use los tokens por defecto para cosas como resetear contraseñas, etc. y el application db context que creamos antes es el que usara para almacenar los usuarios y roles.

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
