namespace ElArteServicios.Models;

public partial class PlantillaTurnoDetalle
{
    public int IdDetalle { get; set; }

    public int IdPlantilla { get; set; }

    public string NombreFranja { get; set; } = null!;

    /// <summary>Bitmask de días (Lun=1, Mar=2, …, Dom=64).</summary>
    public int DiasSemana { get; set; }

    public string HoraInicio { get; set; } = null!;

    public string HoraFin { get; set; } = null!;

    public bool CruzaDiaSiguiente { get; set; }

    public int Orden { get; set; }

    public virtual PlantillaTurno IdPlantillaNavigation { get; set; } = null!;
}
