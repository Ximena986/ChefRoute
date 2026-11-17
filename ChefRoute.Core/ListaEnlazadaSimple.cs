namespace ChefRoute.Core;

public sealed class ListaEnlazadaSimple<T>
{
    private NodoLista<T>? cabeza;
    private NodoLista<T>? cola;

    public int Cantidad { get; private set; }
    public bool EstaVacia => Cantidad == 0;

    public void AgregarAlFinal(T dato)
    {
        NodoLista<T> nuevo = new(dato);
        if (cabeza is null)
        {
            cabeza = nuevo;
            cola = nuevo;
        }
        else
        {
            cola!.Siguiente = nuevo;
            cola = nuevo;
        }
        Cantidad++;
    }

    public void AgregarAlInicio(T dato)
    {
        NodoLista<T> nuevo = new(dato) { Siguiente = cabeza };
        cabeza = nuevo;
        if (cola is null) cola = nuevo;
        Cantidad++;
    }

    public void Insertar(int indice, T dato)
    {
        if (indice < 0 || indice > Cantidad)
            throw new ArgumentOutOfRangeException(nameof(indice));
        if (indice == 0) { AgregarAlInicio(dato); return; }
        if (indice == Cantidad) { AgregarAlFinal(dato); return; }

        NodoLista<T> anterior = ObtenerNodo(indice - 1);
        NodoLista<T> nuevo = new(dato) { Siguiente = anterior.Siguiente };
        anterior.Siguiente = nuevo;
        Cantidad++;
    }

    public T? Obtener(int indice)
    {
        if (indice < 0 || indice >= Cantidad) return default;
        return ObtenerNodo(indice).Dato;
    }

    public bool EliminarEn(int indice)
    {
        if (indice < 0 || indice >= Cantidad) return false;
        if (indice == 0)
        {
            cabeza = cabeza!.Siguiente;
            Cantidad--;
            if (Cantidad == 0) cola = null;
            return true;
        }

        NodoLista<T> anterior = ObtenerNodo(indice - 1);
        NodoLista<T> eliminado = anterior.Siguiente!;
        anterior.Siguiente = eliminado.Siguiente;
        if (ReferenceEquals(eliminado, cola)) cola = anterior;
        Cantidad--;
        return true;
    }

    public bool EliminarPrimeroQueCumple(Predicate<T> condicion)
    {
        ArgumentNullException.ThrowIfNull(condicion);
        NodoLista<T>? actual = cabeza;
        int indice = 0;
        while (actual is not null)
        {
            if (condicion(actual.Dato)) return EliminarEn(indice);
            actual = actual.Siguiente;
            indice++;
        }
        return false;
    }

    public T[] AArreglo()
    {
        T[] resultado = new T[Cantidad];
        NodoLista<T>? actual = cabeza;
        int indice = 0;
        while (actual is not null)
        {
            resultado[indice++] = actual.Dato;
            actual = actual.Siguiente;
        }
        return resultado;
    }

    private NodoLista<T> ObtenerNodo(int indice)
    {
        NodoLista<T> actual = cabeza!;
        for (int i = 0; i < indice; i++) actual = actual.Siguiente!;
        return actual;
    }
}
