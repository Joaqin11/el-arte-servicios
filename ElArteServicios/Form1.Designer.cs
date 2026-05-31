namespace ElArteServicios
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            menuPrincipal = new MenuStrip();
            menuGestion = new ToolStripMenuItem();
            menuEmpleados = new ToolStripMenuItem();
            menuSedes = new ToolStripMenuItem();
            menuTurnos = new ToolStripMenuItem();
            menuPlantillas = new ToolStripMenuItem();
            menuAsignaciones = new ToolStripMenuItem();
            menuArchivo = new ToolStripMenuItem();
            menuExportar = new ToolStripMenuItem();
            menuSalir = new ToolStripMenuItem();
            pnlEncabezado = new Panel();
            lblSubtitulo = new Label();
            lblTitulo = new Label();
            pnlNavegacion = new Panel();
            btnAsignaciones = new Button();
            btnTurnos = new Button();
            btnSedes = new Button();
            btnEmpleados = new Button();
            pnlResumen = new Panel();
            lblResumen = new Label();
            pnlAsignaciones = new Panel();
            pnlBotonesAcciones = new FlowLayoutPanel();
            btnSalir = new Button();
            btnNuevaAsignacion = new Button();
            btnActualizar = new Button();
            btnExportar = new Button();
            dtpFecha = new DateTimePicker();
            lblFecha = new Label();
            lblAsignacionesDia = new Label();
            dgvAsignacionesHoy = new DataGridView();
            statusPrincipal = new StatusStrip();
            lblEstadoDb = new ToolStripStatusLabel();
            menuPrincipal.SuspendLayout();
            pnlEncabezado.SuspendLayout();
            pnlNavegacion.SuspendLayout();
            pnlResumen.SuspendLayout();
            pnlAsignaciones.SuspendLayout();
            pnlBotonesAcciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAsignacionesHoy).BeginInit();
            statusPrincipal.SuspendLayout();
            SuspendLayout();
            // 
            // menuPrincipal
            // 
            menuPrincipal.Items.AddRange(new ToolStripItem[] { menuGestion, menuArchivo });
            menuPrincipal.Location = new Point(0, 0);
            menuPrincipal.Name = "menuPrincipal";
            menuPrincipal.Size = new Size(984, 24);
            menuPrincipal.TabIndex = 0;
            menuPrincipal.Text = "menuPrincipal";
            // 
            // menuGestion
            // 
            menuGestion.DropDownItems.AddRange(new ToolStripItem[] { menuEmpleados, menuSedes, menuTurnos, menuPlantillas, menuAsignaciones });
            menuGestion.Name = "menuGestion";
            menuGestion.Size = new Size(59, 20);
            menuGestion.Text = "&Gestión";
            // 
            // menuEmpleados
            // 
            menuEmpleados.Name = "menuEmpleados";
            menuEmpleados.Size = new Size(180, 22);
            menuEmpleados.Text = "&Empleados / Vigiladores";
            menuEmpleados.Click += menuEmpleados_Click;
            // 
            // menuSedes
            // 
            menuSedes.Name = "menuSedes";
            menuSedes.Size = new Size(180, 22);
            menuSedes.Text = "&Sedes";
            menuSedes.Click += menuSedes_Click;
            // 
            // menuTurnos
            // 
            menuTurnos.Name = "menuTurnos";
            menuTurnos.Size = new Size(180, 22);
            menuTurnos.Text = "&Turnos";
            menuTurnos.Click += menuTurnos_Click;
            // 
            // menuPlantillas
            // 
            menuPlantillas.Name = "menuPlantillas";
            menuPlantillas.Size = new Size(220, 22);
            menuPlantillas.Text = "Parametrización de &turnos";
            menuPlantillas.Click += menuPlantillas_Click;
            // 
            // menuAsignaciones
            // 
            menuAsignaciones.Name = "menuAsignaciones";
            menuAsignaciones.Size = new Size(180, 22);
            menuAsignaciones.Text = "&Asignaciones";
            menuAsignaciones.Click += menuAsignaciones_Click;
            // 
            // menuArchivo
            // 
            menuArchivo.DropDownItems.AddRange(new ToolStripItem[] { menuExportar, menuSalir });
            menuArchivo.Name = "menuArchivo";
            menuArchivo.Size = new Size(60, 20);
            menuArchivo.Text = "&Archivo";
            // 
            // menuExportar
            // 
            menuExportar.Name = "menuExportar";
            menuExportar.Size = new Size(220, 22);
            menuExportar.Text = "&Exportar planilla…";
            menuExportar.Click += menuExportar_Click;
            // 
            // menuSalir
            // 
            menuSalir.Name = "menuSalir";
            menuSalir.Size = new Size(180, 22);
            menuSalir.Text = "&Salir";
            menuSalir.Click += menuSalir_Click;
            // 
            // pnlEncabezado
            // 
            pnlEncabezado.BackColor = Color.FromArgb(41, 53, 65);
            pnlEncabezado.Controls.Add(lblSubtitulo);
            pnlEncabezado.Controls.Add(lblTitulo);
            pnlEncabezado.Dock = DockStyle.Top;
            pnlEncabezado.Location = new Point(0, 24);
            pnlEncabezado.Name = "pnlEncabezado";
            pnlEncabezado.Size = new Size(984, 72);
            pnlEncabezado.TabIndex = 1;
            // 
            // lblSubtitulo
            // 
            lblSubtitulo.AutoSize = true;
            lblSubtitulo.Font = new Font("Segoe UI", 10F);
            lblSubtitulo.ForeColor = Color.FromArgb(200, 210, 220);
            lblSubtitulo.Location = new Point(16, 42);
            lblSubtitulo.Name = "lblSubtitulo";
            lblSubtitulo.Size = new Size(163, 19);
            lblSubtitulo.TabIndex = 1;
            lblSubtitulo.Text = "Gestión de turnos y personal";
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(12, 9);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(220, 32);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "El Arte Servicios";
            // 
            // pnlNavegacion
            // 
            pnlNavegacion.Controls.Add(btnAsignaciones);
            pnlNavegacion.Controls.Add(btnTurnos);
            pnlNavegacion.Controls.Add(btnSedes);
            pnlNavegacion.Controls.Add(btnEmpleados);
            pnlNavegacion.Dock = DockStyle.Top;
            pnlNavegacion.Location = new Point(0, 96);
            pnlNavegacion.Name = "pnlNavegacion";
            pnlNavegacion.Padding = new Padding(12, 10, 12, 6);
            pnlNavegacion.Size = new Size(984, 56);
            pnlNavegacion.TabIndex = 2;
            // 
            // btnAsignaciones
            // 
            btnAsignaciones.Location = new Point(498, 13);
            btnAsignaciones.Name = "btnAsignaciones";
            btnAsignaciones.Size = new Size(150, 32);
            btnAsignaciones.TabIndex = 3;
            btnAsignaciones.Text = "Asignaciones";
            btnAsignaciones.UseVisualStyleBackColor = true;
            btnAsignaciones.Click += btnAsignaciones_Click;
            // 
            // btnTurnos
            // 
            btnTurnos.Location = new Point(336, 13);
            btnTurnos.Name = "btnTurnos";
            btnTurnos.Size = new Size(150, 32);
            btnTurnos.TabIndex = 2;
            btnTurnos.Text = "Turnos";
            btnTurnos.UseVisualStyleBackColor = true;
            btnTurnos.Click += btnTurnos_Click;
            // 
            // btnSedes
            // 
            btnSedes.Location = new Point(174, 13);
            btnSedes.Name = "btnSedes";
            btnSedes.Size = new Size(150, 32);
            btnSedes.TabIndex = 1;
            btnSedes.Text = "Sedes";
            btnSedes.UseVisualStyleBackColor = true;
            btnSedes.Click += btnSedes_Click;
            // 
            // btnEmpleados
            // 
            btnEmpleados.Location = new Point(12, 13);
            btnEmpleados.Name = "btnEmpleados";
            btnEmpleados.Size = new Size(150, 32);
            btnEmpleados.TabIndex = 0;
            btnEmpleados.Text = "Empleados";
            btnEmpleados.UseVisualStyleBackColor = true;
            btnEmpleados.Click += btnEmpleados_Click;
            // 
            // pnlResumen
            // 
            pnlResumen.Controls.Add(lblResumen);
            pnlResumen.Dock = DockStyle.Top;
            pnlResumen.Location = new Point(0, 152);
            pnlResumen.Name = "pnlResumen";
            pnlResumen.Padding = new Padding(12, 4, 12, 4);
            pnlResumen.Size = new Size(984, 32);
            pnlResumen.TabIndex = 3;
            // 
            // lblResumen
            // 
            lblResumen.AutoSize = true;
            lblResumen.Font = new Font("Segoe UI", 9F);
            lblResumen.Location = new Point(12, 8);
            lblResumen.Name = "lblResumen";
            lblResumen.Size = new Size(59, 15);
            lblResumen.TabIndex = 0;
            lblResumen.Text = "Cargando…";
            // 
            // pnlAsignaciones
            // 
            pnlAsignaciones.Controls.Add(pnlBotonesAcciones);
            pnlAsignaciones.Controls.Add(dtpFecha);
            pnlAsignaciones.Controls.Add(lblFecha);
            pnlAsignaciones.Controls.Add(lblAsignacionesDia);
            pnlAsignaciones.Dock = DockStyle.Top;
            pnlAsignaciones.Location = new Point(0, 184);
            pnlAsignaciones.Name = "pnlAsignaciones";
            pnlAsignaciones.Padding = new Padding(12, 6, 12, 6);
            pnlAsignaciones.Size = new Size(984, 48);
            pnlAsignaciones.TabIndex = 4;
            // 
            // pnlBotonesAcciones
            // 
            pnlBotonesAcciones.AutoSize = true;
            pnlBotonesAcciones.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlBotonesAcciones.Controls.Add(btnSalir);
            pnlBotonesAcciones.Controls.Add(btnNuevaAsignacion);
            pnlBotonesAcciones.Controls.Add(btnExportar);
            pnlBotonesAcciones.Controls.Add(btnActualizar);
            pnlBotonesAcciones.Dock = DockStyle.Right;
            pnlBotonesAcciones.FlowDirection = FlowDirection.LeftToRight;
            pnlBotonesAcciones.Location = new Point(568, 6);
            pnlBotonesAcciones.Margin = new Padding(12, 0, 0, 0);
            pnlBotonesAcciones.Name = "pnlBotonesAcciones";
            pnlBotonesAcciones.Padding = new Padding(0, 2, 0, 0);
            pnlBotonesAcciones.Size = new Size(392, 38);
            pnlBotonesAcciones.TabIndex = 5;
            pnlBotonesAcciones.WrapContents = false;
            // 
            // btnSalir
            // 
            btnSalir.Margin = new Padding(0, 0, 8, 0);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(75, 32);
            btnSalir.TabIndex = 2;
            btnSalir.Text = "Salir";
            btnSalir.UseVisualStyleBackColor = true;
            btnSalir.Click += menuSalir_Click;
            // 
            // btnNuevaAsignacion
            // 
            btnNuevaAsignacion.Margin = new Padding(0, 0, 8, 0);
            btnNuevaAsignacion.Name = "btnNuevaAsignacion";
            btnNuevaAsignacion.Size = new Size(130, 32);
            btnNuevaAsignacion.TabIndex = 1;
            btnNuevaAsignacion.Text = "+ Nueva asignación";
            btnNuevaAsignacion.UseVisualStyleBackColor = true;
            btnNuevaAsignacion.Click += btnNuevaAsignacion_Click;
            // 
            // btnExportar
            // 
            btnExportar.Margin = new Padding(0, 0, 8, 0);
            btnExportar.Name = "btnExportar";
            btnExportar.Size = new Size(90, 32);
            btnExportar.TabIndex = 3;
            btnExportar.Text = "Exportar";
            btnExportar.UseVisualStyleBackColor = true;
            btnExportar.Click += btnExportar_Click;
            // 
            // btnActualizar
            // 
            btnActualizar.Margin = new Padding(0);
            btnActualizar.Name = "btnActualizar";
            btnActualizar.Size = new Size(85, 32);
            btnActualizar.TabIndex = 0;
            btnActualizar.Text = "Actualizar";
            btnActualizar.UseVisualStyleBackColor = true;
            btnActualizar.Click += btnActualizar_Click;
            // 
            // dtpFecha
            // 
            dtpFecha.Format = DateTimePickerFormat.Short;
            dtpFecha.Location = new Point(268, 10);
            dtpFecha.Name = "dtpFecha";
            dtpFecha.Size = new Size(110, 23);
            dtpFecha.TabIndex = 2;
            dtpFecha.ValueChanged += dtpFecha_ValueChanged;
            // 
            // lblFecha
            // 
            lblFecha.AutoSize = true;
            lblFecha.Location = new Point(220, 14);
            lblFecha.Name = "lblFecha";
            lblFecha.Size = new Size(42, 15);
            lblFecha.TabIndex = 1;
            lblFecha.Text = "Fecha:";
            // 
            // lblAsignacionesDia
            // 
            lblAsignacionesDia.AutoSize = true;
            lblAsignacionesDia.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            lblAsignacionesDia.Location = new Point(12, 12);
            lblAsignacionesDia.Name = "lblAsignacionesDia";
            lblAsignacionesDia.Size = new Size(145, 17);
            lblAsignacionesDia.TabIndex = 0;
            lblAsignacionesDia.Text = "Asignaciones del día";
            // 
            // dgvAsignacionesHoy
            // 
            dgvAsignacionesHoy.AllowUserToAddRows = false;
            dgvAsignacionesHoy.AllowUserToDeleteRows = false;
            dgvAsignacionesHoy.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAsignacionesHoy.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvAsignacionesHoy.Dock = DockStyle.Fill;
            dgvAsignacionesHoy.Location = new Point(0, 232);
            dgvAsignacionesHoy.MultiSelect = false;
            dgvAsignacionesHoy.Name = "dgvAsignacionesHoy";
            dgvAsignacionesHoy.ReadOnly = true;
            dgvAsignacionesHoy.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAsignacionesHoy.Size = new Size(984, 315);
            dgvAsignacionesHoy.TabIndex = 5;
            // 
            // statusPrincipal
            // 
            statusPrincipal.Items.AddRange(new ToolStripItem[] { lblEstadoDb });
            statusPrincipal.Location = new Point(0, 539);
            statusPrincipal.Name = "statusPrincipal";
            statusPrincipal.Size = new Size(984, 22);
            statusPrincipal.TabIndex = 6;
            statusPrincipal.Text = "statusStrip1";
            // 
            // lblEstadoDb
            // 
            lblEstadoDb.Name = "lblEstadoDb";
            lblEstadoDb.Size = new Size(969, 17);
            lblEstadoDb.Spring = true;
            lblEstadoDb.Text = "Base de datos: —";
            lblEstadoDb.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 561);
            Controls.Add(dgvAsignacionesHoy);
            Controls.Add(pnlAsignaciones);
            Controls.Add(pnlResumen);
            Controls.Add(pnlNavegacion);
            Controls.Add(pnlEncabezado);
            Controls.Add(statusPrincipal);
            Controls.Add(menuPrincipal);
            MainMenuStrip = menuPrincipal;
            MinimumSize = new Size(800, 500);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "El Arte Servicios";
            Load += Form1_Load;
            menuPrincipal.ResumeLayout(false);
            menuPrincipal.PerformLayout();
            pnlEncabezado.ResumeLayout(false);
            pnlEncabezado.PerformLayout();
            pnlNavegacion.ResumeLayout(false);
            pnlResumen.ResumeLayout(false);
            pnlResumen.PerformLayout();
            pnlAsignaciones.ResumeLayout(false);
            pnlAsignaciones.PerformLayout();
            pnlBotonesAcciones.ResumeLayout(false);
            pnlBotonesAcciones.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAsignacionesHoy).EndInit();
            statusPrincipal.ResumeLayout(false);
            statusPrincipal.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuPrincipal;
        private ToolStripMenuItem menuGestion;
        private ToolStripMenuItem menuEmpleados;
        private ToolStripMenuItem menuSedes;
        private ToolStripMenuItem menuTurnos;
        private ToolStripMenuItem menuPlantillas;
        private ToolStripMenuItem menuAsignaciones;
        private ToolStripMenuItem menuArchivo;
        private ToolStripMenuItem menuExportar;
        private ToolStripMenuItem menuSalir;
        private Panel pnlEncabezado;
        private Label lblTitulo;
        private Label lblSubtitulo;
        private Panel pnlNavegacion;
        private Button btnEmpleados;
        private Button btnSedes;
        private Button btnTurnos;
        private Button btnAsignaciones;
        private Panel pnlResumen;
        private Label lblResumen;
        private Panel pnlAsignaciones;
        private FlowLayoutPanel pnlBotonesAcciones;
        private Label lblAsignacionesDia;
        private Label lblFecha;
        private DateTimePicker dtpFecha;
        private Button btnSalir;
        private Button btnNuevaAsignacion;
        private Button btnExportar;
        private Button btnActualizar;
        private DataGridView dgvAsignacionesHoy;
        private StatusStrip statusPrincipal;
        private ToolStripStatusLabel lblEstadoDb;
    }
}
