using ElArteServicios.Data;
using ElArteServicios.Models;

namespace ElArteServicios.Repositories
{
    public class SedeRepository
    {
        private readonly ServiciosContext _context;

        public SedeRepository(ServiciosContext context)
        {
            _context = context;
        }

        // Obtener todas las sedes
        public List<Sede> GetAll()
        {
            return _context.Sedes.ToList();
        }

        // Obtener sede por ID
        public Sede GetById(int id)
        {
            return _context.Sedes.FirstOrDefault(s => s.IdSede == id);
        }

        // Agregar nueva sede
        public void Add(Sede sede)
        {
            _context.Sedes.Add(sede);
            _context.SaveChanges();
        }

        // Actualizar sede
        public void Update(Sede sede)
        {
            _context.Sedes.Update(sede);
            _context.SaveChanges();
        }

        // Eliminar sede
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
}
