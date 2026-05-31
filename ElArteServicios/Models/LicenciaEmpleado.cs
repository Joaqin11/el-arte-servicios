namespace ElArteServicios.Models;

public partial class LicenciaEmpleado
{
    public int IdLicencia { get; set; }

    public int IdEmpleado { get; set; }

    public DateOnly Desde { get; set; }

    public DateOnly Hasta { get; set; }

    public string? Motivo { get; set; }

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
}
