namespace GestionPublica.BE;

public class InstalacionBE
{
    public int Id { get; set; }
    public int IdEspacio { get; set; }
    public int IdTipoInstalacion { get; set; }        
    public string Nombre { get; set; }
    public int Capacidad { get; set; }
    public string Descripcion { get; set; }
    public string Estado { get; set; }      // [disponible, mantenimiento]
}
