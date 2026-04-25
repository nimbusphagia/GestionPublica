using GestionPublica.BC;
using GestionPublica.BE;
using GestionPublica.DALC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionPublic.GUI.Controllers;

[Authorize(Roles = "superadmin")]
public class SuperadminController : Controller
{
    private readonly EspacioBC _espacioBC = new EspacioBC();
    private readonly InstalacionBC _instalacionBC = new InstalacionBC();
    private readonly PenalidadBC _penalidadBC = new PenalidadBC();
    private readonly UsuarioBC _usuarioBC = new UsuarioBC();
    private readonly TipoInstalacionDALC _tipoInstalacionDALC = new();

    public IActionResult Index()
    {
        ViewData["Title"] = "Dashboard";
        ViewData["Active"] = "dashboard";
        ViewBag.TotalEspacios = _espacioBC.ObtenerTodos().Count;
        ViewBag.TotalUsuarios = _usuarioBC.ObtenerTodos().Count;
        ViewBag.PenalidadesActivas = _penalidadBC.ObtenerTodas().Count(p => p.Estado == "activa");
        ViewBag.UsuariosBloqueados = _usuarioBC.ObtenerTodos().Count(u => u.Estado == "bloqueado");
        return View();
    }

    public IActionResult Espacios()
    {
        ViewData["Title"] = "Gestión de Espacios";
        ViewData["Active"] = "espacios";
        var espacios = _espacioBC.ObtenerTodos();
        return View(espacios);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CrearEspacio(EspacioBE espacio)
    {
        try
        {
            _espacioBC.Registrar(espacio);
            return RedirectToAction("Espacios");
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Espacios");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ActualizarEstadoEspacio(int id, string estado)
    {
        try
        {
            _espacioBC.ActualizarEstado(id, estado);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Espacios");
    }

    public IActionResult Instalaciones(int idEspacio)
    {
        ViewData["Title"] = "Instalaciones";
        ViewData["Active"] = "espacios";
        var espacio = _espacioBC.ObtenerPorId(idEspacio);
        var instalaciones = _instalacionBC.ObtenerPorEspacio(idEspacio);
        var tipos = _tipoInstalacionDALC.ObtenerTodos();
        ViewBag.Espacio = espacio;
        ViewBag.Tipos = tipos;
        return View(instalaciones);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CrearInstalacion(InstalacionBE instalacion)
    {
        try
        {
            _instalacionBC.Registrar(instalacion);
            return RedirectToAction("Instalaciones", new { idEspacio = instalacion.IdEspacio });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Instalaciones", new { idEspacio = instalacion.IdEspacio });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ActualizarEstadoInstalacion(int id, string estado, int idEspacio)
    {
        try
        {
            _instalacionBC.ActualizarEstado(id, estado);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Instalaciones", new { idEspacio });
    }

    public IActionResult Penalidades()
    {
        ViewData["Title"] = "Gestión de Penalidades";
        ViewData["Active"] = "penalidades";
        var penalidades = _penalidadBC.ObtenerTodas();
        var usuarios = _usuarioBC.ObtenerTodos();
        ViewBag.Usuarios = usuarios.ToDictionary(u => u.Id, u => $"{u.Nombre} {u.Apellido}");
        return View(penalidades);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult LevantarPenalidad(int idUsuario)
    {
        try
        {
            var idAdmin = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            _penalidadBC.Levantar(idUsuario, idAdmin);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Penalidades");
    }

    public IActionResult Usuarios()
    {
        ViewData["Title"] = "Gestión de Usuarios";
        ViewData["Active"] = "usuarios";
        var usuarios = _usuarioBC.ObtenerTodos();
        return View(usuarios);
    }
}