using ElArteServicios.Models;

namespace ElArteServicios.Services;

/// <summary>
/// Reglas de negocio: un vigilador no puede mezclar mañana, tarde y sereno de forma incompatible.
/// </summary>
public static class AsignacionReglasHelper
{
    public static FranjaTurno Clasificar(Turno turno)
    {
        if (turno.CruzaMedianoche || turno.FechaFin > turno.Fecha)
            return FranjaTurno.NocheSereno;

        var horaInicio = TurnoCalculoHelper.ParseHora(turno.HoraInicio);

        if (horaInicio.Hours < 12)
            return FranjaTurno.Manana;

        if (horaInicio.Hours < 20)
            return FranjaTurno.Tarde;

        return FranjaTurno.NocheSereno;
    }

    public static string NombreFranja(FranjaTurno franja) => franja switch
    {
        FranjaTurno.Manana => "Mañana",
        FranjaTurno.Tarde => "Tarde",
        FranjaTurno.NocheSereno => "Sereno / Nocturno",
        _ => "Turno"
    };

    /// <summary>
    /// Devuelve mensaje de error si el empleado no puede tomar el turno nuevo dado lo ya asignado.
    /// </summary>
    public static string? ValidarConflictoEmpleado(Turno turnoNuevo, IEnumerable<Turno> turnosYaAsignados)
    {
        var franjaNuevo = Clasificar(turnoNuevo);

        foreach (var otro in turnosYaAsignados)
        {
            var franjaOtro = Clasificar(otro);

            if (TurnoCalculoHelper.SeSolapan(
                    turnoNuevo.Fecha, TurnoCalculoHelper.ParseHora(turnoNuevo.HoraInicio),
                    turnoNuevo.FechaFin, TurnoCalculoHelper.ParseHora(turnoNuevo.HoraFin),
                    otro.Fecha, TurnoCalculoHelper.ParseHora(otro.HoraInicio),
                    otro.FechaFin, TurnoCalculoHelper.ParseHora(otro.HoraFin)))
            {
                return $"El horario se superpone con {TurnoService.DescribirTurno(otro)}.";
            }

            // Mañana y tarde el mismo día calendario
            if (MismoDia(turnoNuevo.Fecha, otro.Fecha))
            {
                if (franjaNuevo == FranjaTurno.Manana && franjaOtro == FranjaTurno.Tarde)
                {
                    return $"Ya tiene turno de tarde el {turnoNuevo.Fecha:dd/MM/yyyy}. " +
                           "No puede asignarse también a la mañana ese día.";
                }

                if (franjaNuevo == FranjaTurno.Tarde && franjaOtro == FranjaTurno.Manana)
                {
                    return $"Ya tiene turno de mañana el {turnoNuevo.Fecha:dd/MM/yyyy}. " +
                           "No puede asignarse también a la tarde ese día.";
                }
            }

            // Mañana día D ↔ sereno que termina en la mañana de D
            if (franjaNuevo == FranjaTurno.Manana && franjaOtro == FranjaTurno.NocheSereno
                && otro.FechaFin == turnoNuevo.Fecha)
            {
                return $"Tuvo sereno/nocturno que termina el {turnoNuevo.Fecha:dd/MM/yyyy} por la mañana. " +
                       "No puede asignarse a la mañana de ese día.";
            }

            if (franjaNuevo == FranjaTurno.NocheSereno && franjaOtro == FranjaTurno.Manana
                && turnoNuevo.FechaFin == otro.Fecha)
            {
                return $"Ya tiene turno de mañana el {otro.Fecha:dd/MM/yyyy}. " +
                       "No puede asignarse a sereno que termina ese día en la mañana.";
            }

            // Tarde día D ↔ sereno que empieza esa noche (inicio en D)
            if (franjaNuevo == FranjaTurno.Tarde && franjaOtro == FranjaTurno.NocheSereno
                && otro.Fecha == turnoNuevo.Fecha)
            {
                return $"Ya tiene sereno/nocturno que comienza el {turnoNuevo.Fecha:dd/MM/yyyy}. " +
                       "No puede asignarse también a la tarde ese día.";
            }

            if (franjaNuevo == FranjaTurno.NocheSereno && franjaOtro == FranjaTurno.Tarde
                && turnoNuevo.Fecha == otro.Fecha)
            {
                return $"Ya tiene turno de tarde el {turnoNuevo.Fecha:dd/MM/yyyy}. " +
                       "No puede asignarse a sereno esa misma noche.";
            }
        }

        return null;
    }

    private static bool MismoDia(DateOnly a, DateOnly b) => a == b;
}
