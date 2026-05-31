using ElArteServicios.Data;
using ElArteServicios.Models;
using Microsoft.EntityFrameworkCore;

namespace ElArteServicios.Repositories;

public class SedeRepository
{
    private readonly ServiciosContext _context;

    public SedeRepository(ServiciosContext context)
    {
        _context = context;
    }

    public List<Sede> GetAll() => _context.Sedes.OrderBy(s => s.Nombre).ToList();

    public Sede? GetById(int id) => _context.Sedes.FirstOrDefault(s => s.IdSede == id);

    public bool TieneTurnos(int id) => _context.Turnos.Any(t => t.IdSede == id);

    public bool TieneAsignaciones(int id) => _context.Asignaciones.Any(a => a.IdSede == id);

    public void Add(Sede sede)
    {
        _context.Sedes.Add(sede);
        _context.SaveChanges();
    }

    public void Update(Sede sede)
    {
        _context.Sedes.Update(sede);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var sede = _context.Sedes.Find(id);
        if (sede != null)
        {
            _context.Sedes.Remove(sede);
            _context.SaveChanges();
        }
    }
}
