using GestionPublica.BE;
using GestionPublica.DALC;

namespace GestionPublica.BC;

public class InstalacionBC
{
    private readonly InstalacionDALC _instalacionDALC = new InstalacionDALC();
    private readonly EspacioDALC _espacioDALC = new EspacioDALC();

    public void Registrar(InstalacionBE instalacion)
    {
        var espacio = _espacioDALC.ObtenerPorId(instalacion.IdEspacio)
                      ?? throw new Exception("El espacio indicado no existe.");

        if (espacio.Estado != "activo")
            throw new Exception("No se pueden agregar instalaciones a un espacio inactivo.");

        instalacion.Estado = "disponible";
        _instalacionDALC.Insertar(instalacion);
    }

    public InstalacionBE ObtenerPorId(int id)
    {
        return _instalacionDALC.ObtenerPorId(id)
               ?? throw new Exception("Instalación no encontrada.");
    }

    public List<InstalacionBE> ObtenerPorEspacio(int idEspacio)
    {
        return _instalacionDALC.ObtenerPorEspacio(idEspacio);
    }

    public List<InstalacionBE> ObtenerDisponiblesPorFecha(DateTime fecha)
    {
        if (fecha.Date < DateTime.Today)
            throw new Exception("La fecha de consulta no puede ser en el pasado.");

        return _instalacionDALC.ObtenerDisponiblesPorFecha(fecha);
    }

    public void ActualizarEstado(int id, string estado)
    {
        var instalacion = _instalacionDALC.ObtenerPorId(id)
                          ?? throw new Exception("Instalación no encontrada.");
        _instalacionDALC.ActualizarEstado(id, estado);
    }
}