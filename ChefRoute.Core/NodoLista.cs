namespace ChefRoute.Core;
public sealed class NodoLista<T>
{
    public T Dato { get; set; }
    public NodoLista<T>? Siguiente { get; set; }

    public NodoLista(T dato)
    {
        Dato = dato;
        Siguiente = null;
    }
}
