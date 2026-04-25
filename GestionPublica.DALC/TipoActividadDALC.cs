using GestionPublica.BE;
using Microsoft.Data.SqlClient;

namespace GestionPublica.DALC;

public class TipoActividadDALC
{
    public List<TipoActividadBE> ObtenerTodos()
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("SELECT * FROM TipoActividad", con);
        var lista = new List<TipoActividadBE>();

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            lista.Add(new TipoActividadBE { Id = (int)reader["Id"], Nombre = reader["Nombre"].ToString() });
        return lista;
    } 
}