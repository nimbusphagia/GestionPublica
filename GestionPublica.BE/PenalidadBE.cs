namespace GestionPublica.BE;

public class PenalidadBE
{
    public int IdPenalidad { get; set; }
    public int IdUsuario { get; set; }
    public int IdIncidencia { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }          // FechaInicio + 15 días
    public string Motivo { get; set; }
    public string Estado { get; set; }               // activa | levantada
    public int? IdAdminQueLevanta { get; set; }      // null mientras esté activa
    public DateTime? FechaLevantamiento { get; set; } // null mientras esté activa
}

