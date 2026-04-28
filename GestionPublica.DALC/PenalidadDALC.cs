using GestionPublica.BE;
using Microsoft.Data.SqlClient;

namespace GestionPublica.DALC;

public class PenalidadDALC
{
    public void Insertar(PenalidadBE penalidad)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand(@"
                INSERT INTO Penalidad (IdUsuario, IdIncidencia, FechaFin, Motivo, Estado)
                VALUES (@IdUsuario, @IdIncidencia, @FechaFin, @Motivo, 'activa')", con);

        cmd.Parameters.AddWithValue("@IdUsuario", penalidad.IdUsuario);
        cmd.Parameters.AddWithValue("@IdIncidencia", penalidad.IdIncidencia);
        cmd.Parameters.AddWithValue("@FechaFin", penalidad.FechaFin);
        cmd.Parameters.AddWithValue("@Motivo", penalidad.Motivo);
        cmd.ExecuteNonQuery();
    }

    public PenalidadBE ObtenerActivaPorUsuario(int idUsuario)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand(@"
                SELECT * FROM Penalidad 
                WHERE IdUsuario = @IdUsuario 
                AND Estado = 'activa'", con);
        cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

        using var reader = cmd.ExecuteReader();
        return reader.Read() ? MapearPenalidad(reader) : null;
    }

    public List<PenalidadBE> ObtenerTodas()
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("SELECT * FROM Penalidad ORDER BY FechaInicio DESC", con);
        var lista = new List<PenalidadBE>();

        using var reader = cmd.ExecuteReader();
        while (reader.Read()) lista.Add(MapearPenalidad(reader));
        return lista;
    }

    public void Levantar(int id, int idAdmin)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand(@"
                UPDATE Penalidad 
                SET Estado = 'levantada',
                    IdAdminQueLevanta = @IdAdmin,
                    FechaLevantamiento = GETDATE()
                WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@IdAdmin", idAdmin);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }

    private PenalidadBE MapearPenalidad(SqlDataReader reader)
    {
        return new PenalidadBE
        {
            Id = (int)reader["Id"],
            IdUsuario = (int)reader["IdUsuario"],
            IdIncidencia = (int)reader["IdIncidencia"],
            FechaInicio = (DateTime)reader["FechaInicio"],
            FechaFin = (DateTime)reader["FechaFin"],
            Motivo = reader["Motivo"].ToString(),
            Estado = reader["Estado"].ToString(),
            IdAdminQueLevanta = reader["IdAdminQueLevanta"] == DBNull.Value ? null : (int?)reader["IdAdminQueLevanta"],
            FechaLevantamiento = reader["FechaLevantamiento"] == DBNull.Value
                ? null
                : (DateTime?)reader["FechaLevantamiento"]
        };
    }
}