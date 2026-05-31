using ElArteServicios.Services;
using ElArteServicios.UI;
using ElArteServicios.Views.Gestion_de_plantillas;

namespace ElArteServicios.Views.Gestion_de_turnos;

public class GestiondeTurnos : GestionFormBase
{
    private Button btnParametrizacion = null!;
    private ComboBox cboSede = null!;
    private DateTimePicker dtpFechaInicio = null!;
    private DateTimePicker dtpFechaFin = null!;
    private DateTimePicker dtpHoraInicio = null!;
    private DateTimePicker dtpHoraFin = null!;
    private Label lblHorasCalculadas = null!;
    private Label lblResumenTurno = null!;
    private CheckBox chkTerminaDiaSiguiente = null!;
    private CheckBox chkCancelado = null!;
    private DataGridView dgv = null!;
    private Button btnGuardar = null!;
    private Button btnModificar = null!;
    private Button btnEliminar = null!;
    private Button btnLimpiar = null!;
    private Button btnCerrar = null!;
    private bool _actualizandoFechas;
    private bool _cargandoSeleccion;

    public GestiondeTurnos() : base("Gestión de Turnos", "Turnos concretos por sede. Excepciones manuales no se pisan al regenerar.")
    {
        ConstruirInterfaz();
        CargarSedes();
        LimpiarFormulario();
        CargarLista();
        EstablecerEstado("Usá Parametrización para generar turnos automáticos, o cargá excepciones acá.");
    }

    private void ConstruirInterfaz()
    {
        var pnlForm = new Panel { Dock = DockStyle.Top, Height = 330 };
        btnParametrizacion = new Button { Text = "Parametrización / Generar", Width = 180, Height = 28 };
        UiTheme.ConfigurarBotonPrimario(btnParametrizacion);
        btnParametrizacion.Height = 28;
        btnParametrizacion.Click += (_, _) =>
        {
            using var frm = new GestionPlantillasTurnos();
            frm.ShowDialog();
            CargarSedes();
            CargarLista();
        };
        var pnlParam = new Panel { Dock = DockStyle.Top, Height = 36, Padding = new Padding(0, 0, 0, 6) };
        pnlParam.Controls.Add(btnParametrizacion);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 7,
            Padding = new Padding(8, 6, 8, 6)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));  // sede
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));  // título inicio
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));  // inicio
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));  // título fin
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));  // fin
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));  // checkbox sereno
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));  // duración y resumen

        // Fila 0: Sede
        layout.Controls.Add(new Label { Text = "Sede:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 0);
        cboSede = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
        layout.SetColumnSpan(cboSede, 3);
        layout.Controls.Add(cboSede, 1, 0);
        cboSede.SelectedIndexChanged += (_, _) => { CargarLista(); DetectarCambiosFormulario(); };

        // Fila 1: encabezado inicio
        var lblIni = new Label
        {
            Text = "Inicio del turno",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.FromArgb(41, 53, 65),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        layout.SetColumnSpan(lblIni, 4);
        layout.Controls.Add(lblIni, 0, 1);

        // Fila 2: fecha/hora inicio
        layout.Controls.Add(new Label { Text = "Fecha:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 2);
        dtpFechaInicio = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short };
        layout.Controls.Add(dtpFechaInicio, 1, 2);
        layout.Controls.Add(new Label { Text = "Hora:", Anchor = AnchorStyles.Left, AutoSize = true }, 2, 2);
        dtpHoraInicio = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Time, ShowUpDown = true };
        layout.Controls.Add(dtpHoraInicio, 3, 2);

        // Fila 3: encabezado fin
        var lblFin = new Label
        {
            Text = "Fin del turno",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.FromArgb(41, 53, 65),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        layout.SetColumnSpan(lblFin, 4);
        layout.Controls.Add(lblFin, 0, 3);

        // Fila 4: fecha/hora fin
        layout.Controls.Add(new Label { Text = "Fecha:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 4);
        dtpFechaFin = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short };
        layout.Controls.Add(dtpFechaFin, 1, 4);
        layout.Controls.Add(new Label { Text = "Hora:", Anchor = AnchorStyles.Left, AutoSize = true }, 2, 4);
        dtpHoraFin = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Time, ShowUpDown = true };
        layout.Controls.Add(dtpHoraFin, 3, 4);

        // Fila 5: turno nocturno / sereno
        chkTerminaDiaSiguiente = new CheckBox
        {
            Text = "Termina al día siguiente (guardia nocturna o sereno)",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9.75F),
            Padding = new Padding(4, 6, 0, 0),
            AutoSize = false
        };
        chkTerminaDiaSiguiente.CheckedChanged += (_, _) =>
        {
            if (_actualizandoFechas) return;
            if (chkTerminaDiaSiguiente.Checked)
                dtpFechaFin.Value = dtpFechaInicio.Value.Date.AddDays(1);
            else
                dtpFechaFin.Value = dtpFechaInicio.Value.Date;
            ActualizarCalculo();
            DetectarCambiosFormulario();
        };
        layout.SetColumnSpan(chkTerminaDiaSiguiente, 4);
        layout.Controls.Add(chkTerminaDiaSiguiente, 0, 5);

        chkCancelado = new CheckBox
        {
            Text = "Turno cancelado (no se hace — aparece «-» en planilla)",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9.75F),
            Padding = new Padding(4, 0, 0, 0),
            AutoSize = false
        };
        chkCancelado.CheckedChanged += (_, _) => DetectarCambiosFormulario();
        layout.RowCount = 8;
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        layout.SetColumnSpan(chkCancelado, 4);
        layout.Controls.Add(chkCancelado, 0, 7);

        // Fila 6: duración y resumen — row index 6 unchanged
        var pnlResumen = new Panel { Dock = DockStyle.Fill };
        lblHorasCalculadas = new Label
        {
            Text = "Duración: —",
            AutoSize = true,
            Location = new Point(0, 2),
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.FromArgb(52, 152, 219)
        };
        lblResumenTurno = new Label
        {
            Text = "",
            AutoSize = true,
            Location = new Point(0, 26),
            ForeColor = Color.FromArgb(80, 80, 80),
            MaximumSize = new Size(820, 0)
        };
        pnlResumen.Controls.AddRange(new Control[] { lblHorasCalculadas, lblResumenTurno });
        layout.SetColumnSpan(pnlResumen, 4);
        layout.Controls.Add(pnlResumen, 0, 6);

        var gbx = new GroupBox { Text = "Configurar turno", Dock = DockStyle.Fill, Padding = new Padding(4) };
        gbx.Controls.Add(layout);
        pnlForm.Controls.Add(gbx);

        void EnlazarCalculo(Control c)
        {
            if (c is DateTimePicker dtp)
                dtp.ValueChanged += (_, _) => { ActualizarCalculo(); DetectarCambiosFormulario(); };
        }
        EnlazarCalculo(dtpFechaInicio);
        EnlazarCalculo(dtpFechaFin);
        EnlazarCalculo(dtpHoraInicio);
        EnlazarCalculo(dtpHoraFin);

        var pnlBotones = CrearPanelBotones(out btnGuardar, out btnModificar, out btnEliminar, out btnLimpiar, out btnCerrar);
        btnLimpiar.Text = "Nuevo";
        foreach (var btn in new[] { btnGuardar, btnModificar, btnEliminar, btnLimpiar, btnCerrar })
            btn.Click += Boton_Click;

        dgv = new DataGridView();
        UiTheme.ConfigurarGrilla(dgv);
        dgv.SelectionChanged += (_, _) => CargarSeleccion();

        var lblLista = new Label
        {
            Text = "Turnos de la sede seleccionada",
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
        pnlContenido.Controls.Add(pnlParam);
    }

    private void DetectarCambiosFormulario()
    {
        if (_cargandoSeleccion || !IdSeleccionado.HasValue) return;

        var turno = Services.TurnoService.ObtenerTurnoPorId(IdSeleccionado.Value);
        if (turno == null) return;

        var (fi, ff, hi, hf) = LeerFormulario();
        var cambio =
            turno.IdSede != ObtenerIdSede() ||
            turno.Fecha != fi ||
            turno.FechaFin != ff ||
            turno.HoraInicio != TurnoService.FormatearHora(hi) ||
            turno.HoraFin != TurnoService.FormatearHora(hf) ||
            turno.Cancelado != chkCancelado.Checked;

        if (cambio)
        {
            IdSeleccionado = null;
            btnGuardar.Enabled = true;
            btnModificar.Enabled = false;
            btnEliminar.Enabled = false;
            dgv.ClearSelection();
            EstablecerEstado("Datos modificados: se guardará como turno nuevo.");
        }
    }

    private void ActualizarCalculo()
    {
        if (_actualizandoFechas) return;

        try
        {
            _actualizandoFechas = true;

            var fechaInicio = DateOnly.FromDateTime(dtpFechaInicio.Value);
            var horaInicio = dtpHoraInicio.Value.TimeOfDay;
            var horaFin = dtpHoraFin.Value.TimeOfDay;

            if (!chkTerminaDiaSiguiente.Checked && horaFin <= horaInicio)
            {
                dtpFechaFin.Value = dtpFechaInicio.Value.Date.AddDays(1);
                chkTerminaDiaSiguiente.Checked = true;
            }
            else if (!chkTerminaDiaSiguiente.Checked)
            {
                dtpFechaFin.Value = dtpFechaInicio.Value.Date;
            }

            var fechaFin = DateOnly.FromDateTime(dtpFechaFin.Value);
            var horas = TurnoCalculoHelper.CalcularHoras(fechaInicio, horaInicio, fechaFin, horaFin);

            lblHorasCalculadas.Text = $"Duración: {horas:0.#} horas";
            lblHorasCalculadas.ForeColor = Color.FromArgb(52, 152, 219);

            var ini = $"{fechaInicio:dd/MM/yyyy} {TurnoService.FormatearHora(horaInicio)}";
            var fin = $"{fechaFin:dd/MM/yyyy} {TurnoService.FormatearHora(horaFin)}";
            lblResumenTurno.Text = fechaFin > fechaInicio
                ? $"Vas a guardar: del {ini} al {fin}"
                : $"Vas a guardar: {fechaInicio:dd/MM/yyyy} de {TurnoService.FormatearHora(horaInicio)} a {TurnoService.FormatearHora(horaFin)}";
        }
        catch
        {
            lblHorasCalculadas.Text = "Duración: revisá fechas y horas";
            lblHorasCalculadas.ForeColor = Color.DarkRed;
            lblResumenTurno.Text = "";
        }
        finally
        {
            _actualizandoFechas = false;
        }
    }

    private void CargarSedes()
    {
        cboSede.DisplayMember = "Nombre";
        cboSede.ValueMember = "IdSede";
        cboSede.DataSource = Services.SedeService.ObtenerSedes();
    }

    private void Boton_Click(object? sender, EventArgs e)
    {
        if (sender == btnGuardar) Guardar();
        else if (sender == btnModificar) Modificar();
        else if (sender == btnEliminar) Eliminar();
        else if (sender == btnLimpiar) LimpiarFormulario();
        else if (sender == btnCerrar) Close();
    }

    private int ObtenerIdSede() =>
        cboSede.SelectedValue is int id ? id : throw new InvalidOperationException("Seleccione una sede.");

    private (DateOnly fechaInicio, DateOnly fechaFin, TimeSpan horaInicio, TimeSpan horaFin) LeerFormulario() => (
        DateOnly.FromDateTime(dtpFechaInicio.Value),
        DateOnly.FromDateTime(dtpFechaFin.Value),
        dtpHoraInicio.Value.TimeOfDay,
        dtpHoraFin.Value.TimeOfDay
    );

    private void Guardar()
    {
        if (cboSede.SelectedValue == null) { MostrarInfo("Primero creá al menos una sede."); return; }
        try
        {
            var (fi, ff, hi, hf) = LeerFormulario();
            Services.TurnoService.CrearTurno(ObtenerIdSede(), fi, hi, ff, hf, chkCancelado.Checked);
            EstablecerEstado("Turno guardado.");
            LimpiarFormulario();
            CargarLista();
        }
        catch (Exception ex) { MostrarError(ex); EstablecerEstado(ex.Message, true); }
    }

    private void Modificar()
    {
        if (!IdSeleccionado.HasValue) { MostrarInfo("Seleccioná un turno de la lista para modificar."); return; }
        try
        {
            var (fi, ff, hi, hf) = LeerFormulario();
            Services.TurnoService.ActualizarTurno(IdSeleccionado.Value, ObtenerIdSede(), fi, hi, ff, hf, chkCancelado.Checked);
            EstablecerEstado("Turno actualizado.");
            LimpiarFormulario();
            CargarLista();
        }
        catch (Exception ex) { MostrarError(ex); EstablecerEstado(ex.Message, true); }
    }

    private void Eliminar()
    {
        if (!IdSeleccionado.HasValue) { MostrarInfo("Seleccioná un turno de la lista."); return; }
        if (!Confirmar("¿Eliminar el turno seleccionado?")) return;
        try
        {
            Services.TurnoService.EliminarTurno(IdSeleccionado.Value);
            EstablecerEstado("Turno eliminado.");
            LimpiarFormulario();
            CargarLista();
        }
        catch (Exception ex) { MostrarError(ex); EstablecerEstado(ex.Message, true); }
    }

    private void CargarLista()
    {
        if (cboSede.SelectedValue is not int idSede)
        {
            dgv.DataSource = null;
            return;
        }

        dgv.DataSource = Services.TurnoService.ObtenerTurnosPorSede(idSede)
            .Select(t => new
            {
                t.IdTurno,
                Inicio = $"{t.Fecha:dd/MM/yyyy} {t.HoraInicio}",
                Fin = $"{t.FechaFin:dd/MM/yyyy} {t.HoraFin}",
                Horas = t.CantHoras,
                Origen = TurnoService.DescribirOrigen(t),
                Estado = t.Cancelado ? "Cancelado" : "Activo",
                Tipo = t.CruzaMedianoche ? "Nocturno" : "Diurno"
            }).ToList();

        if (dgv.Columns.Contains("IdTurno")) dgv.Columns["IdTurno"].Visible = false;
    }

    private void CargarSeleccion()
    {
        if (dgv.CurrentRow?.Cells["IdTurno"].Value is not int id) return;
        var turno = Services.TurnoService.ObtenerTurnoPorId(id);
        if (turno == null) return;

        _cargandoSeleccion = true;
        _actualizandoFechas = true;
        IdSeleccionado = id;
        cboSede.SelectedValue = turno.IdSede;
        dtpFechaInicio.Value = turno.Fecha.ToDateTime(TimeOnly.MinValue);
        dtpFechaFin.Value = turno.FechaFin.ToDateTime(TimeOnly.MinValue);
        dtpHoraInicio.Value = DateTime.Today.Add(TurnoCalculoHelper.ParseHora(turno.HoraInicio));
        dtpHoraFin.Value = DateTime.Today.Add(TurnoCalculoHelper.ParseHora(turno.HoraFin));
        chkTerminaDiaSiguiente.Checked = turno.CruzaMedianoche;
        chkCancelado.Checked = turno.Cancelado;
        _actualizandoFechas = false;
        _cargandoSeleccion = false;

        ActualizarCalculo();
        btnGuardar.Enabled = false;
        btnModificar.Enabled = true;
        btnEliminar.Enabled = true;
        EstablecerEstado("Turno seleccionado. Modificá y usá «Modificar», o «Nuevo» para cargar otro horario.");
    }

    private void LimpiarFormulario()
    {
        IdSeleccionado = null;
        dtpFechaInicio.Value = DateTime.Today;
        dtpFechaFin.Value = DateTime.Today;
        dtpHoraInicio.Value = DateTime.Today.AddHours(8);
        dtpHoraFin.Value = DateTime.Today.AddHours(16);
        chkTerminaDiaSiguiente.Checked = false;
        chkCancelado.Checked = false;
        ActualizarCalculo();
        btnGuardar.Enabled = true;
        btnModificar.Enabled = false;
        btnEliminar.Enabled = false;
        dgv.ClearSelection();
        EstablecerEstado("Formulario listo para un turno nuevo.");
    }
}
