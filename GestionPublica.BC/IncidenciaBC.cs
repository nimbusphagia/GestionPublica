using GestionPublica.BE;
using GestionPublica.DALC;

namespace GestionPublica.BC;

public class IncidenciaBC
{
    private readonly IncidenciaDALC _incidenciaDALC = new IncidenciaDALC();
    private readonly ReservaDALC _reservaDALC = new ReservaDALC();
    private readonly PenalidadDALC _penalidadDALC = new PenalidadDALC();
    private readonly UsuarioDALC _usuarioDALC = new UsuarioDALC();

    public void Registrar(IncidenciaBE incidencia)
    {
        var reserva = _reservaDALC.ObtenerPorId(incidencia.IdReserva)
                      ?? throw new Exception("Reserva no encontrada.");

        if (reserva.Estado != "finalizada")
            throw new Exception("Solo se pueden registrar incidencias en reservas finalizadas.");

        int idIncidencia = _incidenciaDALC.Insertar(incidencia);

        int totalIncidencias = _incidenciaDALC.ContarIncidenciasPorUsuario(reserva.IdUsuario);

        if (totalIncidencias >= 2)
        {
            var penalidad = new PenalidadBE
            {
                IdUsuario = reserva.IdUsuario,
                IdIncidencia = idIncidencia,
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddDays(15),
                Motivo = $"Bloqueo automático por acumular {totalIncidencias} incidencias.",
                Estado = "activa"
            };

            _penalidadDALC.Insertar(penalidad);
            _usuarioDALC.ActualizarEstado(reserva.IdUsuario, "bloqueado");
        }
    }

    public List<IncidenciaBE> ObtenerPorUsuario(int idUsuario)
    {
        return _incidenciaDALC.ObtenerPorUsuario(idUsuario);
    }

    public void Resolver(int id)
    {
        var incidencia = _incidenciaDALC.ObtenerPorId(id)
                         ?? throw new Exception("Incidencia no encontrada.");
        _incidenciaDALC.ActualizarEstado(id, "resuelta");
    }
}