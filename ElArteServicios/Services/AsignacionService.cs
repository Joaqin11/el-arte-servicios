using ElArteServicios.Models;
using ElArteServicios.Repositories;

namespace ElArteServicios.Services;

public class AsignacionService
{
    private readonly AsignacionRepository _repo;
    private readonly TurnoRepository _turnos;
    private readonly LicenciaEmpleadoRepository _licencias;

    public AsignacionService(
        AsignacionRepository repo,
        TurnoRepository turnos,
        LicenciaEmpleadoRepository licencias)
    {
        _repo = repo;
        _turnos = turnos;
        _licencias = licencias;
    }

    public void CrearAsignacion(int empleadoId, int turnoId, int sedeId, string? notas)
    {
        ValidarAsignacion(empleadoId, turnoId, sedeId, null);

        _repo.Add(new Asignacion
        {
            IdEmpleado = empleadoId,
            IdTurno = turnoId,
            IdSede = sedeId,
            Notas = string.IsNullOrWhiteSpace(notas) ? null : notas.Trim()
        });
    }

    public List<Asignacion> ObtenerAsignaciones() => _repo.GetAll();

    public Asignacion? ObtenerAsignacionPorId(int id) => _repo.GetById(id);

    public void ActualizarAsignacion(int id, int empleadoId, int turnoId, int sedeId, string? notas)
    {
        ValidarAsignacion(empleadoId, turnoId, sedeId, id);

        var asignacion = _repo.GetById(id)
            ?? throw new InvalidOperationException("Asignación no encontrada.");

        asignacion.IdEmpleado = empleadoId;
        asignacion.IdTurno = turnoId;
        asignacion.IdSede = sedeId;
        asignacion.Notas = string.IsNullOrWhiteSpace(notas) ? null : notas.Trim();
        _repo.Update(asignacion);
    }

    public void EliminarAsignacion(int id)
    {
        if (_repo.GetById(id) == null)
            throw new InvalidOperationException("Asignación no encontrada.");

        _repo.Delete(id);
    }

    private void ValidarAsignacion(int empleadoId, int turnoId, int sedeId, int? excluirAsignacionId)
    {
        var turno = _turnos.GetById(turnoId)
            ?? throw new InvalidOperationException("El turno seleccionado no existe.");

        if (turno.IdSede != sedeId)
            throw new InvalidOperationException("El turno no pertenece a la sede seleccionada.");

        if (turno.Cancelado)
            throw new InvalidOperationException("No se puede asignar a un turno cancelado.");

        if (_licencias.EstaEnLicencia(empleadoId, turno.Fecha))
            throw new InvalidOperationException("El empleado está de licencia en la fecha del turno.");

        var ocupante = _repo.GetAsignacionEnTurno(turnoId, excluirAsignacionId);
        if (ocupante != null && ocupante.IdEmpleado != empleadoId)
        {
            var nombre = $"{ocupante.IdEmpleadoNavigation.Nombre} {ocupante.IdEmpleadoNavigation.Apellido}".Trim();
            throw new InvalidOperationException(
                $"Este turno ya tiene vigilador asignado ({nombre}). " +
                "Elegí otro horario o modificá la asignación existente.");
        }

        if (_repo.ExisteAsignacion(empleadoId, turnoId, excluirAsignacionId))
            throw new InvalidOperationException("Este empleado ya está asignado a ese turno.");

        // Reglas solo para el mismo vigilador (otro empleado puede cubrir otro turno el mismo día)
        var otrasAsignacionesDelMismo = _repo.GetByEmpleado(empleadoId, excluirAsignacionId);
        var turnosDelEmpleado = otrasAsignacionesDelMismo
            .Select(a => a.IdTurnoNavigation)
            .Where(t => t != null)
            .Cast<Turno>();

        var conflicto = AsignacionReglasHelper.ValidarConflictoEmpleado(turno, turnosDelEmpleado);
        if (conflicto != null)
            throw new InvalidOperationException(conflicto);
    }
}
