using System.Text.Json.Serialization;

namespace FaunaScanApp.Models;

public class AnimalResult
{
    [JsonPropertyName("nb_detections")]
    public int NbDetections { get; set; }

    [JsonPropertyName("detections")]
    public List<Detection> Detections { get; set; } = new();

    [JsonPropertyName("sauvegarde_id")]
    public string SauvegardeId { get; set; } = "";
}

public class Detection
{
    [JsonPropertyName("animal")]
    public string Animal { get; set; } = "";

    [JsonPropertyName("confiance")]
    public double Confiance { get; set; }

    [JsonPropertyName("bbox")]
    public List<double> Bbox { get; set; } = new();

    [JsonPropertyName("wikipedia")]
    public Wikipedia Wikipedia { get; set; } = new();
}

public class Wikipedia
{
    [JsonPropertyName("nom")]
    public string Nom { get; set; } = "";

    [JsonPropertyName("nom_wikipedia")]
    public string NomWikipedia { get; set; } = "";

    [JsonPropertyName("description_generale")]
    public string DescriptionGenerale { get; set; } = "";

    [JsonPropertyName("habitat")]
    public string? Habitat { get; set; }

    [JsonPropertyName("url_wikipedia")]
    public string UrlWikipedia { get; set; } = "";
}