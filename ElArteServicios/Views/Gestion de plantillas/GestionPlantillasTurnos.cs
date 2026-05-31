using ElArteServicios.Models;
using ElArteServicios.Services;
using ElArteServicios.UI;

namespace ElArteServicios.Views.Gestion_de_plantillas;

public class GestionPlantillasTurnos : GestionFormBase
{
    private ComboBox cboSede = null!;
    private DataGridView dgvPlantillas = null!;
    private DataGridView dgvDetalles = null!;
    private TextBox txtNombre = null!;
    private DateTimePicker dtpVigenciaDesde = null!;
    private DateTimePicker dtpVigenciaHasta = null!;
    private CheckBox chkSinFechaFin = null!;
    private CheckBox chkActiva = null!;
    private DateTimePicker dtpGenDesde = null!;
    private DateTimePicker dtpGenHasta = null!;
    private Button btnGuardar = null!;
    private Button btnNueva = null!;
    private Button btnEliminar = null!;
    private Button btnEjemplo24 = null!;
    private Button btnAgregarFranja = null!;
    private Button btnQuitarFranja = null!;
    private Button btnGenerar = null!;
    private Button btnCerrar = null!;

    private int? _idPlantillaSeleccionada;
    private readonly List<PlantillaTurnoDetalle> _detallesEdit = [];

    public GestionPlantillasTurnos() : base("Parametrización de Turnos",
        "Definí horarios base por sede y generá turnos automáticamente")
    {
        ConstruirInterfaz();
        CargarSedes();
        LimpiarFormulario();
        EstablecerEstado("Creá una plantilla con vigencia y franjas horarias, luego generá el mes.");
    }

    private void ConstruirInterfaz()
    {
        var pnlTop = new Panel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(0, 4, 0, 4) };
        pnlTop.Controls.Add(new Label { Text = "Sede:", AutoSize = true, Location = new Point(0, 12) });
        cboSede = new ComboBox { Location = new Point(50, 8), Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };
        cboSede.SelectedIndexChanged += (_, _) => { CargarPlantillas(); LimpiarFormulario(); };
        pnlTop.Controls.Add(cboSede);

        var pnlPlantillas = new Panel { Dock = DockStyle.Top, Height = 140 };
        dgvPlantillas = new DataGridView();
        UiTheme.ConfigurarGrilla(dgvPlantillas);
        dgvPlantillas.Height = 110;
        dgvPlantillas.Dock = DockStyle.Fill;
        dgvPlantillas.SelectionChanged += (_, _) => CargarPlantillaSeleccionada();
        var lblPl = new Label { Text = "Plantillas de la sede", Dock = DockStyle.Top, Height = 22, Font = new Font("Segoe UI", 9.75F, FontStyle.Bold) };
        pnlPlantillas.Controls.Add(dgvPlantillas);
        pnlPlantillas.Controls.Add(lblPl);

        var pnlForm = new Panel { Dock = DockStyle.Top, Height = 100 };
        var gbx = new GroupBox { Text = "Datos de la plantilla", Dock = DockStyle.Fill, Padding = new Padding(10) };
        gbx.Controls.Add(new Label { Text = "Nombre:", Location = new Point(12, 28), AutoSize = true });
        txtNombre = new TextBox { Location = new Point(80, 24), Width = 220 };
        gbx.Controls.Add(txtNombre);
        gbx.Controls.Add(new Label { Text = "Vigencia desde:", Location = new Point(320, 28), AutoSize = true });
        dtpVigenciaDesde = new DateTimePicker { Location = new Point(433, 24), Width = 110, Format = DateTimePickerFormat.Short };
        gbx.Controls.Add(dtpVigenciaDesde);
        gbx.Controls.Add(new Label { Text = "Hasta:", Location = new Point(545, 28), AutoSize = true });
        dtpVigenciaHasta = new DateTimePicker { Location = new Point(595, 24), Width = 110, Format = DateTimePickerFormat.Short };
        gbx.Controls.Add(dtpVigenciaHasta);
        chkSinFechaFin = new CheckBox { Text = "Sin fecha de fin", Location = new Point(710, 26), AutoSize = true, Checked = true };
        chkSinFechaFin.CheckedChanged += (_, _) => dtpVigenciaHasta.Enabled = !chkSinFechaFin.Checked;
        gbx.Controls.Add(chkSinFechaFin);
        chkActiva = new CheckBox { Text = "Activa", Location = new Point(12, 58), AutoSize = true, Checked = true };
        gbx.Controls.Add(chkActiva);
        pnlForm.Controls.Add(gbx);

        var pnlDet = new Panel { Dock = DockStyle.Top, Height = 160 };
        dgvDetalles = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        btnAgregarFranja = new Button { Text = "+ Franja", Width = 90, Location = new Point(0, 0) };
        btnQuitarFranja = new Button { Text = "− Franja", Width = 90, Location = new Point(100, 0) };
        btnEjemplo24 = new Button { Text = "Ejemplo 24 hs", Width = 120, Location = new Point(200, 0) };
        UiTheme.ConfigurarBotonSecundario(btnAgregarFranja);
        UiTheme.ConfigurarBotonSecundario(btnQuitarFranja);
        UiTheme.ConfigurarBotonSecundario(btnEjemplo24);
        btnAgregarFranja.Click += (_, _) => AgregarFranjaVacia();
        btnQuitarFranja.Click += (_, _) => QuitarFranja();
        btnEjemplo24.Click += (_, _) => CargarEjemplo24();
        var pnlDetBtn = new Panel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(0, 4, 0, 4) };
        pnlDetBtn.Controls.AddRange(new Control[] { btnAgregarFranja, btnQuitarFranja, btnEjemplo24 });
        var lblDet = new Label { Text = "Franjas horarias (días y horarios)", Dock = DockStyle.Top, Height = 22, Font = new Font("Segoe UI", 9.75F, FontStyle.Bold) };
        pnlDet.Controls.Add(dgvDetalles);
        pnlDet.Controls.Add(pnlDetBtn);
        pnlDet.Controls.Add(lblDet);
        dgvDetalles.CellDoubleClick += (_, _) => EditarFranjaSeleccionada();

        var pnlGen = new Panel { Dock = DockStyle.Top, Height = 52 };
        var gbxGen = new GroupBox { Text = "Generar turnos", Dock = DockStyle.Fill, Padding = new Padding(10, 8, 10, 8) };
        gbxGen.Controls.Add(new Label { Text = "Desde:", Location = new Point(12, 22), AutoSize = true });
        dtpGenDesde = new DateTimePicker { Location = new Point(66, 18), Width = 110, Format = DateTimePickerFormat.Short };
        gbxGen.Controls.Add(dtpGenDesde);
        gbxGen.Controls.Add(new Label { Text = "Hasta:", Location = new Point(193, 22), AutoSize = true });
        dtpGenHasta = new DateTimePicker { Location = new Point(243, 18), Width = 110, Format = DateTimePickerFormat.Short };
        gbxGen.Controls.Add(dtpGenHasta);
        btnGenerar = new Button { Text = "Generar turnos", Location = new Point(370, 16), Width = 140 };
        UiTheme.ConfigurarBotonPrimario(btnGenerar);
        btnGenerar.Click += (_, _) => GenerarTurnos();
        gbxGen.Controls.Add(btnGenerar);
        pnlGen.Controls.Add(gbxGen);

        var pnlBotones = CrearPanelBotones(out btnGuardar, out _, out btnEliminar, out btnNueva, out btnCerrar);
        btnGuardar.Text = "Guardar plantilla";
        btnNueva.Text = "Nueva";
        btnEliminar.Text = "Eliminar plantilla";
        btnGuardar.Click += (_, _) => GuardarPlantilla();
        btnNueva.Click += (_, _) => LimpiarFormulario();
        btnEliminar.Click += (_, _) => EliminarPlantilla();
        btnCerrar.Click += (_, _) => Close();

        var pnlSpacer = new Panel { Dock = DockStyle.Fill };

        pnlContenido.Controls.Add(pnlSpacer);
        pnlContenido.Controls.Add(pnlBotones);
        pnlContenido.Controls.Add(pnlGen);
        pnlContenido.Controls.Add(pnlDet);
        pnlContenido.Controls.Add(pnlForm);
        pnlContenido.Controls.Add(pnlPlantillas);
        pnlContenido.Controls.Add(pnlTop);

        ClientSize = new Size(980, 680);
        MinimumSize = new Size(900, 640);
    }

    private void CargarSedes()
    {
        cboSede.DisplayMember = "Nombre";
        cboSede.ValueMember = "IdSede";
        cboSede.DataSource = Services.SedeService.ObtenerSedes();
    }

    private int ObtenerIdSede() =>
        cboSede.SelectedValue is int id ? id : throw new InvalidOperationException("Seleccione una sede.");

    private void CargarPlantillas()
    {
        if (cboSede.SelectedValue is not int idSede) return;
        dgvPlantillas.DataSource = Services.PlantillaTurnoService.ObtenerPorSede(idSede)
            .Select(p => new
            {
                p.IdPlantilla,
                p.Nombre,
                Desde = p.VigenciaDesde.ToString("dd/MM/yyyy"),
                Hasta = p.VigenciaHasta?.ToString("dd/MM/yyyy") ?? "Indefinida",
                Activa = p.Activa ? "Sí" : "No",
                Franjas = p.Detalles.Count
            }).ToList();
        if (dgvPlantillas.Columns.Contains("IdPlantilla"))
            dgvPlantillas.Columns["IdPlantilla"].Visible = false;
    }

    private void CargarPlantillaSeleccionada()
    {
        if (dgvPlantillas.CurrentRow?.Cells["IdPlantilla"].Value is not int id) return;
        var p = Services.PlantillaTurnoService.ObtenerPorId(id);
        if (p == null) return;

        _idPlantillaSeleccionada = id;
        txtNombre.Text = p.Nombre;
        dtpVigenciaDesde.Value = p.VigenciaDesde.ToDateTime(TimeOnly.MinValue);
        if (p.VigenciaHasta.HasValue)
        {
            chkSinFechaFin.Checked = false;
            dtpVigenciaHasta.Value = p.VigenciaHasta.Value.ToDateTime(TimeOnly.MinValue);
        }
        else
        {
            chkSinFechaFin.Checked = true;
        }
        chkActiva.Checked = p.Activa;
        _detallesEdit.Clear();
        _detallesEdit.AddRange(p.Detalles.Select(d => new PlantillaTurnoDetalle
        {
            IdDetalle = d.IdDetalle,
            IdPlantilla = d.IdPlantilla,
            NombreFranja = d.NombreFranja,
            DiasSemana = d.DiasSemana,
            HoraInicio = d.HoraInicio,
            HoraFin = d.HoraFin,
            CruzaDiaSiguiente = d.CruzaDiaSiguiente,
            Orden = d.Orden
        }));
        RefrescarGrillaDetalles();
        EstablecerEstado($"Plantilla «{p.Nombre}» cargada.");
    }

    private void RefrescarGrillaDetalles()
    {
        dgvDetalles.DataSource = _detallesEdit
            .Select((d, i) => new
            {
                d.NombreFranja,
                Dias = DiasSemanaHelper.Describir(d.DiasSemana),
                Horario = $"{d.HoraInicio} → {d.HoraFin}" + (d.CruzaDiaSiguiente ? " (+1 día)" : ""),
                Index = i
            }).ToList();
    }

    private void AgregarFranjaVacia()
    {
        _detallesEdit.Add(new PlantillaTurnoDetalle
        {
            NombreFranja = "Nueva franja",
            DiasSemana = (int)DiasSemanaFlags.Todos,
            HoraInicio = "08:00",
            HoraFin = "16:00"
        });
        RefrescarGrillaDetalles();
        EditarFranjaSeleccionada();
    }

    private void QuitarFranja()
    {
        if (dgvDetalles.CurrentRow?.Cells["Index"].Value is not int idx) return;
        if (idx < 0 || idx >= _detallesEdit.Count) return;
        _detallesEdit.RemoveAt(idx);
        RefrescarGrillaDetalles();
    }

    private void EditarFranjaSeleccionada()
    {
        if (dgvDetalles.CurrentRow?.Cells["Index"].Value is not int idx) return;
        if (idx < 0 || idx >= _detallesEdit.Count) return;
        var d = _detallesEdit[idx];

        using var dlg = new Form
        {
            Text = "Editar franja horaria",
            Size = new Size(499, 320),
            StartPosition = FormStartPosition.CenterParent,
            Font = Font
        };
        var txtNom = new TextBox { Text = d.NombreFranja, Location = new Point(100, 16), Width = 280 };
        var dtpIni = new DateTimePicker { Value = DateTime.Today.Add(TurnoCalculoHelper.ParseHora(d.HoraInicio)), Format = DateTimePickerFormat.Time, ShowUpDown = true, Location = new Point(100, 52), Width = 120 };
        var dtpFin = new DateTimePicker { Value = DateTime.Today.Add(TurnoCalculoHelper.ParseHora(d.HoraFin)), Format = DateTimePickerFormat.Time, ShowUpDown = true, Location = new Point(100, 88), Width = 120 };
        var chkCruza = new CheckBox { Text = "Termina al día siguiente", Location = new Point(100, 120), Checked = d.CruzaDiaSiguiente, AutoSize = true };
        var chkLun = CrearDia("Lun", (d.DiasSemana & (int)DiasSemanaFlags.Lunes) != 0, 60);
        var chkMar = CrearDia("Mar", (d.DiasSemana & (int)DiasSemanaFlags.Martes) != 0, 115);
        var chkMie = CrearDia("Mié", (d.DiasSemana & (int)DiasSemanaFlags.Miercoles) != 0, 169);
        var chkJue = CrearDia("Jue", (d.DiasSemana & (int)DiasSemanaFlags.Jueves) != 0, 223);
        var chkVie = CrearDia("Vie", (d.DiasSemana & (int)DiasSemanaFlags.Viernes) != 0, 277);
        var chkSab = CrearDia("Sáb", (d.DiasSemana & (int)DiasSemanaFlags.Sabado) != 0, 331);
        var chkDom = CrearDia("Dom", (d.DiasSemana & (int)DiasSemanaFlags.Domingo) != 0, 385);
        CheckBox CrearDia(string t, bool c, int x) => new() { Text = t, Checked = c, Location = new Point(x, 156), AutoSize = true };
        var btnOk = new Button { Text = "Aceptar", DialogResult = DialogResult.OK, Location = new Point(220, 230), Width = 80, Height = 30 };
        var btnCancel = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new Point(320, 230), Width = 80, Height = 30 };
        dlg.Controls.AddRange(new Control[]
        {
            new Label { Text = "Nombre:", Location = new Point(16, 20), AutoSize = true }, txtNom,
            new Label { Text = "Inicio:", Location = new Point(16, 56), AutoSize = true }, dtpIni,
            new Label { Text = "Fin:", Location = new Point(16, 92), AutoSize = true }, dtpFin, chkCruza,
            new Label { Text = "Días:", Location = new Point(16, 158), AutoSize = true },
            chkLun, chkMar, chkMie, chkJue, chkVie, chkSab, chkDom, btnOk, btnCancel
        });
        dlg.AcceptButton = btnOk;
        dlg.CancelButton = btnCancel;
        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        var dias = 0;
        if (chkLun.Checked) dias |= (int)DiasSemanaFlags.Lunes;
        if (chkMar.Checked) dias |= (int)DiasSemanaFlags.Martes;
        if (chkMie.Checked) dias |= (int)DiasSemanaFlags.Miercoles;
        if (chkJue.Checked) dias |= (int)DiasSemanaFlags.Jueves;
        if (chkVie.Checked) dias |= (int)DiasSemanaFlags.Viernes;
        if (chkSab.Checked) dias |= (int)DiasSemanaFlags.Sabado;
        if (chkDom.Checked) dias |= (int)DiasSemanaFlags.Domingo;

        d.NombreFranja = txtNom.Text.Trim();
        d.HoraInicio = TurnoService.FormatearHora(dtpIni.Value.TimeOfDay);
        d.HoraFin = TurnoService.FormatearHora(dtpFin.Value.TimeOfDay);
        d.CruzaDiaSiguiente = chkCruza.Checked;
        d.DiasSemana = dias;
        RefrescarGrillaDetalles();
    }

    private void CargarEjemplo24()
    {
        _detallesEdit.Clear();
        _detallesEdit.AddRange(Services.PlantillaTurnoService.CrearDetalles24Horas());
        if (string.IsNullOrWhiteSpace(txtNombre.Text))
            txtNombre.Text = "Guardia 24 horas";
        RefrescarGrillaDetalles();
    }

    private void GuardarPlantilla()
    {
        try
        {
            var plantilla = new PlantillaTurno
            {
                IdPlantilla = _idPlantillaSeleccionada ?? 0,
                IdSede = ObtenerIdSede(),
                Nombre = txtNombre.Text.Trim(),
                VigenciaDesde = DateOnly.FromDateTime(dtpVigenciaDesde.Value),
                VigenciaHasta = chkSinFechaFin.Checked ? null : DateOnly.FromDateTime(dtpVigenciaHasta.Value),
                Activa = chkActiva.Checked
            };

            _idPlantillaSeleccionada = Services.PlantillaTurnoService.GuardarPlantilla(plantilla, _detallesEdit);
            CargarPlantillas();
            EstablecerEstado("Plantilla guardada.");
        }
        catch (Exception ex) { MostrarError(ex); EstablecerEstado(ex.Message, true); }
    }

    private void EliminarPlantilla()
    {
        if (!_idPlantillaSeleccionada.HasValue) { MostrarInfo("Seleccioná una plantilla."); return; }
        if (!Confirmar("¿Eliminar la plantilla seleccionada?")) return;
        try
        {
            Services.PlantillaTurnoService.EliminarPlantilla(_idPlantillaSeleccionada.Value);
            LimpiarFormulario();
            CargarPlantillas();
            EstablecerEstado("Plantilla eliminada.");
        }
        catch (Exception ex) { MostrarError(ex); EstablecerEstado(ex.Message, true); }
    }

    private void GenerarTurnos()
    {
        if (cboSede.SelectedValue is not int idSede) return;
        try
        {
            var desde = DateOnly.FromDateTime(dtpGenDesde.Value);
            var hasta = DateOnly.FromDateTime(dtpGenHasta.Value);
            var resultado = Services.TurnoGeneradorService.Generar(idSede, desde, hasta);
            MostrarInfo($"Generación completada.\n\n{resultado.Resumen}");
            EstablecerEstado(resultado.Resumen);
        }
        catch (Exception ex) { MostrarError(ex); EstablecerEstado(ex.Message, true); }
    }

    private void LimpiarFormulario()
    {
        _idPlantillaSeleccionada = null;
        txtNombre.Clear();
        dtpVigenciaDesde.Value = DateTime.Today;
        chkSinFechaFin.Checked = true;
        chkActiva.Checked = true;
        _detallesEdit.Clear();
        RefrescarGrillaDetalles();
        dgvPlantillas.ClearSelection();

        var hoy = DateTime.Today;
        dtpGenDesde.Value = new DateTime(hoy.Year, hoy.Month, 1);
        dtpGenHasta.Value = dtpGenDesde.Value.AddMonths(1).AddDays(-1);
    }
}
