namespace ChefRoute.Core;

public sealed class ListaEnlazadaCircular<T>
{
    private NodoLista<T>? ultimo;
    public int Cantidad { get; private set; }
    public bool EstaVacia => ultimo is null;

    public void Agregar(T dato)
    {
        NodoLista<T> nuevo = new(dato);
        if (ultimo is null)
        {
            ultimo = nuevo;
            nuevo.Siguiente = nuevo;
        }
        else
        {
            nuevo.Siguiente = ultimo.Siguiente;
            ultimo.Siguiente = nuevo;
            ultimo = nuevo;
        }
        Cantidad++;
    }

    public T[] RecorrerUnaVuelta()
    {
        T[] resultado = new T[Cantidad];
        if (ultimo is null) return resultado;

        NodoLista<T> primero = ultimo.Siguiente!;
        NodoLista<T> actual = primero;
        int indice = 0;
        do
        {
            resultado[indice++] = actual.Dato;
            actual = actual.Siguiente!;
        }
        while (!ReferenceEquals(actual, primero) && indice < Cantidad);

        return resultado;
    }
}
