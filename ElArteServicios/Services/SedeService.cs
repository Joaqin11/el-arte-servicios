using ElArteServicios.Models;
using ElArteServicios.Repositories;

namespace ElArteServicios.Services
{
    public class SedeService
    {
        private readonly SedeRepository _repo;

        public SedeService(SedeRepository repo)
        {
            _repo = repo;
        }

        // Crear sede
        public void CrearSede(string nombre)
        {
            var sede = new Sede
            {
                Nombre = nombre
            };

            _repo.Add(sede);
        }

        // Obtener todas las sedes
        public List<Sede> ObtenerSedes()
        {
            return _repo.GetAll();
        }

        // Obtener sede por ID
        public Sede ObtenerSedePorId(int id)
        {
            return _repo.GetById(id);
        }

        // Actualizar sede
        public void ActualizarSede(int id, string nombre)
        {
            var sede = _repo.GetById(id);
            if (sede != null)
            {
                sede.Nombre = nombre;
                _repo.Update(sede);
            }
        }

        // Eliminar sede
        public void EliminarSede(int id)
        {
            _repo.Delete(id);
        }
    }
}
