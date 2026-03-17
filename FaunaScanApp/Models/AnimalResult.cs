using System;
using System.Collections.Generic;
using System.Text;

namespace FaunaScanApp.Models;

public class AnimalResult
{
    public string Species { get; set; } = "";
    public double Confidence { get; set; }
    public string Description { get; set; } = "";
    public string Habitat { get; set; } = "";
    public string ImageUrl { get; set; } = "";

    public List<(string Name, double Confidence)> TopPredictions { get; set; } = new();
}
