using GestionPublica.BE;
using Microsoft.Data.SqlClient;

namespace GestionPublica.DALC;

public class TipoInstalacionDALC
{
    public List<TipoInstalacionBE> ObtenerTodos()
    {
        using var con = Connection.GetConnection();
        var cmd = new SqlCommand("SELECT * FROM TipoInstalacion", con);
        var lista = new List<TipoInstalacionBE>();

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            lista.Add(new TipoInstalacionBE { Id = (int)reader["Id"], Nombre = reader["Nombre"].ToString() });
        return lista;
    } 
}