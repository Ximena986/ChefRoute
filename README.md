# ChefRoute — versión final preparada para Visual Studio

Solución con tres proyectos:
- `ChefRoute.Core`: clases, listas enlazadas propias, grafo, DFS, Kahn y JSON.
- `ChefRoute.WinForms`: interfaz gráfica Windows Forms (plus visual indicado por el profesor).
- `ChefRoute.Consola`: menú de consola solicitado en la rúbrica escrita.

## Requisitos
- Windows 10/11.
- Visual Studio 2022 actualizado con la carga de trabajo **Desarrollo de escritorio de .NET**.
- SDK .NET 10.

## Abrir
1. Extraer el ZIP completo.
2. Abrir `ChefRoute.sln` en Visual Studio.
3. Esperar a que se restauren los proyectos.
4. Para la interfaz: clic derecho en `ChefRoute.WinForms` > **Establecer como proyecto de inicio** > F5.
5. Para consola: clic derecho en `ChefRoute.Consola` > **Establecer como proyecto de inicio** > Ctrl+F5.

## Decisiones para cumplir la rúbrica
- `NodoLista<T>` implementa manualmente dato y referencia `Siguiente`.
- `ListaEnlazadaSimple<T>` tiene inserción al inicio/final, inserción por índice, consulta, eliminación y recorrido.
- `ListaEnlazadaCircular<T>` conserva la conexión del último nodo con el primero y recorre una vuelta controlada.
- El grafo usa `ListaEnlazadaSimple<Arista>[]` como lista de adyacencia.
- DFS utiliza estados 0 (no visitado), 1 (en proceso) y 2 (visitado/terminado).
- Kahn almacena el orden en `ListaEnlazadaSimple<Tarea>`.
- La ronda usa `ListaEnlazadaCircular<Tarea>`.
- No se usa `List<T>` ni `LinkedList<T>` para el grafo ni para el flujo de tareas. Los arreglos se usan para datos de tamaño conocido, índices de Kahn y el formato JSON.
- El JSON respeta el esquema de la guía (`plato`, `tareas`, `dependencias` con `desde`, `hacia`, `tiempoMinutos`) y añade campos opcionales para descripciones y duraciones.

## Pruebas manuales
1. Ejecutar DFS con la receta de ejemplo: no debe detectar ciclos.
2. Ejecutar Kahn: `Hacer Salsa` debe aparecer después de `Cortar Verduras` y `Picar Carne`; `Mezclar y Servir` después de `Hacer Salsa` y `Cocer Pasta`.
3. Ronda circular: debe listar cada tarea exactamente una vez.
4. Cargar `Datos/receta_ejemplo.json`, guardar con otro nombre y volver a cargar.
5. Probar un JSON con dependencia hacia una tarea inexistente: debe mostrarse un error sin reemplazar el grafo anterior.
6. Para demostrar ciclo, agregar en una copia de prueba dependencias `Mezclar y Servir` → `Cortar Verduras` (además de las existentes). DFS debe detectarlo y Kahn debe informar que no hay orden completo.

## Nota de verificación
El entorno donde se generó este paquete no tiene instalado el SDK de .NET, por lo que no se afirma una compilación ejecutada aquí. El código y las referencias de proyectos se revisaron estáticamente. La comprobación final de compilación debe hacerse en Visual Studio en Windows con .NET 10 instalado.


## Corrección de carga inicial
La receta inicial se carga desde `receta_ejemplo.json` ubicado junto al ejecutable. El constructor valida que el archivo exista y utiliza el mismo proceso de deserialización y validación que la opción de carga manual. También se rechazan nombres de tareas duplicados y elementos de dependencia nulos.
