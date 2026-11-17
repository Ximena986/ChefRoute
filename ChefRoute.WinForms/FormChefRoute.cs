using ChefRoute.Core;

namespace ChefRoute.WinForms;

public sealed class FormChefRoute : Form
{
    private readonly GrafoDirigido grafo = new();
    private readonly ListBox listaTareas = new();
    private readonly ListBox listaDependencias = new();
    private readonly TextBox resultado = new();
    private readonly Label etiquetaReceta = new();

    public FormChefRoute()
    {
        Text = "ChefRoute | Organizador inteligente de recetas";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1050, 720);
        Size = new Size(1200, 800);
        BackColor = Color.FromArgb(238, 243, 241);
        Font = new Font("Segoe UI", 9.5F);

        Panel encabezado = new()
        {
            Dock = DockStyle.Top,
            Height = 105,
            BackColor = Color.FromArgb(31, 77, 67),
            Padding = new Padding(22, 12, 15, 8)
        };
        Label titulo = new()
        {
            Text = "CHEFROUTE",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 25, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(22, 10)
        };
        etiquetaReceta.Text = "Receta actual: " + grafo.NombreReceta;
        etiquetaReceta.ForeColor = Color.FromArgb(224, 239, 232);
        etiquetaReceta.Font = new Font("Segoe UI", 10);
        etiquetaReceta.AutoSize = true;
        etiquetaReceta.Location = new Point(27, 62);
        encabezado.Controls.Add(titulo);
        encabezado.Controls.Add(etiquetaReceta);

        TableLayoutPanel distribucion = new()
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(16),
            ColumnCount = 2,
            RowCount = 3,
            AutoScroll = true
        };
        distribucion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        distribucion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        distribucion.RowStyles.Add(new RowStyle(SizeType.Percent, 42));
        distribucion.RowStyles.Add(new RowStyle(SizeType.Absolute, 125));
        distribucion.RowStyles.Add(new RowStyle(SizeType.Percent, 58));

        GroupBox grupoTareas = CrearGrupo("Tareas de la receta");
        listaTareas.Dock = DockStyle.Fill;
        listaTareas.Font = new Font("Segoe UI", 10);
        listaTareas.BorderStyle = BorderStyle.None;
        grupoTareas.Controls.Add(listaTareas);

        GroupBox grupoDependencias = CrearGrupo("Dependencias dirigidas");
        listaDependencias.Dock = DockStyle.Fill;
        listaDependencias.Font = new Font("Segoe UI", 9.5F);
        listaDependencias.BorderStyle = BorderStyle.None;
        grupoDependencias.Controls.Add(listaDependencias);

        GroupBox grupoAcciones = CrearGrupo("Herramientas y algoritmos");
        FlowLayoutPanel acciones = new()
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(8),
            WrapContents = true,
            AutoScroll = true
        };
        acciones.Controls.Add(CrearBoton("Ver tareas", (_, _) => Mostrar(grafo.MostrarTareas())));
        acciones.Controls.Add(CrearBoton("Ver dependencias", (_, _) => Mostrar(grafo.MostrarDependencias())));
        acciones.Controls.Add(CrearBoton("Detectar ciclos (DFS)", (_, _) => EjecutarDfs()));
        acciones.Controls.Add(CrearBoton("Orden topológico (Kahn)", (_, _) => Mostrar(grafo.MostrarOrdenTopologico())));
        acciones.Controls.Add(CrearBoton("Ronda circular", (_, _) => Mostrar(grafo.RondaDelChef())));
        acciones.Controls.Add(CrearBoton("Cargar JSON", CargarJson));
        acciones.Controls.Add(CrearBoton("Guardar JSON", GuardarJson));
        grupoAcciones.Controls.Add(acciones);

        GroupBox grupoResultado = CrearGrupo("Resultado de la operación");
        resultado.Dock = DockStyle.Fill;
        resultado.Multiline = true;
        resultado.ReadOnly = true;
        resultado.ScrollBars = ScrollBars.Both;
        resultado.WordWrap = false;
        resultado.Font = new Font("Consolas", 10);
        resultado.BackColor = Color.White;
        grupoResultado.Controls.Add(resultado);

        distribucion.Controls.Add(grupoTareas, 0, 0);
        distribucion.Controls.Add(grupoDependencias, 1, 0);
        distribucion.Controls.Add(grupoAcciones, 0, 1);
        distribucion.SetColumnSpan(grupoAcciones, 2);
        distribucion.Controls.Add(grupoResultado, 0, 2);
        distribucion.SetColumnSpan(grupoResultado, 2);

        Controls.Add(distribucion);
        Controls.Add(encabezado);
        ActualizarListas();
        Mostrar("Bienvenido a ChefRoute. Seleccione una operación para comenzar.");
    }

    private static GroupBox CrearGrupo(string titulo)
    {
        return new GroupBox
        {
            Text = titulo,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(39, 75, 66),
            BackColor = Color.White,
            Padding = new Padding(10),
            Margin = new Padding(6)
        };
    }

    private static Button CrearBoton(string texto, EventHandler accion)
    {
        Button boton = new()
        {
            Text = texto,
            AutoSize = true,
            Height = 38,
            Padding = new Padding(12, 4, 12, 4),
            BackColor = Color.FromArgb(43, 112, 94),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Margin = new Padding(5)
        };
        boton.FlatAppearance.BorderSize = 0;
        boton.Click += accion;
        return boton;
    }

    private void ActualizarListas()
    {
        listaTareas.Items.Clear();
        listaDependencias.Items.Clear();
        for (int i = 0; i < grafo.Cantidad; i++) listaTareas.Items.Add(grafo.ObtenerTarea(i));
        foreach (string linea in grafo.MostrarDependencias().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
            listaDependencias.Items.Add(linea);
        etiquetaReceta.Text = "Receta actual: " + grafo.NombreReceta;
    }

    private void EjecutarDfs()
    {
        bool hayCiclo = grafo.TieneCiclo(out string detalle);
        string encabezado = hayCiclo
            ? "RECETA INVÁLIDA: SE DETECTÓ UNA DEPENDENCIA CIRCULAR."
            : "RECETA VÁLIDA: NO SE DETECTARON CICLOS.";
        Mostrar(encabezado + Environment.NewLine + Environment.NewLine + detalle);
    }

    private void Mostrar(string texto) => resultado.Text = texto;

    private void CargarJson(object? sender, EventArgs e)
    {
        using OpenFileDialog dialogo = new()
        {
            Filter = "Archivos JSON (*.json)|*.json|Todos los archivos (*.*)|*.*",
            Title = "Cargar receta ChefRoute",
            InitialDirectory = AppContext.BaseDirectory
        };
        if (dialogo.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            grafo.CargarJson(dialogo.FileName);
            ActualizarListas();
            Mostrar("Receta cargada correctamente." + Environment.NewLine + grafo.MostrarTareas());
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "No se pudo cargar la receta: " + ex.Message, "ChefRoute", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void GuardarJson(object? sender, EventArgs e)
    {
        using SaveFileDialog dialogo = new()
        {
            Filter = "Archivos JSON (*.json)|*.json",
            FileName = "receta_chefroute.json",
            Title = "Guardar receta ChefRoute",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };
        if (dialogo.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            grafo.GuardarJson(dialogo.FileName);
            MessageBox.Show(this, "La receta se guardó correctamente.", "ChefRoute", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "No se pudo guardar la receta: " + ex.Message, "ChefRoute", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
