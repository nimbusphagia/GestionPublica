namespace GestionPublica.GUI.Controllers;

using GestionPublica.BC;
using GestionPublica.DALC;
using GestionPublica.GUI.DTO;
using Microsoft.AspNetCore.Mvc;

[Route("api")]
[ApiController]
public class ApiController : ControllerBase
{
    private readonly InstalacionBC _instalacionBC = new InstalacionBC();
    private readonly EspacioBC _espacioBC = new EspacioBC();
    private readonly ReservaBC _reservaBC = new ReservaBC();
    private readonly TipoInstalacionDALC _tipoInstalacionDALC = new TipoInstalacionDALC();

    [HttpGet("espacios/disponibilidad")]
    public IActionResult ObtenerDisponibilidad([FromQuery] string fecha)
    {
        if (string.IsNullOrWhiteSpace(fecha))
            return BadRequest("Debe proporcionar una fecha.");

        if (!DateTime.TryParse(fecha, out var fechaParsed))
            return BadRequest("Formato de fecha inválido. Use yyyy-MM-dd.");

        try
        {
            var instalaciones = _instalacionBC.ObtenerDisponiblesPorFecha(fechaParsed);
            var espacios = _espacioBC.ObtenerActivos()
                .ToDictionary(e => e.Id, e => e);
            var tipos = _tipoInstalacionDALC.ObtenerTodos()
                .ToDictionary(t => t.Id, t => t.Nombre);

            var resultado = new DisponibilidadDTO
            {
                Fecha = fechaParsed.ToString("yyyy-MM-dd"),
                Total = instalaciones.Count,
                Instalaciones = instalaciones.Select(i =>
                {
                    var espacio = espacios[i.IdEspacio];
                    return new EspacioDisponibleDTO
                    {
                        Id = i.Id,
                        Nombre = i.Nombre,
                        Tipo = tipos[i.IdTipoInstalacion],
                        Capacidad = i.Capacidad,
                        Descripcion = i.Descripcion,
                        EspacioNombre = espacio.Nombre,
                        EspacioDistrito = espacio.Distrito,
                        EspacioDireccion = espacio.Direccion,
                        HoraApertura = espacio.HoraApertura.ToString(@"hh\:mm"),
                        HoraCierre = espacio.HoraCierre.ToString(@"hh\:mm")
                    };
                }).ToList()
            };

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("reservas/estado/{id}")]
    public IActionResult ObtenerEstadoReserva(int id)
    {
        try
        {
            var reserva = _reservaBC.ObtenerPorId(id);

            var resultado = new ReservaEstadoDTO
            {
                Id = reserva.Id,
                Estado = reserva.Estado,
                FechaUso = reserva.FechaUso.ToString("yyyy-MM-dd"),
                HoraInicio = reserva.HoraInicio.ToString(@"hh\:mm"),
                HoraFin = reserva.HoraFin.ToString(@"hh\:mm"),
                FechaRespuesta = reserva.FechaRespuesta?.ToString("yyyy-MM-dd HH:mm"),
                MotivoRechazo = reserva.MotivoRechazo
            };

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}