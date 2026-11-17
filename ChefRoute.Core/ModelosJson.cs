using System.Text.Json.Serialization;

namespace ChefRoute.Core;

public sealed class RecetaJson
{
    [JsonPropertyName("plato")]
    public string Plato { get; set; } = "";

    [JsonPropertyName("tareas")]
    public string[] Tareas { get; set; } = Array.Empty<string>();

    [JsonPropertyName("descripciones")]
    public string[] Descripciones { get; set; } = Array.Empty<string>();

    [JsonPropertyName("duracionesMinutos")]
    public int[] DuracionesMinutos { get; set; } = Array.Empty<int>();

    [JsonPropertyName("dependencias")]
    public DependenciaJson[] Dependencias { get; set; } = Array.Empty<DependenciaJson>();
}

public sealed class DependenciaJson
{
    [JsonPropertyName("desde")]
    public string Desde { get; set; } = "";

    [JsonPropertyName("hacia")]
    public string Hacia { get; set; } = "";

    [JsonPropertyName("tiempoMinutos")]
    public int TiempoMinutos { get; set; }
}
