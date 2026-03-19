namespace FaunaScanApp.ViewModels;

public partial class ResultPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string species = "";

    [ObservableProperty]
    private string description = "";

    [ObservableProperty]
    private string habitat = "";

    [RelayCommand]
    private async Task ScanAgain()
    {
        await Application.Current!.Windows[0].Page.Navigation.PushAsync(new Views.CameraPage());
    }
}