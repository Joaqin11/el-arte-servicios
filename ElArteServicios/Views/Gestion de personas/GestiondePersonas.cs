using ElArteServicios.Models;
using ElArteServicios.UI;

namespace ElArteServicios.Views.Gestion_de_personas;

public class GestionDePersona : GestionFormBase
{
    private TextBox txtCodigo = null!;
    private TextBox txtNombre = null!;
    private TextBox txtApellido = null!;
    private DataGridView dgv = null!;
    private Button btnLicencias = null!;
    private Button btnGuardar = null!;
    private Button btnModificar = null!;
    private Button btnEliminar = null!;
    private Button btnLimpiar = null!;
    private Button btnCerrar = null!;

    public GestionDePersona() : base("Empleados / Vigiladores", "Alta, modificación y baja de personal")
    {
        ConstruirInterfaz();
        LimpiarFormulario();
        CargarLista();
        EstablecerEstado("Listo.");
    }

    private void ConstruirInterfaz()
    {
        var pnlForm = new Panel { Dock = DockStyle.Top, Height = 120 };
        var gbx = new GroupBox { Text = "Datos del vigilador", Dock = DockStyle.Fill, Padding = new Padding(12) };

        var lblCodigo = new Label { Text = "Código:", AutoSize = true, Location = new Point(16, 32) };
        txtCodigo = new TextBox { Location = new Point(80, 28), Width = 120 };

        var lblNombre = new Label { Text = "Nombre:", AutoSize = true, Location = new Point(220, 32) };
        txtNombre = new TextBox { Location = new Point(290, 28), Width = 180 };

        var lblApellido = new Label { Text = "Apellido:", AutoSize = true, Location = new Point(490, 32) };
        txtApellido = new TextBox { Location = new Point(560, 28), Width = 180 };

        gbx.Controls.AddRange(new Control[] { lblCodigo, txtCodigo, lblNombre, txtNombre, lblApellido, txtApellido });
        pnlForm.Controls.Add(gbx);

        var pnlBotones = CrearPanelBotones(out btnGuardar, out btnModificar, out btnEliminar, out btnLimpiar, out btnCerrar);
        btnLicencias = new Button { Text = "Licencias", Width = 100 };
        UiTheme.ConfigurarBotonSecundario(btnLicencias);
        btnLicencias.Enabled = false;
        btnLicencias.Click += (_, _) => AbrirLicencias();
        pnlBotones.Controls.Add(btnLicencias);
        pnlBotones.Controls.SetChildIndex(btnLicencias, 0);

        foreach (var btn in new[] { btnGuardar, btnModificar, btnEliminar, btnLimpiar, btnCerrar })
            btn.Click += Boton_Click;

        dgv = new DataGridView();
        UiTheme.ConfigurarGrilla(dgv);
        dgv.SelectionChanged += (_, _) => CargarSeleccion();

        var lblLista = new Label
        {
            Text = "Listado de empleados",
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
            Services.EmpleadoService.CrearEmpleado(txtCodigo.Text, txtNombre.Text, txtApellido.Text);
            EstablecerEstado("Empleado guardado correctamente.");
            LimpiarFormulario();
            CargarLista();
        }
        catch (Exception ex) { MostrarError(ex); EstablecerEstado(ex.Message, true); }
    }

    private void Modificar()
    {
        if (!IdSeleccionado.HasValue) { MostrarInfo("Seleccione un empleado de la lista."); return; }
        try
        {
            Services.EmpleadoService.ActualizarEmpleado(IdSeleccionado.Value, txtCodigo.Text, txtNombre.Text, txtApellido.Text);
            EstablecerEstado("Empleado actualizado.");
            LimpiarFormulario();
            CargarLista();
        }
        catch (Exception ex) { MostrarError(ex); EstablecerEstado(ex.Message, true); }
    }

    private void Eliminar()
    {
        if (!IdSeleccionado.HasValue) { MostrarInfo("Seleccione un empleado de la lista."); return; }
        if (!Confirmar("¿Eliminar el empleado seleccionado?")) return;
        try
        {
            Services.EmpleadoService.EliminarEmpleado(IdSeleccionado.Value);
            EstablecerEstado("Empleado eliminado.");
            LimpiarFormulario();
            CargarLista();
        }
        catch (Exception ex) { MostrarError(ex); EstablecerEstado(ex.Message, true); }
    }

    private void CargarLista()
    {
        var datos = Services.EmpleadoService.ObtenerEmpleados()
            .Select(e => new
            {
                e.IdEmpleado,
                e.Codigo,
                e.Nombre,
                e.Apellido
            }).ToList();

        dgv.DataSource = datos;
        if (dgv.Columns.Contains("IdEmpleado")) dgv.Columns["IdEmpleado"].Visible = false;
    }

    private void CargarSeleccion()
    {
        if (dgv.CurrentRow?.Cells["IdEmpleado"].Value is not int id) return;
        var emp = Services.EmpleadoService.ObtenerEmpleadoPorId(id);
        if (emp == null) return;
        IdSeleccionado = id;
        txtCodigo.Text = emp.Codigo ?? "";
        txtNombre.Text = emp.Nombre ?? "";
        txtApellido.Text = emp.Apellido ?? "";
        btnGuardar.Enabled = false;
        btnModificar.Enabled = true;
        btnEliminar.Enabled = true;
        btnLicencias.Enabled = true;
    }

    private void AbrirLicencias()
    {
        if (!IdSeleccionado.HasValue) return;
        var nombre = $"{txtNombre.Text} {txtApellido.Text}".Trim();
        using var frm = new GestionLicenciasEmpleado(IdSeleccionado.Value, nombre);
        frm.ShowDialog();
    }

    private void LimpiarFormulario()
    {
        IdSeleccionado = null;
        txtCodigo.Clear();
        txtNombre.Clear();
        txtApellido.Clear();
        btnGuardar.Enabled = true;
        btnModificar.Enabled = false;
        btnEliminar.Enabled = false;
        btnLicencias.Enabled = false;
        dgv.ClearSelection();
    }
}
