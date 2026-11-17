using ChefRoute.Core;

namespace ChefRoute.Consola;

internal static class Program
{
    private static void Main()
    {
        GrafoDirigido grafo = new();
        bool continuar = true;
        while (continuar)
        {
            Console.Clear();
            Console.WriteLine("       CHEFROUTE - GESTIÓN DE RECETAS");
            Console.WriteLine("..............................................");
            Console.WriteLine($"Receta actual: {grafo.NombreReceta}");
            Console.WriteLine("1. Mostrar tareas");
            Console.WriteLine("2. Mostrar dependencias");
            Console.WriteLine("3. Detectar ciclos (DFS)");
            Console.WriteLine("4. Orden de preparación (Kahn)");
            Console.WriteLine("5. Ronda circular del chef");
            Console.WriteLine("6. Cargar receta JSON");
            Console.WriteLine("7. Guardar receta JSON");
            Console.WriteLine("0. Salir");
            Console.Write("Seleccione una opción: ");
            string? opcion = Console.ReadLine();

            try
            {
                switch (opcion)
                {
                    case "1": Console.WriteLine(grafo.MostrarTareas()); Pausar(); break;
                    case "2": Console.WriteLine(grafo.MostrarDependencias()); Pausar(); break;
                    case "3":
                        bool ciclo = grafo.TieneCiclo(out string detalle);
                        Console.WriteLine(ciclo ? "La receta tiene ciclos." : "No se encontraron ciclos.");
                        Console.WriteLine(detalle); Pausar(); break;
                    case "4": Console.WriteLine(grafo.MostrarOrdenTopologico()); Pausar(); break;
                    case "5": Console.WriteLine(grafo.RondaDelChef()); Pausar(); break;
                    case "6":
                        Console.Write("Ruta del archivo JSON: ");
                        grafo.CargarJson(LeerRuta());
                        Console.WriteLine("Receta cargada correctamente."); Pausar(); break;
                    case "7":
                        Console.Write("Ruta para guardar (ej. receta.json): ");
                        grafo.GuardarJson(LeerRuta());
                        Console.WriteLine("Receta guardada correctamente."); Pausar(); break;
                    case "0": continuar = false; break;
                    default: Console.WriteLine("Opción no válida."); Pausar(); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("No se pudo completar la operación: " + ex.Message);
                Pausar();
            }
        }
    }

    private static string LeerRuta() => (Console.ReadLine() ?? string.Empty).Trim().Trim('"');

    private static void Pausar()
    {
        Console.WriteLine();
        Console.Write("Presione ENTER para volver al menú...");
        Console.ReadLine();
    }
}
