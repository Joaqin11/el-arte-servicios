using ElArteServicios.UI;

namespace ElArteServicios.Views.Gestion_de_sedes;

public class GestiondeSedes : GestionFormBase
{
    private TextBox txtNombre = null!;
    private DataGridView dgv = null!;
    private Button btnGuardar = null!;
    private Button btnModificar = null!;
    private Button btnEliminar = null!;
    private Button btnLimpiar = null!;
    private Button btnCerrar = null!;

    public GestiondeSedes() : base("Gestión de Sedes", "Sucursales y ubicaciones de servicio")
    {
        ConstruirInterfaz();
        LimpiarFormulario();
        CargarLista();
        EstablecerEstado("Listo.");
    }

    private void ConstruirInterfaz()
    {
        var pnlForm = new Panel { Dock = DockStyle.Top, Height = 90 };
        var gbx = new GroupBox { Text = "Datos de la sede", Dock = DockStyle.Fill, Padding = new Padding(12) };
        var lblNombre = new Label { Text = "Nombre:", AutoSize = true, Location = new Point(16, 32) };
        txtNombre = new TextBox { Location = new Point(90, 28), Width = 400 };
        gbx.Controls.AddRange(new Control[] { lblNombre, txtNombre });
        pnlForm.Controls.Add(gbx);

        var pnlBotones = CrearPanelBotones(out btnGuardar, out btnModificar, out btnEliminar, out btnLimpiar, out btnCerrar);
        foreach (var btn in new[] { btnGuardar, btnModificar, btnEliminar, btnLimpiar, btnCerrar })
            btn.Click += Boton_Click;

        dgv = new DataGridView();
        UiTheme.ConfigurarGrilla(dgv);
        dgv.SelectionChanged += (_, _) => CargarSeleccion();

        var lblLista = new Label
        {
            Text = "Listado de sedes",
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

    private void Boton_Click(object? sender, EventArgs e)
    {
        if (sender == btnGuardar) Guardar();
        else if (sender == btnModificar) Modificar();
        else if (sender == btnEliminar) Eliminar();
        else if (sender == btnLimpiar) LimpiarFormulario();
        else if (sender == btnCerrar) Close();
    }

    private void Guardar()
    {
        try
        {
            Services.SedeService.CrearSede(txtNombre.Text);
            EstablecerEstado("Sede guardada correctamente.");
            LimpiarFormulario();
            CargarLista();
        }
        catch (Exception ex) { MostrarError(ex); EstablecerEstado(ex.Message, true); }
    }

    private void Modificar()
    {
        if (!IdSeleccionado.HasValue) { MostrarInfo("Seleccione una sede de la lista."); return; }
        try
        {
            Services.SedeService.ActualizarSede(IdSeleccionado.Value, txtNombre.Text);
            EstablecerEstado("Sede actualizada.");
            LimpiarFormulario();
            CargarLista();
        }
        catch (Exception ex) { MostrarError(ex); EstablecerEstado(ex.Message, true); }
    }

    private void Eliminar()
    {
        if (!IdSeleccionado.HasValue) { MostrarInfo("Seleccione una sede de la lista."); return; }
        if (!Confirmar("¿Eliminar la sede seleccionada?")) return;
        try
        {
            Services.SedeService.EliminarSede(IdSeleccionado.Value);
            EstablecerEstado("Sede eliminada.");
            LimpiarFormulario();
            CargarLista();
        }
        catch (Exception ex) { MostrarError(ex); EstablecerEstado(ex.Message, true); }
    }

    private void CargarLista()
    {
        dgv.DataSource = Services.SedeService.ObtenerSedes()
            .Select(s => new { s.IdSede, s.Nombre }).ToList();
        if (dgv.Columns.Contains("IdSede")) dgv.Columns["IdSede"].Visible = false;
    }

    private void CargarSeleccion()
    {
        if (dgv.CurrentRow?.Cells["IdSede"].Value is not int id) return;
        var sede = Services.SedeService.ObtenerSedePorId(id);
        if (sede == null) return;
        IdSeleccionado = id;
        txtNombre.Text = sede.Nombre;
        btnGuardar.Enabled = false;
        btnModificar.Enabled = true;
        btnEliminar.Enabled = true;
    }

    private void LimpiarFormulario()
    {
        IdSeleccionado = null;
        txtNombre.Clear();
        btnGuardar.Enabled = true;
        btnModificar.Enabled = false;
        btnEliminar.Enabled = false;
        dgv.ClearSelection();
    }
}
