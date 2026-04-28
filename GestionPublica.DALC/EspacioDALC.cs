using GestionPublica.BE;
using Microsoft.Data.SqlClient;

namespace GestionPublica.DALC;

public class EspacioDALC
{
    public void Insertar(EspacioBE espacio)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand(@"
                INSERT INTO Espacio (Nombre, Direccion, Distrito, Descripcion, HoraApertura, HoraCierre, Estado)
                VALUES (@Nombre, @Direccion, @Distrito, @Descripcion, @HoraApertura, @HoraCierre, @Estado)", con);

        cmd.Parameters.AddWithValue("@Nombre", espacio.Nombre);
        cmd.Parameters.AddWithValue("@Direccion", espacio.Direccion);
        cmd.Parameters.AddWithValue("@Distrito", espacio.Distrito);
        cmd.Parameters.AddWithValue("@Descripcion", (object)espacio.Descripcion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@HoraApertura", espacio.HoraApertura);
        cmd.Parameters.AddWithValue("@HoraCierre", espacio.HoraCierre);
        cmd.Parameters.AddWithValue("@Estado", espacio.Estado);
        cmd.ExecuteNonQuery();
    }

    public EspacioBE ObtenerPorId(int id)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("SELECT * FROM Espacio WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Id", id);

        using var reader = cmd.ExecuteReader();
        return reader.Read() ? MapearEspacio(reader) : null;
    }

    public List<EspacioBE> ObtenerTodos()
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("SELECT * FROM Espacio", con);
        var lista = new List<EspacioBE>();

        using var reader = cmd.ExecuteReader();
        while (reader.Read()) lista.Add(MapearEspacio(reader));
        return lista;
    }

    public List<EspacioBE> ObtenerActivos()
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("SELECT * FROM Espacio WHERE Estado = 'activo'", con);
        var lista = new List<EspacioBE>();

        using var reader = cmd.ExecuteReader();
        while (reader.Read()) lista.Add(MapearEspacio(reader));
        return lista;
    }

    public void ActualizarEstado(int id, string estado)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("UPDATE Espacio SET Estado = @Estado WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Estado", estado);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }
    public void Actualizar(EspacioBE espacio)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand(@"
        UPDATE Espacio 
        SET Nombre = @Nombre,
            Direccion = @Direccion,
            Distrito = @Distrito,
            Descripcion = @Descripcion,
            HoraApertura = @HoraApertura,
            HoraCierre = @HoraCierre
        WHERE Id = @Id", con);

        cmd.Parameters.AddWithValue("@Nombre", espacio.Nombre);
        cmd.Parameters.AddWithValue("@Direccion", espacio.Direccion);
        cmd.Parameters.AddWithValue("@Distrito", espacio.Distrito);
        cmd.Parameters.AddWithValue("@Descripcion", (object)espacio.Descripcion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@HoraApertura", espacio.HoraApertura);
        cmd.Parameters.AddWithValue("@HoraCierre", espacio.HoraCierre);
        cmd.Parameters.AddWithValue("@Id", espacio.Id);
        cmd.ExecuteNonQuery();
    }
    private EspacioBE MapearEspacio(SqlDataReader reader)
    {
        return new EspacioBE
        {
            Id = (int)reader["Id"],
            Nombre = reader["Nombre"].ToString(),
            Direccion = reader["Direccion"].ToString(),
            Distrito = reader["Distrito"].ToString(),
            Descripcion = reader["Descripcion"] == DBNull.Value ? null : reader["Descripcion"].ToString(),
            HoraApertura = (TimeSpan)reader["HoraApertura"],
            HoraCierre = (TimeSpan)reader["HoraCierre"],
            Estado = reader["Estado"].ToString()
        };
    }
}