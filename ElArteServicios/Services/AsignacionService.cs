using ElArteServicios.Models;
using ElArteServicios.Repositories;

namespace ElArteServicios.Services
{
    public class AsignacionService
    {
        private readonly AsignacionRepository _repo;

        public AsignacionService(AsignacionRepository repo)
        {
            _repo = repo;
        }

        // Crear asignación
        public void CrearAsignacion(int empleadoId, int turnoId, int sedeId, string notas)
        {
            var asignacion = new Asignacion
            {
                IdEmpleado = empleadoId,
                IdTurno = turnoId,
                IdSede = sedeId,
                Notas = notas
            };

            _repo.Add(asignacion);
        }

        // Obtener todas las asignaciones
        public List<Asignacion> ObtenerAsignaciones()
        {
            return _repo.GetAll();
        }

        // Obtener asignación por ID
        public Asignacion ObtenerAsignacionPorId(int id)
        {
            return _repo.GetById(id);
        }

        // Actualizar asignación
        public void ActualizarAsignacion(int id, int empleadoId, int turnoId, int sedeId, string notas)
        {
            var asignacion = _repo.GetById(id);
            if (asignacion != null)
            {
                asignacion.IdEmpleado = empleadoId;
                asignacion.IdTurno = turnoId;
                asignacion.IdSede = sedeId;
                asignacion.Notas = notas;
                _repo.Update(asignacion);
            }
        }

        // Eliminar asignación
        public void EliminarAsignacion(int id)
        {
            _repo.Delete(id);
        }
    }
}
