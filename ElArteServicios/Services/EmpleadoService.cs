using ElArteServicios.Models;

namespace ElArteServicios.Services
{
    public class EmpleadoService
    {
        private readonly EmpleadoRepository _repo;

        public EmpleadoService(EmpleadoRepository repo)
        {
            _repo = repo;
        }

        // Crear empleado
        public void CrearEmpleado(string codigo, string nombre, string apellido)
        {
            var empleado = new Empleado
            {
                Codigo = codigo,
                Nombre = nombre,
                Apellido = apellido
            };

            _repo.Add(empleado);
        }

        // Obtener todos los empleados
        public List<Empleado> ObtenerEmpleados()
        {
            return _repo.GetAll();
        }

        // Obtener empleado por ID
        public Empleado ObtenerEmpleadoPorId(int id)
        {
            return _repo.GetById(id);
        }

        // Actualizar empleado
        public void ActualizarEmpleado(int id, string nombre, string apellido)
        {
            var empleado = _repo.GetById(id);
            if (empleado != null)
            {
                empleado.Nombre = nombre;
                empleado.Apellido = apellido;
                _repo.Update(empleado);
            }
        }

        // Eliminar empleado
        public void EliminarEmpleado(int id)
        {
            _repo.Delete(id);
        }
    }
}
