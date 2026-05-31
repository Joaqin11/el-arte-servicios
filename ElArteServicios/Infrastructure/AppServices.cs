using ElArteServicios.Data;
using ElArteServicios.Repositories;
using ElArteServicios.Services;

namespace ElArteServicios.Infrastructure;

public sealed class AppServices : IDisposable
{
    public ServiciosContext Context { get; }

    public EmpleadoRepository Empleados { get; }
    public SedeRepository Sedes { get; }
    public TurnoRepository Turnos { get; }
    public AsignacionRepository Asignaciones { get; }
    public PlantillaTurnoRepository PlantillasTurno { get; }
    public LicenciaEmpleadoRepository LicenciasEmpleado { get; }

    public EmpleadoService EmpleadoService { get; }
    public SedeService SedeService { get; }
    public TurnoService TurnoService { get; }
    public AsignacionService AsignacionService { get; }
    public PlantillaTurnoService PlantillaTurnoService { get; }
    public TurnoGeneradorService TurnoGeneradorService { get; }
    public LicenciaEmpleadoService LicenciaEmpleadoService { get; }
    public PlanillaExportService PlanillaExportService { get; }

    public AppServices()
    {
        Context = new ServiciosContext();
        DatabaseInitializer.EnsureDatabase(Context);

        Empleados = new EmpleadoRepository(Context);
        Sedes = new SedeRepository(Context);
        Turnos = new TurnoRepository(Context);
        Asignaciones = new AsignacionRepository(Context);
        PlantillasTurno = new PlantillaTurnoRepository(Context);
        LicenciasEmpleado = new LicenciaEmpleadoRepository(Context);

        EmpleadoService = new EmpleadoService(Empleados);
        SedeService = new SedeService(Sedes);
        TurnoService = new TurnoService(Turnos, Sedes);
        AsignacionService = new AsignacionService(Asignaciones, Turnos, LicenciasEmpleado);
        PlantillaTurnoService = new PlantillaTurnoService(PlantillasTurno, Sedes);
        TurnoGeneradorService = new TurnoGeneradorService(PlantillasTurno, Turnos);
        LicenciaEmpleadoService = new LicenciaEmpleadoService(LicenciasEmpleado);
        PlanillaExportService = new PlanillaExportService(Sedes, Turnos, Asignaciones, PlantillasTurno, LicenciasEmpleado);
    }

    public static AppServices Create() => new();

    public void Dispose() => Context.Dispose();
}
