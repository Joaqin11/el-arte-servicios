namespace ElArteServicios.Services;

public class ResultadoGeneracionTurnos
{
    public int Creados { get; set; }
    public int OmitidosExistentes { get; set; }
    public int OmitidosBloqueados { get; set; }
    public int DiasSinPlantilla { get; set; }

    public string Resumen =>
        $"Creados: {Creados}  |  Ya existían: {OmitidosExistentes}  |  Bloqueados/excepción: {OmitidosBloqueados}  |  Días sin plantilla: {DiasSinPlantilla}";
}
