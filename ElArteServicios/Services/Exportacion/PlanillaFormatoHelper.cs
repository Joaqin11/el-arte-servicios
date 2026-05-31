namespace ElArteServicios.Services.Exportacion;

public static class PlanillaFormatoHelper
{
    public static string FormatearHorarioPlanilla(string horaInicio, string horaFin) =>
        $"{CompactarHora(horaInicio)} {CompactarHora(horaFin)}";

    public static string CompactarHora(string hora)
    {
        var ts = Services.TurnoCalculoHelper.ParseHora(hora);
        return ts.ToString(@"hhmm");
    }

    public static string EncabezadoDia(DateOnly fecha)
    {
        var letra = fecha.DayOfWeek switch
        {
            DayOfWeek.Monday => "L",
            DayOfWeek.Tuesday => "M",
            DayOfWeek.Wednesday => "X",
            DayOfWeek.Thursday => "J",
            DayOfWeek.Friday => "V",
            DayOfWeek.Saturday => "S",
            DayOfWeek.Sunday => "D",
            _ => "?"
        };
        return letra + fecha.Day.ToString("00");
    }

    public static bool EsFinDeSemana(DateOnly fecha) =>
        fecha.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
}
