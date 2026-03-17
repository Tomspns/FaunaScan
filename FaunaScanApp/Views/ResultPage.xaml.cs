using FaunaScanApp.Models;

namespace FaunaScanApp.Views;

public partial class ResultPage : ContentPage
{
    public ResultPage(AnimalResult result)
    {
        InitializeComponent();

        SpeciesLabel.Text = result.Species;
        DescriptionLabel.Text = result.Description;
        HabitatLabel.Text = result.Habitat;
        ConfidenceLabel.Text = $"Confiance : {result.Confidence * 100:F0}%";

        AnimalImage.Source = result.ImageUrl;
    }
}