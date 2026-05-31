using ElArteServicios.Infrastructure;
using ElArteServicios.Services;
using ElArteServicios.UI;
using ElArteServicios.Views.Exportacion;
using ElArteServicios.Views.Gestion_de_asignaciones;
using ElArteServicios.Views.Gestion_de_personas;
using ElArteServicios.Views.Gestion_de_sedes;
using ElArteServicios.Views.Gestion_de_plantillas;
using ElArteServicios.Views.Gestion_de_turnos;
using Microsoft.EntityFrameworkCore;

namespace ElArteServicios;

public partial class Form1 : Form
{
    private AppServices? _services;

    public Form1()
    {
        InitializeComponent();
        dtpFecha.Value = DateTime.Today;
        UiTheme.ConfigurarGrilla(dgvAsignacionesHoy);
        UiTheme.ConfigurarBotonPrimario(btnNuevaAsignacion);
        UiTheme.ConfigurarBotonSecundario(btnActualizar);
        UiTheme.ConfigurarBotonSecundario(btnExportar);
        UiTheme.ConfigurarBotonSecundario(btnSalir);
        pnlBotonesAcciones.PerformLayout();
    }

    private void Form1_Load(object sender, EventArgs e) => CargarDatos();

    private void CargarDatos()
    {
        try
        {
            _services?.Dispose();
            _services = AppServices.Create();

            lblEstadoDb.Text = "Base de datos: conectada";
            lblEstadoDb.ForeColor = Color.DarkGreen;

            CargarResumen();
            CargarAsignacionesDelDia();
        }
        catch (Exception ex)
        {
            lblEstadoDb.Text = $"Base de datos: error — {ex.Message}";
            lblEstadoDb.ForeColor = Color.DarkRed;
            lblResumen.Text = "No se pudo cargar el resumen.";
            dgvAsignacionesHoy.DataSource = null;
        }
    }

    private void CargarResumen()
    {
        if (_services == null) return;

        var ctx = _services.Context;
        var empleados = ctx.Empleados.Count();
        var sedes = ctx.Sedes.Count();
        var fecha = DateOnly.FromDateTime(dtpFecha.Value);
        var turnosHoy = ctx.Turnos.Count(t => t.Fecha == fecha);
        var asignacionesHoy = ContarAsignacionesDelDia();

        lblResumen.Text =
            $"Empleados: {empleados}  |  Sedes: {sedes}  |  Turnos en la fecha: {turnosHoy}  |  Asignaciones en la fecha: {asignacionesHoy}";
    }

    private int ContarAsignacionesDelDia()
    {
        if (_services == null) return 0;

        var fecha = DateOnly.FromDateTime(dtpFecha.Value);
        return _services.Context.Asignaciones
            .Include(a => a.IdTurnoNavigation)
            .Count(a => a.IdTurnoNavigation.Fecha == fecha);
    }

    private void CargarAsignacionesDelDia()
    {
        if (_services == null) return;

        var fecha = DateOnly.FromDateTime(dtpFecha.Value);

        var asignaciones = _services.Context.Asignaciones
            .Include(a => a.IdEmpleadoNavigation)
            .Include(a => a.IdTurnoNavigation)
            .Include(a => a.IdSedeNavigation)
            .Where(a => a.IdTurnoNavigation.Fecha == fecha)
            .OrderBy(a => a.IdSedeNavigation.Nombre)
            .ThenBy(a => a.IdTurnoNavigation.HoraInicio)
            .Select(a => new
            {
                Empleado = (a.IdEmpleadoNavigation.Nombre ?? "") + " " + (a.IdEmpleadoNavigation.Apellido ?? ""),
                Codigo = a.IdEmpleadoNavigation.Codigo ?? "",
                Sede = a.IdSedeNavigation.Nombre,
                Horario = TurnoService.DescribirRangoHorario(a.IdTurnoNavigation),
                Horas = a.IdTurnoNavigation.CantHoras,
                Notas = a.Notas ?? ""
            })
            .ToList();

        dgvAsignacionesHoy.DataSource = asignaciones;
    }

    private void AbrirEmpleados() => AbrirFormulario(new GestionDePersona());
    private void AbrirSedes() => AbrirFormulario(new GestiondeSedes());
    private void AbrirTurnos() => AbrirFormulario(new GestiondeTurnos());
    private void AbrirPlantillas() => AbrirFormulario(new GestionPlantillasTurnos());
    private void AbrirAsignaciones() => AbrirFormulario(new GestiondeAsignaciones());

    private void AbrirFormulario(Form form)
    {
        form.ShowDialog();
        CargarDatos();
    }

    private void btnEmpleados_Click(object sender, EventArgs e) => AbrirEmpleados();
    private void btnSedes_Click(object sender, EventArgs e) => AbrirSedes();
    private void btnTurnos_Click(object sender, EventArgs e) => AbrirTurnos();
    private void btnAsignaciones_Click(object sender, EventArgs e) => AbrirAsignaciones();
    private void btnNuevaAsignacion_Click(object sender, EventArgs e) => AbrirAsignaciones();

    private void menuEmpleados_Click(object sender, EventArgs e) => AbrirEmpleados();
    private void menuSedes_Click(object sender, EventArgs e) => AbrirSedes();
    private void menuTurnos_Click(object sender, EventArgs e) => AbrirTurnos();
    private void menuPlantillas_Click(object sender, EventArgs e) => AbrirPlantillas();
    private void menuAsignaciones_Click(object sender, EventArgs e) => AbrirAsignaciones();

    private void btnActualizar_Click(object sender, EventArgs e) => CargarDatos();
    private void dtpFecha_ValueChanged(object sender, EventArgs e) => CargarDatos();

    private void menuSalir_Click(object sender, EventArgs e) => Close();

    private void menuExportar_Click(object sender, EventArgs e) => ExportarPlanilla();
    private void btnExportar_Click(object sender, EventArgs e) => ExportarPlanilla();

    private void ExportarPlanilla()
    {
        if (_services == null) return;
        using var dlg = new ExportarPlanillaDialog(_services);
        dlg.ShowDialog();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _services?.Dispose();
        base.OnFormClosed(e);
    }
}
