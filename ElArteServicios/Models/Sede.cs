using System;
using System.Collections.Generic;

namespace ElArteServicios.Models;

public partial class Sede
{
    public int IdSede { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Asignacion> Asignacions { get; set; } = new List<Asignacion>();
}
