namespace GestionPublica.GUI.Controllers;

using System.Security.Claims;
using GestionPublica.BC;
using GestionPublica.BE;
using GestionPublica.DALC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "ciudadano")]
public class CiudadanoController : Controller
{
    private readonly ReservaBC _reservaBC = new ReservaBC();
    private readonly InstalacionBC _instalacionBC = new InstalacionBC();
    private readonly EspacioBC _espacioBC = new EspacioBC();
    private readonly TipoActividadDALC _tipoActividadDALC = new TipoActividadDALC();
    private readonly TipoInstalacionDALC _tipoInstalacionDALC = new TipoInstalacionDALC();

    private int GetUsuarioId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    public IActionResult Index()
    {
        ViewData["Active"] = "dashboard";
        var reservas = _reservaBC.ObtenerPorUsuario(GetUsuarioId());
        ViewBag.TotalReservas = reservas.Count;
        ViewBag.Pendientes = reservas.Count(r => r.Estado == "pendiente");
        ViewBag.Aprobadas = reservas.Count(r => r.Estado == "aprobada");
        ViewBag.UltimaReserva = reservas.FirstOrDefault();
        return View();
    }

    public IActionResult BuscarInstalaciones(DateTime? fecha)
    {
        ViewData["Active"] = "buscar";
        ViewBag.Fecha = fecha?.ToString("yyyy-MM-dd") ?? DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
        ViewBag.Tipos = _tipoInstalacionDALC.ObtenerTodos();

        if (!fecha.HasValue) return View();
        try
        {
            var instalaciones = _instalacionBC.ObtenerDisponiblesPorFecha(fecha.Value);
            var espacios = _espacioBC.ObtenerActivos()
                .ToDictionary(e => e.Id, e => e);
            var tipos = _tipoInstalacionDALC.ObtenerTodos()
                .ToDictionary(t => t.Id, t => t.Nombre);
            ViewBag.Instalaciones = instalaciones;
            ViewBag.Espacios = espacios;
            ViewBag.TiposDict = tipos;
        }
        catch (Exception ex)
        {
            ViewBag.Error = ex.Message;
        }

        return View();
    }

    public IActionResult NuevaReserva(int idInstalacion, string fecha)
    {
        ViewData["Active"] = "buscar";

        try
        {
            var instalacion = _instalacionBC.ObtenerPorId(idInstalacion);
            var espacio = _espacioBC.ObtenerPorId(instalacion.IdEspacio);
            var tipos = _tipoActividadDALC.ObtenerTodos();
            ViewBag.Instalacion = instalacion;
            ViewBag.Espacio = espacio;
            ViewBag.Tipos = tipos;
            ViewBag.Fecha = fecha;
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("BuscarInstalaciones");
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CrearReserva(int idInstalacion, string fechaUso,
        string horaInicio, string horaFin, int idTipoActividad, string descActividad)
    {
        try
        {
            var reserva = new ReservaBE
            {
                IdInstalacion = idInstalacion,
                IdUsuario = GetUsuarioId(),
                IdTipoActividad = idTipoActividad,
                FechaUso = DateTime.Parse(fechaUso),
                HoraInicio = TimeSpan.Parse(horaInicio),
                HoraFin = TimeSpan.Parse(horaFin),
                DescActividad = descActividad
            };
            _reservaBC.Solicitar(reserva);
            TempData["Success"] = "Solicitud enviada correctamente. Espera la aprobación del administrador.";
            return RedirectToAction("MisReservas");
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("NuevaReserva", new { idInstalacion, fecha = fechaUso });
        }
    }

    public IActionResult MisReservas()
    {
        ViewData["Active"] = "reservas";
        var reservas = _reservaBC.ObtenerPorUsuario(GetUsuarioId());
        var tipos = _tipoActividadDALC.ObtenerTodos()
            .ToDictionary(t => t.Id, t => t.Nombre);
        ViewBag.Tipos = tipos;
        return View(reservas);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Cancelar(int id)
    {
        try
        {
            _reservaBC.Cancelar(id, GetUsuarioId());
            TempData["Success"] = "Reserva cancelada correctamente.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("MisReservas");
    }
}