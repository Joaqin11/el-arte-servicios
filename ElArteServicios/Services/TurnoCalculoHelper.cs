namespace ElArteServicios.Services;

/// <summary>
/// Cálculo de duración, fechas e intervalos para turnos (incluye cruces de medianoche).
/// </summary>
public static class TurnoCalculoHelper
{
    public static DateTime ObtenerInicio(DateOnly fechaInicio, TimeSpan horaInicio) =>
        fechaInicio.ToDateTime(TimeOnly.FromTimeSpan(horaInicio));

    public static DateTime ObtenerFin(DateOnly fechaFin, TimeSpan horaFin) =>
        fechaFin.ToDateTime(TimeOnly.FromTimeSpan(horaFin));

    public static decimal CalcularHoras(DateOnly fechaInicio, TimeSpan horaInicio, DateOnly fechaFin, TimeSpan horaFin)
    {
        var inicio = ObtenerInicio(fechaInicio, horaInicio);
        var fin = ObtenerFin(fechaFin, horaFin);

        if (fin <= inicio)
            throw new ArgumentException("La fecha/hora de fin debe ser posterior al inicio del turno.");

        return Math.Round((decimal)(fin - inicio).TotalHours, 1, MidpointRounding.AwayFromZero);
    }

    public static (DateOnly FechaFin, decimal Horas) Resolver(
        DateOnly fechaInicio,
        TimeSpan horaInicio,
        DateOnly fechaFin,
        TimeSpan horaFin)
    {
        if (fechaFin < fechaInicio)
            throw new ArgumentException("La fecha de fin no puede ser anterior a la fecha de inicio.");

        var horas = CalcularHoras(fechaInicio, horaInicio, fechaFin, horaFin);
        return (fechaFin, horas);
    }

    public static bool SeSolapan(
        DateOnly fechaInicioA, TimeSpan horaInicioA, DateOnly fechaFinA, TimeSpan horaFinA,
        DateOnly fechaInicioB, TimeSpan horaInicioB, DateOnly fechaFinB, TimeSpan horaFinB)
    {
        var inicioA = ObtenerInicio(fechaInicioA, horaInicioA);
        var finA = ObtenerFin(fechaFinA, horaFinA);
        var inicioB = ObtenerInicio(fechaInicioB, horaInicioB);
        var finB = ObtenerFin(fechaFinB, horaFinB);

        return inicioA < finB && inicioB < finA;
    }

    public static TimeSpan ParseHora(string hora)
    {
        if (TimeSpan.TryParse(hora, out var ts)) return ts;
        return TimeSpan.ParseExact(hora, @"hh\:mm", null);
    }
}
