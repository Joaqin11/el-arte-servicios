using ElArteServicios.Data;
using ElArteServicios.Models;
using Microsoft.EntityFrameworkCore;

namespace ElArteServicios.Repositories
{
    public class AsignacionRepository
    {
        private readonly ServiciosContext _context;

        public AsignacionRepository(ServiciosContext context)
        {
            _context = context;
        }

        // Obtener todas las asignaciones con navegación
        public List<Asignacion> GetAll()
        {
            return _context.Asignaciones
                .Include(a => a.IdEmpleadoNavigation)
                .Include(a => a.IdTurnoNavigation)
                .Include(a => a.IdSedeNavigation)
                .ToList();
        }

        // Obtener asignación por ID
        public Asignacion GetById(int id)
        {
            return _context.Asignaciones
                .Include(a => a.IdEmpleadoNavigation)
                .Include(a => a.IdTurnoNavigation)
                .Include(a => a.IdSedeNavigation)
                .FirstOrDefault(a => a.IdAsignacion == id);
        }

        // Agregar nueva asignación
        public void Add(Asignacion asignacion)
        {
            _context.Asignaciones.Add(asignacion);
            _context.SaveChanges();
        }

        // Actualizar asignación
        public void Update(Asignacion asignacion)
        {
            _context.Asignaciones.Update(asignacion);
            _context.SaveChanges();
        }

        // Eliminar asignación
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
}
