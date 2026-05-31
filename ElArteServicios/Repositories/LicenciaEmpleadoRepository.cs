using ElArteServicios.Data;
using ElArteServicios.Models;
using Microsoft.EntityFrameworkCore;

namespace ElArteServicios.Repositories;

public class LicenciaEmpleadoRepository
{
    private readonly ServiciosContext _context;

    public LicenciaEmpleadoRepository(ServiciosContext context)
    {
        _context = context;
    }

    public List<LicenciaEmpleado> GetByEmpleado(int idEmpleado) =>
        _context.LicenciasEmpleado
            .Include(l => l.IdEmpleadoNavigation)
            .Where(l => l.IdEmpleado == idEmpleado)
            .OrderByDescending(l => l.Desde)
            .ToList();

    public List<LicenciaEmpleado> GetEnRango(DateOnly desde, DateOnly hasta) =>
        _context.LicenciasEmpleado
            .Include(l => l.IdEmpleadoNavigation)
            .Where(l => l.Desde <= hasta && l.Hasta >= desde)
            .OrderBy(l => l.Desde)
            .ToList();

    public bool EstaEnLicencia(int idEmpleado, DateOnly fecha) =>
        _context.LicenciasEmpleado.Any(l =>
            l.IdEmpleado == idEmpleado &&
            l.Desde <= fecha &&
            l.Hasta >= fecha);

    public LicenciaEmpleado? GetById(int id) =>
        _context.LicenciasEmpleado
            .Include(l => l.IdEmpleadoNavigation)
            .FirstOrDefault(l => l.IdLicencia == id);

    public void Add(LicenciaEmpleado licencia)
    {
        _context.LicenciasEmpleado.Add(licencia);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var licencia = _context.LicenciasEmpleado.Find(id);
        if (licencia != null)
        {
            _context.LicenciasEmpleado.Remove(licencia);
            _context.SaveChanges();
        }
    }
}
