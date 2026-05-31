namespace ElArteServicios.Models;

public partial class PlantillaTurno
{
    public int IdPlantilla { get; set; }

    public int IdSede { get; set; }

    public string Nombre { get; set; } = null!;

    public DateOnly VigenciaDesde { get; set; }

    public DateOnly? VigenciaHasta { get; set; }

    public bool Activa { get; set; } = true;

    public virtual Sede IdSedeNavigation { get; set; } = null!;

    public virtual ICollection<PlantillaTurnoDetalle> Detalles { get; set; } = new List<PlantillaTurnoDetalle>();
}
