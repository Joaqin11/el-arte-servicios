namespace ElArteServicios.Services.Exportacion;

public class PlanillaCelda
{
    public string TextoSuperior { get; set; } = "";
    public string TextoInferior { get; set; } = "";
    public bool EsCancelado { get; set; }
}

public class PlanillaDiaColumna
{
    public DateOnly Fecha { get; set; }
    public string Encabezado { get; set; } = "";
    public bool EsFinDeSemana { get; set; }
}

public class PlanillaFilaHorario
{
    public string Etiqueta { get; set; } = "";
    public Dictionary<DateOnly, string> HorariosPorDia { get; set; } = new();
}

public class PlanillaFilaAsignacion
{
    public string EtiquetaFranja { get; set; } = "";
    public Dictionary<DateOnly, PlanillaCelda> CeldasPorDia { get; set; } = new();
}

public class PlanillaBloqueSede
{
    public string NombreSede { get; set; } = "";
    public List<PlanillaFilaHorario> FilasHorario { get; set; } = new();
    public Dictionary<DateOnly, decimal> HorasDiarias { get; set; } = new();
    public List<PlanillaFilaAsignacion> FilasAsignacion { get; set; } = new();
    public decimal TotalHorasAsignadas { get; set; }
}

public class PlanillaResumenVigilador
{
    public string Codigo { get; set; } = "";
    public string Nombre { get; set; } = "";
    public decimal Horas { get; set; }
    public int Jornadas { get; set; }
}

public class PlanillaResumenSede
{
    public string Nombre { get; set; } = "";
    public decimal Horas { get; set; }
}

public class PlanillaExportData
{
    public DateOnly Desde { get; set; }
    public DateOnly Hasta { get; set; }
    public List<PlanillaDiaColumna> Columnas { get; set; } = new();
    public List<PlanillaBloqueSede> Sedes { get; set; } = new();
    public List<PlanillaResumenVigilador> Vigiladores { get; set; } = new();
    public List<PlanillaResumenSede> ResumenSedes { get; set; } = new();
    public decimal TotalHorasVigiladores { get; set; }
    public decimal TotalHorasSedes { get; set; }
    public bool TotalesCuadran { get; set; }
    public List<string> Notas { get; set; } = new();
}
