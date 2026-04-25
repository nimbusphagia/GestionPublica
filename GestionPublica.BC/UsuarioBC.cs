using System.Security.Claims;
using GestionPublica.BE;
using GestionPublica.DALC;

namespace GestionPublica.BC;

public class UsuarioBC
{
    private readonly UsuarioDALC _usuarioDALC = new UsuarioDALC();

    public void Registrar(UsuarioBE usuario)
    {
        if (_usuarioDALC.ObtenerPorCorreo(usuario.Correo) != null)
            throw new Exception("Ya existe una cuenta registrada con ese correo.");

        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash);
        usuario.Estado = "activo";
        usuario.Rol = "ciudadano";
        _usuarioDALC.Insertar(usuario);
    }

    public UsuarioBE Login(string correo, string password)
    {
        var usuario = _usuarioDALC.ObtenerPorCorreo(correo)
                      ?? throw new Exception("Correo o contraseña incorrectos.");

        if (!BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
            throw new Exception("Correo o contraseña incorrectos.");

        if (usuario.Estado == "bloqueado")
            throw new Exception("Tu cuenta está bloqueada por incidencias registradas.");

        return usuario;
    }

    public UsuarioBE ObtenerPorId(int id)
    {
        return _usuarioDALC.ObtenerPorId(id)
               ?? throw new Exception("Usuario no encontrado.");
    }

    public List<UsuarioBE> ObtenerTodos()
    {
        return _usuarioDALC.ObtenerTodos();
    }

    public void ActualizarEstado(int id, string estado)
    {
        var usuario = _usuarioDALC.ObtenerPorId(id)
                      ?? throw new Exception("Usuario no encontrado.");
        _usuarioDALC.ActualizarEstado(id, estado);
    }
}