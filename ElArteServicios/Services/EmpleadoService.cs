using ElArteServicios.Models;
using ElArteServicios.Repositories;

namespace ElArteServicios.Services;

public class EmpleadoService
{
    private readonly EmpleadoRepository _repo;

    public EmpleadoService(EmpleadoRepository repo)
    {
        _repo = repo;
    }

    public void CrearEmpleado(string codigo, string nombre, string apellido)
    {
        ValidarDatos(codigo, nombre, apellido);

        if (_repo.GetByCodigo(codigo.Trim()) != null)
            throw new InvalidOperationException($"Ya existe un empleado con el código '{codigo}'.");

        _repo.Add(new Empleado
        {
            Codigo = codigo.Trim(),
            Nombre = nombre.Trim(),
            Apellido = apellido.Trim()
        });
    }

    public List<Empleado> ObtenerEmpleados() => _repo.GetAll();

    public Empleado? ObtenerEmpleadoPorId(int id) => _repo.GetById(id);

    public void ActualizarEmpleado(int id, string codigo, string nombre, string apellido)
    {
        ValidarDatos(codigo, nombre, apellido);

        var empleado = _repo.GetById(id)
            ?? throw new InvalidOperationException("Empleado no encontrado.");

        var duplicado = _repo.GetByCodigo(codigo.Trim());
        if (duplicado != null && duplicado.IdEmpleado != id)
            throw new InvalidOperationException($"Ya existe otro empleado con el código '{codigo}'.");

        empleado.Codigo = codigo.Trim();
        empleado.Nombre = nombre.Trim();
        empleado.Apellido = apellido.Trim();
        _repo.Update(empleado);
    }

    public void EliminarEmpleado(int id)
    {
        if (_repo.GetById(id) == null)
            throw new InvalidOperationException("Empleado no encontrado.");

        if (_repo.TieneAsignaciones(id))
            throw new InvalidOperationException("No se puede eliminar: el empleado tiene asignaciones.");

        _repo.Delete(id);
    }

    private static void ValidarDatos(string codigo, string nombre, string apellido)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("El código es obligatorio.");
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre es obligatorio.");
        if (string.IsNullOrWhiteSpace(apellido))
            throw new ArgumentException("El apellido es obligatorio.");
    }
}
