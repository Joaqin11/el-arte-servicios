using ElArteServicios.Models;
using ElArteServicios.Repositories;
using ElArteServicios.Services.Exportacion;

namespace ElArteServicios.Services;

public class PlanillaExportService
{
    private readonly SedeRepository _sedes;
    private readonly TurnoRepository _turnos;
    private readonly AsignacionRepository _asignaciones;
    private readonly PlantillaTurnoRepository _plantillas;
    private readonly LicenciaEmpleadoRepository _licencias;

    public PlanillaExportService(
        SedeRepository sedes,
        TurnoRepository turnos,
        AsignacionRepository asignaciones,
        PlantillaTurnoRepository plantillas,
        LicenciaEmpleadoRepository licencias)
    {
        _sedes = sedes;
        _turnos = turnos;
        _asignaciones = asignaciones;
        _plantillas = plantillas;
        _licencias = licencias;
    }

    public PlanillaExportData Construir(DateOnly desde, DateOnly hasta)
    {
        if (hasta < desde)
            throw new ArgumentException("La fecha hasta debe ser posterior o igual a la fecha desde.");

        var columnas = GenerarColumnas(desde, hasta);
        var turnosRango = _turnos.GetEnRango(desde, hasta);
        var asignacionesRango = _asignaciones.GetEnRango(desde, hasta)
            .Where(a => !a.IdTurnoNavigation.Cancelado)
            .ToList();

        var data = new PlanillaExportData
        {
            Desde = desde,
            Hasta = hasta,
            Columnas = columnas,
            Notas = ConstruirNotas(desde, hasta)
        };

        foreach (var sede in _sedes.GetAll())
        {
            var turnosSede = turnosRango.Where(t => t.IdSede == sede.IdSede).ToList();
            var slots = ObtenerFranjas(sede.IdSede, desde, hasta, turnosSede);
            if (slots.Count == 0 && turnosSede.Count == 0)
                continue;

            var bloque = new PlanillaBloqueSede { NombreSede = sede.Nombre ?? $"Sede {sede.IdSede}" };

            foreach (var slot in slots)
            {
                var filaHorario = new PlanillaFilaHorario
                {
                    Etiqueta = slots.IndexOf(slot) == 0
                        ? $"Horario por {bloque.NombreSede}"
                        : slot.NombreFranja
                };

                var filaAsignacion = new PlanillaFilaAsignacion { EtiquetaFranja = slot.NombreFranja };

                foreach (var col in columnas)
                {
                    var detalle = ObtenerDetalleVigente(sede.IdSede, col.Fecha, slot);
                    var turno = BuscarTurno(turnosSede, col.Fecha, slot, detalle);

                    if (detalle != null || turno != null)
                    {
                        var hi = turno?.HoraInicio ?? detalle!.HoraInicio;
                        var hf = turno?.HoraFin ?? detalle!.HoraFin;
                        filaHorario.HorariosPorDia[col.Fecha] = PlanillaFormatoHelper.FormatearHorarioPlanilla(hi, hf);
                    }

                    filaAsignacion.CeldasPorDia[col.Fecha] = ConstruirCeldaAsignacion(turno, detalle);
                }

                bloque.FilasHorario.Add(filaHorario);
                bloque.FilasAsignacion.Add(filaAsignacion);
            }

            foreach (var col in columnas)
            {
                bloque.HorasDiarias[col.Fecha] = CalcularHorasDiarias(
                    sede.IdSede, col.Fecha, slots, turnosSede);
            }

            bloque.TotalHorasAsignadas = asignacionesRango
                .Where(a => a.IdSede == sede.IdSede)
                .Sum(a => a.IdTurnoNavigation.CantHoras ?? 0);

            data.Sedes.Add(bloque);
        }

        data.Vigiladores = asignacionesRango
            .GroupBy(a => a.IdEmpleado)
            .Select(g =>
            {
                var emp = g.First().IdEmpleadoNavigation;
                return new PlanillaResumenVigilador
                {
                    Codigo = emp.Codigo ?? "?",
                    Nombre = $"{emp.Nombre} {emp.Apellido}".Trim(),
                    Horas = g.Sum(a => a.IdTurnoNavigation.CantHoras ?? 0),
                    Jornadas = g.Count()
                };
            })
            .OrderBy(v => v.Codigo)
            .ToList();

        data.ResumenSedes = data.Sedes
            .Select(s => new PlanillaResumenSede { Nombre = s.NombreSede, Horas = s.TotalHorasAsignadas })
            .OrderBy(s => s.Nombre)
            .ToList();

        data.TotalHorasVigiladores = data.Vigiladores.Sum(v => v.Horas);
        data.TotalHorasSedes = data.ResumenSedes.Sum(s => s.Horas);
        data.TotalesCuadran = data.TotalHorasVigiladores == data.TotalHorasSedes;

        return data;
    }

    public void ExportarExcel(DateOnly desde, DateOnly hasta, string rutaArchivo)
    {
        var data = Construir(desde, hasta);
        PlanillaExcelExporter.Exportar(data, rutaArchivo);
    }

    private static List<PlanillaDiaColumna> GenerarColumnas(DateOnly desde, DateOnly hasta)
    {
        var columnas = new List<PlanillaDiaColumna>();
        for (var fecha = desde; fecha <= hasta; fecha = fecha.AddDays(1))
        {
            columnas.Add(new PlanillaDiaColumna
            {
                Fecha = fecha,
                Encabezado = PlanillaFormatoHelper.EncabezadoDia(fecha),
                EsFinDeSemana = PlanillaFormatoHelper.EsFinDeSemana(fecha)
            });
        }
        return columnas;
    }

    private List<string> ConstruirNotas(DateOnly desde, DateOnly hasta) =>
        _licencias.GetEnRango(desde, hasta)
            .Select(LicenciaEmpleadoService.DescribirLicencia)
            .ToList();

    private sealed record FranjaSlot(int Orden, string NombreFranja, string HoraInicio, string HoraFin, int? IdPlantillaDetalle);

    private List<FranjaSlot> ObtenerFranjas(int idSede, DateOnly desde, DateOnly hasta, List<Turno> turnosSede)
    {
        var porOrden = new Dictionary<int, FranjaSlot>();

        for (var fecha = desde; fecha <= hasta; fecha = fecha.AddDays(1))
        {
            var plantilla = _plantillas.GetVigente(idSede, fecha);
            if (plantilla == null) continue;

            foreach (var detalle in plantilla.Detalles.Where(d => DiasSemanaHelper.IncluyeDia(d.DiasSemana, fecha)))
            {
                if (!porOrden.ContainsKey(detalle.Orden))
                {
                    porOrden[detalle.Orden] = new FranjaSlot(
                        detalle.Orden,
                        detalle.NombreFranja,
                        detalle.HoraInicio,
                        detalle.HoraFin,
                        detalle.IdDetalle);
                }
            }
        }

        var siguienteOrden = porOrden.Count == 0 ? 0 : porOrden.Keys.Max() + 1;
        foreach (var turno in turnosSede.Where(t => t.Fecha >= desde && t.Fecha <= hasta))
        {
            if (turno.IdPlantillaDetalle.HasValue &&
                porOrden.Values.Any(s => s.IdPlantillaDetalle == turno.IdPlantillaDetalle))
                continue;

            if (porOrden.Values.Any(s => s.HoraInicio == turno.HoraInicio && s.HoraFin == turno.HoraFin))
                continue;

            porOrden[siguienteOrden] = new FranjaSlot(
                siguienteOrden,
                $"Turno {turno.HoraInicio}-{turno.HoraFin}",
                turno.HoraInicio,
                turno.HoraFin,
                turno.IdPlantillaDetalle);
            siguienteOrden++;
        }

        return porOrden.Values.OrderBy(s => s.Orden).ToList();
    }

    private PlantillaTurnoDetalle? ObtenerDetalleVigente(int idSede, DateOnly fecha, FranjaSlot slot)
    {
        var plantilla = _plantillas.GetVigente(idSede, fecha);
        if (plantilla == null) return null;

        if (slot.IdPlantillaDetalle.HasValue)
        {
            var porId = plantilla.Detalles.FirstOrDefault(d => d.IdDetalle == slot.IdPlantillaDetalle);
            if (porId != null && DiasSemanaHelper.IncluyeDia(porId.DiasSemana, fecha))
                return porId;
        }

        return plantilla.Detalles.FirstOrDefault(d =>
            DiasSemanaHelper.IncluyeDia(d.DiasSemana, fecha) &&
            d.HoraInicio == slot.HoraInicio &&
            d.HoraFin == slot.HoraFin);
    }

    private static Turno? BuscarTurno(
        List<Turno> turnosSede,
        DateOnly fecha,
        FranjaSlot slot,
        PlantillaTurnoDetalle? detalle) =>
        turnosSede.FirstOrDefault(t =>
            t.Fecha == fecha &&
            (t.IdPlantillaDetalle == slot.IdPlantillaDetalle ||
             (t.HoraInicio == slot.HoraInicio && t.HoraFin == slot.HoraFin) ||
             (detalle != null && t.IdPlantillaDetalle == detalle.IdDetalle)));

    private static PlanillaCelda ConstruirCeldaAsignacion(Turno? turno, PlantillaTurnoDetalle? detalle)
    {
        if (turno == null && detalle == null)
            return new PlanillaCelda();

        if (turno?.Cancelado == true)
            return new PlanillaCelda { TextoSuperior = "-", EsCancelado = true };

        var asignacion = turno?.Asignacions.FirstOrDefault();
        if (asignacion == null)
            return new PlanillaCelda();

        return new PlanillaCelda
        {
            TextoSuperior = asignacion.IdEmpleadoNavigation.Codigo ?? "?",
            TextoInferior = PlanillaFormatoHelper.FormatearHorarioPlanilla(turno!.HoraInicio, turno.HoraFin)
        };
    }

    private decimal CalcularHorasDiarias(
        int idSede,
        DateOnly fecha,
        List<FranjaSlot> slots,
        List<Turno> turnosSede)
    {
        decimal total = 0;

        foreach (var slot in slots)
        {
            var detalle = ObtenerDetalleVigente(idSede, fecha, slot);
            var turno = BuscarTurno(turnosSede, fecha, slot, detalle);

            if (turno == null && detalle == null)
                continue;

            if (turno != null)
            {
                if (!turno.Cancelado)
                    total += turno.CantHoras ?? 0;
            }
            else if (detalle != null)
            {
                var hi = TurnoCalculoHelper.ParseHora(detalle.HoraInicio);
                var hf = TurnoCalculoHelper.ParseHora(detalle.HoraFin);
                var fechaFin = detalle.CruzaDiaSiguiente || hf <= hi ? fecha.AddDays(1) : fecha;
                total += TurnoCalculoHelper.CalcularHoras(fecha, hi, fechaFin, hf);
            }
        }

        return total;
    }
}
