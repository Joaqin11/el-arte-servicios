using ElArteServicios.Data;
using ElArteServicios.Models;

namespace ElArteServicios.Repositories;

public class EmpleadoRepository
{
    private readonly ServiciosContext _context;

    public EmpleadoRepository(ServiciosContext context)
    {
        _context = context;
    }

    public List<Empleado> GetAll() => _context.Empleados.OrderBy(e => e.Apellido).ThenBy(e => e.Nombre).ToList();

    public Empleado? GetById(int id) => _context.Empleados.FirstOrDefault(e => e.IdEmpleado == id);

    public Empleado? GetByCodigo(string codigo) =>
        _context.Empleados.FirstOrDefault(e => e.Codigo == codigo);

    public bool TieneAsignaciones(int id) =>
        _context.Asignaciones.Any(a => a.IdEmpleado == id);

    public void Add(Empleado empleado)
    {
        _context.Empleados.Add(empleado);
        _context.SaveChanges();
    }

    public void Update(Empleado empleado)
    {
        _context.Empleados.Update(empleado);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var empleado = _context.Empleados.Find(id);
        if (empleado != null)
        {
            _context.Empleados.Remove(empleado);
            _context.SaveChanges();
        }
    }
}
