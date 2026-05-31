namespace ElArteServicios.UI;

public static class UiTheme
{
    public static readonly Color HeaderBack = Color.FromArgb(41, 53, 65);
    public static readonly Color HeaderSubtext = Color.FromArgb(200, 210, 220);
    public static readonly Color Accent = Color.FromArgb(52, 152, 219);

    private static readonly Font BotonFont = new("Segoe UI", 9.75F);

    public static void AplicarEncabezado(Panel panel, Label titulo, Label subtitulo, string textoTitulo, string textoSubtitulo)
    {
        panel.BackColor = HeaderBack;
        panel.Dock = DockStyle.Top;
        panel.Height = 72;
        panel.Padding = new Padding(12, 8, 12, 8);

        titulo.AutoSize = true;
        titulo.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        titulo.ForeColor = Color.White;
        titulo.Text = textoTitulo;
        titulo.Location = new Point(12, 8);

        subtitulo.AutoSize = true;
        subtitulo.Font = new Font("Segoe UI", 10F);
        subtitulo.ForeColor = HeaderSubtext;
        subtitulo.Text = textoSubtitulo;
        subtitulo.Location = new Point(14, 40);

        if (!panel.Controls.Contains(titulo)) panel.Controls.Add(titulo);
        if (!panel.Controls.Contains(subtitulo)) panel.Controls.Add(subtitulo);
    }

    public static void ConfigurarGrilla(DataGridView grid)
    {
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.None;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        grid.Dock = DockStyle.Fill;
        grid.MultiSelect = false;
        grid.ReadOnly = true;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    }

    public static void ConfigurarBotonPrimario(Button btn)
    {
        btn.FlatStyle = FlatStyle.Flat;
        btn.BackColor = Accent;
        btn.ForeColor = Color.White;
        btn.FlatAppearance.BorderSize = 0;
        btn.Cursor = Cursors.Hand;
        btn.Font = BotonFont;
        btn.Height = 36;
        AjustarAnchoBoton(btn);
    }

    public static void ConfigurarBotonSecundario(Button btn)
    {
        btn.UseVisualStyleBackColor = true;
        btn.FlatStyle = FlatStyle.Standard;
        btn.Font = BotonFont;
        btn.Height = 36;
        btn.Cursor = Cursors.Hand;
        AjustarAnchoBoton(btn);
    }

    public static void AplicarBarraBotones(params Button[] botones)
    {
        var x = 0;
        const int separacion = 12;

        foreach (var btn in botones)
        {
            var esGuardar = btn.Text.Contains("Guardar", StringComparison.OrdinalIgnoreCase);
            
            if (esGuardar)
                ConfigurarBotonPrimario(btn);
            else
                ConfigurarBotonSecundario(btn);
            
            btn.Location = new Point(x, 8);
            x += btn.Width + separacion;
        }
    }

    private static void AjustarAnchoBoton(Button btn)
    {
        var esLicencias = btn.Text.Contains("Licencias", StringComparison.OrdinalIgnoreCase);
        var texto = TextRenderer.MeasureText(btn.Text, btn.Font);
        if (esLicencias)
            btn.Location = new Point(635, 8);
        btn.Width = Math.Max(115, texto.Width + 32);
    }
}
