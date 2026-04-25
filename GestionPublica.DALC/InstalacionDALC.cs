using System.Data;
using GestionPublica.BE;
using Microsoft.Data.SqlClient;

namespace GestionPublica.DALC;

public class InstalacionDALC
{
    public void Insertar(InstalacionBE instalacion)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand(@"
                INSERT INTO Instalacion (IdEspacio, IdTipoInstalacion, Nombre, Capacidad, Descripcion, Estado)
                VALUES (@IdEspacio, @IdTipoInstalacion, @Nombre, @Capacidad, @Descripcion, @Estado)", con);

        cmd.Parameters.AddWithValue("@IdEspacio", instalacion.IdEspacio);
        cmd.Parameters.AddWithValue("@IdTipoInstalacion", instalacion.IdTipoInstalacion);
        cmd.Parameters.AddWithValue("@Nombre", instalacion.Nombre);
        cmd.Parameters.AddWithValue("@Capacidad", instalacion.Capacidad);
        cmd.Parameters.AddWithValue("@Descripcion", (object)instalacion.Descripcion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Estado", instalacion.Estado);
        cmd.ExecuteNonQuery();
    }

    public InstalacionBE ObtenerPorId(int id)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("SELECT * FROM Instalacion WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Id", id);

        using var reader = cmd.ExecuteReader();
        if (reader.Read()) return MapearInstalacion(reader);
        return null;
    }

    public List<InstalacionBE> ObtenerPorEspacio(int idEspacio)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("SELECT * FROM Instalacion WHERE IdEspacio = @IdEspacio", con);
        cmd.Parameters.AddWithValue("@IdEspacio", idEspacio);
        var lista = new List<InstalacionBE>();

        using var reader = cmd.ExecuteReader();
        while (reader.Read()) lista.Add(MapearInstalacion(reader));
        return lista;
    }

    public List<InstalacionBE> ObtenerDisponiblesPorFecha(DateTime fecha)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("sp_ObtenerInstalacionesDisponiblesPorFecha", con);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Fecha", fecha.Date);

        var lista = new List<InstalacionBE>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read()) lista.Add(MapearInstalacion(reader));
        return lista;
    }


    public void ActualizarEstado(int id, string estado)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("UPDATE Instalacion SET Estado = @Estado WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Estado", estado);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }

    private InstalacionBE MapearInstalacion(SqlDataReader reader)
    {
        return new InstalacionBE
        {
            Id = (int)reader["Id"],
            IdEspacio = (int)reader["IdEspacio"],
            IdTipoInstalacion = (int)reader["IdTipoInstalacion"],
            Nombre = reader["Nombre"].ToString(),
            Capacidad = (int)reader["Capacidad"],
            Descripcion = reader["Descripcion"] == DBNull.Value ? null : reader["Descripcion"].ToString(),
            Estado = reader["Estado"].ToString()
        };
    }
}