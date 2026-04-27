namespace GestionPublica.GUI.DTO;

public class DisponibilidadDTO
{
    public string Fecha { get; set; }
    public int Total { get; set; }
    public List<EspacioDisponibleDTO> Instalaciones { get; set; }
}