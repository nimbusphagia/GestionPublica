using System.Data;
using GestionPublica.BE;
using Microsoft.Data.SqlClient;

namespace GestionPublica.DALC;

public class IncidenciaDALC
{
    public void Insertar(IncidenciaBE incidencia)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand(@"
                INSERT INTO Incidencia (IdReserva, IdAdministrador, Tipo, Descripcion, Estado)
                VALUES (@IdReserva, @IdAdministrador, @Tipo, @Descripcion, 'abierta')", con);

        cmd.Parameters.AddWithValue("@IdReserva", incidencia.IdReserva);
        cmd.Parameters.AddWithValue("@IdAdministrador", incidencia.IdAdministrador);
        cmd.Parameters.AddWithValue("@Tipo", incidencia.Tipo);
        cmd.Parameters.AddWithValue("@Descripcion", incidencia.Descripcion);
        cmd.ExecuteNonQuery();
    }

    public IncidenciaBE ObtenerPorId(int id)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("SELECT * FROM Incidencia WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Id", id);

        using var reader = cmd.ExecuteReader();
        if (reader.Read()) return MapearIncidencia(reader);
        return null;
    }

    public List<IncidenciaBE> ObtenerPorUsuario(int idUsuario)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("sp_ObtenerIncidenciasPorUsuario", con);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

        var lista = new List<IncidenciaBE>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read()) lista.Add(MapearIncidencia(reader));
        return lista;
    }


    public int ContarIncidenciasPorUsuario(int idUsuario)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("sp_ContarIncidenciasPorUsuario", con);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
        return (int)cmd.ExecuteScalar();
    }


    public void ActualizarEstado(int id, string estado)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("UPDATE Incidencia SET Estado = @Estado WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Estado", estado);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }

    private IncidenciaBE MapearIncidencia(SqlDataReader reader)
    {
        return new IncidenciaBE
        {
            Id = (int)reader["Id"],
            IdReserva = (int)reader["IdReserva"],
            IdAdministrador = (int)reader["IdAdministrador"],
            FechaRegistro = (DateTime)reader["FechaRegistro"],
            Tipo = reader["Tipo"].ToString(),
            Descripcion = reader["Descripcion"].ToString(),
            Estado = reader["Estado"].ToString()
        };
    }
}