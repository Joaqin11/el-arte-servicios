using ElArteServicios.Infrastructure;
using ElArteServicios.UI;

namespace ElArteServicios.Views.Exportacion;

public class ExportarPlanillaDialog : Form
{
    private readonly AppServices _services;
    private DateTimePicker dtpDesde = null!;
    private DateTimePicker dtpHasta = null!;
    private Button btnExportar = null!;
    private Button btnCancelar = null!;

    public ExportarPlanillaDialog(AppServices? services = null)
    {
        _services = services ?? AppServices.Create();
        _ownServices = services == null;

        Text = "Exportar planilla operativa";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(420, 200);
        Font = new Font("Segoe UI", 9F);

        ConstruirInterfaz();

        var hoy = DateTime.Today;
        dtpDesde.Value = new DateTime(hoy.Year, hoy.Month, 1);
        dtpHasta.Value = hoy;
    }

    private readonly bool _ownServices;

    private void ConstruirInterfaz()
    {
        var lblTitulo = new Label
        {
            Text = "Seleccioná el rango de fechas a exportar",
            Location = new Point(16, 16),
            AutoSize = true,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };

        var lblDesde = new Label { Text = "Desde:", Location = new Point(16, 56), AutoSize = true };
        dtpDesde = new DateTimePicker { Location = new Point(80, 52), Width = 120, Format = DateTimePickerFormat.Short };

        var lblHasta = new Label { Text = "Hasta:", Location = new Point(220, 56), AutoSize = true };
        dtpHasta = new DateTimePicker { Location = new Point(270, 52), Width = 120, Format = DateTimePickerFormat.Short };

        var lblInfo = new Label
        {
            Text = "Se exportan todas las sedes en un archivo Excel (.xlsx) con 2 hojas.",
            Location = new Point(16, 88),
            Size = new Size(380, 36),
            ForeColor = Color.FromArgb(80, 80, 80)
        };

        btnExportar = new Button { Text = "Exportar…", Location = new Point(200, 140), Size = new Size(100, 32) };
        btnCancelar = new Button { Text = "Cancelar", Location = new Point(310, 140), Size = new Size(90, 32) };
        UiTheme.ConfigurarBotonPrimario(btnExportar);
        UiTheme.ConfigurarBotonSecundario(btnCancelar);

        btnExportar.Click += BtnExportar_Click;
        btnCancelar.Click += (_, _) => Close();

        Controls.AddRange(new Control[]
        {
            lblTitulo, lblDesde, dtpDesde, lblHasta, dtpHasta, lblInfo, btnExportar, btnCancelar
        });
    }

    private void BtnExportar_Click(object? sender, EventArgs e)
    {
        var desde = DateOnly.FromDateTime(dtpDesde.Value.Date);
        var hasta = DateOnly.FromDateTime(dtpHasta.Value.Date);

        if (hasta < desde)
        {
            MessageBox.Show("La fecha hasta debe ser posterior o igual a la fecha desde.", "Validación",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var dlg = new SaveFileDialog
        {
            Filter = "Excel (*.xlsx)|*.xlsx",
            FileName = $"Planilla_{desde:yyyyMMdd}_{hasta:yyyyMMdd}.xlsx",
            Title = "Guardar planilla operativa"
        };

        if (dlg.ShowDialog() != DialogResult.OK)
            return;

        try
        {
            _services.PlanillaExportService.ExportarExcel(desde, hasta, dlg.FileName);
            MessageBox.Show($"Planilla exportada correctamente.\n\n{dlg.FileName}", "Exportación",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error al exportar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        if (_ownServices)
            _services.Dispose();
        base.OnFormClosed(e);
    }
}
