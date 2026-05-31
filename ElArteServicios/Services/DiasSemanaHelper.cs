namespace ElArteServicios.Services;

[Flags]
public enum DiasSemanaFlags
{
    Ninguno = 0,
    Lunes = 1,
    Martes = 2,
    Miercoles = 4,
    Jueves = 8,
    Viernes = 16,
    Sabado = 32,
    Domingo = 64,
    Laborables = Lunes | Martes | Miercoles | Jueves | Viernes,
    Todos = 127
}

public static class DiasSemanaHelper
{
    public static int FromDayOfWeek(DayOfWeek dia) => dia switch
    {
        DayOfWeek.Monday => (int)DiasSemanaFlags.Lunes,
        DayOfWeek.Tuesday => (int)DiasSemanaFlags.Martes,
        DayOfWeek.Wednesday => (int)DiasSemanaFlags.Miercoles,
        DayOfWeek.Thursday => (int)DiasSemanaFlags.Jueves,
        DayOfWeek.Friday => (int)DiasSemanaFlags.Viernes,
        DayOfWeek.Saturday => (int)DiasSemanaFlags.Sabado,
        DayOfWeek.Sunday => (int)DiasSemanaFlags.Domingo,
        _ => 0
    };

    public static int FromDateOnly(DateOnly fecha) =>
        FromDayOfWeek(fecha.ToDateTime(TimeOnly.MinValue).DayOfWeek);

    public static bool IncluyeDia(int bitmask, DateOnly fecha) =>
        (bitmask & FromDateOnly(fecha)) != 0;

    public static string Describir(int bitmask)
    {
        if (bitmask == (int)DiasSemanaFlags.Todos) return "Todos los días";
        if (bitmask == (int)DiasSemanaFlags.Laborables) return "Lun–Vie";

        var partes = new List<string>();
        if ((bitmask & (int)DiasSemanaFlags.Lunes) != 0) partes.Add("Lun");
        if ((bitmask & (int)DiasSemanaFlags.Martes) != 0) partes.Add("Mar");
        if ((bitmask & (int)DiasSemanaFlags.Miercoles) != 0) partes.Add("Mié");
        if ((bitmask & (int)DiasSemanaFlags.Jueves) != 0) partes.Add("Jue");
        if ((bitmask & (int)DiasSemanaFlags.Viernes) != 0) partes.Add("Vie");
        if ((bitmask & (int)DiasSemanaFlags.Sabado) != 0) partes.Add("Sáb");
        if ((bitmask & (int)DiasSemanaFlags.Domingo) != 0) partes.Add("Dom");
        return partes.Count == 0 ? "—" : string.Join(", ", partes);
    }
}
