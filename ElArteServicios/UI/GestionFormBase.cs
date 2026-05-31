using ElArteServicios.Infrastructure;

namespace ElArteServicios.UI;

public class GestionFormBase : Form
{
    protected readonly AppServices Services;
    protected Panel pnlEncabezado = null!;
    protected Label lblTituloForm = null!;
    protected Label lblSubtituloForm = null!;
    protected StatusStrip statusBar = null!;
    protected ToolStripStatusLabel lblEstado = null!;
    protected Panel pnlContenido = null!;

    protected int? IdSeleccionado { get; set; }

    protected GestionFormBase(string titulo, string subtitulo)
    {
        Services = AppServices.Create();
        Text = titulo;
        MinimumSize = new Size(900, 560);
        StartPosition = FormStartPosition.CenterParent;
        Font = new Font("Segoe UI", 9F);

        CrearChrome(titulo, subtitulo);
        ClientSize = new Size(960, 620);
    }

    private void CrearChrome(string titulo, string subtitulo)
    {
        pnlEncabezado = new Panel();
        lblTituloForm = new Label();
        lblSubtituloForm = new Label();
        UiTheme.AplicarEncabezado(pnlEncabezado, lblTituloForm, lblSubtituloForm, titulo, subtitulo);

        statusBar = new StatusStrip();
        lblEstado = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleLeft };
        statusBar.Items.Add(lblEstado);

        pnlContenido = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12, 10, 12, 10)
        };

        Controls.Add(pnlContenido);
        Controls.Add(statusBar);
        Controls.Add(pnlEncabezado);
    }

    protected Panel CrearPanelBotones(
        out Button btnGuardar,
        out Button btnModificar,
        out Button btnEliminar,
        out Button btnLimpiar,
        out Button btnCerrar)
    {
        var panel = new Panel { Dock = DockStyle.Top, Height = 52, Padding = new Padding(0, 8, 0, 0) };

        btnGuardar = new Button { Text = "Guardar" };
        btnModificar = new Button { Text = "Modificar" };
        btnEliminar = new Button { Text = "Eliminar" };
        btnLimpiar = new Button { Text = "Limpiar" };
        btnCerrar = new Button { Text = "Cerrar" };

        UiTheme.AplicarBarraBotones(btnGuardar, btnModificar, btnEliminar, btnLimpiar, btnCerrar);

        panel.Controls.AddRange(new Control[] { btnGuardar, btnModificar, btnEliminar, btnLimpiar, btnCerrar });
        return panel;
    }

    protected void EstablecerEstado(string mensaje, bool esError = false)
    {
        lblEstado.Text = mensaje;
        lblEstado.ForeColor = esError ? Color.DarkRed : Color.DarkGreen;
    }

    protected static void MostrarError(Exception ex) =>
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    protected static void MostrarInfo(string mensaje) =>
        MessageBox.Show(mensaje, "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

    protected static bool Confirmar(string mensaje) =>
        MessageBox.Show(mensaje, "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        Services.Dispose();
        base.OnFormClosed(e);
    }
}
