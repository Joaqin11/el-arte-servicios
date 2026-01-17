using System;
using System.Collections.Generic;

namespace ElArteServicios.Models;

public partial class Turno
{
    public int IdTurno { get; set; }

    public DateOnly Fecha { get; set; }

    public string HoraInicio { get; set; } = null!;

    public string HoraFin { get; set; } = null!;

    public decimal? CantHoras { get; set; }

    public virtual ICollection<Asignacion> Asignacions { get; set; } = new List<Asignacion>();
}
