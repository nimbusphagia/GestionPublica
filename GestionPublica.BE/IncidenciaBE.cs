namespace GestionPublica.BE;

public class IncidenciaBE
{
    public int IdIncidencia { get; set; }
    public int IdReserva { get; set; }
    public int IdAdministrador { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string Tipo { get; set; }        // dano fisico | desorden | uso indebido
    public string Descripcion { get; set; }
    public string Estado { get; set; }      // abierta | resuelta
}

