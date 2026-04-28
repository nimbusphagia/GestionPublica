namespace GestionPublica.BE;
public class UsuarioBE
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string DNI { get; set; }
    public string Correo { get; set; }
    public string PasswordHash { get; set; }
    public string Telefono { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string Estado { get; set; }      
    public string Rol { get; set; }         
}

