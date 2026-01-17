using System;
using System.Collections.Generic;

namespace ElArteServicios.Models;

public partial class Asignacion
{
    public int IdAsignacion { get; set; }

    public int IdEmpleado { get; set; }

    public int IdTurno { get; set; }

    public int IdSede { get; set; }

    public string? Notas { get; set; }

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    public virtual Sede IdSedeNavigation { get; set; } = null!;

    public virtual Turno IdTurnoNavigation { get; set; } = null!;
}
