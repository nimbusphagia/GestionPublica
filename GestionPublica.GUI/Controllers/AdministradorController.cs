namespace GestionPublica.GUI.Controllers;

using System.Security.Claims;
using GestionPublica.BC;
using GestionPublica.BE;
using GestionPublica.DALC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "administrador")]
public class AdministradorController : Controller
{
    private readonly ReservaBC _reservaBC = new ReservaBC();
    private readonly IncidenciaBC _incidenciaBC = new IncidenciaBC();
    private readonly UsuarioBC _usuarioBC = new UsuarioBC();
    private readonly TipoActividadDALC _tipoActividadDALC = new TipoActividadDALC();

    private int GetAdminId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    public IActionResult Index()
    {
        ViewData["Title"] = "Dashboard";
        ViewData["Active"] = "dashboard";
        ViewBag.TotalPendientes = _reservaBC.ObtenerPendientes().Count;
        ViewBag.TotalAprobadas = _reservaBC.ObtenerAprobadas().Count;
        ViewBag.FinalizadasSinIncidencia = _reservaBC.ObtenerFinalizadasSinIncidencia().Count;
        return View();
    }

    public IActionResult Solicitudes()
    {
        ViewData["Title"] = "Solicitudes pendientes";
        ViewData["Active"] = "solicitudes";
        var solicitudes = _reservaBC.ObtenerPendientes();
        var tipos = _tipoActividadDALC.ObtenerTodos();
        var usuarios = _usuarioBC.ObtenerTodos();
        ViewBag.Tipos = tipos.ToDictionary(t => t.Id, t => t.Nombre);
        ViewBag.Usuarios = usuarios.ToDictionary(u => u.Id, u => $"{u.Nombre} {u.Apellido}");
        return View(solicitudes);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Aprobar(int id)
    {
        try
        {
            _reservaBC.Aprobar(id, GetAdminId());
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Solicitudes");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Rechazar(int id, string motivo)
    {
        try
        {
            _reservaBC.Rechazar(id, motivo, GetAdminId());
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Solicitudes");
    }

    public IActionResult ReservasAprobadas()
    {
        ViewData["Title"] = "Reservas aprobadas";
        ViewData["Active"] = "reservas";
        var reservas = _reservaBC.ObtenerAprobadas();
        System.Diagnostics.Debug.WriteLine($"Reservas aprobadas: {reservas.Count}");
        var tipos = _tipoActividadDALC.ObtenerTodos();
        var usuarios = _usuarioBC.ObtenerTodos();
        ViewBag.Tipos = tipos.ToDictionary(t => t.Id, t => t.Nombre);
        ViewBag.Usuarios = usuarios.ToDictionary(u => u.Id, u => $"{u.Nombre} {u.Apellido}");
        return View(reservas);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Finalizar(int id)
    {
        try
        {
            _reservaBC.Finalizar(id);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("ReservasAprobadas");
    }

    public IActionResult Incidencias()
    {
        ViewData["Title"] = "Incidencias";
        ViewData["Active"] = "incidencias";
        var finalizadasSinIncidencia = _reservaBC.ObtenerFinalizadasSinIncidencia();
        var usuarios = _usuarioBC.ObtenerTodos();
        ViewBag.FinalizadasSinIncidencia = finalizadasSinIncidencia;
        ViewBag.Usuarios = usuarios.ToDictionary(u => u.Id, u => $"{u.Nombre} {u.Apellido}");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RegistrarIncidencia(int idReserva, string tipo, string descripcion)
    {
        try
        {
            var incidencia = new IncidenciaBE
            {
                IdReserva = idReserva,
                IdAdministrador = GetAdminId(),
                Tipo = tipo,
                Descripcion = descripcion,
                Estado = "abierta"
            };
            _incidenciaBC.Registrar(incidencia);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Incidencias");
    }
}