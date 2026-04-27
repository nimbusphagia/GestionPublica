using System.Security.Claims;
using GestionPublica.BC;
using GestionPublica.BE;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace GestionPublica.GUI.Controllers;

public class AuthController : Controller
{
    private readonly UsuarioBC _usuarioBC = new UsuarioBC();

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity.IsAuthenticated)
            return RedirectByRole((User.FindFirst(ClaimTypes.Role)?.Value));
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string correo, string password)
    {
        try
        {
            var usuario = _usuarioBC.Login(correo, password);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Surname, usuario.Apellido),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return RedirectByRole(usuario.Rol);
        }
        catch (Exception ex)
        {
            ViewBag.Error = ex.Message;
            return View();
        }
    }

    [HttpGet]
    public IActionResult Registro()
    {
        if (User.Identity.IsAuthenticated)
            return RedirectByRole(User.FindFirst(ClaimTypes.Role)?.Value);
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Registro(string nombre, string apellido, string dni, string telefono, string correo,
        string password)
    {
        try
        {
            var usuario = new UsuarioBE
            {
                Nombre = nombre,
                Apellido = apellido,
                DNI = dni,
                Telefono = telefono,
                Correo = correo,
                PasswordHash = password
            };

            _usuarioBC.Registrar(usuario);
            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            ViewBag.Error = ex.Message;
            return View();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    private IActionResult RedirectByRole(string rol)
    {
        return rol switch
        {
            "superadmin" => RedirectToAction("Index", "Superadmin"),
            "administrador" => RedirectToAction("Index", "Administrador"),
            _ => RedirectToAction("Index", "Ciudadano")
        };
    }

}