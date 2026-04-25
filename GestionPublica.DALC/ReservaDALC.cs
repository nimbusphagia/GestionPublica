using System.Data;
using GestionPublica.BE;
using Microsoft.Data.SqlClient;

namespace GestionPublica.DALC;

public class ReservaDALC
{
    public void Insertar(ReservaBE reserva)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand(@"
                INSERT INTO Reserva (IdInstalacion, IdUsuario, IdTipoActividad, FechaUso, HoraInicio, HoraFin, DescActividad, Estado)
                VALUES (@IdInstalacion, @IdUsuario, @IdTipoActividad, @FechaUso, @HoraInicio, @HoraFin, @DescActividad, 'pendiente')",
            con);

        cmd.Parameters.AddWithValue("@IdInstalacion", reserva.IdInstalacion);
        cmd.Parameters.AddWithValue("@IdUsuario", reserva.IdUsuario);
        cmd.Parameters.AddWithValue("@IdTipoActividad", reserva.IdTipoActividad);
        cmd.Parameters.AddWithValue("@FechaUso", reserva.FechaUso);
        cmd.Parameters.AddWithValue("@HoraInicio", reserva.HoraInicio);
        cmd.Parameters.AddWithValue("@HoraFin", reserva.HoraFin);
        cmd.Parameters.AddWithValue("@DescActividad", reserva.DescActividad);
        cmd.ExecuteNonQuery();
    }

    public ReservaBE ObtenerPorId(int id)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("SELECT * FROM Reserva WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Id", id);

        using var reader = cmd.ExecuteReader();
        if (reader.Read()) return MapearReserva(reader);
        return null;
    }

    public List<ReservaBE> ObtenerPorUsuario(int idUsuario)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("SELECT * FROM Reserva WHERE IdUsuario = @IdUsuario ORDER BY FechaSolicitud DESC",
            con);
        cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
        var lista = new List<ReservaBE>();

        using var reader = cmd.ExecuteReader();
        while (reader.Read()) lista.Add(MapearReserva(reader));
        return lista;
    }

    public List<ReservaBE> ObtenerPendientes()
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("SELECT * FROM Reserva WHERE Estado = 'pendiente' ORDER BY FechaSolicitud ASC", con);
        var lista = new List<ReservaBE>();

        using var reader = cmd.ExecuteReader();
        while (reader.Read()) lista.Add(MapearReserva(reader));
        return lista;
    }

    public List<ReservaBE> ObtenerFinalizadasSinIncidencia()
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand(@"
                SELECT * FROM Reserva 
                WHERE Estado = 'finalizada'
                AND Id NOT IN (SELECT IdReserva FROM Incidencia)", con);
        var lista = new List<ReservaBE>();

        using var reader = cmd.ExecuteReader();
        while (reader.Read()) lista.Add(MapearReserva(reader));
        return lista;
    }


    public bool ExisteConflicto(int idInstalacion, DateTime fechaUso, TimeSpan horaInicio, TimeSpan horaFin)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("sp_ExisteConflictoReserva", con);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@IdInstalacion", idInstalacion);
        cmd.Parameters.AddWithValue("@FechaUso", fechaUso.Date);
        cmd.Parameters.AddWithValue("@HoraInicio", horaInicio);
        cmd.Parameters.AddWithValue("@HoraFin", horaFin);

        return (int)cmd.ExecuteScalar() > 0;
    }


    public void ActualizarEstado(int id, string estado, string motivoRechazo, int idAdmin)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand(@"
                UPDATE Reserva 
                SET Estado = @Estado,
                    MotivoRechazo = @MotivoRechazo,
                    FechaRespuesta = GETDATE(),
                    IdAdminResponde = @IdAdmin
                WHERE Id = @Id", con);

        cmd.Parameters.AddWithValue("@Estado", estado);
        cmd.Parameters.AddWithValue("@MotivoRechazo", (object)motivoRechazo ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@IdAdmin", idAdmin);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }

    public void Cancelar(int id)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("UPDATE Reserva SET Estado = 'cancelada' WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }

    public void Finalizar(int id)
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("UPDATE Reserva SET Estado = 'finalizada' WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }

    private ReservaBE MapearReserva(SqlDataReader reader)
    {
        return new ReservaBE
        {
            Id = (int)reader["Id"],
            IdInstalacion = (int)reader["IdInstalacion"],
            IdUsuario = (int)reader["IdUsuario"],
            IdTipoActividad = (int)reader["IdTipoActividad"],
            FechaSolicitud = (DateTime)reader["FechaSolicitud"],
            FechaUso = (DateTime)reader["FechaUso"],
            HoraInicio = (TimeSpan)reader["HoraInicio"],
            HoraFin = (TimeSpan)reader["HoraFin"],
            DescActividad = reader["DescActividad"].ToString(),
            Estado = reader["Estado"].ToString(),
            MotivoRechazo = reader["MotivoRechazo"] == DBNull.Value ? null : reader["MotivoRechazo"].ToString(),
            FechaRespuesta = reader["FechaRespuesta"] == DBNull.Value ? null : (DateTime?)reader["FechaRespuesta"],
            IdAdminResponde = reader["IdAdminResponde"] == DBNull.Value ? null : (int?)reader["IdAdminResponde"]
        };
    }
}