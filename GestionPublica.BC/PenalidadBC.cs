using GestionPublica.BE;
using GestionPublica.DALC;

namespace GestionPublica.BC;

public class PenalidadBC
{
    private readonly PenalidadDALC _penalidadDALC = new PenalidadDALC();
    private readonly UsuarioDALC _usuarioDALC = new UsuarioDALC();

    public PenalidadBE ObtenerActivaPorUsuario(int idUsuario)
    {
        return _penalidadDALC.ObtenerActivaPorUsuario(idUsuario);
    }

    public List<PenalidadBE> ObtenerTodas()
    {
        return _penalidadDALC.ObtenerTodas();
    }

    public void Levantar(int id, int idAdmin)
    {
        var penalidad = _penalidadDALC.ObtenerActivaPorUsuario(id)
                        ?? throw new Exception("No se encontró una penalidad activa para este usuario.");

        _penalidadDALC.Levantar(penalidad.Id, idAdmin);
        _usuarioDALC.ActualizarEstado(penalidad.IdUsuario, "activo");
    }
}