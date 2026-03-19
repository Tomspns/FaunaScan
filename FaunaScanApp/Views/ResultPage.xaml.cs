namespace FaunaScanApp.Views;

public partial class ResultPage : ContentPage
{
    public ResultPage(AnimalResultDisplay result)
    {
        InitializeComponent();

        SpeciesLabel.Text = result.Species;
        DescriptionLabel.Text = result.Description;
        HabitatLabel.Text = result.Habitat;
        ConfidenceLabel.Text = $"{result.Confidence * 100:F1}%";
        AnimalImage.Source = result.ImagePath;
    }
}