using System;
using System.Collections.Generic;

namespace ElArteServicios.Models;

public partial class Empleado
{
    public int IdEmpleado { get; set; }

    public string? Codigo { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public virtual ICollection<Asignacion> Asignacions { get; set; } = new List<Asignacion>();
}
