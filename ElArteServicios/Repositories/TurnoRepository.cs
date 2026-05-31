using ElArteServicios.Data;
using ElArteServicios.Models;
using Microsoft.EntityFrameworkCore;

namespace ElArteServicios.Repositories;

public class TurnoRepository
{
    private readonly ServiciosContext _context;

    public TurnoRepository(ServiciosContext context)
    {
        _context = context;
    }

    public List<Turno> GetAll() =>
        _context.Turnos
            .Include(t => t.IdSedeNavigation)
            .OrderByDescending(t => t.Fecha)
            .ThenBy(t => t.HoraInicio)
            .ToList();

    public List<Turno> GetBySedeEnRango(int idSede, DateOnly desde, DateOnly hasta) =>
        _context.Turnos
            .Include(t => t.IdSedeNavigation)
            .Include(t => t.Asignacions)
                .ThenInclude(a => a.IdEmpleadoNavigation)
            .Where(t => t.IdSede == idSede)
            .Where(t => t.Fecha <= hasta && t.FechaFin >= desde)
            .ToList();

    public List<Turno> GetEnRango(DateOnly desde, DateOnly hasta) =>
        _context.Turnos
            .Include(t => t.IdSedeNavigation)
            .Include(t => t.Asignacions)
                .ThenInclude(a => a.IdEmpleadoNavigation)
            .Where(t => t.Fecha <= hasta && t.FechaFin >= desde)
            .ToList();

    public bool ExisteEquivalente(int idSede, DateOnly fecha, DateOnly fechaFin, string horaInicio, string horaFin, int? excluirId = null)
    {
        var query = _context.Turnos.Where(t =>
            t.IdSede == idSede &&
            t.Fecha == fecha &&
            t.FechaFin == fechaFin &&
            t.HoraInicio == horaInicio &&
            t.HoraFin == horaFin);

        if (excluirId.HasValue)
            query = query.Where(t => t.IdTurno != excluirId.Value);

        return query.Any();
    }

    public void AddRange(IEnumerable<Turno> turnos)
    {
        _context.Turnos.AddRange(turnos);
        _context.SaveChanges();
    }

    public List<Turno> GetBySede(int idSede, DateOnly? fecha = null)
    {
        var query = _context.Turnos
            .Include(t => t.IdSedeNavigation)
            .Where(t => t.IdSede == idSede);

        if (fecha.HasValue)
            query = query.Where(t => t.Fecha == fecha.Value);

        return query
            .OrderBy(t => t.Fecha)
            .ThenBy(t => t.HoraInicio)
            .ToList();
    }

    public Turno? GetById(int id) =>
        _context.Turnos
            .Include(t => t.IdSedeNavigation)
            .FirstOrDefault(t => t.IdTurno == id);

    public bool TieneAsignaciones(int id) =>
        _context.Asignaciones.Any(a => a.IdTurno == id);

    /// <summary>Turnos de la sede cuyo intervalo de fechas podría solaparse con el nuevo.</summary>
    public List<Turno> GetCandidatosSolapamiento(int idSede, DateOnly fechaInicio, DateOnly fechaFin, int? excluirId = null)
    {
        var query = _context.Turnos
            .Where(t => t.IdSede == idSede)
            .Where(t => t.Fecha <= fechaFin && t.FechaFin >= fechaInicio);

        if (excluirId.HasValue)
            query = query.Where(t => t.IdTurno != excluirId.Value);

        return query.ToList();
    }

    public void Add(Turno turno)
    {
        _context.Turnos.Add(turno);
        _context.SaveChanges();
    }

    public void Update(Turno turno)
    {
        _context.Turnos.Update(turno);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var turno = _context.Turnos.Find(id);
        if (turno != null)
        {
            _context.Turnos.Remove(turno);
            _context.SaveChanges();
        }
    }
}
