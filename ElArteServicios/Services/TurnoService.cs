using ElArteServicios.Models;
using ElArteServicios.Repositories;

namespace ElArteServicios.Services
{
    public class TurnoService
    {
        private readonly TurnoRepository _repo;

        public TurnoService(TurnoRepository repo)
        {
            _repo = repo;
        }

        // Crear turno
        public void CrearTurno(string fecha, string horaInicio, string horaFin, decimal cantHoras)
        {
            var turno = new Turno
            {
                Fecha = DateOnly.Parse(fecha),
                HoraInicio = horaInicio,
                HoraFin = horaFin,
                CantHoras = cantHoras
            };

            _repo.Add(turno);
        }

        // Obtener todos los turnos
        public List<Turno> ObtenerTurnos()
        {
            return _repo.GetAll();
        }

        // Obtener turno por ID
        public Turno ObtenerTurnoPorId(int id)
        {
            return _repo.GetById(id);
        }

        // Actualizar turno
        public void ActualizarTurno(int id, string fecha, string horaInicio, string horaFin, decimal cantHoras)
        {
            var turno = _repo.GetById(id);
            if (turno != null)
            {
                turno.Fecha = DateOnly.Parse(fecha);
                turno.HoraInicio = horaInicio;
                turno.HoraFin = horaFin;
                turno.CantHoras = cantHoras;
                _repo.Update(turno);
            }
        }

        // Eliminar turno
        public void EliminarTurno(int id)
        {
            _repo.Delete(id);
        }
    }
}
