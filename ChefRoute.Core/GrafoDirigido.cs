using System.Text.Json;

namespace ChefRoute.Core;

public sealed class GrafoDirigido
{
    private ListaEnlazadaSimple<Tarea> tareas = new();
    private ListaEnlazadaSimple<Arista>[] adyacencia = Array.Empty<ListaEnlazadaSimple<Arista>>();

    public string NombreReceta { get; private set; } = "Pasta Bolognesa Especial";
    public int Cantidad => tareas.Cantidad;

    public GrafoDirigido()
    {
        string ruta = Path.Combine(AppContext.BaseDirectory, "receta_ejemplo.json");
        if (!File.Exists(ruta))
            throw new FileNotFoundException("No se encontró receta_ejemplo.json junto al programa.", ruta);

        CargarJson(ruta);
    }

    public Tarea? ObtenerTarea(int indice) => tareas.Obtener(indice);

    public void CargarDesdeModeloJson(RecetaJson receta)
    {
        ArgumentNullException.ThrowIfNull(receta);
        if (receta.Tareas is null || receta.Tareas.Length == 0)
            throw new InvalidDataException("La receta debe contener al menos una tarea.");
        if (string.IsNullOrWhiteSpace(receta.Plato))
            throw new InvalidDataException("El nombre del plato está vacío.");

        ListaEnlazadaSimple<Tarea> nuevasTareas = new();
        for (int i = 0; i < receta.Tareas.Length; i++)
        {
            string nombre = receta.Tareas[i];
            if (string.IsNullOrWhiteSpace(nombre)) throw new InvalidDataException($"La tarea {i + 1} no tiene nombre.");
            for (int j = 0; j < i; j++)
                if (string.Equals(receta.Tareas[j], nombre, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException($"El nombre de tarea '{nombre}' está repetido.");
            int duracion = i < (receta.DuracionesMinutos?.Length ?? 0) ? receta.DuracionesMinutos[i] : 0;
            string descripcion = i < (receta.Descripciones?.Length ?? 0) ? receta.Descripciones[i] : string.Empty;
            if (duracion < 0) throw new InvalidDataException($"La duración de '{nombre}' no puede ser negativa.");
            nuevasTareas.AgregarAlFinal(new Tarea { Id = i, Nombre = nombre, Descripcion = descripcion, DuracionMinutos = duracion });
        }

        ListaEnlazadaSimple<Arista>[] nuevaAdyacencia = new ListaEnlazadaSimple<Arista>[receta.Tareas.Length];
        for (int i = 0; i < nuevaAdyacencia.Length; i++) nuevaAdyacencia[i] = new ListaEnlazadaSimple<Arista>();

        foreach (DependenciaJson dependencia in receta.Dependencias ?? Array.Empty<DependenciaJson>())
        {
            if (dependencia is null)
                throw new InvalidDataException("La lista de dependencias contiene un registro nulo.");

            int origen = BuscarIndice(receta.Tareas, dependencia.Desde);
            int destino = BuscarIndice(receta.Tareas, dependencia.Hacia);
            if (origen < 0 || destino < 0)
                throw new InvalidDataException($"La dependencia '{dependencia.Desde}' -> '{dependencia.Hacia}' referencia una tarea inexistente.");
            if (origen == destino) throw new InvalidDataException("Una tarea no puede depender de sí misma.");
            if (dependencia.TiempoMinutos < 0) throw new InvalidDataException("El peso de una dependencia no puede ser negativo.");
            nuevaAdyacencia[origen].AgregarAlFinal(new Arista(destino, dependencia.TiempoMinutos));
        }

        tareas = nuevasTareas;
        adyacencia = nuevaAdyacencia;
        NombreReceta = receta.Plato;
    }

    private static int BuscarIndice(string[] nombres, string nombre)
    {
        for (int i = 0; i < nombres.Length; i++)
            if (string.Equals(nombres[i], nombre, StringComparison.OrdinalIgnoreCase)) return i;
        return -1;
    }

    public string MostrarTareas()
    {
        string texto = $"RECETA: {NombreReceta}{Environment.NewLine}{Environment.NewLine}TAREAS:{Environment.NewLine}";
        for (int i = 0; i < tareas.Cantidad; i++)
        {
            Tarea? tarea = tareas.Obtener(i);
            texto += $"{tarea}{Environment.NewLine}   Descripción: {tarea?.Descripcion}{Environment.NewLine}";
        }
        return texto;
    }

    public string MostrarDependencias()
    {
        string texto = $"DEPENDENCIAS DIRIGIDAS DE: {NombreReceta}{Environment.NewLine}{Environment.NewLine}";
        for (int origen = 0; origen < Cantidad; origen++)
        {
            texto += $"{tareas.Obtener(origen)?.Nombre}:{Environment.NewLine}";
            foreach (Arista arista in adyacencia[origen].AArreglo())
                texto += $"   -> {tareas.Obtener(arista.Destino)?.Nombre} | peso: {arista.Peso} min{Environment.NewLine}";
        }
        return texto;
    }

    public bool TieneCiclo(out string detalle)
    {
        int[] estado = new int[Cantidad];
        detalle = "RECORRIDO DFS (0 = No visitado, 1 = En proceso, 2 = Visitado)" + Environment.NewLine;
        for (int i = 0; i < Cantidad; i++)
            if (estado[i] == 0 && VisitarDFS(i, estado, ref detalle)) return true;
        return false;
    }

    private bool VisitarDFS(int actual, int[] estado, ref string detalle)
    {
        estado[actual] = 1;
        detalle += $"En proceso: {tareas.Obtener(actual)?.Nombre}{Environment.NewLine}";
        foreach (Arista arista in adyacencia[actual].AArreglo())
        {
            int vecino = arista.Destino;
            if (estado[vecino] == 1)
            {
                detalle += $"CICLO: se encontró una arista hacia '{tareas.Obtener(vecino)?.Nombre}', que sigue en proceso.{Environment.NewLine}";
                return true;
            }
            if (estado[vecino] == 0 && VisitarDFS(vecino, estado, ref detalle)) return true;
        }
        estado[actual] = 2;
        detalle += $"Visitado/terminado: {tareas.Obtener(actual)?.Nombre}{Environment.NewLine}";
        return false;
    }

    public bool IntentarOrdenTopologico(out ListaEnlazadaSimple<Tarea> orden, out string detalle)
    {
        int n = Cantidad;
        int[] gradoEntrada = new int[n];
        for (int origen = 0; origen < n; origen++)
            foreach (Arista arista in adyacencia[origen].AArreglo()) gradoEntrada[arista.Destino]++;

        int[] cola = new int[n];
        int frente = 0;
        int final = 0;
        for (int i = 0; i < n; i++)
            if (gradoEntrada[i] == 0) cola[final++] = i;

        orden = new ListaEnlazadaSimple<Tarea>();
        detalle = "ALGORITMO DE KAHN" + Environment.NewLine + "Grados de entrada iniciales:" + Environment.NewLine;
        for (int i = 0; i < n; i++) detalle += $"- {tareas.Obtener(i)?.Nombre}: {gradoEntrada[i]}" + Environment.NewLine;

        while (frente < final)
        {
            int actual = cola[frente++];
            Tarea tareaActual = tareas.Obtener(actual)!;
            orden.AgregarAlFinal(tareaActual);
            detalle += $"Se agrega al orden: {tareaActual.Nombre}" + Environment.NewLine;

            foreach (Arista arista in adyacencia[actual].AArreglo())
            {
                gradoEntrada[arista.Destino]--;
                detalle += $"  Se reduce el grado de '{tareas.Obtener(arista.Destino)?.Nombre}' a {gradoEntrada[arista.Destino]}." + Environment.NewLine;
                if (gradoEntrada[arista.Destino] == 0) cola[final++] = arista.Destino;
            }
        }

        if (orden.Cantidad != n)
        {
            detalle += "No se pudieron ordenar todas las tareas: el grafo contiene al menos un ciclo.";
            return false;
        }
        return true;
    }

    public string MostrarOrdenTopologico()
    {
        bool valido = IntentarOrdenTopologico(out ListaEnlazadaSimple<Tarea> orden, out string detalle);
        if (!valido) return detalle;
        string texto = "SECUENCIA VÁLIDA DE PREPARACIÓN (KAHN)" + Environment.NewLine + Environment.NewLine;
        for (int i = 0; i < orden.Cantidad; i++) texto += $"{i + 1}. {orden.Obtener(i)?.Nombre}{Environment.NewLine}";
        return texto + Environment.NewLine + detalle;
    }

    public string RondaDelChef()
    {
        if (!IntentarOrdenTopologico(out ListaEnlazadaSimple<Tarea> tareasListas, out _))
            return "No se puede iniciar la ronda: la receta contiene dependencias circulares.";

        ListaEnlazadaCircular<Tarea> ronda = new();
        for (int i = 0; i < tareasListas.Cantidad; i++) ronda.Agregar(tareasListas.Obtener(i)!);
        Tarea[] vuelta = ronda.RecorrerUnaVuelta();
        string texto = "RONDA CIRCULAR DEL CHEF — UNA VUELTA" + Environment.NewLine +
                       "Se supervisan las tareas en un orden que respeta las dependencias." + Environment.NewLine + Environment.NewLine;
        for (int i = 0; i < vuelta.Length; i++) texto += $"{i + 1}. {vuelta[i].Nombre}{Environment.NewLine}";
        return texto + $"{Environment.NewLine}Tareas recorridas: {vuelta.Length}. La referencia circular permanece intacta.";
    }

    public void GuardarJson(string ruta)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ruta);
        RecetaJson receta = CrearModeloJson();
        JsonSerializerOptions opciones = new() { WriteIndented = true };
        string contenido = JsonSerializer.Serialize(receta, opciones);
        File.WriteAllText(ruta, contenido);
    }

    private RecetaJson CrearModeloJson()
    {
        string[] nombres = new string[Cantidad];
        string[] descripciones = new string[Cantidad];
        int[] duraciones = new int[Cantidad];
        ListaEnlazadaSimple<DependenciaJson> dependencias = new();
        for (int i = 0; i < Cantidad; i++)
        {
            Tarea tarea = tareas.Obtener(i)!;
            nombres[i] = tarea.Nombre;
            descripciones[i] = tarea.Descripcion;
            duraciones[i] = tarea.DuracionMinutos;
            foreach (Arista arista in adyacencia[i].AArreglo())
                dependencias.AgregarAlFinal(new DependenciaJson
                {
                    Desde = tarea.Nombre,
                    Hacia = tareas.Obtener(arista.Destino)!.Nombre,
                    TiempoMinutos = arista.Peso
                });
        }
        return new RecetaJson
        {
            Plato = NombreReceta,
            Tareas = nombres,
            Descripciones = descripciones,
            DuracionesMinutos = duraciones,
            Dependencias = dependencias.AArreglo()
        };
    }

    public void CargarJson(string ruta)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ruta);
        string contenido = File.ReadAllText(ruta);
        RecetaJson receta = JsonSerializer.Deserialize<RecetaJson>(contenido)
            ?? throw new InvalidDataException("El archivo JSON está vacío o no tiene una receta válida.");
        CargarDesdeModeloJson(receta);
    }
}
