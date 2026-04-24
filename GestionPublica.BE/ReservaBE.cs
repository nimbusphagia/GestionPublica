namespace GestionPublica.BE;

public class ReservaBE
{
    public int IdReserva { get; set; }
    public int IdInstalacion { get; set; }
    public int IdUsuario { get; set; }
    public DateTime FechaSolicitud { get; set; }
    public DateTime FechaUso { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }
    public string TipoActividad { get; set; }   // deportiva | cultural | social | educativa | otra
    public string DescActividad { get; set; }
    public string Estado { get; set; }           // pendiente | aprobada | rechazada | cancelada | finalizada
    public string MotivoRechazo { get; set; }    // null si no fue rechazada
    public DateTime? FechaRespuesta { get; set; }
    public int? IdAdminResponde { get; set; }    // null hasta que el admin responda
}

