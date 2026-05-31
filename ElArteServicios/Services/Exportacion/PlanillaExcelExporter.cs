using ClosedXML.Excel;
using ElArteServicios.Services.Exportacion;

namespace ElArteServicios.Services;

public static class PlanillaExcelExporter
{
    private static readonly XLColor ColorEncabezado = XLColor.FromHtml("#293541");
    private static readonly XLColor ColorFinDeSemana = XLColor.FromHtml("#C6EFCE");
    private static readonly XLColor ColorAlerta = XLColor.FromHtml("#FFC7CE");

    public static void Exportar(PlanillaExportData data, string rutaArchivo)
    {
        using var workbook = new XLWorkbook();
        EscribirPlanillaOperativa(workbook, data);
        EscribirResumen(workbook, data);
        workbook.SaveAs(rutaArchivo);
    }

    private static void EscribirPlanillaOperativa(XLWorkbook workbook, PlanillaExportData data)
    {
        var ws = workbook.Worksheets.Add("Planilla Operativa");
        ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
        ws.PageSetup.PaperSize = XLPaperSize.A4Paper;
        ws.PageSetup.FitToPages(1, 0);

        var fila = 1;
        ws.Cell(fila, 1).Value = "PLANILLA OPERATIVA — El Arte Servicios";
        ws.Range(fila, 1, fila, 3 + data.Columnas.Count).Merge().Style
            .Font.SetBold().Font.SetFontSize(14).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        fila++;

        ws.Cell(fila, 1).Value = $"Período: {data.Desde:dd/MM/yyyy} al {data.Hasta:dd/MM/yyyy}";
        ws.Range(fila, 1, fila, 3 + data.Columnas.Count).Merge();
        fila += 2;

        foreach (var bloque in data.Sedes)
        {
            fila = EscribirBloqueSede(ws, bloque, data.Columnas, fila);
            fila += 2;
        }

        if (data.Notas.Count > 0)
        {
            ws.Cell(fila, 1).Value = "NOTAS";
            ws.Cell(fila, 1).Style.Font.SetBold();
            fila++;
            foreach (var nota in data.Notas)
            {
                ws.Cell(fila, 1).Value = nota;
                ws.Range(fila, 1, fila, 3 + data.Columnas.Count).Merge();
                fila++;
            }
        }

        ws.Columns().AdjustToContents(1, 8);
        ws.Column(1).Width = 22;
    }

    private static int EscribirBloqueSede(
        IXLWorksheet ws,
        PlanillaBloqueSede bloque,
        List<PlanillaDiaColumna> columnas,
        int filaInicio)
    {
        var fila = filaInicio;
        var colTotal = 2 + columnas.Count;

        ws.Cell(fila, 1).Value = $"OBJETIVO / PORTERÍA {bloque.NombreSede.ToUpperInvariant()}";
        ws.Range(fila, 1, fila, colTotal).Merge().Style
            .Font.SetBold()
            .Fill.SetBackgroundColor(ColorEncabezado)
            .Font.SetFontColor(XLColor.White);
        fila++;

        ws.Cell(fila, 1).Value = "";
        ws.Cell(fila, colTotal).Value = "TOTAL HORAS";
        ws.Cell(fila, colTotal).Style.Font.SetBold().Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        for (var i = 0; i < columnas.Count; i++)
        {
            var col = columnas[i];
            var celda = ws.Cell(fila, 2 + i);
            celda.Value = col.Encabezado;
            celda.Style.Font.SetBold().Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            if (col.EsFinDeSemana)
                celda.Style.Fill.SetBackgroundColor(ColorFinDeSemana);
        }
        fila++;

        foreach (var filaHorario in bloque.FilasHorario)
        {
            ws.Cell(fila, 1).Value = filaHorario.Etiqueta;
            ws.Cell(fila, 1).Style.Font.SetBold();
            for (var i = 0; i < columnas.Count; i++)
            {
                var fecha = columnas[i].Fecha;
                var celda = ws.Cell(fila, 2 + i);
                if (filaHorario.HorariosPorDia.TryGetValue(fecha, out var horario))
                    celda.Value = horario;
                celda.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                if (columnas[i].EsFinDeSemana)
                    celda.Style.Fill.SetBackgroundColor(ColorFinDeSemana);
            }
            fila++;
        }

        ws.Cell(fila, 1).Value = "Cant de horas";
        ws.Cell(fila, 1).Style.Font.SetBold();
        for (var i = 0; i < columnas.Count; i++)
        {
            var fecha = columnas[i].Fecha;
            var celda = ws.Cell(fila, 2 + i);
            if (bloque.HorasDiarias.TryGetValue(fecha, out var horas) && horas > 0)
                celda.Value = horas;
            celda.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetBold();
            if (columnas[i].EsFinDeSemana)
                celda.Style.Fill.SetBackgroundColor(ColorFinDeSemana);
        }
        fila++;

        foreach (var filaAsig in bloque.FilasAsignacion)
        {
            ws.Cell(fila, 1).Value = filaAsig.EtiquetaFranja;
            for (var i = 0; i < columnas.Count; i++)
            {
                var fecha = columnas[i].Fecha;
                var celda = ws.Cell(fila, 2 + i);
                if (filaAsig.CeldasPorDia.TryGetValue(fecha, out var planillaCelda))
                    EscribirCeldaAsignacion(celda, planillaCelda);
                if (columnas[i].EsFinDeSemana)
                    celda.Style.Fill.SetBackgroundColor(ColorFinDeSemana);
            }
            fila++;
        }

        ws.Cell(filaInicio + 1, colTotal).Value = bloque.TotalHorasAsignadas;
        ws.Cell(filaInicio + 1, colTotal).Style.Font.SetBold().Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        ws.Range(filaInicio + 1, colTotal, fila - 1, colTotal).Merge().Style
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        return fila;
    }

    private static void EscribirCeldaAsignacion(IXLCell celda, PlanillaCelda planillaCelda)
    {
        celda.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        celda.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

        if (string.IsNullOrEmpty(planillaCelda.TextoSuperior) && string.IsNullOrEmpty(planillaCelda.TextoInferior))
            return;

        if (!string.IsNullOrEmpty(planillaCelda.TextoInferior))
        {
            celda.Value = planillaCelda.TextoSuperior + Environment.NewLine + planillaCelda.TextoInferior;
            celda.Style.Alignment.SetWrapText(true);
        }
        else
        {
            celda.Value = planillaCelda.TextoSuperior;
        }
    }

    private static void EscribirResumen(XLWorkbook workbook, PlanillaExportData data)
    {
        var ws = workbook.Worksheets.Add("Resumen");
        ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
        ws.PageSetup.PaperSize = XLPaperSize.A4Paper;

        var fila = 1;
        ws.Cell(fila, 1).Value = "RESUMEN DE HORAS ASIGNADAS";
        ws.Range(fila, 1, fila, 4).Merge().Style.Font.SetBold().Font.SetFontSize(13);
        fila++;
        ws.Cell(fila, 1).Value = $"Período: {data.Desde:dd/MM/yyyy} al {data.Hasta:dd/MM/yyyy}";
        fila += 2;

        ws.Cell(fila, 1).Value = "Por vigilador";
        ws.Cell(fila, 1).Style.Font.SetBold().Font.SetFontSize(11);
        fila++;

        ws.Cell(fila, 1).Value = "Código";
        ws.Cell(fila, 2).Value = "Nombre";
        ws.Cell(fila, 3).Value = "Horas";
        ws.Cell(fila, 4).Value = "Jornadas";
        ws.Range(fila, 1, fila, 4).Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.LightGray);
        fila++;

        foreach (var v in data.Vigiladores)
        {
            ws.Cell(fila, 1).Value = v.Codigo;
            ws.Cell(fila, 2).Value = v.Nombre;
            ws.Cell(fila, 3).Value = v.Horas;
            ws.Cell(fila, 4).Value = v.Jornadas;
            fila++;
        }

        ws.Cell(fila, 2).Value = "Total vigiladores:";
        ws.Cell(fila, 2).Style.Font.SetBold();
        ws.Cell(fila, 3).Value = data.TotalHorasVigiladores;
        ws.Cell(fila, 3).Style.Font.SetBold();
        fila += 2;

        ws.Cell(fila, 1).Value = "Por sede / objetivo";
        ws.Cell(fila, 1).Style.Font.SetBold().Font.SetFontSize(11);
        fila++;

        ws.Cell(fila, 1).Value = "Sede";
        ws.Cell(fila, 2).Value = "Horas asignadas";
        ws.Range(fila, 1, fila, 2).Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.LightGray);
        fila++;

        foreach (var s in data.ResumenSedes)
        {
            ws.Cell(fila, 1).Value = s.Nombre;
            ws.Cell(fila, 2).Value = s.Horas;
            fila++;
        }

        ws.Cell(fila, 1).Value = "Total sedes:";
        ws.Cell(fila, 1).Style.Font.SetBold();
        ws.Cell(fila, 2).Value = data.TotalHorasSedes;
        ws.Cell(fila, 2).Style.Font.SetBold();
        fila += 2;

        var celdaControl = ws.Cell(fila, 1);
        celdaControl.Value = data.TotalesCuadran
            ? "Control: totales cuadrados (vigiladores = sedes)."
            : $"Control: ATENCIÓN — no cuadran (vigiladores: {data.TotalHorasVigiladores}, sedes: {data.TotalHorasSedes}).";
        celdaControl.Style.Font.SetBold();
        if (!data.TotalesCuadran)
            celdaControl.Style.Fill.SetBackgroundColor(ColorAlerta);

        ws.Columns().AdjustToContents();
    }
}
