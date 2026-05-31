using ElArteServicios.Models;
using ElArteServicios.Repositories;

namespace ElArteServicios.Services;

public class TurnoService
{
    private readonly TurnoRepository _repo;
    private readonly SedeRepository _sedes;

    public TurnoService(TurnoRepository repo, SedeRepository sedes)
    {
        _repo = repo;
        _sedes = sedes;
    }

    public void CrearTurno(
        int idSede,
        DateOnly fechaInicio,
        TimeSpan horaInicio,
        DateOnly fechaFin,
        TimeSpan horaFin,
        bool cancelado = false)
    {
        var (fechaFinResuelta, horas) = ValidarYResolver(idSede, fechaInicio, horaInicio, fechaFin, horaFin, null);

        _repo.Add(new Turno
        {
            IdSede = idSede,
            Fecha = fechaInicio,
            FechaFin = fechaFinResuelta,
            HoraInicio = FormatearHora(horaInicio),
            HoraFin = FormatearHora(horaFin),
            CantHoras = horas,
            Origen = TurnoOrigen.Manual,
            BloqueadoRegeneracion = true,
            Cancelado = cancelado
        });
    }

    public List<Turno> ObtenerTurnos() => _repo.GetAll();

    public List<Turno> ObtenerTurnosPorSede(int idSede, DateOnly? fecha = null) =>
        _repo.GetBySede(idSede, fecha);

    public Turno? ObtenerTurnoPorId(int id) => _repo.GetById(id);

    public void ActualizarTurno(
        int id,
        int idSede,
        DateOnly fechaInicio,
        TimeSpan horaInicio,
        DateOnly fechaFin,
        TimeSpan horaFin,
        bool cancelado)
    {
        var (fechaFinResuelta, horas) = ValidarYResolver(idSede, fechaInicio, horaInicio, fechaFin, horaFin, id);

        var turno = _repo.GetById(id)
            ?? throw new InvalidOperationException("Turno no encontrado.");

        turno.IdSede = idSede;
        turno.Fecha = fechaInicio;
        turno.FechaFin = fechaFinResuelta;
        turno.HoraInicio = FormatearHora(horaInicio);
        turno.HoraFin = FormatearHora(horaFin);
        turno.CantHoras = horas;
        turno.Cancelado = cancelado;
        if (turno.Origen == TurnoOrigen.Generado)
            turno.Origen = TurnoOrigen.Excepcion;
        turno.BloqueadoRegeneracion = true;
        _repo.Update(turno);
    }

    public void EliminarTurno(int id)
    {
        if (_repo.GetById(id) == null)
            throw new InvalidOperationException("Turno no encontrado.");

        if (_repo.TieneAsignaciones(id))
            throw new InvalidOperationException("No se puede eliminar: el turno tiene asignaciones.");

        _repo.Delete(id);
    }

    private (DateOnly FechaFin, decimal Horas) ValidarYResolver(
        int idSede,
        DateOnly fechaInicio,
        TimeSpan horaInicio,
        DateOnly fechaFin,
        TimeSpan horaFin,
        int? excluirTurnoId = null)
    {
        if (_sedes.GetById(idSede) == null)
            throw new InvalidOperationException("La sede seleccionada no existe.");

        var resultado = TurnoCalculoHelper.Resolver(fechaInicio, horaInicio, fechaFin, horaFin);

        if (resultado.Horas > 24)
            throw new ArgumentException("Un turno no puede superar las 24 horas.");

        if (resultado.Horas < 0.5m)
            throw new ArgumentException("El turno debe durar al menos media hora.");

        ValidarSinSolapamiento(idSede, fechaInicio, horaInicio, resultado.FechaFin, horaFin, excluirTurnoId);

        return resultado;
    }

    private void ValidarSinSolapamiento(
        int idSede,
        DateOnly fechaInicio,
        TimeSpan horaInicio,
        DateOnly fechaFin,
        TimeSpan horaFin,
        int? excluirTurnoId)
    {
        var candidatos = _repo.GetCandidatosSolapamiento(idSede, fechaInicio, fechaFin, excluirTurnoId);

        foreach (var existente in candidatos)
        {
            if (TurnoCalculoHelper.SeSolapan(
                    fechaInicio, horaInicio, fechaFin, horaFin,
                    existente.Fecha, TurnoCalculoHelper.ParseHora(existente.HoraInicio),
                    existente.FechaFin, TurnoCalculoHelper.ParseHora(existente.HoraFin)))
            {
                throw new InvalidOperationException(
                    $"El horario se superpone con un turno existente ({DescribirRangoHorario(existente)}). " +
                    "Podés cargar varios turnos el mismo día si no comparten horario.");
            }
        }
    }

    public static string FormatearHora(TimeSpan hora) => hora.ToString(@"hh\:mm");

    public static string DescribirTurno(Turno turno)
    {
        var horas = turno.CantHoras?.ToString("0.#") ?? "?";
        if (turno.FechaFin > turno.Fecha)
        {
            return $"{turno.Fecha:dd/MM/yyyy} {turno.HoraInicio} → {turno.FechaFin:dd/MM/yyyy} {turno.HoraFin} ({horas} h)";
        }

        return $"{turno.Fecha:dd/MM/yyyy} {turno.HoraInicio}-{turno.HoraFin} ({horas} h)";
    }

    public static string DescribirOrigen(Turno turno) => turno.Origen switch
    {
        TurnoOrigen.Generado => "Generado",
        TurnoOrigen.Excepcion => "Excepción",
        _ => "Manual"
    };

    public static string DescribirRangoHorario(Turno turno)
    {
        if (turno.FechaFin > turno.Fecha)
            return $"{turno.HoraInicio} ({turno.Fecha:dd/MM}) → {turno.HoraFin} ({turno.FechaFin:dd/MM})";

        return $"{turno.HoraInicio} - {turno.HoraFin}";
    }
}
