namespace GestionPublica.GUI.DTO;

public class ReservaEstadoDTO
{
    public int Id { get; set; }
    public string Estado { get; set; }
    public string FechaUso { get; set; }
    public string HoraInicio { get; set; }
    public string HoraFin { get; set; }
    public string FechaRespuesta { get; set; }
    public string MotivoRechazo { get; set; }
}