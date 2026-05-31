namespace ElArteServicios.Models;

public partial class Turno
{
    public int IdTurno { get; set; }

    public int IdSede { get; set; }

    public DateOnly Fecha { get; set; }

    public DateOnly FechaFin { get; set; }

    public string HoraInicio { get; set; } = null!;

    public string HoraFin { get; set; } = null!;

    public decimal? CantHoras { get; set; }

    public TurnoOrigen Origen { get; set; } = TurnoOrigen.Manual;

    public int? IdPlantillaDetalle { get; set; }

    public bool BloqueadoRegeneracion { get; set; }

    public bool Cancelado { get; set; }

    public virtual Sede IdSedeNavigation { get; set; } = null!;

    public virtual PlantillaTurnoDetalle? IdPlantillaDetalleNavigation { get; set; }

    public virtual ICollection<Asignacion> Asignacions { get; set; } = new List<Asignacion>();

    public bool CruzaMedianoche => FechaFin > Fecha;
}
