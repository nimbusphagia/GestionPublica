namespace GestionPublica.BE;

public class EspacioBE
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Direccion { get; set; }
    public string Distrito { get; set; }
    public string Descripcion { get; set; }
    public TimeSpan HoraApertura { get; set; }
    public TimeSpan HoraCierre { get; set; }
    public string Estado { get; set; }    
}

