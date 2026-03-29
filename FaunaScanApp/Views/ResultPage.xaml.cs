namespace FaunaScanApp.Views;

public partial class ResultPage : ContentPage
{
    public ResultPage(AnimalResultDisplay result)
    {
        InitializeComponent();

        BindingContext = new ViewModels.ResultPageViewModel(); // 🔥 IMPORTANT

        SpeciesLabel.Text = result.Species;
        DescriptionLabel.Text = result.Description;
        HabitatLabel.Text = result.Habitat;
        ConfidenceLabel.Text = $"{result.Confidence * 100:F1}%";
        AnimalImage.Source = result.ImagePath;
    }
}