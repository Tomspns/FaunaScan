namespace FaunaScanApp.Models;

public class AnimalResultDisplay
{
    public string Species { get; set; } = "";
    public double Confidence { get; set; }
    public string Description { get; set; } = "";
    public string Habitat { get; set; } = "";
    public string ImagePath { get; set; } = "";
}