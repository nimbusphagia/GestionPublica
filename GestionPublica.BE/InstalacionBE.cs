namespace GestionPublica.BE;

public class InstalacionBE
{
    public int IdInstalacion { get; set; }
    public int IdEspacio { get; set; }
    public string Nombre { get; set; }
    public string Tipo { get; set; }        // cancha deportiva | zona de parrillas | auditorio | salon comunal | otro
    public int Capacidad { get; set; }
    public string Descripcion { get; set; }
    public string Estado { get; set; }      // disponible | mantenimiento
}
