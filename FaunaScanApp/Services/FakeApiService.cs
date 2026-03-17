using FaunaScanApp.Models;

namespace FaunaScanApp.Services;

public class FakeApiService
{
    public async Task<AnimalResult> PredictAnimal()
    {
        await Task.Delay(2000);

        return new AnimalResult
        {
            Species = "Renard roux",
            Confidence = 0.93,
            Description = "Mammifère carnivore très adaptable.",
            Habitat = "Forêts et zones urbaines",
            ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/1/16/2010-brown-fox.jpg",

            TopPredictions = new List<(string Name, double Confidence)>
            {
                ("Renard roux", 0.93),
                ("Loup gris", 0.85),
                ("Chien sauvage", 0.72)
            }
        };
    } 
}