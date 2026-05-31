using ElArteServicios.Models;
using ElArteServicios.Repositories;

namespace ElArteServicios.Services;

public class PlantillaTurnoService
{
    private readonly PlantillaTurnoRepository _repo;
    private readonly SedeRepository _sedes;

    public PlantillaTurnoService(PlantillaTurnoRepository repo, SedeRepository sedes)
    {
        _repo = repo;
        _sedes = sedes;
    }

    public List<PlantillaTurno> ObtenerPorSede(int idSede) => _repo.GetBySede(idSede);

    public PlantillaTurno? ObtenerPorId(int id) => _repo.GetById(id);

    public PlantillaTurno? ObtenerVigente(int idSede, DateOnly fecha) => _repo.GetVigente(idSede, fecha);

    public int GuardarPlantilla(PlantillaTurno plantilla, List<PlantillaTurnoDetalle> detalles)
    {
        ValidarPlantilla(plantilla, detalles);

        if (plantilla.IdPlantilla == 0)
        {
            _repo.Add(plantilla);
        }
        else
        {
            _repo.Update(plantilla);
        }

        _repo.GuardarDetalles(plantilla.IdPlantilla, detalles);
        return plantilla.IdPlantilla;
    }

    public void EliminarPlantilla(int id)
    {
        if (_repo.GetById(id) == null)
            throw new InvalidOperationException("Plantilla no encontrada.");
        _repo.Delete(id);
    }

    public List<PlantillaTurnoDetalle> CrearDetalles24Horas() =>
    [
        new PlantillaTurnoDetalle
        {
            NombreFranja = "Mañana",
            DiasSemana = (int)DiasSemanaFlags.Todos,
            HoraInicio = "07:00",
            HoraFin = "15:00",
            CruzaDiaSiguiente = false
        },
        new PlantillaTurnoDetalle
        {
            NombreFranja = "Tarde",
            DiasSemana = (int)DiasSemanaFlags.Todos,
            HoraInicio = "15:00",
            HoraFin = "23:00",
            CruzaDiaSiguiente = false
        },
        new PlantillaTurnoDetalle
        {
            NombreFranja = "Noche",
            DiasSemana = (int)DiasSemanaFlags.Todos,
            HoraInicio = "23:00",
            HoraFin = "07:00",
            CruzaDiaSiguiente = true
        }
    ];

    private void ValidarPlantilla(PlantillaTurno plantilla, List<PlantillaTurnoDetalle> detalles)
    {
        if (_sedes.GetById(plantilla.IdSede) == null)
            throw new InvalidOperationException("La sede no existe.");

        if (string.IsNullOrWhiteSpace(plantilla.Nombre))
            throw new ArgumentException("El nombre de la plantilla es obligatorio.");

        if (plantilla.VigenciaHasta.HasValue && plantilla.VigenciaHasta < plantilla.VigenciaDesde)
            throw new ArgumentException("La vigencia hasta no puede ser anterior a la vigencia desde.");

        if (detalles.Count == 0)
            throw new ArgumentException("Agregá al menos una franja horaria.");

        foreach (var d in detalles)
        {
            if (string.IsNullOrWhiteSpace(d.NombreFranja))
                throw new ArgumentException("Cada franja debe tener un nombre.");
            if (d.DiasSemana == 0)
                throw new ArgumentException($"La franja '{d.NombreFranja}' debe incluir al menos un día.");
            _ = TurnoCalculoHelper.ParseHora(d.HoraInicio);
            _ = TurnoCalculoHelper.ParseHora(d.HoraFin);
        }
    }
}
