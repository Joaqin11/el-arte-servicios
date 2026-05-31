using ElArteServicios.Models;
using ElArteServicios.Repositories;

namespace ElArteServicios.Services;

public class TurnoGeneradorService
{
    private readonly PlantillaTurnoRepository _plantillas;
    private readonly TurnoRepository _turnos;

    public TurnoGeneradorService(PlantillaTurnoRepository plantillas, TurnoRepository turnos)
    {
        _plantillas = plantillas;
        _turnos = turnos;
    }

    public ResultadoGeneracionTurnos Generar(int idSede, DateOnly desde, DateOnly hasta)
    {
        if (hasta < desde)
            throw new ArgumentException("La fecha hasta debe ser posterior o igual a la fecha desde.");

        var resultado = new ResultadoGeneracionTurnos();
        var existentesEnRango = _turnos.GetBySedeEnRango(idSede, desde, hasta);
        var nuevos = new List<Turno>();

        for (var fecha = desde; fecha <= hasta; fecha = fecha.AddDays(1))
        {
            var plantilla = _plantillas.GetVigente(idSede, fecha);
            if (plantilla == null || plantilla.Detalles.Count == 0)
            {
                resultado.DiasSinPlantilla++;
                continue;
            }

            foreach (var detalle in plantilla.Detalles.OrderBy(d => d.Orden))
            {
                if (!DiasSemanaHelper.IncluyeDia(detalle.DiasSemana, fecha))
                    continue;

                var horaInicio = TurnoCalculoHelper.ParseHora(detalle.HoraInicio);
                var horaFin = TurnoCalculoHelper.ParseHora(detalle.HoraFin);
                var fechaFin = detalle.CruzaDiaSiguiente || horaFin <= horaInicio
                    ? fecha.AddDays(1)
                    : fecha;

                var horaIniStr = TurnoService.FormatearHora(horaInicio);
                var horaFinStr = TurnoService.FormatearHora(horaFin);

                if (ExisteEquivalente(existentesEnRango, idSede, fecha, fechaFin, horaIniStr, horaFinStr))
                {
                    resultado.OmitidosExistentes++;
                    continue;
                }

                if (HayConflictoBloqueado(existentesEnRango, idSede, fecha, fechaFin, horaInicio, horaFin))
                {
                    resultado.OmitidosBloqueados++;
                    continue;
                }

                var horas = TurnoCalculoHelper.CalcularHoras(fecha, horaInicio, fechaFin, horaFin);
                var turno = new Turno
                {
                    IdSede = idSede,
                    Fecha = fecha,
                    FechaFin = fechaFin,
                    HoraInicio = horaIniStr,
                    HoraFin = horaFinStr,
                    CantHoras = horas,
                    Origen = TurnoOrigen.Generado,
                    IdPlantillaDetalle = detalle.IdDetalle,
                    BloqueadoRegeneracion = false
                };

                nuevos.Add(turno);
                existentesEnRango.Add(turno);
                resultado.Creados++;
            }
        }

        if (nuevos.Count > 0)
            _turnos.AddRange(nuevos);

        return resultado;
    }

    private static bool ExisteEquivalente(
        List<Turno> existentes, int idSede, DateOnly fecha, DateOnly fechaFin, string hi, string hf) =>
        existentes.Any(t =>
            t.IdSede == idSede &&
            t.Fecha == fecha &&
            t.FechaFin == fechaFin &&
            t.HoraInicio == hi &&
            t.HoraFin == hf);

    private bool HayConflictoBloqueado(
        List<Turno> existentes, int idSede, DateOnly fecha, DateOnly fechaFin,
        TimeSpan horaInicio, TimeSpan horaFin)
    {
        foreach (var t in existentes.Where(x => x.IdSede == idSede && x.IdTurno > 0))
        {
            if (!TurnoCalculoHelper.SeSolapan(
                    fecha, horaInicio, fechaFin, horaFin,
                    t.Fecha, TurnoCalculoHelper.ParseHora(t.HoraInicio),
                    t.FechaFin, TurnoCalculoHelper.ParseHora(t.HoraFin)))
                continue;

            if (t.BloqueadoRegeneracion || t.Origen != TurnoOrigen.Generado)
                return true;

            if (_turnos.TieneAsignaciones(t.IdTurno))
                return true;
        }
        return false;
    }
}
