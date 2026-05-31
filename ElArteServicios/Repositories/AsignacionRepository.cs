using ElArteServicios.Data;
using ElArteServicios.Models;
using Microsoft.EntityFrameworkCore;

namespace ElArteServicios.Repositories;

public class AsignacionRepository
{
    private readonly ServiciosContext _context;

    public AsignacionRepository(ServiciosContext context)
    {
        _context = context;
    }

    public List<Asignacion> GetAll() =>
        _context.Asignaciones
            .Include(a => a.IdEmpleadoNavigation)
            .Include(a => a.IdTurnoNavigation)
            .ThenInclude(t => t!.IdSedeNavigation)
            .Include(a => a.IdSedeNavigation)
            .OrderByDescending(a => a.IdTurnoNavigation.Fecha)
            .ThenBy(a => a.IdTurnoNavigation.HoraInicio)
            .ToList();

    public Asignacion? GetById(int id) =>
        _context.Asignaciones
            .Include(a => a.IdEmpleadoNavigation)
            .Include(a => a.IdTurnoNavigation)
            .Include(a => a.IdSedeNavigation)
            .FirstOrDefault(a => a.IdAsignacion == id);

    public List<Asignacion> GetByEmpleado(int empleadoId, int? excluirAsignacionId = null)
    {
        var query = _context.Asignaciones
            .Include(a => a.IdTurnoNavigation)
            .Where(a => a.IdEmpleado == empleadoId);

        if (excluirAsignacionId.HasValue)
            query = query.Where(a => a.IdAsignacion != excluirAsignacionId.Value);

        return query.ToList();
    }

    public Asignacion? GetAsignacionEnTurno(int turnoId, int? excluirAsignacionId = null)
    {
        var query = _context.Asignaciones
            .Include(a => a.IdEmpleadoNavigation)
            .Where(a => a.IdTurno == turnoId);

        if (excluirAsignacionId.HasValue)
            query = query.Where(a => a.IdAsignacion != excluirAsignacionId.Value);

        return query.FirstOrDefault();
    }

    public bool ExisteAsignacion(int empleadoId, int turnoId, int? excluirId = null)
    {
        var query = _context.Asignaciones
            .Where(a => a.IdEmpleado == empleadoId && a.IdTurno == turnoId);

        if (excluirId.HasValue)
            query = query.Where(a => a.IdAsignacion != excluirId.Value);

        return query.Any();
    }

    public List<Asignacion> GetEnRango(DateOnly desde, DateOnly hasta) =>
        _context.Asignaciones
            .Include(a => a.IdEmpleadoNavigation)
            .Include(a => a.IdTurnoNavigation)
            .Include(a => a.IdSedeNavigation)
            .Where(a => a.IdTurnoNavigation.Fecha >= desde && a.IdTurnoNavigation.Fecha <= hasta)
            .ToList();

    public void Add(Asignacion asignacion)
    {
        _context.Asignaciones.Add(asignacion);
        _context.SaveChanges();
    }

    public void Update(Asignacion asignacion)
    {
        _context.Asignaciones.Update(asignacion);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var asignacion = _context.Asignaciones.Find(id);
        if (asignacion != null)
        {
            _context.Asignaciones.Remove(asignacion);
            _context.SaveChanges();
        }
    }
}
