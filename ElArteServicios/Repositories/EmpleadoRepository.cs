using ElArteServicios.Data;
using ElArteServicios.Models;

public class EmpleadoRepository
{
    private readonly ServiciosContext _context;

    public EmpleadoRepository(ServiciosContext context)
    {
        _context = context;
    }

    public List<Empleado> GetAll()
    {
        return _context.Empleados.ToList();
    }

    public Empleado GetById(int id)
    {
        return _context.Empleados.FirstOrDefault(e => e.IdEmpleado == id);
    }

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
