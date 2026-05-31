using ElArteServicios.Models;
using ElArteServicios.Repositories;

namespace ElArteServicios.Services;

public class SedeService
{
    private readonly SedeRepository _repo;

    public SedeService(SedeRepository repo)
    {
        _repo = repo;
    }

    public void CrearSede(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la sede es obligatorio.");

        _repo.Add(new Sede { Nombre = nombre.Trim() });
    }

    public List<Sede> ObtenerSedes() => _repo.GetAll();

    public Sede? ObtenerSedePorId(int id) => _repo.GetById(id);

    public void ActualizarSede(int id, string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la sede es obligatorio.");

        var sede = _repo.GetById(id)
            ?? throw new InvalidOperationException("Sede no encontrada.");

        sede.Nombre = nombre.Trim();
        _repo.Update(sede);
    }

    public void EliminarSede(int id)
    {
        if (_repo.GetById(id) == null)
            throw new InvalidOperationException("Sede no encontrada.");

        if (_repo.TieneTurnos(id))
            throw new InvalidOperationException("No se puede eliminar la sede porque tiene turnos asociados.");

        if (_repo.TieneAsignaciones(id))
            throw new InvalidOperationException("No se puede eliminar la sede porque tiene asignaciones.");

        _repo.Delete(id);
    }
}
