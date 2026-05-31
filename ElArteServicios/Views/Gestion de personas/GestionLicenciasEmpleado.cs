using ElArteServicios.UI;

namespace ElArteServicios.Views.Gestion_de_personas;

public class GestionLicenciasEmpleado : GestionFormBase
{
    private readonly int _idEmpleado;
    private readonly string _nombreEmpleado;
    private DataGridView dgv = null!;
    private DateTimePicker dtpDesde = null!;
    private DateTimePicker dtpHasta = null!;
    private TextBox txtMotivo = null!;
    private Button btnAgregar = null!;
    private Button btnEliminar = null!;
    private Button btnCerrar = null!;

    public GestionLicenciasEmpleado(int idEmpleado, string nombreEmpleado)
        : base("Licencias", $"Indisponibilidad de {nombreEmpleado}")
    {
        _idEmpleado = idEmpleado;
        _nombreEmpleado = nombreEmpleado;
        ConstruirInterfaz();
        CargarLista();
        EstablecerEstado("Las licencias bloquean nuevas asignaciones en el período indicado.");
    }

    private void ConstruirInterfaz()
    {
        var pnlForm = new Panel { Dock = DockStyle.Top, Height = 100 };
        var gbx = new GroupBox { Text = "Nueva licencia", Dock = DockStyle.Fill, Padding = new Padding(12) };

        var lblDesde = new Label { Text = "Desde:", AutoSize = true, Location = new Point(16, 32) };
        dtpDesde = new DateTimePicker { Location = new Point(70, 28), Width = 110, Format = DateTimePickerFormat.Short };

        var lblHasta = new Label { Text = "Hasta:", AutoSize = true, Location = new Point(200, 32) };
        dtpHasta = new DateTimePicker { Location = new Point(250, 28), Width = 110, Format = DateTimePickerFormat.Short };

        var lblMotivo = new Label { Text = "Motivo:", AutoSize = true, Location = new Point(380, 32) };
        txtMotivo = new TextBox { Location = new Point(440, 28), Width = 280 };

        gbx.Controls.AddRange(new Control[] { lblDesde, dtpDesde, lblHasta, dtpHasta, lblMotivo, txtMotivo });
        pnlForm.Controls.Add(gbx);

        var pnlBotones = new Panel { Dock = DockStyle.Top, Height = 48, Padding = new Padding(0, 8, 0, 0) };
        btnAgregar = new Button { Text = "Agregar licencia" };
        btnEliminar = new Button { Text = "Eliminar seleccionada" };
        btnCerrar = new Button { Text = "Cerrar" };
        UiTheme.AplicarBarraBotones(btnAgregar, btnEliminar, btnCerrar);
        btnAgregar.Click += (_, _) => Agregar();
        btnEliminar.Click += (_, _) => Eliminar();
        btnCerrar.Click += (_, _) => Close();
        pnlBotones.Controls.AddRange(new Control[] { btnAgregar, btnEliminar, btnCerrar });

        dgv = new DataGridView();
        UiTheme.ConfigurarGrilla(dgv);

        var lblLista = new Label
        {
            Text = $"Licencias de {_nombreEmpleado}",
            Dock = DockStyle.Top,
            Height = 24,
            Font = new Font("Segoe UI", 9.75F, FontStyle.Bold)
        };

        var pnlGrid = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 8, 0, 0) };
        pnlGrid.Controls.Add(dgv);
        pnlGrid.Controls.Add(lblLista);

        pnlContenido.Controls.Add(pnlGrid);
        pnlContenido.Controls.Add(pnlBotones);
        pnlContenido.Controls.Add(pnlForm);
    }

    private void Agregar()
    {
        try
        {
            var desde = DateOnly.FromDateTime(dtpDesde.Value);
            var hasta = DateOnly.FromDateTime(dtpHasta.Value);
            Services.LicenciaEmpleadoService.CrearLicencia(_idEmpleado, desde, hasta, txtMotivo.Text);
            txtMotivo.Clear();
            CargarLista();
            EstablecerEstado("Licencia registrada.");
        }
        catch (Exception ex)
        {
            MostrarError(ex);
            EstablecerEstado(ex.Message, true);
        }
    }

    private void Eliminar()
    {
        if (dgv.CurrentRow?.Cells["IdLicencia"].Value is not int id)
        {
            MostrarInfo("Seleccioná una licencia de la lista.");
            return;
        }

        if (!Confirmar("¿Eliminar la licencia seleccionada?")) return;

        try
        {
            Services.LicenciaEmpleadoService.EliminarLicencia(id);
            CargarLista();
            EstablecerEstado("Licencia eliminada.");
        }
        catch (Exception ex)
        {
            MostrarError(ex);
            EstablecerEstado(ex.Message, true);
        }
    }

    private void CargarLista()
    {
        dgv.DataSource = Services.LicenciaEmpleadoService.ObtenerPorEmpleado(_idEmpleado)
            .Select(l => new
            {
                l.IdLicencia,
                Desde = l.Desde.ToString("dd/MM/yyyy"),
                Hasta = l.Hasta.ToString("dd/MM/yyyy"),
                Motivo = l.Motivo ?? ""
            }).ToList();

        if (dgv.Columns.Contains("IdLicencia"))
            dgv.Columns["IdLicencia"].Visible = false;
    }
}
