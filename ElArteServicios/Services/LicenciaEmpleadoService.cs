using ElArteServicios.Models;
using ElArteServicios.Repositories;

namespace ElArteServicios.Services;

public class LicenciaEmpleadoService
{
    private readonly LicenciaEmpleadoRepository _repo;

    public LicenciaEmpleadoService(LicenciaEmpleadoRepository repo)
    {
        _repo = repo;
    }

    public List<LicenciaEmpleado> ObtenerPorEmpleado(int idEmpleado) =>
        _repo.GetByEmpleado(idEmpleado);

    public List<LicenciaEmpleado> ObtenerEnRango(DateOnly desde, DateOnly hasta) =>
        _repo.GetEnRango(desde, hasta);

    public bool EstaEnLicencia(int idEmpleado, DateOnly fecha) =>
        _repo.EstaEnLicencia(idEmpleado, fecha);

    public void CrearLicencia(int idEmpleado, DateOnly desde, DateOnly hasta, string? motivo)
    {
        if (hasta < desde)
            throw new ArgumentException("La fecha hasta debe ser posterior o igual a la fecha desde.");

        _repo.Add(new LicenciaEmpleado
        {
            IdEmpleado = idEmpleado,
            Desde = desde,
            Hasta = hasta,
            Motivo = string.IsNullOrWhiteSpace(motivo) ? null : motivo.Trim()
        });
    }

    public void EliminarLicencia(int id)
    {
        if (_repo.GetById(id) == null)
            throw new InvalidOperationException("Licencia no encontrada.");

        _repo.Delete(id);
    }

    public static string DescribirLicencia(LicenciaEmpleado licencia)
    {
        var codigo = licencia.IdEmpleadoNavigation.Codigo ?? "?";
        var nombre = $"{licencia.IdEmpleadoNavigation.Nombre} {licencia.IdEmpleadoNavigation.Apellido}".Trim();
        var motivo = string.IsNullOrWhiteSpace(licencia.Motivo) ? "licencia" : licencia.Motivo.Trim();
        return $"{codigo} ({nombre}): {licencia.Desde:dd/MM/yyyy} – {licencia.Hasta:dd/MM/yyyy} — {motivo}";
    }
}
