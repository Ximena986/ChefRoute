namespace ChefRoute.Core;
public sealed class Arista
{
    public int Destino { get; }
    public int Peso { get; }

    public Arista(int destino, int peso)
    {
        Destino = destino;
        Peso = peso;
    }
}
