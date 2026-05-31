using ElArteServicios.Services;
using ElArteServicios.UI;

namespace ElArteServicios.Views.Gestion_de_asignaciones;

public class GestiondeAsignaciones : GestionFormBase
{
    private ComboBox cboEmpleado = null!;
    private ComboBox cboSede = null!;
    private ComboBox cboTurno = null!;
    private TextBox txtNotas = null!;
    private DataGridView dgv = null!;
    private Button btnGuardar = null!;
    private Button btnModificar = null!;
    private Button btnEliminar = null!;
    private Button btnLimpiar = null!;
    private Button btnCerrar = null!;
    private GroupBox gbxForm = null!;
    private bool _cargandoSeleccion;
    private bool _suprimirSeleccionGrid;

    public GestiondeAsignaciones() : base("Gestión de Asignaciones", "Asignar vigiladores a los turnos disponibles de cada sede")
    {
        ConstruirInterfaz();
        CargarCombos();
        LimpiarFormulario();
        CargarLista();
        EstablecerEstado("Elegí sede, turno y empleado. Cada turno admite un vigilador; distintos empleados pueden cubrir distintos horarios el mismo día.");
    }

    private void ConstruirInterfaz()
    {
        var pnlForm = new Panel { Dock = DockStyle.Top, Height = 130 };
        gbxForm = new GroupBox { Text = "Nueva asignación", Dock = DockStyle.Fill, Padding = new Padding(12) };

        var lblEmp = new Label { Text = "Empleado:", AutoSize = true, Location = new Point(16, 30) };
        cboEmpleado = new ComboBox { Location = new Point(90, 26), Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };
        cboEmpleado.SelectedIndexChanged += (_, _) => DetectarModoNuevo();

        var lblSede = new Label { Text = "Sede:", AutoSize = true, Location = new Point(370, 30) };
        cboSede = new ComboBox { Location = new Point(410, 26), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
        cboSede.SelectedIndexChanged += (_, _) => { CargarTurnosPorSede(); CargarListaPorSede(); };

        var lblTurno = new Label { Text = "Turno:", AutoSize = true, Location = new Point(16, 68) };
        cboTurno = new ComboBox { Location = new Point(90, 64), Width = 520, DropDownStyle = ComboBoxStyle.DropDownList };
        cboTurno.SelectedIndexChanged += (_, _) => DetectarModoNuevo();

        var lblNotas = new Label { Text = "Notas:", AutoSize = true, Location = new Point(630, 68) };
        txtNotas = new TextBox { Location = new Point(680, 64), Width = 180 };
        txtNotas.TextChanged += (_, _) => DetectarModoNuevo();

        gbxForm.Controls.AddRange(new Control[]
        {
            lblEmp, cboEmpleado, lblSede, cboSede, lblTurno, cboTurno, lblNotas, txtNotas
        });
        pnlForm.Controls.Add(gbxForm);

        var pnlBotones = CrearPanelBotones(out btnGuardar, out btnModificar, out btnEliminar, out btnLimpiar, out btnCerrar);
        btnLimpiar.Text = "Nuevo";
        foreach (var btn in new[] { btnGuardar, btnModificar, btnEliminar, btnLimpiar, btnCerrar })
            btn.Click += Boton_Click;

        dgv = new DataGridView();
        UiTheme.ConfigurarGrilla(dgv);
        dgv.SelectionChanged += (_, _) => CargarSeleccion();

        var lblLista = new Label
        {
            Text = "Asignaciones de la sede seleccionada",
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

    /// <summary>
    /// Si hay una fila seleccionada y el usuario cambia datos, pasa a modo alta (Guardar habilitado).
    /// </summary>
    private void DetectarModoNuevo()
    {
        if (_cargandoSeleccion || !IdSeleccionado.HasValue) return;

        var actual = Services.AsignacionService.ObtenerAsignacionPorId(IdSeleccionado.Value);
        if (actual == null) return;

        var empleadoId = ObtenerComboInt(cboEmpleado);
        var turnoId = ObtenerComboInt(cboTurno);
        var notas = txtNotas.Text.Trim();

        var cambio = empleadoId != actual.IdEmpleado
            || turnoId != actual.IdTurno
            || notas != (actual.Notas ?? "").Trim();

        if (!cambio) return;

        IdSeleccionado = null;
        btnGuardar.Enabled = true;
        btnModificar.Enabled = false;
        btnEliminar.Enabled = false;
        gbxForm.Text = "Nueva asignación";
        dgv.ClearSelection();
        EstablecerEstado("Creando asignación nueva. Usá Guardar para confirmar.");
    }

    private void CargarCombos()
    {
        var empleados = Services.EmpleadoService.ObtenerEmpleados();
        cboEmpleado.DisplayMember = "Display";
        cboEmpleado.ValueMember = "IdEmpleado";
        cboEmpleado.DataSource = empleados.Select(e => new
        {
            e.IdEmpleado,
            Display = $"{e.Codigo} - {e.Nombre} {e.Apellido}"
        }).ToList();

        var sedes = Services.SedeService.ObtenerSedes();
        cboSede.DisplayMember = "Nombre";
        cboSede.ValueMember = "IdSede";
        cboSede.DataSource = sedes;

        CargarTurnosPorSede();
    }

    private void CargarTurnosPorSede()
    {
        if (cboSede.SelectedValue is not int idSede)
        {
            cboTurno.DataSource = null;
            return;
        }

        var turnos = Services.TurnoService.ObtenerTurnosPorSede(idSede);
        var asignaciones = Services.AsignacionService.ObtenerAsignaciones()
            .Where(a => a.IdSede == idSede)
            .ToList();

        cboTurno.DisplayMember = "Display";
        cboTurno.ValueMember = "IdTurno";
        cboTurno.DataSource = turnos.Select(t =>
        {
            var ocupado = asignaciones.FirstOrDefault(a => a.IdTurno == t.IdTurno);
            var estado = ocupado == null
                ? "Disponible"
                : $"Ocupado: {ocupado.IdEmpleadoNavigation.Codigo}";
            return new
            {
                t.IdTurno,
                Display = $"[{AsignacionReglasHelper.NombreFranja(AsignacionReglasHelper.Clasificar(t))}] {TurnoService.DescribirTurno(t)} — {estado}"
            };
        }).ToList();

        if (turnos.Count == 0)
            EstablecerEstado("Esta sede no tiene turnos. Configurálos en Gestión de Turnos.", esError: true);
        else
            EstablecerEstado($"{turnos.Count} turno(s) en la sede. Podés asignar un vigilador distinto a cada horario.");
    }

    private void Boton_Click(object? sender, EventArgs e)
    {
        if (sender == btnGuardar) Guardar();
        else if (sender == btnModificar) Modificar();
        else if (sender == btnEliminar) Eliminar();
        else if (sender == btnLimpiar) LimpiarFormulario();
        else if (sender == btnCerrar) Close();
    }

    private static int ObtenerComboInt(ComboBox combo)
    {
        if (combo.SelectedValue is int id) return id;
        if (combo.SelectedValue != null && int.TryParse(combo.SelectedValue.ToString(), out var parsed))
            return parsed;
        throw new InvalidOperationException($"Seleccione un valor válido en {combo.Name}.");
    }

    private void Guardar()
    {
        if (!ValidarSeleccionCombos()) return;
        try
        {
            Services.AsignacionService.CrearAsignacion(
                ObtenerComboInt(cboEmpleado),
                ObtenerComboInt(cboTurno),
                ObtenerComboInt(cboSede),
                txtNotas.Text);

            FinalizarOperacion(
                "Asignación guardada correctamente. Verificá la fila en la grilla inferior.");
        }
        catch (Exception ex) { MostrarError(ex); EstablecerEstado(ex.Message, true); }
    }

    private void Modificar()
    {
        if (!IdSeleccionado.HasValue) { MostrarInfo("Seleccioná una asignación de la lista para modificar."); return; }
        if (!ValidarSeleccionCombos()) return;
        try
        {
            Services.AsignacionService.ActualizarAsignacion(
                IdSeleccionado.Value,
                ObtenerComboInt(cboEmpleado),
                ObtenerComboInt(cboTurno),
                ObtenerComboInt(cboSede),
                txtNotas.Text);

            FinalizarOperacion("Asignación actualizada. La grilla se refrescó sin seleccionar filas.");
        }
        catch (Exception ex) { MostrarError(ex); EstablecerEstado(ex.Message, true); }
    }

    private void Eliminar()
    {
        if (!IdSeleccionado.HasValue) { MostrarInfo("Seleccioná una asignación de la lista."); return; }
        if (!Confirmar("¿Eliminar la asignación seleccionada?")) return;
        try
        {
            Services.AsignacionService.EliminarAsignacion(IdSeleccionado.Value);
            FinalizarOperacion("Asignación eliminada.");
        }
        catch (Exception ex) { MostrarError(ex); EstablecerEstado(ex.Message, true); }
    }

    /// <summary>
    /// Tras guardar/modificar/eliminar: refresca grilla y combos sin auto-seleccionar la primera fila.
    /// </summary>
    private void FinalizarOperacion(string mensaje)
    {
        _suprimirSeleccionGrid = true;
        try
        {
            LimpiarFormulario();
            CargarListaPorSede();
            CargarTurnosPorSede();
            EstablecerEstado($"{mensaje} Usá «Nuevo» para otra asignación o hacé clic en una fila para modificar.");
        }
        finally
        {
            _suprimirSeleccionGrid = false;
        }
    }

    private bool ValidarSeleccionCombos()
    {
        if (cboEmpleado.SelectedValue == null || cboSede.SelectedValue == null || cboTurno.SelectedValue == null)
        {
            MostrarInfo("Seleccioná empleado, sede y turno.");
            return false;
        }
        return true;
    }

    private void CargarLista() => CargarListaPorSede();

    private void CargarListaPorSede()
    {
        var asignaciones = Services.AsignacionService.ObtenerAsignaciones();
        if (cboSede.SelectedValue is int idSede)
            asignaciones = asignaciones.Where(a => a.IdSede == idSede).ToList();

        dgv.DataSource = asignaciones
            .Select(a => new
            {
                a.IdAsignacion,
                Empleado = $"{a.IdEmpleadoNavigation.Codigo} - {a.IdEmpleadoNavigation.Nombre} {a.IdEmpleadoNavigation.Apellido}",
                Sede = a.IdSedeNavigation.Nombre,
                Franja = AsignacionReglasHelper.NombreFranja(AsignacionReglasHelper.Clasificar(a.IdTurnoNavigation)),
                Turno = TurnoService.DescribirTurno(a.IdTurnoNavigation),
                Notas = a.Notas ?? ""
            }).ToList();

        if (dgv.Columns.Contains("IdAsignacion")) dgv.Columns["IdAsignacion"].Visible = false;

        if (_suprimirSeleccionGrid)
            dgv.ClearSelection();
    }

    private void CargarSeleccion()
    {
        if (_suprimirSeleccionGrid || dgv.SelectedRows.Count == 0) return;
        if (dgv.CurrentRow?.Cells["IdAsignacion"].Value is not int id) return;
        var a = Services.AsignacionService.ObtenerAsignacionPorId(id);
        if (a == null) return;

        _cargandoSeleccion = true;
        IdSeleccionado = id;
        cboEmpleado.SelectedValue = a.IdEmpleado;
        cboSede.SelectedValue = a.IdSede;
        CargarTurnosPorSede();
        cboTurno.SelectedValue = a.IdTurno;
        txtNotas.Text = a.Notas ?? "";
        _cargandoSeleccion = false;

        btnGuardar.Enabled = false;
        btnModificar.Enabled = true;
        btnEliminar.Enabled = true;
        gbxForm.Text = "Modificar asignación";
        EstablecerEstado("Asignación seleccionada. Usá Modificar para cambiarla o Nuevo para cargar otra.");
    }

    private void LimpiarFormulario()
    {
        IdSeleccionado = null;
        txtNotas.Clear();
        btnGuardar.Enabled = true;
        btnModificar.Enabled = false;
        btnEliminar.Enabled = false;
        gbxForm.Text = "Nueva asignación";
        dgv.ClearSelection();
        EstablecerEstado("Completá empleado, sede y turno, luego Guardar. Para editar una existente, seleccioná una fila en la grilla.");
    }
}
