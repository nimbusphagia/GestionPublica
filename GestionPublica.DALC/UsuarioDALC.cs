namespace GestionPublica.DALC;

using GestionPublica.BE;
using Microsoft.Data.SqlClient;

public class UsuarioDALC
{
    public void Insertar(UsuarioBE usuario)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand(@"
                INSERT INTO Usuario (Nombre, Apellido, DNI, Correo, PasswordHash, Telefono, Estado, Rol)
                VALUES (@Nombre, @Apellido, @DNI, @Correo, @PasswordHash, @Telefono, @Estado, @Rol)", con);

        cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
        cmd.Parameters.AddWithValue("@Apellido", usuario.Apellido);
        cmd.Parameters.AddWithValue("@DNI", usuario.DNI);
        cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
        cmd.Parameters.AddWithValue("@PasswordHash", usuario.PasswordHash);
        cmd.Parameters.AddWithValue("@Telefono", usuario.Telefono);
        cmd.Parameters.AddWithValue("@Estado", usuario.Estado);
        cmd.Parameters.AddWithValue("@Rol", usuario.Rol);
        cmd.ExecuteNonQuery();
    }

    public UsuarioBE ObtenerPorId(int id)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("SELECT * FROM Usuario WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Id", id);

        using var reader = cmd.ExecuteReader();
        if (reader.Read()) return MapearUsuario(reader);
        return null;
    }

    public UsuarioBE ObtenerPorCorreo(string correo)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("SELECT * FROM Usuario WHERE Correo = @Correo", con);
        cmd.Parameters.AddWithValue("@Correo", correo);

        using var reader = cmd.ExecuteReader();
        if (reader.Read()) return MapearUsuario(reader);
        return null;
    }

    public List<UsuarioBE> ObtenerTodos()
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("SELECT * FROM Usuario", con);
        var lista = new List<UsuarioBE>();

        using var reader = cmd.ExecuteReader();
        while (reader.Read()) lista.Add(MapearUsuario(reader));
        return lista;
    }

    public void ActualizarEstado(int id, string estado)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("UPDATE Usuario SET Estado = @Estado WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Estado", estado);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }

    private UsuarioBE MapearUsuario(SqlDataReader reader)
    {
        return new UsuarioBE
        {
            Id = (int)reader["Id"],
            Nombre = reader["Nombre"].ToString(),
            Apellido = reader["Apellido"].ToString(),
            DNI = reader["DNI"].ToString(),
            Correo = reader["Correo"].ToString(),
            PasswordHash = reader["PasswordHash"].ToString(),
            Telefono = reader["Telefono"].ToString(),
            FechaRegistro = (DateTime)reader["FechaRegistro"],
            Estado = reader["Estado"].ToString(),
            Rol = reader["Rol"].ToString()
        };
    }
}