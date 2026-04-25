using GestionPublica.BE;
using GestionPublica.DALC;

namespace GestionPublica.BC;

public class EspacioBC
{
    private readonly EspacioDALC _espacioDALC = new EspacioDALC();

    public void Registrar(EspacioBE espacio)
    {
        if (espacio.HoraCierre <= espacio.HoraApertura)
            throw new Exception("La hora de cierre debe ser mayor a la hora de apertura.");

        espacio.Estado = "activo";
        _espacioDALC.Insertar(espacio);
    }

    public EspacioBE ObtenerPorId(int id)
    {
        return _espacioDALC.ObtenerPorId(id)
               ?? throw new Exception("Espacio no encontrado.");
    }

    public List<EspacioBE> ObtenerTodos()
    {
        return _espacioDALC.ObtenerTodos();
    }

    public List<EspacioBE> ObtenerActivos()
    {
        return _espacioDALC.ObtenerActivos();
    }

    public void ActualizarEstado(int id, string estado)
    {
        var espacio = _espacioDALC.ObtenerPorId(id)
            ?? throw new Exception("Espacio no encontrado.");
        _espacioDALC.ActualizarEstado(id, estado);
    }
}