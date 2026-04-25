using GestionPublica.BE;
using GestionPublica.DALC;

namespace GestionPublica.BC;

public class ReservaBC
{
    private readonly ReservaDALC _reservaDALC = new ReservaDALC();
    private readonly InstalacionDALC _instalacionDALC = new InstalacionDALC();
    private readonly UsuarioDALC _usuarioDALC = new UsuarioDALC();

    public void Solicitar(ReservaBE reserva)
    {
        var usuario = _usuarioDALC.ObtenerPorId(reserva.IdUsuario)
                      ?? throw new Exception("Usuario no encontrado.");

        if (usuario.Estado == "bloqueado")
            throw new Exception("Tu cuenta está bloqueada y no puedes realizar reservas.");

        if (reserva.FechaUso < DateTime.Today.AddDays(1))
            throw new Exception("La reserva debe realizarse con al menos 24 horas de anticipación.");

        var instalacion = _instalacionDALC.ObtenerPorId(reserva.IdInstalacion)
                          ?? throw new Exception("Instalación no encontrada.");

        if (instalacion.Estado != "disponible")
            throw new Exception("La instalación no está disponible.");

        if (_reservaDALC.ExisteConflicto(reserva.IdInstalacion, reserva.FechaUso, reserva.HoraInicio, reserva.HoraFin))
            throw new Exception("La instalación ya tiene una reserva aprobada en ese horario.");

        _reservaDALC.Insertar(reserva);
    }

    public ReservaBE ObtenerPorId(int id)
    {
        return _reservaDALC.ObtenerPorId(id)
               ?? throw new Exception("Reserva no encontrada.");
    }

    public List<ReservaBE> ObtenerPorUsuario(int idUsuario)
    {
        return _reservaDALC.ObtenerPorUsuario(idUsuario);
    }

    public List<ReservaBE> ObtenerPendientes()
    {
        return _reservaDALC.ObtenerPendientes();
    }

    public List<ReservaBE> ObtenerFinalizadasSinIncidencia()
    {
        return _reservaDALC.ObtenerFinalizadasSinIncidencia();
    }

    public void Aprobar(int id, int idAdmin)
    {
        var reserva = _reservaDALC.ObtenerPorId(id)
                      ?? throw new Exception("Reserva no encontrada.");

        if (reserva.Estado != "pendiente")
            throw new Exception("Solo se pueden aprobar reservas en estado pendiente.");

        if (_reservaDALC.ExisteConflicto(reserva.IdInstalacion, reserva.FechaUso, reserva.HoraInicio, reserva.HoraFin))
            throw new Exception("Existe un conflicto de horario con otra reserva ya aprobada.");

        _reservaDALC.ActualizarEstado(id, "aprobada", null, idAdmin);
    }

    public void Rechazar(int id, string motivo, int idAdmin)
    {
        var reserva = _reservaDALC.ObtenerPorId(id)
                      ?? throw new Exception("Reserva no encontrada.");

        if (reserva.Estado != "pendiente")
            throw new Exception("Solo se pueden rechazar reservas en estado pendiente.");

        if (string.IsNullOrWhiteSpace(motivo))
            throw new Exception("Debe indicar un motivo de rechazo.");

        _reservaDALC.ActualizarEstado(id, "rechazada", motivo, idAdmin);
    }

    public void Cancelar(int id, int idUsuario)
    {
        var reserva = _reservaDALC.ObtenerPorId(id)
                      ?? throw new Exception("Reserva no encontrada.");

        if (reserva.IdUsuario != idUsuario)
            throw new Exception("No tienes permiso para cancelar esta reserva.");

        if (reserva.Estado != "aprobada" && reserva.Estado != "pendiente")
            throw new Exception("Solo se pueden cancelar reservas pendientes o aprobadas.");

        if (reserva.FechaUso <= DateTime.Today)
            throw new Exception("No se puede cancelar una reserva con menos de 24 horas de anticipación.");

        _reservaDALC.Cancelar(id);
    }

    public void Finalizar(int id)
    {
        var reserva = _reservaDALC.ObtenerPorId(id)
                      ?? throw new Exception("Reserva no encontrada.");

        if (reserva.Estado != "aprobada")
            throw new Exception("Solo se pueden finalizar reservas aprobadas.");

        _reservaDALC.Finalizar(id);
    }
}