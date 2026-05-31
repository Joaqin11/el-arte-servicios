using ElArteServicios.Data;
using ElArteServicios.Models;
using Microsoft.EntityFrameworkCore;

namespace ElArteServicios.Repositories;

public class PlantillaTurnoRepository
{
    private readonly ServiciosContext _context;

    public PlantillaTurnoRepository(ServiciosContext context)
    {
        _context = context;
    }

    public List<PlantillaTurno> GetBySede(int idSede) =>
        _context.PlantillasTurno
            .Include(p => p.Detalles.OrderBy(d => d.Orden))
            .Where(p => p.IdSede == idSede)
            .OrderByDescending(p => p.VigenciaDesde)
            .ToList();

    public PlantillaTurno? GetById(int id) =>
        _context.PlantillasTurno
            .Include(p => p.Detalles.OrderBy(d => d.Orden))
            .FirstOrDefault(p => p.IdPlantilla == id);

    public PlantillaTurno? GetVigente(int idSede, DateOnly fecha) =>
        _context.PlantillasTurno
            .Include(p => p.Detalles.OrderBy(d => d.Orden))
            .Where(p => p.IdSede == idSede && p.Activa)
            .Where(p => p.VigenciaDesde <= fecha)
            .Where(p => p.VigenciaHasta == null || p.VigenciaHasta >= fecha)
            .OrderByDescending(p => p.VigenciaDesde)
            .FirstOrDefault();

    public void Add(PlantillaTurno plantilla)
    {
        _context.PlantillasTurno.Add(plantilla);
        _context.SaveChanges();
    }

    public void Update(PlantillaTurno plantilla)
    {
        _context.PlantillasTurno.Update(plantilla);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var plantilla = _context.PlantillasTurno.Find(id);
        if (plantilla != null)
        {
            _context.PlantillasTurno.Remove(plantilla);
            _context.SaveChanges();
        }
    }

    public void GuardarDetalles(int idPlantilla, List<PlantillaTurnoDetalle> detalles)
    {
        var existentes = _context.PlantillasTurnoDetalle.Where(d => d.IdPlantilla == idPlantilla).ToList();
        _context.PlantillasTurnoDetalle.RemoveRange(existentes);

        for (var i = 0; i < detalles.Count; i++)
        {
            detalles[i].IdPlantilla = idPlantilla;
            detalles[i].Orden = i;
            detalles[i].IdDetalle = 0;
        }

        _context.PlantillasTurnoDetalle.AddRange(detalles);
        _context.SaveChanges();
    }
}
