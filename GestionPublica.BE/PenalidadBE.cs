namespace GestionPublica.BE;

public class PenalidadBE
{
    public int Id { get; set; }
    public int IdUsuario { get; set; }
    public int IdIncidencia { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }          
    public string Motivo { get; set; }
    public string Estado { get; set; }               // [activa, levantada]
    public int? IdAdminQueLevanta { get; set; }      
    public DateTime? FechaLevantamiento { get; set; } 
}

