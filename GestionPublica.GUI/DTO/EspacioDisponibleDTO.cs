namespace GestionPublica.GUI.DTO;

public class EspacioDisponibleDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Tipo { get; set; }
    public int Capacidad { get; set; }
    public string Descripcion { get; set; }
    public string EspacioNombre { get; set; }
    public string EspacioDistrito { get; set; }
    public string EspacioDireccion { get; set; }
    public string HoraApertura { get; set; }
    public string HoraCierre { get; set; }
}