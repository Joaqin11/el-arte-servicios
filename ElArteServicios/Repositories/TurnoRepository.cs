using ElArteServicios.Data;
using ElArteServicios.Models;

namespace ElArteServicios.Repositories
{
    public class TurnoRepository
    {
        private readonly ServiciosContext _context;

        public TurnoRepository(ServiciosContext context)
        {
            _context = context;
        }

        // Obtener todos los turnos
        public List<Turno> GetAll()
        {
            return _context.Turnos.ToList();
        }

        // Obtener turno por ID
        public Turno GetById(int id)
        {
            return _context.Turnos.FirstOrDefault(t => t.IdTurno == id);
        }

        // Agregar nuevo turno
        public void Add(Turno turno)
        {
            _context.Turnos.Add(turno);
            _context.SaveChanges();
        }

        // Actualizar turno
        public void Update(Turno turno)
        {
            _context.Turnos.Update(turno);
            _context.SaveChanges();
        }

        // Eliminar turno
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
}
