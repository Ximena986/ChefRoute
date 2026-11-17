namespace ChefRoute.Core;

public sealed class Tarea
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int DuracionMinutos { get; set; }

    public override string ToString() => $"{Id + 1}. {Nombre} ({DuracionMinutos} min)";
}
